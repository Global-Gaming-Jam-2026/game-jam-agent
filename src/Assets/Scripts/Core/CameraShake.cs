using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance { get; private set; }

    [Header("Zoom Settings")]
    [SerializeField] private float defaultOrthoSize = 5f;
    [SerializeField] private float zoomSpeed = 8f;

    private Vector3 originalPosition;
    private Coroutine shakeCoroutine;
    private Coroutine zoomCoroutine;
    private Camera mainCamera;
    private float targetOrthoSize;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        originalPosition = transform.localPosition;

        mainCamera = GetComponent<Camera>();
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        if (mainCamera != null && mainCamera.orthographic)
        {
            defaultOrthoSize = mainCamera.orthographicSize;
            targetOrthoSize = defaultOrthoSize;
        }
    }

    private void Update()
    {
        // Smooth zoom interpolation
        if (mainCamera != null && mainCamera.orthographic)
        {
            if (Mathf.Abs(mainCamera.orthographicSize - targetOrthoSize) > 0.01f)
            {
                mainCamera.orthographicSize = Mathf.Lerp(
                    mainCamera.orthographicSize,
                    targetOrthoSize,
                    Time.unscaledDeltaTime * zoomSpeed
                );
            }
        }
    }

    /// <summary>
    /// Trigger camera shake with specified intensity and duration
    /// </summary>
    /// <param name="intensity">Shake magnitude (0.1 = subtle, 0.3 = medium, 0.5 = heavy)</param>
    /// <param name="duration">How long to shake in seconds</param>
    public void Shake(float intensity, float duration)
    {
        // Check if screen shake is enabled in settings
        // SettingsUI.ScreenShakeEnabled defaults to true, so this is safe even if SettingsUI hasn't loaded
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

    #region Zoom Effects

    /// <summary>
    /// Subtle zoom in during boss attacks for impact
    /// </summary>
    public void ZoomForBossAttack()
    {
        if (zoomCoroutine != null) StopCoroutine(zoomCoroutine);
        zoomCoroutine = StartCoroutine(ZoomPulse(0.85f, 0.3f));
    }

    /// <summary>
    /// Dramatic zoom pulse for phase transitions
    /// </summary>
    public void ZoomForPhaseTransition()
    {
        if (zoomCoroutine != null) StopCoroutine(zoomCoroutine);
        zoomCoroutine = StartCoroutine(ZoomPulse(0.75f, 0.5f));
    }

    /// <summary>
    /// Slow dramatic zoom for victory/defeat
    /// </summary>
    public void ZoomForGameEnd()
    {
        if (zoomCoroutine != null) StopCoroutine(zoomCoroutine);
        targetOrthoSize = defaultOrthoSize * 0.8f;
    }

    /// <summary>
    /// Reset zoom to default
    /// </summary>
    public void ResetZoom()
    {
        targetOrthoSize = defaultOrthoSize;
    }

    private IEnumerator ZoomPulse(float zoomMultiplier, float duration)
    {
        float originalTarget = targetOrthoSize;
        float zoomedSize = defaultOrthoSize * zoomMultiplier;

        // Zoom in
        targetOrthoSize = zoomedSize;
        yield return new WaitForSecondsRealtime(duration);

        // Zoom back out
        targetOrthoSize = originalTarget;
        zoomCoroutine = null;
    }

    #endregion

    #region Combined Effects

    /// <summary>
    /// Boss slam: heavy shake + zoom pulse
    /// </summary>
    public void BossSlamImpact()
    {
        ShakeBossAttack();
        ZoomForBossAttack();
    }

    /// <summary>
    /// Phase transition: dramatic shake + sustained zoom
    /// </summary>
    public void PhaseTransitionEffect()
    {
        ShakeHeavy();
        ZoomForPhaseTransition();
    }

    /// <summary>
    /// Death effect: heavy shake + zoom in
    /// </summary>
    public void DeathEffect()
    {
        Shake(0.5f, 0.4f);
        ZoomForGameEnd();
    }

    #endregion
}
