using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BossController : MonoBehaviour
{
    public enum BossState { Idle, Telegraph, Attacking, Recovering, Staggered, Dead }

    [Header("State Timing")]
    [SerializeField] private float idleDuration = 1.5f;
    [SerializeField] private float recoveryDuration = 1f;
    [SerializeField] private float staggerDuration = 2f;

    [Header("Phase Settings")]
    [SerializeField] private float phase2HealthThreshold = 0.5f;
    [SerializeField] private float phase2SpeedMultiplier = 1.25f;

    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private List<BossAttackPattern> patterns;

    // Components
    private BossHealth bossHealth;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    // State
    private BossState currentState = BossState.Idle;
    private BossAttackPattern currentPattern;
    private float stateTimer;
    private int currentPhase = 1;
    private bool isInitialized;

    // Animation hashes
    private static readonly int AnimState = Animator.StringToHash("State");
    private static readonly int AnimAttackIndex = Animator.StringToHash("AttackIndex");

    public BossState CurrentState => currentState;
    public int CurrentPhase => currentPhase;
    public Transform Player => player;

    public event System.Action<BossState> OnStateChanged;
    public event System.Action<int> OnPhaseChanged;

    private void Awake()
    {
        bossHealth = GetComponent<BossHealth>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        // Find player if not assigned
        if (player == null)
        {
            var playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }

        // Subscribe to health events
        if (bossHealth != null)
        {
            bossHealth.OnHealthChanged += CheckPhaseTransition;
            bossHealth.OnDeath += HandleDeath;
            bossHealth.OnStagger += HandleStagger;
        }

        // Initialize patterns
        foreach (var pattern in patterns)
        {
            pattern.Initialize(this);
        }

        isInitialized = true;
        SetState(BossState.Idle);
    }

    private void Update()
    {
        if (!isInitialized) return;

        if (GameManager.Instance != null && GameManager.Instance.CurrentState != GameManager.GameState.Playing)
        {
            return;
        }

        if (currentState == BossState.Dead) return;

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

            case BossState.Telegraph:
                // Pattern handles its own telegraph timing
                break;

            case BossState.Attacking:
                // Pattern handles attack execution
                break;

            case BossState.Recovering:
                if (stateTimer <= 0)
                {
                    SetState(BossState.Idle);
                }
                break;

            case BossState.Staggered:
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
                float idleTime = idleDuration;
                if (currentPhase >= 2)
                {
                    idleTime /= phase2SpeedMultiplier;
                }
                stateTimer = idleTime;
                break;

            case BossState.Recovering:
                float recoverTime = recoveryDuration;
                if (currentPhase >= 2)
                {
                    recoverTime /= phase2SpeedMultiplier;
                }
                stateTimer = recoverTime;
                break;

            case BossState.Staggered:
                stateTimer = staggerDuration;
                if (currentPattern != null)
                {
                    currentPattern.Cancel();
                    currentPattern = null;
                }
                break;

            case BossState.Dead:
                if (currentPattern != null)
                {
                    currentPattern.Cancel();
                    currentPattern = null;
                }
                break;
        }

        // Update animator
        if (animator != null)
        {
            animator.SetInteger(AnimState, (int)newState);
        }

        OnStateChanged?.Invoke(newState);
    }

    private void SelectAndStartAttack()
    {
        if (patterns == null || patterns.Count == 0)
        {
            SetState(BossState.Idle);
            return;
        }

        // Filter available patterns based on phase
        List<BossAttackPattern> availablePatterns = new List<BossAttackPattern>();
        foreach (var pattern in patterns)
        {
            if (pattern.MinPhaseRequired <= currentPhase)
            {
                availablePatterns.Add(pattern);
            }
        }

        if (availablePatterns.Count == 0)
        {
            SetState(BossState.Idle);
            return;
        }

        // Weighted random selection
        float totalWeight = 0;
        foreach (var pattern in availablePatterns)
        {
            totalWeight += pattern.SelectionWeight;
        }

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

        // Fallback
        StartPattern(availablePatterns[0]);
    }

    private void StartPattern(BossAttackPattern pattern)
    {
        currentPattern = pattern;

        // Update animator
        if (animator != null)
        {
            animator.SetInteger(AnimAttackIndex, patterns.IndexOf(pattern));
        }

        // Get speed multiplier for phase 2
        float speedMult = currentPhase >= 2 ? phase2SpeedMultiplier : 1f;

        // Start pattern execution
        StartCoroutine(ExecutePatternCoroutine(pattern, speedMult));
    }

    private IEnumerator ExecutePatternCoroutine(BossAttackPattern pattern, float speedMultiplier)
    {
        // Telegraph phase
        SetState(BossState.Telegraph);
        yield return pattern.Telegraph(speedMultiplier);

        if (currentState == BossState.Staggered || currentState == BossState.Dead)
        {
            yield break;
        }

        // Attack phase
        SetState(BossState.Attacking);
        yield return pattern.Execute(speedMultiplier);

        if (currentState == BossState.Staggered || currentState == BossState.Dead)
        {
            yield break;
        }

        // Recovery phase
        currentPattern = null;
        SetState(BossState.Recovering);
    }

    private void CheckPhaseTransition(float healthPercent)
    {
        if (currentPhase == 1 && healthPercent <= phase2HealthThreshold)
        {
            currentPhase = 2;
            OnPhaseChanged?.Invoke(currentPhase);

            // Visual feedback for phase transition
            StartCoroutine(PhaseTransitionEffect());
        }
    }

    private IEnumerator PhaseTransitionEffect()
    {
        // Brief pause
        Time.timeScale = 0.1f;
        yield return new WaitForSecondsRealtime(0.3f);
        Time.timeScale = 1f;

        // Screen shake
        if (CameraShake.Instance != null)
        {
            CameraShake.Instance.ShakeHeavy();
        }
    }

    private void HandleStagger()
    {
        SetState(BossState.Staggered);
    }

    private void HandleDeath()
    {
        SetState(BossState.Dead);

        // Notify game manager
        if (GameManager.Instance != null)
        {
            StartCoroutine(DelayedWin());
        }
    }

    private IEnumerator DelayedWin()
    {
        // Death animation time
        yield return new WaitForSeconds(1.5f);
        GameManager.Instance.PlayerWon();
    }

    private void OnDestroy()
    {
        if (bossHealth != null)
        {
            bossHealth.OnHealthChanged -= CheckPhaseTransition;
            bossHealth.OnDeath -= HandleDeath;
            bossHealth.OnStagger -= HandleStagger;
        }
    }
}
