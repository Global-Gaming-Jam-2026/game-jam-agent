using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Initializes the boss with BossData configuration.
/// Attach this to the Boss GameObject alongside other boss components.
/// Reads from GameConfig.selectedBoss and applies stats/colors.
/// </summary>
public class BossInitializer : MonoBehaviour
{
    [Header("Manual Override (optional)")]
    [Tooltip("If set, uses this instead of GameConfig")]
    [SerializeField] private BossData bossOverride;

    [Header("Component References")]
    [SerializeField] private BossControllerMultiPhase bossController;
    [SerializeField] private BossHealth bossHealth;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private BossData currentBoss;

    private void Awake()
    {
        // Auto-find components if not assigned
        if (bossController == null) bossController = GetComponent<BossControllerMultiPhase>();
        if (bossHealth == null) bossHealth = GetComponent<BossHealth>();
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        // Get boss data (override or from config)
        currentBoss = bossOverride;
        if (currentBoss == null && GameConfig.Instance != null)
        {
            currentBoss = GameConfig.Instance.selectedBoss;
        }

        // Fallback to RuntimeAssetLoader if no data found
        if (currentBoss == null)
        {
            currentBoss = RuntimeAssetLoader.GetDefaultBoss();
            Debug.Log("[BossInitializer] Using RuntimeAssetLoader default boss");
        }

        if (currentBoss != null)
        {
            ApplyBossData();
        }
        else
        {
            Debug.LogWarning("No BossData found - applying fallback visuals");
            ApplyFallbackVisuals();
        }
    }

    private void ApplyFallbackVisuals()
    {
        // Apply runtime-generated sprite even without BossData
        if (spriteRenderer != null)
        {
            var sprite = RuntimeAssetLoader.GetBossSprite("BronzeMask");
            if (sprite != null)
            {
                spriteRenderer.sprite = sprite;
                Debug.Log("[BossInitializer] Applied fallback sprite from RuntimeAssetLoader");
            }
            spriteRenderer.color = new Color(0.8f, 0.5f, 0.2f); // Bronze color
        }
        transform.localScale = new Vector3(2, 2, 1);
        transform.position = new Vector3(3, 1, 0);
    }

    /// <summary>
    /// Apply all boss data to boss components
    /// </summary>
    public void ApplyBossData()
    {
        if (currentBoss == null) return;

        // Apply visuals
        ApplyVisuals();

        // Apply stats
        ApplyStats();

        // Setup phases
        SetupPhases();

        Debug.Log($"Boss initialized: {currentBoss.bossName}");
    }

    private void ApplyVisuals()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = currentBoss.primaryColor;

            // Apply first phase sprite if available
            if (currentBoss.phases.Count > 0 && currentBoss.phases[0].phaseSprite != null)
            {
                spriteRenderer.sprite = currentBoss.phases[0].phaseSprite;
            }
            else
            {
                // Fallback to runtime-generated sprite
                var fallbackSprite = RuntimeAssetLoader.GetBossSprite(currentBoss.bossName);
                if (fallbackSprite != null)
                {
                    spriteRenderer.sprite = fallbackSprite;
                    Debug.Log($"[BossInitializer] Using runtime sprite for {currentBoss.bossName}");
                }
            }
        }

        // Apply scale and position
        transform.localScale = new Vector3(currentBoss.bossScale.x, currentBoss.bossScale.y, 1);
        transform.position = new Vector3(currentBoss.arenaPosition.x, currentBoss.arenaPosition.y, 0);
    }

    private void ApplyStats()
    {
        if (bossHealth != null)
        {
            bossHealth.SetMaxHealth(currentBoss.maxHealth);
            bossHealth.SetStaggerThreshold(currentBoss.staggerThreshold);
        }
    }

    private void SetupPhases()
    {
        if (bossController == null || currentBoss.phases.Count == 0) return;

        // Convert BossPhaseConfig to BossPhaseData
        List<BossPhaseData> phases = new List<BossPhaseData>();

        foreach (var config in currentBoss.phases)
        {
            var phaseData = new BossPhaseData
            {
                phaseName = config.phaseName,
                healthPercentStart = config.healthPercentStart,
                healthPercentEnd = config.healthPercentEnd,
                formSprite = config.phaseSprite,
                formScale = config.scaleMultiplier,
                attackCooldown = config.attackCooldown,
                patternSpeedMultiplier = config.patternSpeedMultiplier,
                transitionDuration = config.transitionDuration,
                transformSound = config.transitionSound,
                phaseMusic = config.phaseMusic,
                patterns = GetPatternsForPhase(config)
            };

            phases.Add(phaseData);
        }

        // Apply to controller (requires adding a method to BossControllerMultiPhase)
        bossController.SetPhases(phases);
    }

    private List<BossAttackPattern> GetPatternsForPhase(BossPhaseConfig config)
    {
        var patterns = new List<BossAttackPattern>();

        // Get all attack pattern components on this object
        if (config.useSweepAttack)
        {
            var sweep = GetComponent<SweepAttack>();
            if (sweep != null) patterns.Add(sweep);
        }

        if (config.useSlamAttack)
        {
            var slam = GetComponent<SlamAttack>();
            if (slam != null) patterns.Add(slam);
        }

        if (config.useBulletPattern)
        {
            var bullet = GetComponent<BulletCirclePattern>();
            if (bullet != null) patterns.Add(bullet);
        }

        if (config.useLaserBeam)
        {
            var laser = GetComponent<LaserBeamPattern>();
            if (laser != null) patterns.Add(laser);
        }

        if (config.useMinionSpawn)
        {
            var minion = GetComponent<MinionSpawnPattern>();
            if (minion != null) patterns.Add(minion);
        }

        if (config.useSpiritProjectiles)
        {
            var spirit = GetComponent<SpiritProjectileAttack>();
            if (spirit != null) patterns.Add(spirit);
        }

        return patterns;
    }

    /// <summary>
    /// Get the current boss data
    /// </summary>
    public BossData GetBossData()
    {
        return currentBoss;
    }

    /// <summary>
    /// Get boss color for external use
    /// </summary>
    public Color GetPrimaryColor()
    {
        return currentBoss != null ? currentBoss.primaryColor : Color.white;
    }

    public Color GetAttackColor()
    {
        return currentBoss != null ? currentBoss.attackColor : Color.red;
    }
}
