using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Player health bar UI
/// </summary>
public class HealthBarUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private Image healthFill;
    [SerializeField] private Image healthBackground;
    [SerializeField] private Image damageFill; // Shows recent damage (lags behind)

    [Header("Colors")]
    [SerializeField] private Color healthColor = new Color(0.2f, 0.8f, 0.3f);
    [SerializeField] private Color damageColor = new Color(0.8f, 0.2f, 0.2f);
    [SerializeField] private Color lowHealthColor = new Color(0.9f, 0.3f, 0.1f);
    [SerializeField] private float lowHealthThreshold = 0.25f;

    [Header("Animation")]
    [SerializeField] private float smoothSpeed = 5f;
    [SerializeField] private float damageDelay = 0.5f;

    private float targetFill;
    private float damageFillTarget;
    private float damageDelayTimer;

    private void Start()
    {
        // Find player health if not assigned
        if (playerHealth == null)
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerHealth = player.GetComponent<PlayerHealth>();
            }
        }

        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged += UpdateHealthBar;
            UpdateHealthBar(playerHealth.HealthPercent);
        }

        // Set initial colors
        if (healthFill != null) healthFill.color = healthColor;
        if (damageFill != null) damageFill.color = damageColor;
    }

    private void Update()
    {
        // Smooth health bar animation
        if (healthFill != null)
        {
            healthFill.fillAmount = Mathf.Lerp(healthFill.fillAmount, targetFill, smoothSpeed * Time.deltaTime);
        }

        // Damage fill lags behind
        if (damageFill != null)
        {
            if (damageDelayTimer > 0)
            {
                damageDelayTimer -= Time.deltaTime;
            }
            else
            {
                damageFill.fillAmount = Mathf.Lerp(damageFill.fillAmount, damageFillTarget, smoothSpeed * 0.5f * Time.deltaTime);
            }
        }

        // Update color based on health
        if (healthFill != null && targetFill <= lowHealthThreshold)
        {
            // Pulse effect for low health
            float pulse = Mathf.Sin(Time.time * 5f) * 0.2f + 0.8f;
            healthFill.color = Color.Lerp(lowHealthColor, healthColor, pulse);
        }
    }

    private void UpdateHealthBar(float healthPercent)
    {
        // Set damage fill to current value before changing target
        if (damageFill != null && healthPercent < targetFill)
        {
            damageFill.fillAmount = healthFill != null ? healthFill.fillAmount : targetFill;
            damageDelayTimer = damageDelay;
        }

        targetFill = healthPercent;
        damageFillTarget = healthPercent;

        // Update color
        if (healthFill != null)
        {
            healthFill.color = healthPercent <= lowHealthThreshold ? lowHealthColor : healthColor;
        }
    }

    private void OnDestroy()
    {
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged -= UpdateHealthBar;
        }
    }
}
