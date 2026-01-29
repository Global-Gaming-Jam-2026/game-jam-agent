using UnityEngine;

/// <summary>
/// ScriptableObject containing all data for a hero character.
/// Create multiple HeroData assets for different playable characters.
/// </summary>
[CreateAssetMenu(fileName = "NewHero", menuName = "Game Jam/Hero Data")]
public class HeroData : ScriptableObject
{
    [Header("Identity")]
    public string heroName = "Bronze Warrior";
    [TextArea(2, 4)]
    public string description = "A balanced fighter with quick attacks.";
    public Sprite portrait;           // For selection screen
    public Sprite idleSprite;         // Default in-game sprite

    [Header("Colors")]
    public Color primaryColor = new Color(0.8f, 0.5f, 0.2f);     // Main body color
    public Color secondaryColor = new Color(0.6f, 0.4f, 0.15f);  // Accent/trim
    public Color attackColor = new Color(1f, 0.8f, 0.3f);        // Attack trail/effect
    public Color damageFlashColor = Color.white;                  // Flash when hit

    [Header("Base Stats")]
    [Range(50, 200)] public float maxHealth = 100f;
    [Range(3, 12)] public float moveSpeed = 6f;
    [Range(8, 20)] public float dodgeSpeed = 12f;
    [Range(10, 50)] public float attackDamage = 25f;

    [Header("Combat Timing")]
    [Range(0.2f, 0.8f)] public float dodgeDuration = 0.4f;
    [Range(0.3f, 1f)] public float dodgeCooldown = 0.6f;
    [Range(0.1f, 0.5f)] public float iFrameDuration = 0.2f;
    [Range(0.5f, 1.5f)] public float comboWindow = 0.8f;         // Time to continue combo

    [Header("Special Ability")]
    public HeroAbilityType specialAbility = HeroAbilityType.None;
    [Range(0.5f, 2f)] public float abilityMultiplier = 1f;       // Strength of special

    [Header("Audio")]
    public AudioClip attackSound1;
    public AudioClip attackSound2;
    public AudioClip attackSound3;
    public AudioClip dodgeSound;
    public AudioClip hurtSound;
    public AudioClip deathSound;

    [Header("Animation")]
    public RuntimeAnimatorController animatorController;
    public float spriteScale = 1f;

    /// <summary>
    /// Get the attack sound for a combo hit
    /// </summary>
    public AudioClip GetAttackSound(int comboIndex)
    {
        return comboIndex switch
        {
            0 => attackSound1,
            1 => attackSound2,
            2 => attackSound3,
            _ => attackSound1
        };
    }
}

/// <summary>
/// Types of special abilities heroes can have
/// </summary>
public enum HeroAbilityType
{
    None,               // No special ability
    DoubleJump,         // Can jump/dodge twice in air
    QuickDash,          // Faster, shorter dodge
    HeavyHitter,        // More damage, slower attacks
    ParryMaster,        // Larger parry window
    Berserker,          // Damage increases at low HP
    Lifesteal,          // Heal on hit
    CounterAttack       // Auto-counter after successful parry
}
