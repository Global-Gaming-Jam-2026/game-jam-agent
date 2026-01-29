using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private float attackDamage = 25f;
    [SerializeField] private float attackWindup = 0.1f;
    [SerializeField] private float attackActive = 0.05f;
    [SerializeField] private float attackRecovery = 0.3f;
    [SerializeField] private int maxComboCount = 3;
    [SerializeField] private float comboResetTime = 0.8f;

    [Header("Hitbox")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private Vector2 attackSize = new Vector2(1.5f, 1f);
    [SerializeField] private LayerMask enemyLayer;

    [Header("Input Buffer")]
    [SerializeField] private float inputBufferTime = 0.15f;

    // Components
    private PlayerController playerController;
    private Animator animator;

    // State
    private bool isAttacking;
    private int currentCombo;
    private float attackTimer;
    private float comboResetTimer;
    private AttackPhase attackPhase;

    // Input buffering
    private bool attackBuffered;
    private float attackBufferTimer;

    // Animation hashes
    private static readonly int AnimAttack = Animator.StringToHash("Attack");
    private static readonly int AnimCombo = Animator.StringToHash("ComboIndex");

    public bool IsAttacking => isAttacking;
    public event System.Action<int> OnAttackStart; // passes combo index
    public event System.Action OnAttackHit;

    private enum AttackPhase { Windup, Active, Recovery }

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.CurrentState != GameManager.GameState.Playing)
        {
            return;
        }

        HandleInput();
        UpdateTimers();
        ProcessBufferedInput();
    }

    private void HandleInput()
    {
        // Attack input (Mouse1 or J key)
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.J))
        {
            if (CanAttack())
            {
                StartAttack();
            }
            else if (CanBufferAttack())
            {
                // Buffer the attack for combo
                attackBuffered = true;
                attackBufferTimer = inputBufferTime;
            }
        }
    }

    private void UpdateTimers()
    {
        // Attack phases
        if (isAttacking)
        {
            attackTimer -= Time.deltaTime;

            if (attackTimer <= 0)
            {
                AdvanceAttackPhase();
            }
        }

        // Combo reset
        if (!isAttacking && currentCombo > 0)
        {
            comboResetTimer -= Time.deltaTime;
            if (comboResetTimer <= 0)
            {
                currentCombo = 0;
            }
        }

        // Input buffer decay
        if (attackBuffered)
        {
            attackBufferTimer -= Time.deltaTime;
            if (attackBufferTimer <= 0)
            {
                attackBuffered = false;
            }
        }
    }

    private void ProcessBufferedInput()
    {
        // Process buffered attack for combo
        if (attackBuffered && CanAttack())
        {
            attackBuffered = false;
            StartAttack();
        }
    }

    private bool CanAttack()
    {
        // Can't attack while dodging
        if (playerController != null && playerController.IsDodging)
        {
            return false;
        }

        // Can't start new attack during windup or active phase
        if (isAttacking && attackPhase != AttackPhase.Recovery)
        {
            return false;
        }

        return true;
    }

    private bool CanBufferAttack()
    {
        // Can buffer during recovery phase for combo
        return isAttacking && attackPhase == AttackPhase.Recovery;
    }

    private void StartAttack()
    {
        isAttacking = true;
        attackPhase = AttackPhase.Windup;
        attackTimer = attackWindup;

        // Advance combo
        currentCombo++;
        if (currentCombo > maxComboCount)
        {
            currentCombo = 1;
        }

        // Trigger animation
        if (animator != null)
        {
            animator.SetInteger(AnimCombo, currentCombo);
            animator.SetTrigger(AnimAttack);
        }

        OnAttackStart?.Invoke(currentCombo);
    }

    private void AdvanceAttackPhase()
    {
        switch (attackPhase)
        {
            case AttackPhase.Windup:
                attackPhase = AttackPhase.Active;
                attackTimer = attackActive;
                PerformHitDetection();
                break;

            case AttackPhase.Active:
                attackPhase = AttackPhase.Recovery;
                attackTimer = attackRecovery;
                break;

            case AttackPhase.Recovery:
                EndAttack();
                break;
        }
    }

    private void PerformHitDetection()
    {
        // Calculate hitbox position based on facing direction
        int facing = playerController != null ? playerController.FacingDirection : 1;
        Vector2 hitboxCenter = (Vector2)attackPoint.position + new Vector2(attackSize.x * 0.5f * facing, 0);

        // Detect enemies in hitbox
        Collider2D[] hits = Physics2D.OverlapBoxAll(hitboxCenter, attackSize, 0, enemyLayer);

        foreach (var hit in hits)
        {
            // Try to deal damage
            var health = hit.GetComponent<BossHealth>();
            if (health != null)
            {
                float finalDamage = attackDamage;

                // Bonus damage on last hit of combo
                if (currentCombo == maxComboCount)
                {
                    finalDamage *= 1.5f;
                }

                health.TakeDamage(finalDamage);
                OnAttackHit?.Invoke();

                // Trigger hit feedback
                TriggerHitFeedback();
            }
        }
    }

    private void TriggerHitFeedback()
    {
        // Use centralized HitFeedback system for hitstop + screen shake
        if (HitFeedback.Instance != null)
        {
            HitFeedback.Instance.PlayerHitEnemy();
        }
        else
        {
            // Fallback if HitFeedback not in scene
            if (CameraShake.Instance != null)
            {
                CameraShake.Instance.ShakeLight();
            }
        }
    }

    private void EndAttack()
    {
        isAttacking = false;
        comboResetTimer = comboResetTime;
    }

    /// <summary>
    /// Set attack damage at runtime (for HeroInitializer)
    /// </summary>
    public void SetAttackDamage(float damage)
    {
        attackDamage = damage;
    }

    /// <summary>
    /// Set combo settings at runtime
    /// </summary>
    public void SetComboSettings(int maxCombo)
    {
        maxComboCount = maxCombo;
    }

    // Debug visualization
    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.color = Color.red;
        int facing = Application.isPlaying && playerController != null ? playerController.FacingDirection : 1;
        Vector2 hitboxCenter = (Vector2)attackPoint.position + new Vector2(attackSize.x * 0.5f * facing, 0);
        Gizmos.DrawWireCube(hitboxCenter, attackSize);
    }
}
