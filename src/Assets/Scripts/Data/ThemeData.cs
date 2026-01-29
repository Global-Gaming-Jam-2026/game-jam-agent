using UnityEngine;

/// <summary>
/// ScriptableObject containing color palette and visual theme data.
/// Create multiple ThemeData assets for different visual styles.
/// </summary>
[CreateAssetMenu(fileName = "NewTheme", menuName = "Game Jam/Theme Data")]
public class ThemeData : ScriptableObject
{
    [Header("Theme Identity")]
    public string themeName = "Bronze Era";
    [TextArea(2, 4)]
    public string description = "Ancient bronze and earth tones.";

    [Header("Background Colors")]
    public Color backgroundColor = new Color(0.24f, 0.16f, 0.08f);  // Camera clear color
    public Color arenaFloorColor = new Color(0.3f, 0.2f, 0.1f);     // Ground
    public Color arenaWallColor = new Color(0.2f, 0.13f, 0.06f);    // Walls/borders

    [Header("UI Colors")]
    public Color uiPrimaryColor = new Color(0.8f, 0.5f, 0.2f);      // Buttons, highlights
    public Color uiSecondaryColor = new Color(0.4f, 0.25f, 0.1f);   // Backgrounds
    public Color uiTextColor = new Color(1f, 0.95f, 0.85f);         // Text
    public Color uiAccentColor = new Color(1f, 0.8f, 0.3f);         // Important highlights

    [Header("Health Bar Colors")]
    public Color playerHealthColor = new Color(0.2f, 0.8f, 0.3f);       // Player HP
    public Color playerHealthLowColor = new Color(0.9f, 0.2f, 0.2f);    // Low HP warning
    public Color playerHealthDamageColor = new Color(0.8f, 0.2f, 0.2f); // Damage lag
    public Color bossHealthColor = new Color(0.8f, 0.5f, 0.2f);         // Boss HP
    public Color bossHealthPhase2Color = new Color(0.9f, 0.2f, 0.3f);   // Boss phase 2

    [Header("Combat Effect Colors")]
    public Color hitFlashColor = Color.white;
    public Color parryEffectColor = new Color(1f, 0.5f, 0.8f);      // Pink parry flash
    public Color dodgeTrailColor = new Color(0.8f, 0.8f, 1f, 0.5f); // Dodge afterimage

    [Header("Projectile Colors")]
    public Color normalBulletColor = new Color(0.9f, 0.7f, 0.3f);   // Standard bullets
    public Color parryableBulletColor = new Color(1f, 0.5f, 0.7f);  // Pink parryable
    public Color dangerBulletColor = new Color(1f, 0.2f, 0.2f);     // Can't parry

    [Header("Super Meter")]
    public Color superMeterFillColor = new Color(1f, 0.9f, 0.4f);
    public Color superMeterReadyColor = new Color(1f, 1f, 0.8f);    // Glowing when full

    [Header("Atmosphere")]
    public Color ambientParticleColor = new Color(0.8f, 0.6f, 0.3f, 0.3f);
    public bool useVignette = true;
    public float vignetteIntensity = 0.3f;

    /// <summary>
    /// Apply this theme to the camera
    /// </summary>
    public void ApplyToCamera(Camera cam)
    {
        if (cam != null)
        {
            cam.backgroundColor = backgroundColor;
        }
    }

    /// <summary>
    /// Get interpolated color for boss health based on phase
    /// </summary>
    public Color GetBossHealthColor(float healthPercent)
    {
        // Transition to phase 2 color below 50%
        if (healthPercent < 0.5f)
        {
            float t = 1f - (healthPercent / 0.5f);
            return Color.Lerp(bossHealthColor, bossHealthPhase2Color, t);
        }
        return bossHealthColor;
    }
}
