#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using System.Collections.Generic;

/// <summary>
/// One-click setup wizard for Mask of the Bronze God
/// Creates all necessary GameObjects, layers, tags, data assets, and configures the scene
/// Now supports multiple heroes, bosses, and themes
/// </summary>
public class GameSetupWizard : EditorWindow
{
    private int selectedHeroIndex = 0;
    private int selectedBossIndex = 0;
    private int selectedThemeIndex = 0;

    [MenuItem("Game Jam/Setup Wizard")]
    public static void ShowWindow()
    {
        GetWindow<GameSetupWizard>("Game Setup Wizard");
    }

    private void OnGUI()
    {
        GUILayout.Label("Mask of the Bronze God - Setup", EditorStyles.boldLabel);
        GUILayout.Space(10);

        // STEP 1: Layers & Tags
        GUILayout.Label("Step 1: Create Layers & Tags", EditorStyles.label);
        if (GUILayout.Button("Setup Layers & Tags"))
        {
            SetupLayersAndTags();
        }

        GUILayout.Space(10);

        // STEP 2: Data Assets (NEW)
        GUILayout.Label("Step 2: Create Data Assets", EditorStyles.label);
        EditorGUILayout.HelpBox(
            "Creates default Heroes, Bosses, Themes, and GameConfig.\n" +
            "You can modify these in Assets/Data/ after creation.",
            MessageType.Info);

        if (GUILayout.Button("Create Default Data Assets"))
        {
            CreateDefaultDataAssets();
        }

        GUILayout.Space(10);

        // STEP 3: Game Scene
        GUILayout.Label("Step 3: Setup Game Scene", EditorStyles.label);
        if (GUILayout.Button("Create Game Scene"))
        {
            CreateGameScene();
        }

        GUILayout.Space(10);

        // STEP 4: Main Menu
        GUILayout.Label("Step 4: Setup Main Menu", EditorStyles.label);
        if (GUILayout.Button("Create Main Menu Scene"))
        {
            CreateMainMenuScene();
        }

        GUILayout.Space(20);

        // Quick Actions
        GUILayout.Label("Quick Actions", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Create Player Prefab"))
        {
            CreatePlayerPrefab();
        }
        if (GUILayout.Button("Create Boss Prefab"))
        {
            CreateBossPrefab();
        }
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(10);

        if (GUILayout.Button("Create Additional Hero Data"))
        {
            CreateHeroDataAsset();
        }
        if (GUILayout.Button("Create Additional Boss Data"))
        {
            CreateBossDataAsset();
        }

        GUILayout.Space(20);

        EditorGUILayout.HelpBox(
            "After setup:\n" +
            "1. Modify Data Assets in Assets/Data/\n" +
            "2. Create/import sprites\n" +
            "3. Add scenes to Build Settings\n" +
            "4. Press Play to test!",
            MessageType.Info);
    }

    #region Layers and Tags

    private void SetupLayersAndTags()
    {
        // Create Tags
        CreateTag("Player");
        CreateTag("Boss");
        CreateTag("PlayerAttack");
        CreateTag("Parryable");

        // Create Layers
        CreateLayer(6, "Player");
        CreateLayer(7, "Enemy");
        CreateLayer(8, "PlayerAttack");
        CreateLayer(9, "EnemyProjectile");

        Debug.Log("Layers and Tags created successfully!");
        EditorUtility.DisplayDialog("Setup Complete", "Layers and Tags have been created.", "OK");
    }

    private void CreateTag(string tagName)
    {
        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty tagsProp = tagManager.FindProperty("tags");

        for (int i = 0; i < tagsProp.arraySize; i++)
        {
            if (tagsProp.GetArrayElementAtIndex(i).stringValue == tagName)
                return;
        }

        tagsProp.InsertArrayElementAtIndex(tagsProp.arraySize);
        tagsProp.GetArrayElementAtIndex(tagsProp.arraySize - 1).stringValue = tagName;
        tagManager.ApplyModifiedProperties();
    }

    private void CreateLayer(int layerIndex, string layerName)
    {
        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty layersProp = tagManager.FindProperty("layers");

        if (layersProp.GetArrayElementAtIndex(layerIndex).stringValue == "")
        {
            layersProp.GetArrayElementAtIndex(layerIndex).stringValue = layerName;
            tagManager.ApplyModifiedProperties();
        }
    }

    #endregion

    #region Data Assets

    private void CreateDefaultDataAssets()
    {
        EnsureDirectoryExists("Assets/Data");
        EnsureDirectoryExists("Assets/Data/Heroes");
        EnsureDirectoryExists("Assets/Data/Bosses");
        EnsureDirectoryExists("Assets/Data/Themes");
        EnsureDirectoryExists("Assets/Resources");

        // Create default heroes
        var hero1 = CreateHeroData("Bronze Warrior", new Color(0.8f, 0.5f, 0.2f), "Assets/Data/Heroes/Hero_BronzeWarrior.asset");
        var hero2 = CreateHeroData("Shadow Dancer", new Color(0.3f, 0.2f, 0.4f), "Assets/Data/Heroes/Hero_ShadowDancer.asset");
        var hero3 = CreateHeroData("Flame Bearer", new Color(0.9f, 0.3f, 0.1f), "Assets/Data/Heroes/Hero_FlameBearer.asset");

        // Create default bosses
        var boss1 = CreateBossData("The Bronze Mask", new Color(0.8f, 0.5f, 0.2f), "Assets/Data/Bosses/Boss_BronzeMask.asset");
        var boss2 = CreateBossData("Chaos Totem", new Color(0.5f, 0.2f, 0.3f), "Assets/Data/Bosses/Boss_ChaosTotem.asset");

        // Create default themes
        var theme1 = CreateThemeData("Bronze Era", new Color(0.24f, 0.16f, 0.08f), "Assets/Data/Themes/Theme_BronzeEra.asset");
        var theme2 = CreateThemeData("Chaos Realm", new Color(0.15f, 0.08f, 0.12f), "Assets/Data/Themes/Theme_ChaosRealm.asset");

        // Create GameConfig
        var config = ScriptableObject.CreateInstance<GameConfig>();
        config.selectedHero = hero1;
        config.selectedBoss = boss1;
        config.selectedTheme = theme1;
        config.availableHeroes = new List<HeroData> { hero1, hero2, hero3 };
        config.availableBosses = new List<BossData> { boss1, boss2 };
        config.availableThemes = new List<ThemeData> { theme1, theme2 };

        AssetDatabase.CreateAsset(config, "Assets/Resources/GameConfig.asset");

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("Default data assets created!");
        EditorUtility.DisplayDialog("Data Assets Created",
            "Created:\n" +
            "- 3 Heroes (Bronze Warrior, Shadow Dancer, Flame Bearer)\n" +
            "- 2 Bosses (Bronze Mask, Chaos Totem)\n" +
            "- 2 Themes (Bronze Era, Chaos Realm)\n" +
            "- 1 GameConfig\n\n" +
            "Find them in Assets/Data/",
            "OK");
    }

    private HeroData CreateHeroData(string name, Color primaryColor, string path)
    {
        if (AssetDatabase.LoadAssetAtPath<HeroData>(path) != null)
        {
            return AssetDatabase.LoadAssetAtPath<HeroData>(path);
        }

        var hero = ScriptableObject.CreateInstance<HeroData>();
        hero.heroName = name;
        hero.primaryColor = primaryColor;
        hero.secondaryColor = primaryColor * 0.7f;
        hero.attackColor = new Color(primaryColor.r + 0.2f, primaryColor.g + 0.2f, primaryColor.b, 1f);

        // Set varied stats based on hero type
        switch (name)
        {
            case "Shadow Dancer":
                hero.description = "Fast and evasive, with quick attacks.";
                hero.moveSpeed = 8f;
                hero.dodgeSpeed = 15f;
                hero.attackDamage = 20f;
                hero.maxHealth = 80f;
                hero.specialAbility = HeroAbilityType.QuickDash;
                break;
            case "Flame Bearer":
                hero.description = "Slow but powerful, deals heavy damage.";
                hero.moveSpeed = 5f;
                hero.dodgeSpeed = 10f;
                hero.attackDamage = 35f;
                hero.maxHealth = 120f;
                hero.specialAbility = HeroAbilityType.HeavyHitter;
                break;
            default: // Bronze Warrior
                hero.description = "A balanced fighter with quick attacks.";
                hero.moveSpeed = 6f;
                hero.dodgeSpeed = 12f;
                hero.attackDamage = 25f;
                hero.maxHealth = 100f;
                hero.specialAbility = HeroAbilityType.None;
                break;
        }

        AssetDatabase.CreateAsset(hero, path);
        return hero;
    }

    private BossData CreateBossData(string name, Color primaryColor, string path)
    {
        if (AssetDatabase.LoadAssetAtPath<BossData>(path) != null)
        {
            return AssetDatabase.LoadAssetAtPath<BossData>(path);
        }

        var boss = ScriptableObject.CreateInstance<BossData>();
        boss.bossName = name;
        boss.primaryColor = primaryColor;
        boss.secondaryColor = primaryColor * 0.6f;
        boss.attackColor = new Color(1f, 0.4f, 0.2f);
        boss.eyeColor = new Color(1f, 0.9f, 0.5f);

        // Phase 1 config
        var phase1 = new BossPhaseConfig
        {
            phaseName = "Phase 1",
            healthPercentStart = 1f,
            healthPercentEnd = 0.5f,
            attackCooldown = 1.5f,
            patternSpeedMultiplier = 1f,
            useSweepAttack = true,
            useSlamAttack = true,
            useBulletPattern = false,
            useLaserBeam = false,
            useMinionSpawn = false,
            useSpiritProjectiles = false
        };

        // Phase 2 config (harder)
        var phase2 = new BossPhaseConfig
        {
            phaseName = "Phase 2",
            healthPercentStart = 0.5f,
            healthPercentEnd = 0f,
            attackCooldown = 1.0f,
            patternSpeedMultiplier = 1.3f,
            phaseTint = new Color(0.9f, 0.3f, 0.3f),
            useSweepAttack = true,
            useSlamAttack = true,
            useBulletPattern = true,
            useLaserBeam = true,
            useMinionSpawn = true,
            useSpiritProjectiles = true
        };

        boss.phases = new List<BossPhaseConfig> { phase1, phase2 };

        if (name == "Chaos Totem")
        {
            boss.description = "A corrupted totem of chaos, unpredictable and deadly.";
            boss.maxHealth = 600f;
            boss.phase2TintColor = new Color(0.8f, 0.1f, 0.5f);
        }
        else
        {
            boss.description = "An ancient totemic guardian awakened by chaos.";
            boss.maxHealth = 500f;
        }

        AssetDatabase.CreateAsset(boss, path);
        return boss;
    }

    private ThemeData CreateThemeData(string name, Color backgroundColor, string path)
    {
        if (AssetDatabase.LoadAssetAtPath<ThemeData>(path) != null)
        {
            return AssetDatabase.LoadAssetAtPath<ThemeData>(path);
        }

        var theme = ScriptableObject.CreateInstance<ThemeData>();
        theme.themeName = name;
        theme.backgroundColor = backgroundColor;
        theme.arenaFloorColor = backgroundColor * 1.3f;
        theme.arenaWallColor = backgroundColor * 0.8f;

        if (name == "Chaos Realm")
        {
            theme.description = "Dark purple chaos dimension.";
            theme.uiPrimaryColor = new Color(0.6f, 0.2f, 0.4f);
            theme.uiAccentColor = new Color(1f, 0.3f, 0.6f);
            theme.parryableBulletColor = new Color(1f, 0.4f, 0.8f);
        }
        else // Bronze Era
        {
            theme.description = "Ancient bronze and earth tones.";
            theme.uiPrimaryColor = new Color(0.8f, 0.5f, 0.2f);
            theme.uiAccentColor = new Color(1f, 0.8f, 0.3f);
        }

        AssetDatabase.CreateAsset(theme, path);
        return theme;
    }

    private void CreateHeroDataAsset()
    {
        EnsureDirectoryExists("Assets/Data/Heroes");
        string path = EditorUtility.SaveFilePanelInProject(
            "Create Hero Data",
            "NewHero",
            "asset",
            "Choose location for new Hero Data",
            "Assets/Data/Heroes");

        if (!string.IsNullOrEmpty(path))
        {
            var hero = ScriptableObject.CreateInstance<HeroData>();
            hero.heroName = "New Hero";
            AssetDatabase.CreateAsset(hero, path);
            AssetDatabase.SaveAssets();
            Selection.activeObject = hero;
            EditorGUIUtility.PingObject(hero);
        }
    }

    private void CreateBossDataAsset()
    {
        EnsureDirectoryExists("Assets/Data/Bosses");
        string path = EditorUtility.SaveFilePanelInProject(
            "Create Boss Data",
            "NewBoss",
            "asset",
            "Choose location for new Boss Data",
            "Assets/Data/Bosses");

        if (!string.IsNullOrEmpty(path))
        {
            var boss = ScriptableObject.CreateInstance<BossData>();
            boss.bossName = "New Boss";
            AssetDatabase.CreateAsset(boss, path);
            AssetDatabase.SaveAssets();
            Selection.activeObject = boss;
            EditorGUIUtility.PingObject(boss);
        }
    }

    #endregion

    #region Scene Creation

    private void CreateGameScene()
    {
        Scene newScene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

        // Setup Camera
        Camera mainCam = Camera.main;
        if (mainCam != null)
        {
            mainCam.backgroundColor = new Color(0.24f, 0.16f, 0.08f);
            mainCam.orthographic = true;
            mainCam.orthographicSize = 5;
            mainCam.gameObject.AddComponent<CameraShake>();
        }

        // Create GameManager
        GameObject gameManager = new GameObject("GameManager");
        gameManager.AddComponent<GameManager>();
        gameManager.AddComponent<HitFeedback>();

        // Create Player
        GameObject player = CreatePlayerObject();

        // Create Boss
        GameObject boss = CreateBossObject();

        // Create Arena Floor
        CreateArenaFloor();

        // Create UI Canvas
        CreateUICanvas();

        // Save scene
        string scenePath = "Assets/Scenes/Game.unity";
        EnsureDirectoryExists("Assets/Scenes");
        EditorSceneManager.SaveScene(newScene, scenePath);

        Debug.Log("Game scene created at: " + scenePath);
        EditorUtility.DisplayDialog("Setup Complete", "Game scene has been created!", "OK");
    }

    private void CreateMainMenuScene()
    {
        Scene newScene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

        Camera mainCam = Camera.main;
        if (mainCam != null)
        {
            mainCam.backgroundColor = new Color(0.24f, 0.16f, 0.08f);
            mainCam.orthographic = true;
        }

        // Create Canvas
        GameObject canvas = new GameObject("Canvas");
        var canvasComp = canvas.AddComponent<Canvas>();
        canvasComp.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.AddComponent<UnityEngine.UI.CanvasScaler>();
        canvas.AddComponent<UnityEngine.UI.GraphicRaycaster>();
        canvas.AddComponent<MainMenuUI>();

        // Create EventSystem
        GameObject eventSystem = new GameObject("EventSystem");
        eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
        eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();

        // Save scene
        string scenePath = "Assets/Scenes/MainMenu.unity";
        EnsureDirectoryExists("Assets/Scenes");
        EditorSceneManager.SaveScene(newScene, scenePath);

        Debug.Log("Main Menu scene created at: " + scenePath);
    }

    #endregion

    #region Object Creation

    private GameObject CreatePlayerObject()
    {
        GameObject player = new GameObject("Player");
        player.tag = "Player";
        player.layer = LayerMask.NameToLayer("Player");

        // Add components
        var sr = player.AddComponent<SpriteRenderer>();
        sr.color = new Color(0.8f, 0.5f, 0.2f); // Bronze

        var rb = player.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0;

        var col = player.AddComponent<BoxCollider2D>();
        col.size = new Vector2(0.8f, 1.5f);

        player.AddComponent<PlayerController>();
        player.AddComponent<PlayerCombat>();
        player.AddComponent<PlayerHealth>();
        player.AddComponent<PlayerParry>();
        player.AddComponent<HeroInitializer>(); // NEW: For hero data

        // Create attack point child
        GameObject attackPoint = new GameObject("AttackPoint");
        attackPoint.transform.SetParent(player.transform);
        attackPoint.transform.localPosition = new Vector3(0.8f, 0, 0);

        // Create hurtbox child for receiving damage from boss attacks
        GameObject hurtbox = new GameObject("Hurtbox");
        hurtbox.transform.SetParent(player.transform);
        hurtbox.transform.localPosition = Vector3.zero;
        hurtbox.layer = LayerMask.NameToLayer("Player");
        var hurtboxCol = hurtbox.AddComponent<BoxCollider2D>();
        hurtboxCol.isTrigger = true;
        hurtboxCol.size = new Vector2(0.6f, 1.3f);
        var hurtboxComp = hurtbox.AddComponent<Hurtbox>();
        hurtboxComp.isPlayerOwned = true;

        player.transform.position = new Vector3(-3, 0, 0);

        return player;
    }

    private GameObject CreateBossObject()
    {
        GameObject boss = new GameObject("Boss");
        boss.tag = "Boss";
        boss.layer = LayerMask.NameToLayer("Enemy");

        var sr = boss.AddComponent<SpriteRenderer>();
        sr.color = new Color(0.8f, 0.5f, 0.2f);

        var rb = boss.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;

        var col = boss.AddComponent<BoxCollider2D>();
        col.size = new Vector2(2f, 2f);

        boss.AddComponent<BossControllerMultiPhase>();
        boss.AddComponent<BossHealth>();
        boss.AddComponent<BossInitializer>(); // NEW: For boss data

        // Add all attack patterns (BossInitializer will enable/disable based on BossData)
        boss.AddComponent<SweepAttack>();
        boss.AddComponent<SlamAttack>();
        boss.AddComponent<BulletCirclePattern>();
        boss.AddComponent<LaserBeamPattern>();
        boss.AddComponent<MinionSpawnPattern>();
        boss.AddComponent<SpiritProjectileAttack>();

        boss.transform.position = new Vector3(3, 1, 0);
        boss.transform.localScale = new Vector3(2, 2, 1);

        return boss;
    }

    private void CreateArenaFloor()
    {
        GameObject floor = new GameObject("ArenaFloor");
        var sr = floor.AddComponent<SpriteRenderer>();
        sr.color = new Color(0.3f, 0.2f, 0.1f);

        var col = floor.AddComponent<BoxCollider2D>();
        col.size = new Vector2(20, 1);

        floor.transform.position = new Vector3(0, -2, 0);
        floor.transform.localScale = new Vector3(20, 1, 1);
    }

    private void CreateUICanvas()
    {
        // Canvas
        GameObject canvas = new GameObject("UICanvas");
        var canvasComp = canvas.AddComponent<Canvas>();
        canvasComp.renderMode = RenderMode.ScreenSpaceOverlay;

        var scaler = canvas.AddComponent<UnityEngine.UI.CanvasScaler>();
        scaler.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        canvas.AddComponent<UnityEngine.UI.GraphicRaycaster>();

        // Add UI components
        canvas.AddComponent<PauseMenuUI>();
        canvas.AddComponent<GameOverUI>();

        // EventSystem
        GameObject eventSystem = new GameObject("EventSystem");
        eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
        eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();

        // Player Health Bar (bottom left)
        CreateHealthBar(canvas.transform, "PlayerHealthBar", new Vector2(200, -50), true);

        // Boss Health Bar (top center)
        CreateHealthBar(canvas.transform, "BossHealthBar", new Vector2(0, -40), false);
    }

    private void CreateHealthBar(Transform parent, string name, Vector2 position, bool isPlayer)
    {
        GameObject healthBar = new GameObject(name);
        healthBar.transform.SetParent(parent);

        var rect = healthBar.AddComponent<RectTransform>();
        rect.anchorMin = isPlayer ? new Vector2(0, 1) : new Vector2(0.5f, 1);
        rect.anchorMax = isPlayer ? new Vector2(0, 1) : new Vector2(0.5f, 1);
        rect.anchoredPosition = position;
        rect.sizeDelta = isPlayer ? new Vector2(300, 30) : new Vector2(600, 40);

        // Background
        var bgImage = healthBar.AddComponent<UnityEngine.UI.Image>();
        bgImage.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);

        // Fill
        GameObject fill = new GameObject("Fill");
        fill.transform.SetParent(healthBar.transform);
        var fillRect = fill.AddComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.sizeDelta = Vector2.zero;
        fillRect.offsetMin = Vector2.zero;
        fillRect.offsetMax = Vector2.zero;

        var fillImage = fill.AddComponent<UnityEngine.UI.Image>();
        fillImage.color = isPlayer ? new Color(0.2f, 0.8f, 0.3f) : new Color(0.8f, 0.5f, 0.2f);
        fillImage.type = UnityEngine.UI.Image.Type.Filled;
        fillImage.fillMethod = UnityEngine.UI.Image.FillMethod.Horizontal;

        // Add UI script
        if (isPlayer)
            healthBar.AddComponent<HealthBarUI>();
        else
            healthBar.AddComponent<BossHealthBarUI>();
    }

    private void CreatePlayerPrefab()
    {
        GameObject player = CreatePlayerObject();
        EnsureDirectoryExists("Assets/Prefabs");
        PrefabUtility.SaveAsPrefabAsset(player, "Assets/Prefabs/Player.prefab");
        DestroyImmediate(player);
        Debug.Log("Player prefab created!");
    }

    private void CreateBossPrefab()
    {
        GameObject boss = CreateBossObject();
        EnsureDirectoryExists("Assets/Prefabs");
        PrefabUtility.SaveAsPrefabAsset(boss, "Assets/Prefabs/Boss.prefab");
        DestroyImmediate(boss);
        Debug.Log("Boss prefab created!");
    }

    #endregion

    #region Utilities

    private void EnsureDirectoryExists(string path)
    {
        if (!AssetDatabase.IsValidFolder(path))
        {
            string[] folders = path.Split('/');
            string currentPath = folders[0];
            for (int i = 1; i < folders.Length; i++)
            {
                string newPath = currentPath + "/" + folders[i];
                if (!AssetDatabase.IsValidFolder(newPath))
                {
                    AssetDatabase.CreateFolder(currentPath, folders[i]);
                }
                currentPath = newPath;
            }
        }
    }

    #endregion
}
#endif
