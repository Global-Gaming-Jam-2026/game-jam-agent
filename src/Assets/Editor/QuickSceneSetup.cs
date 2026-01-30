using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

/// <summary>
/// Editor utility to quickly set up a game scene with all required objects.
/// Use from menu: Tools > Setup Game Scene
/// </summary>
public class QuickSceneSetup : EditorWindow
{
    [MenuItem("Tools/Setup Game Scene")]
    public static void SetupGameScene()
    {
        if (!EditorUtility.DisplayDialog("Setup Game Scene",
            "This will create a new scene with Player, Boss, Camera, and all managers.\n\nProceed?",
            "Yes, Create Scene", "Cancel"))
        {
            return;
        }

        // Create new scene
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        // Create Main Camera
        GameObject cameraObj = new GameObject("Main Camera");
        cameraObj.tag = "MainCamera";
        var camera = cameraObj.AddComponent<Camera>();
        camera.orthographic = true;
        camera.orthographicSize = 5;
        camera.backgroundColor = new Color(0.1f, 0.1f, 0.15f);
        camera.clearFlags = CameraClearFlags.SolidColor;
        cameraObj.transform.position = new Vector3(0, 0, -10);
        cameraObj.AddComponent<AudioListener>();
        cameraObj.AddComponent<CameraShake>();

        // Create Player
        GameObject playerObj = new GameObject("Player");
        playerObj.tag = "Player";
        playerObj.layer = LayerMask.NameToLayer("Default");
        playerObj.transform.position = new Vector3(-3, -1, 0);

        var playerSR = playerObj.AddComponent<SpriteRenderer>();
        playerSR.sortingOrder = 10;

        var playerRB = playerObj.AddComponent<Rigidbody2D>();
        playerRB.bodyType = RigidbodyType2D.Dynamic;
        playerRB.gravityScale = 0;
        playerRB.constraints = RigidbodyConstraints2D.FreezeRotation;

        var playerCollider = playerObj.AddComponent<BoxCollider2D>();
        playerCollider.size = new Vector2(1, 1.5f);

        playerObj.AddComponent<PlayerController>();
        playerObj.AddComponent<PlayerCombat>();
        playerObj.AddComponent<PlayerHealth>();
        playerObj.AddComponent<PlayerParry>();
        playerObj.AddComponent<HeroInitializer>();
        playerObj.AddComponent<SpriteAnimator>();

        // Create Attack Point for player
        GameObject attackPoint = new GameObject("AttackPoint");
        attackPoint.transform.SetParent(playerObj.transform);
        attackPoint.transform.localPosition = new Vector3(0.5f, 0, 0);

        // Create Boss
        GameObject bossObj = new GameObject("Boss");
        bossObj.tag = "Boss";
        bossObj.layer = LayerMask.NameToLayer("Default");
        bossObj.transform.position = new Vector3(3, 0, 0);
        bossObj.transform.localScale = new Vector3(2, 2, 1);

        var bossSR = bossObj.AddComponent<SpriteRenderer>();
        bossSR.sortingOrder = 5;

        var bossCollider = bossObj.AddComponent<BoxCollider2D>();
        bossCollider.size = new Vector2(1.5f, 2f);

        bossObj.AddComponent<BossHealth>();
        bossObj.AddComponent<BossControllerMultiPhase>();
        bossObj.AddComponent<BossInitializer>();
        bossObj.AddComponent<SpriteAnimator>();

        // Create Managers Container
        GameObject managers = new GameObject("---MANAGERS---");

        // GameManager
        GameObject gmObj = new GameObject("GameManager");
        gmObj.transform.SetParent(managers.transform);
        gmObj.AddComponent<GameManager>();

        // AudioManager
        GameObject amObj = new GameObject("AudioManager");
        amObj.transform.SetParent(managers.transform);
        amObj.AddComponent<AudioManager>();

        // HitFeedback
        GameObject hfObj = new GameObject("HitFeedback");
        hfObj.transform.SetParent(managers.transform);
        hfObj.AddComponent<HitFeedback>();

        // ParticleManager
        GameObject pmObj = new GameObject("ParticleManager");
        pmObj.transform.SetParent(managers.transform);
        pmObj.AddComponent<ParticleManager>();

        // DamageNumberUI
        GameObject dnObj = new GameObject("DamageNumberUI");
        dnObj.transform.SetParent(managers.transform);
        dnObj.AddComponent<DamageNumberUI>();

        // IntroSequence
        GameObject isObj = new GameObject("IntroSequence");
        isObj.transform.SetParent(managers.transform);
        isObj.AddComponent<IntroSequence>();

        // BossDialogue
        GameObject bdObj = new GameObject("BossDialogue");
        bdObj.transform.SetParent(managers.transform);
        bdObj.AddComponent<BossDialogue>();

        // Create Arena (simple ground)
        GameObject arena = new GameObject("---ARENA---");

        GameObject ground = new GameObject("Ground");
        ground.transform.SetParent(arena.transform);
        ground.transform.position = new Vector3(0, -2.5f, 0);
        ground.transform.localScale = new Vector3(20, 1, 1);
        var groundSR = ground.AddComponent<SpriteRenderer>();
        groundSR.color = new Color(0.3f, 0.25f, 0.2f);
        groundSR.sortingOrder = -10;
        var groundCollider = ground.AddComponent<BoxCollider2D>();
        groundCollider.size = new Vector2(1, 1);

        // Left wall
        GameObject leftWall = new GameObject("LeftWall");
        leftWall.transform.SetParent(arena.transform);
        leftWall.transform.position = new Vector3(-9, 0, 0);
        var leftWallCollider = leftWall.AddComponent<BoxCollider2D>();
        leftWallCollider.size = new Vector2(1, 10);

        // Right wall
        GameObject rightWall = new GameObject("RightWall");
        rightWall.transform.SetParent(arena.transform);
        rightWall.transform.position = new Vector3(9, 0, 0);
        var rightWallCollider = rightWall.AddComponent<BoxCollider2D>();
        rightWallCollider.size = new Vector2(1, 10);

        // Create UI Canvas
        GameObject canvasObj = new GameObject("GameCanvas");
        var canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;
        canvasObj.AddComponent<UnityEngine.UI.CanvasScaler>();
        canvasObj.AddComponent<UnityEngine.UI.GraphicRaycaster>();

        // Create Scenes folder if needed
        if (!AssetDatabase.IsValidFolder("Assets/Scenes"))
        {
            AssetDatabase.CreateFolder("Assets", "Scenes");
        }

        // Save the scene
        string scenePath = "Assets/Scenes/GameScene.unity";
        EditorSceneManager.SaveScene(scene, scenePath);
        AssetDatabase.Refresh();

        // Add to build settings
        var scenes = new System.Collections.Generic.List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
        bool sceneExists = false;
        foreach (var s in scenes)
        {
            if (s.path == scenePath) { sceneExists = true; break; }
        }
        if (!sceneExists)
        {
            scenes.Add(new EditorBuildSettingsScene(scenePath, true));
            EditorBuildSettings.scenes = scenes.ToArray();
        }

        Debug.Log("[QuickSceneSetup] Game scene created successfully at " + scenePath);
        EditorUtility.DisplayDialog("Scene Created",
            $"Game scene created at:\n{scenePath}\n\nPress Play to test the game!",
            "OK");
    }

    [MenuItem("Tools/Create Placeholder Sprites")]
    public static void CreatePlaceholderSprites()
    {
        // Initialize RuntimeAssetLoader to generate sprites
        Debug.Log("[QuickSceneSetup] Sprites will be generated at runtime by RuntimeAssetLoader");
        EditorUtility.DisplayDialog("Sprites",
            "Sprites are generated procedurally at runtime.\n\nJust press Play and the sprites will appear!",
            "OK");
    }
}
