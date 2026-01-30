using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float iFrameDuration = 0.5f;

    [Header("Visual Feedback")]
    [SerializeField] private float flashDuration = 0.1f;
    [SerializeField] private Color damageFlashColor = Color.white;

    // Components
    private PlayerController playerController;
    private SpriteRenderer spriteRenderer;

    // State
    private float currentHealth;
    private Color originalColor;
    private Coroutine flashCoroutine;

    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;
    public float HealthPercent => currentHealth / maxHealth;
    public bool IsDead => currentHealth <= 0;

    public event System.Action<float> OnHealthChanged; // passes current health percent
    public event System.Action OnDamaged;
    public event System.Action OnDeath;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }

    private void Start()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(HealthPercent);
    }

    public void TakeDamage(float damage)
    {
        // Check invulnerability
        if (playerController != null && playerController.IsInvulnerable)
        {
            return;
        }

        // Already dead
        if (IsDead) return;

        // Apply damage
        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth);

        OnHealthChanged?.Invoke(HealthPercent);
        OnDamaged?.Invoke();

        // Audio feedback
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayPlayerHurt();
        }

        // Grant i-frames
        if (playerController != null)
        {
            playerController.GrantIFrames(iFrameDuration);
            playerController.StopMovement();
        }

        // Visual feedback
        FlashDamage();

        // Screen shake
        if (CameraShake.Instance != null)
        {
            CameraShake.Instance.ShakeMedium();
        }

        // Check death
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        if (IsDead) return;

        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);

        OnHealthChanged?.Invoke(HealthPercent);
    }

    private void FlashDamage()
    {
        if (spriteRenderer == null) return;

        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
        }
        flashCoroutine = StartCoroutine(FlashCoroutine());
    }

    private System.Collections.IEnumerator FlashCoroutine()
    {
        // Flash white
        spriteRenderer.color = damageFlashColor;
        yield return new WaitForSeconds(flashDuration);
        spriteRenderer.color = originalColor;

        // Flicker during i-frames
        float flickerTime = iFrameDuration - flashDuration;
        float flickerInterval = 0.1f;
        float elapsed = 0;

        while (elapsed < flickerTime)
        {
            spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0.5f);
            yield return new WaitForSeconds(flickerInterval / 2);
            spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(flickerInterval / 2);
            elapsed += flickerInterval;
        }

        spriteRenderer.color = originalColor;
        flashCoroutine = null;
    }

    private void Die()
    {
        OnDeath?.Invoke();

        // Notify game manager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.PlayerLost();
        }

        // Disable player controls
        if (playerController != null)
        {
            playerController.enabled = false;
        }

        var combat = GetComponent<PlayerCombat>();
        if (combat != null)
        {
            combat.enabled = false;
        }
    }

    /// <summary>
    /// Reset health to full (for restart)
    /// </summary>
    public void ResetHealth()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(HealthPercent);

        if (playerController != null)
        {
            playerController.enabled = true;
        }

        var combat = GetComponent<PlayerCombat>();
        if (combat != null)
        {
            combat.enabled = true;
        }
    }

    /// <summary>
    /// Set max health (for hero initialization)
    /// </summary>
    public void SetMaxHealth(float newMaxHealth)
    {
        maxHealth = newMaxHealth;
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(HealthPercent);
    }

    /// <summary>
    /// Set damage flash color (for hero customization)
    /// </summary>
    public void SetDamageFlashColor(Color color)
    {
        damageFlashColor = color;
    }

    /// <summary>
    /// Set original color (for hero customization)
    /// </summary>
    public void SetOriginalColor(Color color)
    {
        originalColor = color;
        if (spriteRenderer != null)
        {
            spriteRenderer.color = color;
        }
    }
}
