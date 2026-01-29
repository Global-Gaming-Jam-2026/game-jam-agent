using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// ScriptableObject containing all data for a boss enemy.
/// Create multiple BossData assets for different bosses.
/// </summary>
[CreateAssetMenu(fileName = "NewBoss", menuName = "Game Jam/Boss Data")]
public class BossData : ScriptableObject
{
    [Header("Identity")]
    public string bossName = "The Bronze Mask";
    [TextArea(2, 4)]
    public string description = "An ancient totemic guardian awakened by chaos.";
    public Sprite portrait;            // For intro/UI

    [Header("Colors")]
    public Color primaryColor = new Color(0.8f, 0.5f, 0.2f);      // Main body
    public Color secondaryColor = new Color(0.5f, 0.3f, 0.1f);    // Darker accents
    public Color eyeColor = new Color(1f, 0.9f, 0.5f);            // Glowing eyes
    public Color attackColor = new Color(1f, 0.4f, 0.2f);         // Attack effects
    public Color phase2TintColor = new Color(0.9f, 0.2f, 0.3f);   // Color shift in phase 2

    [Header("Base Stats")]
    [Range(200, 2000)] public float maxHealth = 500f;
    [Range(50, 200)] public float staggerThreshold = 100f;        // Damage to stagger
    [Range(1f, 5f)] public float staggerDuration = 2f;            // Stunned time

    [Header("Phases")]
    public List<BossPhaseConfig> phases = new List<BossPhaseConfig>();

    [Header("Positioning")]
    public Vector2 arenaPosition = new Vector3(3, 1);             // Where boss stands
    public Vector2 bossScale = new Vector2(2, 2);                 // Size multiplier

    [Header("Audio")]
    public AudioClip introSound;       // When boss appears
    public AudioClip hurtSound;        // When damaged
    public AudioClip staggerSound;     // When staggered
    public AudioClip deathSound;       // Final death

    [Header("Visual Effects")]
    public bool hasIdleAnimation = true;
    public bool eyesGlowOnAttack = true;
    public float damageFlashDuration = 0.1f;
}

/// <summary>
/// Configuration for a single boss phase (simplified version of BossPhaseData for ScriptableObject)
/// </summary>
[System.Serializable]
public class BossPhaseConfig
{
    [Header("Phase Trigger")]
    public string phaseName = "Phase 1";
    [Range(0f, 1f)] public float healthPercentStart = 1f;
    [Range(0f, 1f)] public float healthPercentEnd = 0.5f;

    [Header("Visuals")]
    public Sprite phaseSprite;
    public Color phaseTint = Color.white;                  // Color tint for this phase
    public Vector2 scaleMultiplier = Vector2.one;          // Size change

    [Header("Behavior")]
    [Range(0.5f, 3f)] public float attackCooldown = 1.5f;
    [Range(0.5f, 2f)] public float patternSpeedMultiplier = 1f;
    public bool useAggresivePatterns = false;              // Enable harder patterns

    [Header("Available Attacks")]
    public bool useSweepAttack = true;
    public bool useSlamAttack = true;
    public bool useBulletPattern = false;
    public bool useLaserBeam = false;
    public bool useMinionSpawn = false;
    public bool useSpiritProjectiles = false;

    [Header("Transition")]
    [Range(0.5f, 3f)] public float transitionDuration = 1.5f;
    public AudioClip transitionSound;
    public AudioClip phaseMusic;
}
