using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float acceleration = 50f;
    [SerializeField] private float deceleration = 50f;

    [Header("Dodge Roll")]
    [SerializeField] private float dodgeSpeed = 12f;
    [SerializeField] private float dodgeDuration = 0.4f;
    [SerializeField] private float dodgeCooldown = 0.6f;
    [SerializeField] private float iFrameDuration = 0.2f;

    [Header("Input Buffer")]
    [SerializeField] private float inputBufferTime = 0.15f;

    // Components
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    // State
    private float horizontalInput;
    private float currentSpeed;
    private bool isDodging;
    private bool isInvulnerable;
    private float dodgeTimer;
    private float dodgeCooldownTimer;
    private float iFrameTimer;
    private int facingDirection = 1; // 1 = right, -1 = left

    // Input buffering
    private float dodgeBufferTimer;
    private bool dodgeBuffered;

    // Animation hashes (for performance)
    private static readonly int AnimSpeed = Animator.StringToHash("Speed");
    private static readonly int AnimDodge = Animator.StringToHash("Dodge");

    public bool IsInvulnerable => isInvulnerable;
    public bool IsDodging => isDodging;
    public int FacingDirection => facingDirection;

    public event System.Action OnDodgeStart;
    public event System.Action OnDodgeEnd;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        // Ensure kinematic for direct control
        rb.bodyType = RigidbodyType2D.Kinematic;
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

    private void FixedUpdate()
    {
        if (GameManager.Instance != null && GameManager.Instance.CurrentState != GameManager.GameState.Playing)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (isDodging)
        {
            // During dodge, move in dodge direction
            rb.linearVelocity = new Vector2(facingDirection * dodgeSpeed, 0);
        }
        else
        {
            // Normal movement
            ApplyMovement();
        }
    }

    private void HandleInput()
    {
        // Get horizontal input
        horizontalInput = Input.GetAxisRaw("Horizontal");

        // Update facing direction based on input (but not during dodge)
        if (!isDodging && horizontalInput != 0)
        {
            facingDirection = horizontalInput > 0 ? 1 : -1;
            if (spriteRenderer != null)
            {
                spriteRenderer.flipX = facingDirection < 0;
            }
        }

        // Dodge input (with buffering)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (CanDodge())
            {
                StartDodge();
            }
            else
            {
                // Buffer the dodge input
                dodgeBuffered = true;
                dodgeBufferTimer = inputBufferTime;
            }
        }
    }

    private void UpdateTimers()
    {
        // Dodge cooldown
        if (dodgeCooldownTimer > 0)
        {
            dodgeCooldownTimer -= Time.deltaTime;
        }

        // Dodge duration
        if (isDodging)
        {
            dodgeTimer -= Time.deltaTime;
            if (dodgeTimer <= 0)
            {
                EndDodge();
            }
        }

        // I-frames
        if (isInvulnerable && !isDodging)
        {
            iFrameTimer -= Time.deltaTime;
            if (iFrameTimer <= 0)
            {
                isInvulnerable = false;
            }
        }

        // Input buffer decay
        if (dodgeBuffered)
        {
            dodgeBufferTimer -= Time.deltaTime;
            if (dodgeBufferTimer <= 0)
            {
                dodgeBuffered = false;
            }
        }
    }

    private void ProcessBufferedInput()
    {
        // Process buffered dodge
        if (dodgeBuffered && CanDodge())
        {
            dodgeBuffered = false;
            StartDodge();
        }
    }

    private void ApplyMovement()
    {
        float targetSpeed = horizontalInput * moveSpeed;

        // Smooth acceleration/deceleration
        if (Mathf.Abs(horizontalInput) > 0.1f)
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration * Time.fixedDeltaTime);
        }
        else
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0, deceleration * Time.fixedDeltaTime);
        }

        rb.linearVelocity = new Vector2(currentSpeed, rb.linearVelocity.y);

        // Update animator
        if (animator != null)
        {
            animator.SetFloat(AnimSpeed, Mathf.Abs(currentSpeed));
        }
    }

    private bool CanDodge()
    {
        return !isDodging && dodgeCooldownTimer <= 0;
    }

    private void StartDodge()
    {
        isDodging = true;
        isInvulnerable = true;
        dodgeTimer = dodgeDuration;
        dodgeCooldownTimer = dodgeCooldown;

        // Trigger animation
        if (animator != null)
        {
            animator.SetTrigger(AnimDodge);
        }

        OnDodgeStart?.Invoke();
    }

    private void EndDodge()
    {
        isDodging = false;
        // Keep i-frames for a bit after dodge ends
        iFrameTimer = iFrameDuration - dodgeDuration;
        if (iFrameTimer <= 0)
        {
            isInvulnerable = false;
        }

        // Reset velocity
        currentSpeed = facingDirection * moveSpeed * 0.5f;

        OnDodgeEnd?.Invoke();
    }

    /// <summary>
    /// Grant invulnerability frames (called when taking damage)
    /// </summary>
    public void GrantIFrames(float duration)
    {
        isInvulnerable = true;
        iFrameTimer = duration;
    }

    /// <summary>
    /// Force stop movement (for hit stun, etc.)
    /// </summary>
    public void StopMovement()
    {
        currentSpeed = 0;
        rb.linearVelocity = Vector2.zero;
    }
}
