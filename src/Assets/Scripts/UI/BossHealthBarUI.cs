using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Boss health bar UI - typically at top of screen
/// </summary>
public class BossHealthBarUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BossHealth bossHealth;
    [SerializeField] private BossControllerMultiPhase bossController;
    [SerializeField] private Image healthFill;
    [SerializeField] private Image damageFill;
    [SerializeField] private Text bossNameText;
    [SerializeField] private GameObject phaseIndicator;

    [Header("Colors")]
    [SerializeField] private Color healthColor = new Color(0.8f, 0.5f, 0.2f); // Bronze
    [SerializeField] private Color damageColor = new Color(0.9f, 0.3f, 0.1f);
    [SerializeField] private Color phase2Color = new Color(0.9f, 0.2f, 0.3f); // Red for phase 2

    [Header("Animation")]
    [SerializeField] private float smoothSpeed = 8f;
    [SerializeField] private float damageDelay = 0.3f;

    [Header("Boss Info")]
    [SerializeField] private string bossName = "Mask of the Bronze God";

    private float targetFill;
    private float damageDelayTimer;
    private CanvasGroup canvasGroup;

    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        // Find boss if not assigned
        if (bossHealth == null)
        {
            var boss = GameObject.FindGameObjectWithTag("Boss");
            if (boss != null)
            {
                bossHealth = boss.GetComponent<BossHealth>();
                bossController = boss.GetComponent<BossControllerMultiPhase>();
            }
        }

        if (bossHealth != null)
        {
            bossHealth.OnHealthChanged += UpdateHealthBar;
            bossHealth.OnDeath += OnBossDeath;
            UpdateHealthBar(bossHealth.HealthPercent);
        }

        if (bossController != null)
        {
            bossController.OnPhaseChanged += OnPhaseChangedHandler;
        }

        // Set boss name
        if (bossNameText != null)
        {
            bossNameText.text = bossName;
        }

        // Set initial colors
        if (healthFill != null) healthFill.color = healthColor;
        if (damageFill != null) damageFill.color = damageColor;

        // Hide phase indicator initially
        if (phaseIndicator != null)
        {
            phaseIndicator.SetActive(false);
        }

        targetFill = 1f;
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
                damageFill.fillAmount = Mathf.Lerp(damageFill.fillAmount, targetFill, smoothSpeed * 0.5f * Time.deltaTime);
            }
        }
    }

    private void UpdateHealthBar(float healthPercent)
    {
        // Set damage fill to current before changing
        if (damageFill != null && healthPercent < targetFill)
        {
            damageFill.fillAmount = healthFill != null ? healthFill.fillAmount : targetFill;
            damageDelayTimer = damageDelay;
        }

        targetFill = healthPercent;
    }

    private void OnPhaseChangedHandler(int phase, BossPhaseData phaseData)
    {
        // Update boss name from phase data if available
        if (bossNameText != null && phaseData != null && !string.IsNullOrEmpty(phaseData.phaseName))
        {
            bossNameText.text = phaseData.phaseName;
        }

        if (phase >= 2)
        {
            // Show phase indicator
            if (phaseIndicator != null)
            {
                phaseIndicator.SetActive(true);
            }

            // Change color to phase 2 color
            if (healthFill != null)
            {
                StartCoroutine(FlashAndChangeColor(phase2Color));
            }
        }
    }

    private System.Collections.IEnumerator FlashAndChangeColor(Color newColor)
    {
        // Flash white
        if (healthFill != null)
        {
            healthFill.color = Color.white;
            yield return new WaitForSeconds(0.1f);
            healthFill.color = newColor;
        }
    }

    private void OnBossDeath()
    {
        // Fade out health bar
        if (canvasGroup != null)
        {
            StartCoroutine(FadeOut());
        }
    }

    private System.Collections.IEnumerator FadeOut()
    {
        float elapsed = 0;
        float duration = 1f;

        while (elapsed < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = 0;
    }

    private void OnDestroy()
    {
        if (bossHealth != null)
        {
            bossHealth.OnHealthChanged -= UpdateHealthBar;
            bossHealth.OnDeath -= OnBossDeath;
        }

        if (bossController != null)
        {
            bossController.OnPhaseChanged -= OnPhaseChangedHandler;
        }
    }
}
