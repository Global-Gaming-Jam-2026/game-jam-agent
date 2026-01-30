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
    private Coroutine chaosAmbientCoroutine;

    // Chaos theme colors
    private static readonly Color ChaosTint = new Color(0.7f, 0.6f, 0.9f); // Subtle purple tint for phase 2+

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

        // Screen shake + zoom pulse for dramatic effect
        if (newPhase.screenShakeOnTransition && CameraShake.Instance != null)
        {
            CameraShake.Instance.PhaseTransitionEffect();
        }

        // Play transform sound
        if (newPhase.transformSound != null)
        {
            AudioSource.PlayClipAtPoint(newPhase.transformSound, transform.position);
        }
        else if (AudioManager.Instance != null)
        {
            // Use procedural audio fallback
            AudioManager.Instance.PlayBossPhaseTransition();
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

        // Start chaos ambient effects for phase 2+
        if (currentPhaseIndex >= 1 && chaosAmbientCoroutine == null)
        {
            chaosAmbientCoroutine = StartCoroutine(ChaosAmbientEffects());
        }

        OnPhaseChanged?.Invoke(currentPhaseIndex, newPhase);
        OnTransformComplete?.Invoke();

        // Resume fighting
        SetState(BossState.Idle);
    }

    /// <summary>
    /// Spawn ambient chaos particles during Phase 2+ to express the "Chaos" theme
    /// </summary>
    private IEnumerator ChaosAmbientEffects()
    {
        while (currentState != BossState.Dead)
        {
            if (GameManager.Instance != null && GameManager.Instance.CurrentState == GameManager.GameState.Playing)
            {
                // Spawn chaos ambient particles around boss
                if (ParticleManager.Instance != null)
                {
                    ParticleManager.Instance.SpawnChaosAmbient(transform.position);
                }

                // Subtle purple tint pulse on sprite
                if (spriteRenderer != null && currentPhaseIndex >= 1)
                {
                    float pulse = 0.9f + Mathf.Sin(Time.time * 2f) * 0.1f;
                    spriteRenderer.color = new Color(pulse, pulse * 0.9f, 1f);
                }
            }

            yield return new WaitForSeconds(0.5f);
        }
    }

    private IEnumerator FlashTransformation(float duration)
    {
        Color originalColor = spriteRenderer.color;
        Color chaosColor = new Color(0.424f, 0.361f, 0.906f); // Deep Purple #6C5CE7
        float elapsed = 0;
        float flashRate = 0.1f;
        bool white = true;

        // Trigger chaos phase transition feedback
        if (HitFeedback.Instance != null)
        {
            HitFeedback.Instance.BossPhaseTransition(transform.position, currentPhaseIndex + 1);
        }

        while (elapsed < duration * 0.7f)
        {
            // Flash between white and chaos purple for dramatic effect
            spriteRenderer.color = white ? Color.white : chaosColor;
            white = !white;

            // Spawn chaos particles during transformation
            if (ParticleManager.Instance != null)
            {
                ParticleManager.Instance.SpawnChaosSwirl(transform.position, 4);
            }

            yield return new WaitForSeconds(flashRate);
            elapsed += flashRate;
            flashRate *= 0.9f; // Speed up flashing
        }

        // Hold chaos purple briefly before applying new form
        spriteRenderer.color = chaosColor;

        // Big corruption burst at climax
        if (ParticleManager.Instance != null)
        {
            ParticleManager.Instance.SpawnCorruptionBurst(transform.position, 1.5f);
        }

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

        // Stop chaos effects
        if (chaosAmbientCoroutine != null)
        {
            StopCoroutine(chaosAmbientCoroutine);
            chaosAmbientCoroutine = null;
        }

        // Reset sprite color
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.white;
        }

        // Play death sound
        if (currentPhase != null && currentPhase.deathSound != null)
        {
            AudioSource.PlayClipAtPoint(currentPhase.deathSound, transform.position);
        }

        // Trigger death feedback
        if (HitFeedback.Instance != null)
        {
            HitFeedback.Instance.BossDeathFeedback(transform.position);
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

    /// <summary>
    /// Set phases and reinitialize (for BossInitializer)
    /// </summary>
    public void SetPhases(List<BossPhaseData> phaseList)
    {
        phases = phaseList ?? new List<BossPhaseData>();

        // Initialize all patterns in all phases
        foreach (var phase in phases)
        {
            if (phase == null) continue;
            if (phase.patterns != null)
            {
                foreach (var pattern in phase.patterns)
                {
                    if (pattern != null) pattern.Initialize(this);
                }
            }
        }

        // Set first phase if available
        if (phases.Count > 0 && phases[0] != null)
        {
            currentPhase = phases[0];
            currentPhaseIndex = 0;
            ApplyPhaseVisuals(currentPhase);
        }
        else
        {
            currentPhase = null;
            currentPhaseIndex = 0;
            Debug.LogWarning("BossControllerMultiPhase: No valid phases set");
        }
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
