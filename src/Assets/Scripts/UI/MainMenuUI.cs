using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Main menu UI controller - creates all UI dynamically
/// </summary>
public class MainMenuUI : MonoBehaviour
{
    public static MainMenuUI Instance { get; private set; }

    [Header("Scene")]
    [SerializeField] private string gameSceneName = "Game";

    private Canvas menuCanvas;
    private Text titleText;
    private Button playButton;
    private Button quitButton;
    private Vector3 titleOriginalScale;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void AutoCreate()
    {
        // Only create in MainMenu scene
        if (SceneManager.GetActiveScene().name != "MainMenu") return;
        if (Instance != null) return;

        GameObject obj = new GameObject("MainMenuUI");
        obj.AddComponent<MainMenuUI>();
        Debug.Log("[MainMenuUI] Auto-created");
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        CreateMenuUI();
    }

    private void CreateMenuUI()
    {
        // Find existing canvas or create new one
        menuCanvas = FindAnyObjectByType<Canvas>();

        if (menuCanvas == null)
        {
            GameObject canvasObj = new GameObject("MenuCanvas");
            canvasObj.transform.SetParent(transform);
            menuCanvas = canvasObj.AddComponent<Canvas>();
            menuCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            menuCanvas.sortingOrder = 100;

            var scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);

            canvasObj.AddComponent<GraphicRaycaster>();
            Debug.Log("[MainMenuUI] Created new Canvas");
        }
        else
        {
            // Ensure existing canvas has required components
            if (menuCanvas.GetComponent<CanvasScaler>() == null)
            {
                var scaler = menuCanvas.gameObject.AddComponent<CanvasScaler>();
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1920, 1080);
            }
            if (menuCanvas.GetComponent<GraphicRaycaster>() == null)
            {
                menuCanvas.gameObject.AddComponent<GraphicRaycaster>();
            }
            // Make sure it's overlay mode
            menuCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            menuCanvas.sortingOrder = 100;
            Debug.Log("[MainMenuUI] Using existing Canvas");
        }

        // Clear any existing children in canvas
        foreach (Transform child in menuCanvas.transform)
        {
            Destroy(child.gameObject);
        }

        // Background
        GameObject bgObj = new GameObject("Background");
        bgObj.transform.SetParent(menuCanvas.transform, false);
        var bgImage = bgObj.AddComponent<Image>();
        bgImage.color = new Color(0.08f, 0.08f, 0.12f);
        var bgRect = bgImage.rectTransform;
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.sizeDelta = Vector2.zero;

        // Title
        GameObject titleObj = new GameObject("Title");
        titleObj.transform.SetParent(menuCanvas.transform, false);
        titleText = titleObj.AddComponent<Text>();
        titleText.text = "MASK OF THE\nBRONZE GOD";
        titleText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        titleText.fontSize = 72;
        titleText.color = new Color(0.85f, 0.55f, 0.2f);
        titleText.alignment = TextAnchor.MiddleCenter;
        titleText.fontStyle = FontStyle.Bold;
        var titleRect = titleText.rectTransform;
        titleRect.anchorMin = new Vector2(0, 0.6f);
        titleRect.anchorMax = new Vector2(1, 0.95f);
        titleRect.sizeDelta = Vector2.zero;
        titleOriginalScale = Vector3.one;

        // Subtitle
        GameObject subObj = new GameObject("Subtitle");
        subObj.transform.SetParent(menuCanvas.transform, false);
        var subText = subObj.AddComponent<Text>();
        subText.text = "A Cuphead-Inspired Boss Battle";
        subText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        subText.fontSize = 24;
        subText.color = new Color(0.6f, 0.6f, 0.6f);
        subText.alignment = TextAnchor.MiddleCenter;
        var subRect = subText.rectTransform;
        subRect.anchorMin = new Vector2(0, 0.52f);
        subRect.anchorMax = new Vector2(1, 0.6f);
        subRect.sizeDelta = Vector2.zero;

        // Play Button
        playButton = CreateButton(menuCanvas.gameObject, "Play", "PLAY", new Vector2(0.5f, 0.38f), OnPlayClicked);

        // Quit Button
        quitButton = CreateButton(menuCanvas.gameObject, "Quit", "QUIT", new Vector2(0.5f, 0.22f), OnQuitClicked);

        // Instructions
        GameObject instrObj = new GameObject("Instructions");
        instrObj.transform.SetParent(menuCanvas.transform, false);
        var instrText = instrObj.AddComponent<Text>();
        instrText.text = "A/D - Move  |  Left Click - Attack  |  Space - Dodge  |  Q - Parry";
        instrText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        instrText.fontSize = 20;
        instrText.color = new Color(0.5f, 0.5f, 0.5f);
        instrText.alignment = TextAnchor.MiddleCenter;
        var instrRect = instrText.rectTransform;
        instrRect.anchorMin = new Vector2(0, 0.02f);
        instrRect.anchorMax = new Vector2(1, 0.12f);
        instrRect.sizeDelta = Vector2.zero;

        Debug.Log("[MainMenuUI] Menu UI created successfully");
    }

    private Button CreateButton(GameObject parent, string name, string label, Vector2 anchorPos, UnityEngine.Events.UnityAction onClick)
    {
        GameObject btnObj = new GameObject(name + "Button");
        btnObj.transform.SetParent(parent.transform, false);

        var btnImage = btnObj.AddComponent<Image>();
        btnImage.color = new Color(0.75f, 0.45f, 0.15f);

        var btn = btnObj.AddComponent<Button>();
        btn.targetGraphic = btnImage;
        btn.onClick.AddListener(onClick);

        var colors = btn.colors;
        colors.normalColor = new Color(0.75f, 0.45f, 0.15f);
        colors.highlightedColor = new Color(0.95f, 0.6f, 0.2f);
        colors.pressedColor = new Color(0.55f, 0.35f, 0.1f);
        colors.selectedColor = new Color(0.85f, 0.5f, 0.18f);
        btn.colors = colors;

        var btnRect = btnObj.GetComponent<RectTransform>();
        btnRect.anchorMin = new Vector2(anchorPos.x - 0.15f, anchorPos.y - 0.05f);
        btnRect.anchorMax = new Vector2(anchorPos.x + 0.15f, anchorPos.y + 0.05f);
        btnRect.sizeDelta = Vector2.zero;

        // Button text
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(btnObj.transform, false);
        var text = textObj.AddComponent<Text>();
        text.text = label;
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = 40;
        text.color = Color.white;
        text.alignment = TextAnchor.MiddleCenter;
        text.fontStyle = FontStyle.Bold;
        var textRect = text.rectTransform;
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;

        return btn;
    }

    private void Start()
    {
        Time.timeScale = 1f;

        // Ensure EventSystem exists
        if (FindAnyObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            Debug.Log("[MainMenuUI] Created EventSystem");
        }
    }

    private void Update()
    {
        // Title pulse
        if (titleText != null)
        {
            float pulse = 1f + Mathf.Sin(Time.time * 1.5f) * 0.02f;
            titleText.transform.localScale = titleOriginalScale * pulse;
        }

        // Keyboard shortcuts
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            OnPlayClicked();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnQuitClicked();
        }
    }

    private void OnPlayClicked()
    {
        Debug.Log("[MainMenuUI] Starting game...");
        SceneManager.LoadScene(gameSceneName);
    }

    private void OnQuitClicked()
    {
        Debug.Log("[MainMenuUI] Quitting...");
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}
