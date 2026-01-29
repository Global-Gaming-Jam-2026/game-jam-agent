using UnityEngine;
using System.Collections;

/// <summary>
/// Handles hit feedback effects: hitstop, screen shake, flash
/// </summary>
public class HitFeedback : MonoBehaviour
{
    public static HitFeedback Instance { get; private set; }

    [Header("Hitstop Settings")]
    [SerializeField] private float lightHitstopDuration = 0.03f;
    [SerializeField] private float mediumHitstopDuration = 0.05f;
    [SerializeField] private float heavyHitstopDuration = 0.08f;
    [SerializeField] private float hitstopTimeScale = 0.05f;

    private Coroutine hitstopCoroutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// Trigger light hitstop (player light attack)
    /// </summary>
    public void LightHitstop()
    {
        TriggerHitstop(lightHitstopDuration);
    }

    /// <summary>
    /// Trigger medium hitstop (player heavy attack, boss light attack)
    /// </summary>
    public void MediumHitstop()
    {
        TriggerHitstop(mediumHitstopDuration);
    }

    /// <summary>
    /// Trigger heavy hitstop (boss heavy attack, death blows)
    /// </summary>
    public void HeavyHitstop()
    {
        TriggerHitstop(heavyHitstopDuration);
    }

    /// <summary>
    /// Trigger custom hitstop duration
    /// </summary>
    public void TriggerHitstop(float duration)
    {
        if (hitstopCoroutine != null)
        {
            StopCoroutine(hitstopCoroutine);
        }
        hitstopCoroutine = StartCoroutine(HitstopCoroutine(duration));
    }

    private IEnumerator HitstopCoroutine(float duration)
    {
        float originalTimeScale = Time.timeScale;
        Time.timeScale = hitstopTimeScale;

        yield return new WaitForSecondsRealtime(duration);

        Time.timeScale = originalTimeScale;
        hitstopCoroutine = null;
    }

    /// <summary>
    /// Combined hit feedback for standard attacks
    /// </summary>
    public void PlayerHitEnemy()
    {
        LightHitstop();
        if (CameraShake.Instance != null)
        {
            CameraShake.Instance.ShakeLight();
        }
    }

    /// <summary>
    /// Combined hit feedback when player takes damage
    /// </summary>
    public void EnemyHitPlayer()
    {
        MediumHitstop();
        if (CameraShake.Instance != null)
        {
            CameraShake.Instance.ShakeMedium();
        }
    }

    /// <summary>
    /// Combined feedback for boss death
    /// </summary>
    public void BossDeathFeedback()
    {
        HeavyHitstop();
        if (CameraShake.Instance != null)
        {
            CameraShake.Instance.ShakeHeavy();
        }
    }
}
