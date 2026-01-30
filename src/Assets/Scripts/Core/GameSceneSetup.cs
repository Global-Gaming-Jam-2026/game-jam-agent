using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Auto-creates all required game objects and managers.
/// Makes the game work even with completely empty scenes.
/// </summary>
public class GameSceneSetup : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void AutoSetup()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        Debug.Log($"[GameSceneSetup] Setting up scene: {sceneName}");

        // Skip MainMenu - it has its own setup
        if (sceneName == "MainMenu") return;

        // Create Camera if missing
        if (Camera.main == null)
        {
            CreateCamera();
        }

        // Create Player if missing (Game scene only)
        if (sceneName == "Game" && GameObject.FindWithTag("Player") == null)
        {
            CreatePlayer();
        }

        // Create Boss if missing (Game scene only)
        if (sceneName == "Game" && GameObject.FindWithTag("Boss") == null)
        {
            CreateBoss();
        }

        // Create Arena if in Game scene
        if (sceneName == "Game")
        {
            CreateArena();
        }

        // Create all managers
        CreateManagers();

        Debug.Log("[GameSceneSetup] Scene setup complete");
    }

    private static void CreateCamera()
    {
        GameObject camObj = new GameObject("Main Camera");
        camObj.tag = "MainCamera";

        var cam = camObj.AddComponent<Camera>();
        cam.orthographic = true;
        cam.orthographicSize = 6f;  // Wider view for more arena visibility
        cam.backgroundColor = new Color(0.239f, 0.161f, 0.078f); // Dark Bronze #3D2914 from GAME_DESIGN.md
        cam.clearFlags = CameraClearFlags.SolidColor;
        camObj.transform.position = new Vector3(0, 0, -10);

        camObj.AddComponent<AudioListener>();
        camObj.AddComponent<CameraShake>();

        Debug.Log("[GameSceneSetup] Created Main Camera");
    }

    private static void CreatePlayer()
    {
        GameObject playerObj = new GameObject("Player");
        playerObj.tag = "Player";
        playerObj.layer = LayerMask.NameToLayer("Default");
        playerObj.transform.position = new Vector3(-3, -1, 0);

        var sr = playerObj.AddComponent<SpriteRenderer>();
        sr.sortingOrder = 10;

        var rb = playerObj.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 0;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        var col = playerObj.AddComponent<BoxCollider2D>();
        col.size = new Vector2(1f, 1.5f);

        playerObj.AddComponent<PlayerController>();
        playerObj.AddComponent<PlayerCombat>();
        playerObj.AddComponent<PlayerHealth>();
        playerObj.AddComponent<PlayerParry>();
        playerObj.AddComponent<HeroInitializer>();
        playerObj.AddComponent<SpriteAnimator>();

        // Attack point
        GameObject attackPoint = new GameObject("AttackPoint");
        attackPoint.transform.SetParent(playerObj.transform);
        attackPoint.transform.localPosition = new Vector3(0.8f, 0, 0);

        Debug.Log("[GameSceneSetup] Created Player");
    }

    private static void CreateBoss()
    {
        GameObject bossObj = new GameObject("Boss");
        bossObj.tag = "Boss";
        bossObj.layer = LayerMask.NameToLayer("Default");
        bossObj.transform.position = new Vector3(3, 0, 0);
        bossObj.transform.localScale = new Vector3(2, 2, 1);

        var sr = bossObj.AddComponent<SpriteRenderer>();
        sr.sortingOrder = 5;

        var col = bossObj.AddComponent<BoxCollider2D>();
        col.size = new Vector2(1.5f, 2f);

        bossObj.AddComponent<BossHealth>();
        bossObj.AddComponent<BossControllerMultiPhase>();
        bossObj.AddComponent<BossInitializer>();
        bossObj.AddComponent<SpriteAnimator>();

        Debug.Log("[GameSceneSetup] Created Boss");
    }

    private static void CreateArena()
    {
        // Check if arena already exists
        if (GameObject.Find("Arena") != null) return;

        GameObject arena = new GameObject("Arena");

        // Ground
        GameObject ground = new GameObject("Ground");
        ground.transform.SetParent(arena.transform);
        ground.transform.position = new Vector3(0, -3f, 0);
        ground.transform.localScale = new Vector3(26, 1, 1);  // Wider ground

        var groundSR = ground.AddComponent<SpriteRenderer>();
        groundSR.color = new Color(0.35f, 0.22f, 0.1f); // Dark bronze ground, slightly lighter than background
        groundSR.sortingOrder = -10;
        // Create simple ground sprite
        Texture2D tex = new Texture2D(4, 4);
        for (int x = 0; x < 4; x++)
            for (int y = 0; y < 4; y++)
                tex.SetPixel(x, y, Color.white);
        tex.Apply();
        groundSR.sprite = Sprite.Create(tex, new Rect(0, 0, 4, 4), new Vector2(0.5f, 0.5f), 4);

        var groundCol = ground.AddComponent<BoxCollider2D>();
        groundCol.size = new Vector2(1, 1);

        // Left wall - push further out for more play area
        GameObject leftWall = new GameObject("LeftWall");
        leftWall.transform.SetParent(arena.transform);
        leftWall.transform.position = new Vector3(-12, 0, 0);
        var leftCol = leftWall.AddComponent<BoxCollider2D>();
        leftCol.size = new Vector2(1, 10);

        // Right wall - push further out for more play area
        GameObject rightWall = new GameObject("RightWall");
        rightWall.transform.SetParent(arena.transform);
        rightWall.transform.position = new Vector3(12, 0, 0);
        var rightCol = rightWall.AddComponent<BoxCollider2D>();
        rightCol.size = new Vector2(1, 10);

        Debug.Log("[GameSceneSetup] Created Arena");
    }

    private static void CreateManagers()
    {
        if (GameManager.Instance == null)
            CreateManager<GameManager>("GameManager");

        if (AudioManager.Instance == null)
            CreateManager<AudioManager>("AudioManager");

        if (Camera.main != null && CameraShake.Instance == null)
        {
            if (Camera.main.GetComponent<CameraShake>() == null)
                Camera.main.gameObject.AddComponent<CameraShake>();
        }

        if (HitFeedback.Instance == null)
            CreateManager<HitFeedback>("HitFeedback");

        if (ParticleManager.Instance == null)
            CreateManager<ParticleManager>("ParticleManager");

        if (DamageNumberUI.Instance == null)
            CreateManager<DamageNumberUI>("DamageNumberUI");

        if (IntroSequence.Instance == null)
            CreateManager<IntroSequence>("IntroSequence");

        if (BossDialogue.Instance == null)
            CreateManager<BossDialogue>("BossDialogue");

        if (UIScreenTransition.Instance == null)
            CreateManager<UIScreenTransition>("UIScreenTransition");

        if (ComboCounterUI.Instance == null)
            CreateManager<ComboCounterUI>("ComboCounterUI");
    }

    private static T CreateManager<T>(string name) where T : MonoBehaviour
    {
        GameObject obj = new GameObject(name);
        var component = obj.AddComponent<T>();
        Debug.Log($"[GameSceneSetup] Created {name}");
        return component;
    }
}

/// <summary>
/// Auto-starts the intro sequence when game begins.
/// </summary>
public class GameFlowController : MonoBehaviour
{
    [SerializeField] private bool playIntroOnStart = true;
    [SerializeField] private float introDelay = 0.5f;

    private void Start()
    {
        if (playIntroOnStart && IntroSequence.Instance != null)
        {
            StartCoroutine(PlayIntroDelayed());
        }
        else
        {
            // No intro, start directly
            if (AudioManager.Instance != null)
                AudioManager.Instance.PlayMusic(MusicType.Battle);

            if (BossDialogue.Instance != null)
                BossDialogue.Instance.ShowBattleStart();
        }
    }

    private System.Collections.IEnumerator PlayIntroDelayed()
    {
        yield return new WaitForSeconds(introDelay);

        string bossName = "The Bronze Mask";
        Sprite bossPortrait = null;

        var boss = GameObject.FindWithTag("Boss");
        if (boss != null)
        {
            var initializer = boss.GetComponent<BossInitializer>();
            if (initializer != null)
            {
                var data = initializer.GetBossData();
                if (data != null)
                {
                    bossName = data.bossName;
                    bossPortrait = data.portrait;
                }
            }
        }

        IntroSequence.Instance.OnIntroComplete += OnIntroComplete;
        IntroSequence.Instance.PlayIntro(bossName, bossPortrait);
    }

    private void OnIntroComplete()
    {
        IntroSequence.Instance.OnIntroComplete -= OnIntroComplete;

        if (GameManager.Instance != null)
            GameManager.Instance.StartGame();

        if (BossDialogue.Instance != null)
            BossDialogue.Instance.ShowBattleStart();
    }
}
