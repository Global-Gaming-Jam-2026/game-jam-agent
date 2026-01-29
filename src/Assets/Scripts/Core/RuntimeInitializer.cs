using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Ensures essential systems are initialized when the game runs.
/// Creates missing managers and validates game state.
/// </summary>
public class RuntimeInitializer
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        Debug.Log("[RuntimeInitializer] Initializing game systems...");

        // Ensure AudioManager exists
        if (AudioManager.Instance == null)
        {
            CreateAudioManager();
        }

        // Validate GameConfig
        ValidateGameConfig();

        Debug.Log("[RuntimeInitializer] Initialization complete.");
    }

    private static void CreateAudioManager()
    {
        GameObject audioManagerObj = new GameObject("AudioManager");
        audioManagerObj.AddComponent<AudioManager>();
        Object.DontDestroyOnLoad(audioManagerObj);
        Debug.Log("[RuntimeInitializer] Created AudioManager.");
    }

    private static void ValidateGameConfig()
    {
        var config = GameConfig.Instance;
        if (config == null)
        {
            Debug.LogWarning("[RuntimeInitializer] GameConfig not found. Using default values.");
            return;
        }

        if (config.selectedHero == null && config.availableHeroes.Count > 0)
        {
            config.selectedHero = config.availableHeroes[0];
            Debug.Log($"[RuntimeInitializer] Auto-selected hero: {config.selectedHero.heroName}");
        }

        if (config.selectedBoss == null && config.availableBosses.Count > 0)
        {
            config.selectedBoss = config.availableBosses[0];
            Debug.Log($"[RuntimeInitializer] Auto-selected boss: {config.selectedBoss.bossName}");
        }
    }
}
