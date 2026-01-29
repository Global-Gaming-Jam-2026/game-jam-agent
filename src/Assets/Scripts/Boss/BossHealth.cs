using UnityEngine;

public class BossHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 500f;

    [Header("Stagger Settings")]
    [SerializeField] private float staggerThreshold = 100f;
    [SerializeField] private float staggerResetTime = 3f;

    [Header("Visual Feedback")]
    [SerializeField] private float flashDuration = 0.1f;
    [SerializeField] private Color damageFlashColor = Color.white;

    // Components
    private SpriteRenderer spriteRenderer;

    // State
    private float currentHealth;
    private float staggerDamageAccumulated;
    private float staggerResetTimer;
    private Color originalColor;
    private Coroutine flashCoroutine;

    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;
    public float HealthPercent => currentHealth / maxHealth;
    public bool IsDead => currentHealth <= 0;

    public event System.Action<float> OnHealthChanged; // passes health percent
    public event System.Action OnDamaged;
    public event System.Action OnStagger;
    public event System.Action OnDeath;

    private void Awake()
    {
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

    private void Update()
    {
        // Stagger reset timer
        if (staggerDamageAccumulated > 0)
        {
            staggerResetTimer -= Time.deltaTime;
            if (staggerResetTimer <= 0)
            {
                staggerDamageAccumulated = 0;
            }
        }
    }

    public void TakeDamage(float damage)
    {
        if (IsDead) return;

        // Apply damage
        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth);

        // Accumulate stagger damage
        staggerDamageAccumulated += damage;
        staggerResetTimer = staggerResetTime;

        OnHealthChanged?.Invoke(HealthPercent);
        OnDamaged?.Invoke();

        // Visual feedback
        FlashDamage();

        // Check stagger
        if (staggerDamageAccumulated >= staggerThreshold)
        {
            staggerDamageAccumulated = 0;
            OnStagger?.Invoke();
        }

        // Check death
        if (currentHealth <= 0)
        {
            Die();
        }
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
        spriteRenderer.color = damageFlashColor;
        yield return new WaitForSeconds(flashDuration);
        spriteRenderer.color = originalColor;
        flashCoroutine = null;
    }

    private void Die()
    {
        OnDeath?.Invoke();

        // Heavy screen shake
        if (CameraShake.Instance != null)
        {
            CameraShake.Instance.ShakeBossAttack();
        }
    }

    /// <summary>
    /// Reset health (for restart)
    /// </summary>
    public void ResetHealth()
    {
        currentHealth = maxHealth;
        staggerDamageAccumulated = 0;
        OnHealthChanged?.Invoke(HealthPercent);
    }
}
