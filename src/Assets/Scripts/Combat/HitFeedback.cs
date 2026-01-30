using UnityEngine;
using System.Collections;

/// <summary>
/// Handles hit feedback effects: hitstop, screen shake, flash, particles, damage numbers
/// Central hub for all combat juice and game feel.
/// </summary>
public class HitFeedback : MonoBehaviour
{
    public static HitFeedback Instance { get; private set; }

    [Header("Hitstop Settings")]
    [SerializeField] private float lightHitstopDuration = 0.03f;
    [SerializeField] private float mediumHitstopDuration = 0.05f;
    [SerializeField] private float heavyHitstopDuration = 0.08f;
    [SerializeField] private float hitstopTimeScale = 0.05f;

    [Header("Particle Colors")]
    [SerializeField] private Color playerHitColor = new Color(1f, 0.8f, 0.3f);
    [SerializeField] private Color playerDamageColor = new Color(1f, 0.3f, 0.2f);
    [SerializeField] private Color parryColor = new Color(1f, 0.8f, 0.9f);
    [SerializeField] private Color criticalColor = Color.yellow;

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
        PlayerHitEnemy(Vector3.zero, 0);
    }

    /// <summary>
    /// Combined hit feedback for standard attacks with position and damage
    /// </summary>
    public void PlayerHitEnemy(Vector3 hitPosition, float damage, bool isCritical = false)
    {
        // Hitstop
        if (isCritical)
            MediumHitstop();
        else
            LightHitstop();

        // Screen shake
        if (CameraShake.Instance != null)
        {
            if (isCritical)
                CameraShake.Instance.ShakeMedium();
            else
                CameraShake.Instance.ShakeLight();
        }

        // Particles
        if (ParticleManager.Instance != null && hitPosition != Vector3.zero)
        {
            Color color = isCritical ? criticalColor : playerHitColor;
            ParticleManager.Instance.SpawnHitSparks(hitPosition, color, isCritical ? 12 : 8);

            if (isCritical)
            {
                ParticleManager.Instance.SpawnImpactBurst(hitPosition, criticalColor, 1.2f);
            }
        }

        // Damage numbers
        if (DamageNumberUI.Instance != null && damage > 0 && hitPosition != Vector3.zero)
        {
            DamageNumberUI.Instance.ShowDamage(hitPosition, damage, isCritical);
        }
    }

    /// <summary>
    /// Combined hit feedback when player takes damage
    /// </summary>
    public void EnemyHitPlayer()
    {
        EnemyHitPlayer(Vector3.zero, 0);
    }

    /// <summary>
    /// Combined hit feedback when player takes damage with position and damage
    /// </summary>
    public void EnemyHitPlayer(Vector3 hitPosition, float damage)
    {
        MediumHitstop();

        if (CameraShake.Instance != null)
        {
            CameraShake.Instance.ShakeMedium();
        }

        // Particles
        if (ParticleManager.Instance != null && hitPosition != Vector3.zero)
        {
            ParticleManager.Instance.SpawnHitSparks(hitPosition, playerDamageColor, 10);
            ParticleManager.Instance.SpawnImpactBurst(hitPosition, playerDamageColor, 0.8f);
        }

        // Damage numbers
        if (DamageNumberUI.Instance != null && damage > 0 && hitPosition != Vector3.zero)
        {
            DamageNumberUI.Instance.ShowPlayerDamage(hitPosition, damage);
        }
    }

    /// <summary>
    /// Feedback for successful parry
    /// </summary>
    public void ParrySuccess(Vector3 position)
    {
        MediumHitstop();

        if (CameraShake.Instance != null)
        {
            CameraShake.Instance.ShakeMedium();
        }

        // Particles
        if (ParticleManager.Instance != null)
        {
            ParticleManager.Instance.SpawnSparkles(position, parryColor, 16);
            ParticleManager.Instance.SpawnImpactBurst(position, parryColor, 1.5f);
        }

        // UI
        if (DamageNumberUI.Instance != null)
        {
            DamageNumberUI.Instance.ShowParry(position);
        }
    }

    /// <summary>
    /// Feedback for successful dodge
    /// </summary>
    public void DodgeSuccess(Vector3 position, Sprite characterSprite = null)
    {
        // Trail ghost effect
        if (ParticleManager.Instance != null)
        {
            ParticleManager.Instance.SpawnDustPuff(position + Vector3.down * 0.5f, Color.gray, 5);
            if (characterSprite != null)
            {
                ParticleManager.Instance.SpawnTrailGhost(position, characterSprite, new Color(0.7f, 0.9f, 1f));
            }
        }

        // UI
        if (DamageNumberUI.Instance != null)
        {
            DamageNumberUI.Instance.ShowDodge(position);
        }
    }

    /// <summary>
    /// Combined feedback for boss death
    /// </summary>
    public void BossDeathFeedback()
    {
        BossDeathFeedback(Vector3.zero);
    }

    /// <summary>
    /// Combined feedback for boss death with position
    /// </summary>
    public void BossDeathFeedback(Vector3 position)
    {
        HeavyHitstop();

        if (CameraShake.Instance != null)
        {
            CameraShake.Instance.ShakeHeavy();
        }

        // Big death explosion
        if (ParticleManager.Instance != null && position != Vector3.zero)
        {
            ParticleManager.Instance.SpawnDeathExplosion(position, playerHitColor, criticalColor);
        }
    }

    /// <summary>
    /// Feedback for boss phase transition
    /// </summary>
    public void BossPhaseTransition(Vector3 position, int phaseNumber)
    {
        HeavyHitstop();

        if (CameraShake.Instance != null)
        {
            CameraShake.Instance.ShakeHeavy();
        }

        // Particles
        if (ParticleManager.Instance != null)
        {
            ParticleManager.Instance.SpawnSparkles(position, Color.red, 20);
            ParticleManager.Instance.SpawnImpactBurst(position, Color.red, 2f);
        }

        // UI
        if (DamageNumberUI.Instance != null)
        {
            DamageNumberUI.Instance.ShowPhaseChange(position, phaseNumber);
        }
    }

    /// <summary>
    /// Feedback for combo finisher
    /// </summary>
    public void ComboFinisher(Vector3 position, int comboCount)
    {
        MediumHitstop();

        if (CameraShake.Instance != null)
        {
            CameraShake.Instance.ShakeMedium();
        }

        // Extra particles for combo finisher
        if (ParticleManager.Instance != null)
        {
            ParticleManager.Instance.SpawnSparkles(position, criticalColor, 15);
        }

        // UI
        if (DamageNumberUI.Instance != null)
        {
            DamageNumberUI.Instance.ShowCombo(position, comboCount);
        }
    }
}
