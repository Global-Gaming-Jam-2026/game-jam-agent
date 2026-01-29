using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Cuphead-style boss controller with multiple transforming phases
/// Each phase is a completely different form with unique attacks
/// </summary>
public class BossControllerMultiPhase : MonoBehaviour
{
    public enum BossState { Idle, Telegraph, Attacking, Recovering, Transforming, Dead }

    [Header("Phases (Forms)")]
    [SerializeField] private List<BossPhaseData> phases = new List<BossPhaseData>();

    [Header("General Settings")]
    [SerializeField] private float idleDuration = 1f;
    [SerializeField] private float recoveryDuration = 0.8f;

    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;

    // Components
    private BossHealth bossHealth;

    // State
    private BossState currentState = BossState.Idle;
    private int currentPhaseIndex = 0;
    private BossPhaseData currentPhase;
    private BossAttackPattern currentPattern;
    private float stateTimer;
    private bool isInitialized;

    // Animation hashes
    private static readonly int AnimState = Animator.StringToHash("State");
    private static readonly int AnimPhase = Animator.StringToHash("Phase");

    public BossState CurrentState => currentState;
    public int CurrentPhaseIndex => currentPhaseIndex;
    public BossPhaseData CurrentPhase => currentPhase;
    public Transform Player => player;

    public event System.Action<BossState> OnStateChanged;
    public event System.Action<int, BossPhaseData> OnPhaseChanged;
    public event System.Action OnTransformStart;
    public event System.Action OnTransformComplete;

    private void Awake()
    {
        bossHealth = GetComponent<BossHealth>();
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        if (animator == null) animator = GetComponent<Animator>();
    }

    private void Start()
    {
        // Find player if not assigned
        if (player == null)
        {
            var playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
        }

        // Subscribe to health events
        if (bossHealth != null)
        {
            bossHealth.OnHealthChanged += CheckPhaseTransition;
            bossHealth.OnDeath += HandleDeath;
        }

        // Initialize all patterns in all phases
        foreach (var phase in phases)
        {
            if (phase.patterns != null)
            {
                foreach (var pattern in phase.patterns)
                {
                    if (pattern != null) pattern.Initialize(this);
                }
            }
        }

        // Start first phase
        if (phases.Count > 0)
        {
            currentPhase = phases[0];
            ApplyPhaseVisuals(currentPhase);
        }

        isInitialized = true;
        SetState(BossState.Idle);
    }

    private void Update()
    {
        if (!isInitialized || currentState == BossState.Dead || currentState == BossState.Transforming)
            return;

        if (GameManager.Instance != null && GameManager.Instance.CurrentState != GameManager.GameState.Playing)
            return;

        UpdateStateMachine();
    }

    private void UpdateStateMachine()
    {
        stateTimer -= Time.deltaTime;

        switch (currentState)
        {
            case BossState.Idle:
                if (stateTimer <= 0)
                {
                    SelectAndStartAttack();
                }
                break;

            case BossState.Recovering:
                if (stateTimer <= 0)
                {
                    SetState(BossState.Idle);
                }
                break;
        }
    }

    public void SetState(BossState newState)
    {
        if (currentState == newState) return;
        currentState = newState;

        switch (newState)
        {
            case BossState.Idle:
                stateTimer = currentPhase != null ? currentPhase.attackCooldown : idleDuration;
                break;

            case BossState.Recovering:
                stateTimer = recoveryDuration;
                break;

            case BossState.Transforming:
                // Handled by coroutine
                break;

            case BossState.Dead:
                if (currentPattern != null)
                {
                    currentPattern.Cancel();
                    currentPattern = null;
                }
                break;
        }

        if (animator != null)
        {
            animator.SetInteger(AnimState, (int)newState);
        }

        OnStateChanged?.Invoke(newState);
    }

    private void SelectAndStartAttack()
    {
        if (currentPhase == null || currentPhase.patterns == null || currentPhase.patterns.Count == 0)
        {
            SetState(BossState.Idle);
            return;
        }

        // Weighted random selection
        var availablePatterns = currentPhase.patterns.FindAll(p => p != null);
        if (availablePatterns.Count == 0)
        {
            SetState(BossState.Idle);
            return;
        }

        float totalWeight = 0;
        foreach (var pattern in availablePatterns)
            totalWeight += pattern.SelectionWeight;

        float random = Random.Range(0, totalWeight);
        float cumulative = 0;

        foreach (var pattern in availablePatterns)
        {
            cumulative += pattern.SelectionWeight;
            if (random <= cumulative)
            {
                StartPattern(pattern);
                return;
            }
        }

        StartPattern(availablePatterns[0]);
    }

    private void StartPattern(BossAttackPattern pattern)
    {
        currentPattern = pattern;
        float speedMult = currentPhase != null ? currentPhase.patternSpeedMultiplier : 1f;
        StartCoroutine(ExecutePatternCoroutine(pattern, speedMult));
    }

    private IEnumerator ExecutePatternCoroutine(BossAttackPattern pattern, float speedMultiplier)
    {
        // Telegraph
        SetState(BossState.Telegraph);
        yield return pattern.Telegraph(speedMultiplier);

        if (currentState == BossState.Transforming || currentState == BossState.Dead)
            yield break;

        // Attack
        SetState(BossState.Attacking);
        yield return pattern.Execute(speedMultiplier);

        if (currentState == BossState.Transforming || currentState == BossState.Dead)
            yield break;

        // Recovery
        currentPattern = null;
        SetState(BossState.Recovering);
    }

    private void CheckPhaseTransition(float healthPercent)
    {
        if (currentPhase == null) return;

        // Check if we should transition to next phase
        if (healthPercent <= currentPhase.healthPercentEnd && currentPhaseIndex < phases.Count - 1)
        {
            StartCoroutine(TransitionToPhase(currentPhaseIndex + 1));
        }
    }

    private IEnumerator TransitionToPhase(int newPhaseIndex)
    {
        if (newPhaseIndex >= phases.Count) yield break;

        BossPhaseData newPhase = phases[newPhaseIndex];

        // Cancel current attack
        if (currentPattern != null)
        {
            currentPattern.Cancel();
            currentPattern = null;
        }

        SetState(BossState.Transforming);
        OnTransformStart?.Invoke();

        // Invulnerability during transformation
        if (newPhase.invulnerableDuringTransition && bossHealth != null)
        {
            // Could add invulnerability flag to BossHealth
        }

        // Screen shake
        if (newPhase.screenShakeOnTransition && CameraShake.Instance != null)
        {
            CameraShake.Instance.ShakeHeavy();
        }

        // Play transform sound
        if (newPhase.transformSound != null)
        {
            AudioSource.PlayClipAtPoint(newPhase.transformSound, transform.position);
        }

        // Flash white during transformation
        if (spriteRenderer != null)
        {
            yield return FlashTransformation(newPhase.transitionDuration);
        }
        else
        {
            yield return new WaitForSeconds(newPhase.transitionDuration);
        }

        // Apply new phase
        currentPhaseIndex = newPhaseIndex;
        currentPhase = newPhase;
        ApplyPhaseVisuals(newPhase);

        if (animator != null)
        {
            animator.SetInteger(AnimPhase, currentPhaseIndex);
        }

        OnPhaseChanged?.Invoke(currentPhaseIndex, newPhase);
        OnTransformComplete?.Invoke();

        // Resume fighting
        SetState(BossState.Idle);
    }

    private IEnumerator FlashTransformation(float duration)
    {
        Color originalColor = spriteRenderer.color;
        float elapsed = 0;
        float flashRate = 0.1f;
        bool white = true;

        while (elapsed < duration * 0.7f)
        {
            spriteRenderer.color = white ? Color.white : originalColor;
            white = !white;
            yield return new WaitForSeconds(flashRate);
            elapsed += flashRate;
            flashRate *= 0.9f; // Speed up flashing
        }

        // Hold white briefly
        spriteRenderer.color = Color.white;
        yield return new WaitForSeconds(duration * 0.3f);

        spriteRenderer.color = originalColor;
    }

    private void ApplyPhaseVisuals(BossPhaseData phase)
    {
        if (phase == null) return;

        // Apply sprite
        if (spriteRenderer != null && phase.formSprite != null)
        {
            spriteRenderer.sprite = phase.formSprite;
        }

        // Apply scale
        transform.localScale = new Vector3(phase.formScale.x, phase.formScale.y, 1);

        // Apply offset
        transform.localPosition += (Vector3)phase.formOffset;

        // Apply animator
        if (animator != null && phase.animator != null)
        {
            animator.runtimeAnimatorController = phase.animator;
        }
    }

    private void HandleDeath()
    {
        SetState(BossState.Dead);

        // Play death sound
        if (currentPhase != null && currentPhase.deathSound != null)
        {
            AudioSource.PlayClipAtPoint(currentPhase.deathSound, transform.position);
        }

        if (GameManager.Instance != null)
        {
            StartCoroutine(DelayedWin());
        }
    }

    private IEnumerator DelayedWin()
    {
        // Death animation time
        yield return new WaitForSeconds(2f);
        GameManager.Instance.PlayerWon();
    }

    /// <summary>
    /// Initialize the boss with a specific set of phases (for runtime setup)
    /// </summary>
    public void SetupPhases(List<BossPhaseData> phaseList)
    {
        phases = phaseList;
    }

    private void OnDestroy()
    {
        if (bossHealth != null)
        {
            bossHealth.OnHealthChanged -= CheckPhaseTransition;
            bossHealth.OnDeath -= HandleDeath;
        }
    }

    // For getting player reference in patterns
    public void Initialize(BossControllerMultiPhase controller) { }
}
