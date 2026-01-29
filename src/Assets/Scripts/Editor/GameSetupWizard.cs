#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

/// <summary>
/// One-click setup wizard for Mask of the Bronze God
/// Creates all necessary GameObjects, layers, tags, and configures the scene
/// </summary>
public class GameSetupWizard : EditorWindow
{
    [MenuItem("Game Jam/Setup Wizard")]
    public static void ShowWindow()
    {
        GetWindow<GameSetupWizard>("Game Setup Wizard");
    }

    private void OnGUI()
    {
        GUILayout.Label("Mask of the Bronze God - Setup", EditorStyles.boldLabel);
        GUILayout.Space(10);

        GUILayout.Label("Step 1: Create Layers & Tags", EditorStyles.label);
        if (GUILayout.Button("Setup Layers & Tags"))
        {
            SetupLayersAndTags();
        }

        GUILayout.Space(10);

        GUILayout.Label("Step 2: Setup Game Scene", EditorStyles.label);
        if (GUILayout.Button("Create Game Scene"))
        {
            CreateGameScene();
        }

        GUILayout.Space(10);

        GUILayout.Label("Step 3: Setup Main Menu", EditorStyles.label);
        if (GUILayout.Button("Create Main Menu Scene"))
        {
            CreateMainMenuScene();
        }

        GUILayout.Space(20);

        GUILayout.Label("Quick Actions", EditorStyles.boldLabel);
        if (GUILayout.Button("Create Player Prefab"))
        {
            CreatePlayerPrefab();
        }
        if (GUILayout.Button("Create Boss Prefab"))
        {
            CreateBossPrefab();
        }

        GUILayout.Space(20);

        EditorGUILayout.HelpBox(
            "After setup:\n" +
            "1. Create/import sprites\n" +
            "2. Assign sprites to Player and Boss\n" +
            "3. Add scenes to Build Settings\n" +
            "4. Press Play to test!",
            MessageType.Info);
    }

    private void SetupLayersAndTags()
    {
        // Create Tags
        CreateTag("Player");
        CreateTag("Boss");
        CreateTag("PlayerAttack");

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

        // Check if tag exists
        for (int i = 0; i < tagsProp.arraySize; i++)
        {
            if (tagsProp.GetArrayElementAtIndex(i).stringValue == tagName)
                return;
        }

        // Add new tag
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

    private void CreateGameScene()
    {
        // Create new scene
        Scene newScene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

        // Setup Camera
        Camera mainCam = Camera.main;
        if (mainCam != null)
        {
            mainCam.backgroundColor = new Color(0.24f, 0.16f, 0.08f); // Dark Bronze #3D2914
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

        // Create attack point child
        GameObject attackPoint = new GameObject("AttackPoint");
        attackPoint.transform.SetParent(player.transform);
        attackPoint.transform.localPosition = new Vector3(0.8f, 0, 0);

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

        // Add attack patterns
        boss.AddComponent<SweepAttack>();
        boss.AddComponent<SlamAttack>();
        boss.AddComponent<BulletCirclePattern>();

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
}
#endif
