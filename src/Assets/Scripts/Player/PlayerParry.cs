using UnityEngine;
using System.Collections;

/// <summary>
/// Cuphead-style parry mechanic
/// Player can parry pink projectiles by pressing jump/parry while near them
/// Successful parry fills super meter and gives brief invulnerability
/// </summary>
public class PlayerParry : MonoBehaviour
{
    [Header("Parry Settings")]
    [SerializeField] private float parryWindow = 0.2f;        // How long parry is active
    [SerializeField] private float parryCooldown = 0.3f;      // Cooldown between parries
    [SerializeField] private KeyCode parryKey = KeyCode.Space; // Same as dodge in Cuphead
    [SerializeField] private float parryBounceForce = 8f;     // Upward bounce on successful parry

    [Header("Super Meter")]
    [SerializeField] private float superMeterMax = 100f;
    [SerializeField] private float superGainOnParry = 25f;    // Meter gained per parry
    [SerializeField] private float superGainOnHit = 5f;       // Meter gained per attack hit

    [Header("Visual Feedback")]
    [SerializeField] private Color parryFlashColor = new Color(1f, 0.5f, 0.7f); // Pink
    [SerializeField] private float parryFlashDuration = 0.15f;

    [Header("Audio")]
    [SerializeField] private AudioClip parrySound;
    [SerializeField] private AudioClip superReadySound;

    // Components
    private PlayerController playerController;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;

    // State
    private bool isParrying;
    private float parryTimer;
    private float cooldownTimer;
    private float currentSuperMeter;
    private bool superReady;
    private Color originalColor;

    public bool IsParrying => isParrying;
    public float SuperMeter => currentSuperMeter;
    public float SuperMeterPercent => currentSuperMeter / superMeterMax;
    public bool IsSuperReady => superReady;

    public event System.Action OnParryStart;
    public event System.Action OnParrySuccess;
    public event System.Action OnParryFail;
    public event System.Action<float> OnSuperMeterChanged;
    public event System.Action OnSuperReady;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }

    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.CurrentState != GameManager.GameState.Playing)
            return;

        UpdateTimers();
        HandleInput();
    }

    private void UpdateTimers()
    {
        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
        }

        if (isParrying)
        {
            parryTimer -= Time.deltaTime;
            if (parryTimer <= 0)
            {
                EndParry(false);
            }
        }
    }

    private void HandleInput()
    {
        // Parry input (same button as dodge - context dependent)
        // In Cuphead, you parry while airborne or when pressing toward pink objects
        if (Input.GetKeyDown(parryKey) && CanParry())
        {
            StartParry();
        }
    }

    private bool CanParry()
    {
        return !isParrying && cooldownTimer <= 0;
    }

    private void StartParry()
    {
        isParrying = true;
        parryTimer = parryWindow;

        // Visual feedback
        if (spriteRenderer != null)
        {
            spriteRenderer.color = parryFlashColor;
        }

        OnParryStart?.Invoke();
    }

    /// <summary>
    /// Called when player successfully parries a pink projectile
    /// </summary>
    public void SuccessfulParry()
    {
        if (!isParrying) return;

        // Play sound
        if (parrySound != null)
        {
            AudioSource.PlayClipAtPoint(parrySound, transform.position);
        }

        // Bounce upward
        if (rb != null)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, parryBounceForce);
        }

        // Add super meter
        AddSuperMeter(superGainOnParry);

        // Brief invulnerability
        if (playerController != null)
        {
            playerController.GrantIFrames(0.1f);
        }

        // Screen effect
        if (CameraShake.Instance != null)
        {
            CameraShake.Instance.ShakeLight();
        }

        // Hitstop
        StartCoroutine(ParryHitstop());

        OnParrySuccess?.Invoke();

        EndParry(true);
    }

    private void EndParry(bool success)
    {
        isParrying = false;
        cooldownTimer = parryCooldown;

        // Reset color
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }

        if (!success)
        {
            OnParryFail?.Invoke();
        }
    }

    private IEnumerator ParryHitstop()
    {
        float original = Time.timeScale;
        Time.timeScale = 0.1f;
        yield return new WaitForSecondsRealtime(0.08f);
        Time.timeScale = original;
    }

    /// <summary>
    /// Add to super meter (called on parry or attack hit)
    /// </summary>
    public void AddSuperMeter(float amount)
    {
        float oldMeter = currentSuperMeter;
        currentSuperMeter = Mathf.Min(currentSuperMeter + amount, superMeterMax);

        OnSuperMeterChanged?.Invoke(SuperMeterPercent);

        // Check if super just became ready
        if (!superReady && currentSuperMeter >= superMeterMax)
        {
            superReady = true;

            if (superReadySound != null)
            {
                AudioSource.PlayClipAtPoint(superReadySound, transform.position);
            }

            OnSuperReady?.Invoke();
        }
    }

    /// <summary>
    /// Use super attack (if meter is full)
    /// </summary>
    public bool TryUseSuperAttack()
    {
        if (!superReady) return false;

        currentSuperMeter = 0;
        superReady = false;
        OnSuperMeterChanged?.Invoke(0);

        return true;
    }

    /// <summary>
    /// Called when player lands an attack (for meter gain)
    /// </summary>
    public void OnAttackHit()
    {
        AddSuperMeter(superGainOnHit);
    }
}
