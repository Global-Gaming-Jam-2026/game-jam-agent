using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Central configuration for the current game session.
/// Holds references to selected hero, boss, and theme.
/// Create ONE instance and use it across all scenes.
/// </summary>
[CreateAssetMenu(fileName = "GameConfig", menuName = "Game Jam/Game Config")]
public class GameConfig : ScriptableObject
{
    [Header("Current Selections")]
    [Tooltip("The hero the player is using")]
    public HeroData selectedHero;

    [Tooltip("The boss for this fight")]
    public BossData selectedBoss;

    [Tooltip("Visual theme/color palette")]
    public ThemeData selectedTheme;

    [Header("Available Options")]
    [Tooltip("All heroes available in the game")]
    public List<HeroData> availableHeroes = new List<HeroData>();

    [Tooltip("All bosses available in the game")]
    public List<BossData> availableBosses = new List<BossData>();

    [Tooltip("All themes available in the game")]
    public List<ThemeData> availableThemes = new List<ThemeData>();

    [Header("Game Settings")]
    [Range(0.5f, 2f)] public float difficultyMultiplier = 1f;
    public bool tutorialEnabled = true;
    public bool screenShakeEnabled = true;

    // Runtime singleton access
    private static GameConfig _instance;
    public static GameConfig Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Resources.Load<GameConfig>("GameConfig");
                if (_instance == null)
                {
                    Debug.LogWarning("GameConfig not found in Resources. Using defaults.");
                    _instance = CreateInstance<GameConfig>();
                }
            }
            return _instance;
        }
    }

    /// <summary>
    /// Select a hero by index
    /// </summary>
    public void SelectHero(int index)
    {
        if (index >= 0 && index < availableHeroes.Count)
        {
            selectedHero = availableHeroes[index];
        }
    }

    /// <summary>
    /// Select a boss by index
    /// </summary>
    public void SelectBoss(int index)
    {
        if (index >= 0 && index < availableBosses.Count)
        {
            selectedBoss = availableBosses[index];
        }
    }

    /// <summary>
    /// Select a theme by index
    /// </summary>
    public void SelectTheme(int index)
    {
        if (index >= 0 && index < availableThemes.Count)
        {
            selectedTheme = availableThemes[index];
        }
    }

    /// <summary>
    /// Get hero by name
    /// </summary>
    public HeroData GetHeroByName(string name)
    {
        return availableHeroes.Find(h => h.heroName == name);
    }

    /// <summary>
    /// Get boss by name
    /// </summary>
    public BossData GetBossByName(string name)
    {
        return availableBosses.Find(b => b.bossName == name);
    }
}
