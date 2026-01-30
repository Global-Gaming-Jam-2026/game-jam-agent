using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float acceleration = 80f;
    [SerializeField] private float deceleration = 60f;

    [Header("Dodge Roll")]
    [SerializeField] private float dodgeSpeed = 12f;
    [SerializeField] private float dodgeDuration = 0.4f;
    [SerializeField] private float dodgeCooldown = 0.6f;
    [SerializeField] private float iFrameDuration = 0.2f;

    [Header("Input Buffer")]
    [SerializeField] private float inputBufferTime = 0.15f;

    [Header("Dodge Effects")]
    [SerializeField] private float dodgeTrailInterval = 0.05f;
    [SerializeField] private Color dodgeTrailColor = new Color(0.7f, 0.9f, 1f, 0.5f);

    [Header("Debug")]
    [SerializeField] private bool enableDebugLogs = true;

    // Dodge trail
    private float dodgeTrailTimer = 0;

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
    private int facingDirection = 1;

    // Input buffering
    private float dodgeBufferTimer;
    private bool dodgeBuffered;

    // Debug
    private float debugLogTimer;

    // Animation hashes
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

        // Use Dynamic with no gravity for reliable movement in Unity 6
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 0;
        rb.freezeRotation = true;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        if (enableDebugLogs)
        {
            Debug.Log($"[PlayerController] Awake - RB Type: {rb.bodyType}, Gravity: {rb.gravityScale}, Simulated: {rb.simulated}");
        }
    }

    private void Start()
    {
        if (enableDebugLogs)
        {
            Debug.Log($"[PlayerController] Start - Position: {transform.position}, MoveSpeed: {moveSpeed}");
            string gameState = GameManager.Instance != null ? GameManager.Instance.CurrentState.ToString() : "NO GAMEMANAGER";
            Debug.Log($"[PlayerController] GameManager State: {gameState}");
        }
    }

    private void Update()
    {
        // Allow movement during Playing or Initializing (before game fully starts)
        if (GameManager.Instance != null &&
            GameManager.Instance.CurrentState != GameManager.GameState.Playing &&
            GameManager.Instance.CurrentState != GameManager.GameState.Initializing)
        {
            return;
        }

        HandleInput();
        UpdateTimers();
        ProcessBufferedInput();
    }

    private void FixedUpdate()
    {
        // Allow movement during Playing or Initializing
        if (GameManager.Instance != null &&
            GameManager.Instance.CurrentState != GameManager.GameState.Playing &&
            GameManager.Instance.CurrentState != GameManager.GameState.Initializing)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (isDodging)
        {
            // During dodge - direct transform movement
            transform.position += new Vector3(facingDirection * dodgeSpeed * Time.fixedDeltaTime, 0, 0);

            // Spawn dodge trail effects
            dodgeTrailTimer -= Time.fixedDeltaTime;
            if (dodgeTrailTimer <= 0)
            {
                SpawnDodgeTrail();
                dodgeTrailTimer = dodgeTrailInterval;
            }
        }
        else
        {
            ApplyMovement();
        }
    }

    private void HandleInput()
    {
        horizontalInput = GetHorizontalInput();

        if (enableDebugLogs)
        {
            debugLogTimer -= Time.deltaTime;
            if (debugLogTimer <= 0 && Mathf.Abs(horizontalInput) > 0.01f)
            {
                Debug.Log($"[PlayerController] Input: {horizontalInput:F2}, CurrentSpeed: {currentSpeed:F2}, Position: {transform.position}");
                debugLogTimer = 0.5f;
            }
        }

        if (!isDodging && horizontalInput != 0)
        {
            facingDirection = horizontalInput > 0 ? 1 : -1;
            if (spriteRenderer != null)
            {
                spriteRenderer.flipX = facingDirection < 0;
            }
        }

        if (GetDodgeInput())
        {
            if (CanDodge())
            {
                StartDodge();
            }
            else
            {
                dodgeBuffered = true;
                dodgeBufferTimer = inputBufferTime;
            }
        }
    }

    private void UpdateTimers()
    {
        if (dodgeCooldownTimer > 0)
            dodgeCooldownTimer -= Time.deltaTime;

        if (isDodging)
        {
            dodgeTimer -= Time.deltaTime;
            if (dodgeTimer <= 0)
                EndDodge();
        }

        if (isInvulnerable && !isDodging)
        {
            iFrameTimer -= Time.deltaTime;
            if (iFrameTimer <= 0)
                isInvulnerable = false;
        }

        if (dodgeBuffered)
        {
            dodgeBufferTimer -= Time.deltaTime;
            if (dodgeBufferTimer <= 0)
                dodgeBuffered = false;
        }
    }

    private void ProcessBufferedInput()
    {
        if (dodgeBuffered && CanDodge())
        {
            dodgeBuffered = false;
            StartDodge();
        }
    }

    private void ApplyMovement()
    {
        float targetSpeed = horizontalInput * moveSpeed;

        if (Mathf.Abs(horizontalInput) > 0.1f)
            currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration * Time.fixedDeltaTime);
        else
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0, deceleration * Time.fixedDeltaTime);

        // Direct transform movement - more reliable in Unity 6
        transform.position += new Vector3(currentSpeed * Time.fixedDeltaTime, 0, 0);

        if (animator != null)
            animator.SetFloat(AnimSpeed, Mathf.Abs(currentSpeed));
    }

    private bool CanDodge() => !isDodging && dodgeCooldownTimer <= 0;

    private void StartDodge()
    {
        isDodging = true;
        isInvulnerable = true;
        dodgeTimer = dodgeDuration;
        dodgeCooldownTimer = dodgeCooldown;
        dodgeTrailTimer = 0; // Spawn trail immediately

        if (animator != null)
            animator.SetTrigger(AnimDodge);

        // Play dodge sound
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayPlayerDodge();
        }

        // Spawn dust puff at start
        if (ParticleManager.Instance != null)
        {
            ParticleManager.Instance.SpawnDustPuff(transform.position + Vector3.down * 0.5f, Color.gray, 5);
        }

        // Hit feedback for successful dodge
        if (HitFeedback.Instance != null)
        {
            HitFeedback.Instance.DodgeSuccess(transform.position, spriteRenderer?.sprite);
        }

        OnDodgeStart?.Invoke();
    }

    private void EndDodge()
    {
        isDodging = false;
        iFrameTimer = iFrameDuration - dodgeDuration;
        if (iFrameTimer <= 0)
            isInvulnerable = false;

        currentSpeed = facingDirection * moveSpeed * 0.5f;

        // Spawn dust at end
        if (ParticleManager.Instance != null)
        {
            ParticleManager.Instance.SpawnDustPuff(transform.position + Vector3.down * 0.5f, Color.gray, 3);
        }

        OnDodgeEnd?.Invoke();
    }

    private void SpawnDodgeTrail()
    {
        if (ParticleManager.Instance != null && spriteRenderer != null)
        {
            ParticleManager.Instance.SpawnTrailGhost(transform.position, spriteRenderer.sprite, dodgeTrailColor);
        }
    }

    public void GrantIFrames(float duration)
    {
        isInvulnerable = true;
        iFrameTimer = duration;
    }

    public void StopMovement()
    {
        currentSpeed = 0;
        rb.linearVelocity = Vector2.zero;
    }

    public void SetMoveSpeed(float speed) => moveSpeed = speed;
    public void SetDodgeParams(float speed, float duration)
    {
        dodgeSpeed = speed;
        dodgeDuration = duration;
    }

    /// <summary>
    /// Get horizontal input - supports both old and new Input Systems
    /// </summary>
    private float GetHorizontalInput()
    {
        float input = 0f;

        // Try legacy Input System first
        try { input = Input.GetAxisRaw("Horizontal"); }
        catch { /* Legacy input not available */ }

        // Fallback to direct key detection
        if (Mathf.Abs(input) < 0.01f)
        {
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
                input = 1f;
            else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
                input = -1f;
        }

#if ENABLE_INPUT_SYSTEM
        // Try new Input System if available
        if (Mathf.Abs(input) < 0.01f && Keyboard.current != null)
        {
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
                input = 1f;
            else if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
                input = -1f;
        }
#endif

        return input;
    }

    /// <summary>
    /// Check if dodge key was pressed - supports both input systems
    /// </summary>
    private bool GetDodgeInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            return true;

#if ENABLE_INPUT_SYSTEM
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
            return true;
#endif

        return false;
    }
}
