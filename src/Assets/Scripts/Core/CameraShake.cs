using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance { get; private set; }

    private Vector3 originalPosition;
    private Coroutine shakeCoroutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        originalPosition = transform.localPosition;
    }

    /// <summary>
    /// Trigger camera shake with specified intensity and duration
    /// </summary>
    /// <param name="intensity">Shake magnitude (0.1 = subtle, 0.3 = medium, 0.5 = heavy)</param>
    /// <param name="duration">How long to shake in seconds</param>
    public void Shake(float intensity, float duration)
    {
        // Check if screen shake is enabled in settings
        if (!SettingsUI.ScreenShakeEnabled) return;

        if (shakeCoroutine != null)
        {
            StopCoroutine(shakeCoroutine);
        }
        shakeCoroutine = StartCoroutine(ShakeCoroutine(intensity, duration));
    }

    /// <summary>
    /// Quick shake presets for common events
    /// </summary>
    public void ShakeLight() => Shake(0.1f, 0.1f);
    public void ShakeMedium() => Shake(0.2f, 0.15f);
    public void ShakeHeavy() => Shake(0.35f, 0.2f);
    public void ShakeBossAttack() => Shake(0.4f, 0.25f);

    private IEnumerator ShakeCoroutine(float intensity, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * intensity;
            float y = Random.Range(-1f, 1f) * intensity;

            transform.localPosition = originalPosition + new Vector3(x, y, 0);

            elapsed += Time.unscaledDeltaTime;

            // Decay intensity over time for smoother feel
            intensity *= 0.95f;

            yield return null;
        }

        transform.localPosition = originalPosition;
        shakeCoroutine = null;
    }

    public void StopShake()
    {
        if (shakeCoroutine != null)
        {
            StopCoroutine(shakeCoroutine);
            shakeCoroutine = null;
        }
        transform.localPosition = originalPosition;
    }
}
