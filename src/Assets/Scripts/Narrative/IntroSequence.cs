using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Handles the intro cinematic sequence before battle.
/// Displays title, story text, and boss introduction.
/// </summary>
public class IntroSequence : MonoBehaviour
{
    public static IntroSequence Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private Canvas introCanvas;
    [SerializeField] private Text titleText;
    [SerializeField] private Text storyText;
    [SerializeField] private Text bossNameText;
    [SerializeField] private Image bossPortrait;
    [SerializeField] private Image fadePanel;
    [SerializeField] private Text skipPrompt;

    [Header("Timing")]
    [SerializeField] private float titleDuration = 2f;
    [SerializeField] private float storyDuration = 4f;
    [SerializeField] private float bossIntroDuration = 3f;
    [SerializeField] private float fadeSpeed = 1f;
    [SerializeField] private float textTypeSpeed = 0.03f;

    [Header("Story Text")]
    [SerializeField] private string[] storyLines = new string[]
    {
        "In ancient times, the Bronze Mask protected the realm...",
        "But darkness has awakened its dormant fury.",
        "Now only you can restore the balance.",
        "Face the mask. Prove your worth."
    };

    private bool isPlaying = false;
    private bool skipRequested = false;
    private Coroutine sequenceCoroutine;

    public event System.Action OnIntroComplete;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Create UI if not assigned
        if (introCanvas == null)
        {
            CreateIntroUI();
        }
    }

    private void Update()
    {
        // Skip intro on any key press
        if (isPlaying && (Input.anyKeyDown || Input.GetMouseButtonDown(0)))
        {
            skipRequested = true;
        }
    }

    private void CreateIntroUI()
    {
        // Create canvas
        GameObject canvasObj = new GameObject("IntroCanvas");
        canvasObj.transform.SetParent(transform);
        introCanvas = canvasObj.AddComponent<Canvas>();
        introCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        introCanvas.sortingOrder = 1000;

        var scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        canvasObj.AddComponent<GraphicRaycaster>();

        // Fade panel (black background)
        GameObject fadeObj = CreateUIElement(canvasObj, "FadePanel");
        fadePanel = fadeObj.AddComponent<Image>();
        fadePanel.color = Color.black;
        fadePanel.rectTransform.anchorMin = Vector2.zero;
        fadePanel.rectTransform.anchorMax = Vector2.one;
        fadePanel.rectTransform.sizeDelta = Vector2.zero;

        // Title text
        GameObject titleObj = CreateUIElement(canvasObj, "TitleText");
        titleText = titleObj.AddComponent<Text>();
        titleText.text = "MASK OF THE BRONZE GOD";
        titleText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        titleText.fontSize = 72;
        titleText.color = new Color(0.804f, 0.498f, 0.196f); // Official Bronze #CD7F32
        titleText.alignment = TextAnchor.MiddleCenter;
        titleText.rectTransform.anchorMin = new Vector2(0, 0.5f);
        titleText.rectTransform.anchorMax = new Vector2(1, 0.7f);
        titleText.rectTransform.sizeDelta = Vector2.zero;

        // Story text
        GameObject storyObj = CreateUIElement(canvasObj, "StoryText");
        storyText = storyObj.AddComponent<Text>();
        storyText.text = "";
        storyText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        storyText.fontSize = 36;
        storyText.color = Color.white;
        storyText.alignment = TextAnchor.MiddleCenter;
        storyText.rectTransform.anchorMin = new Vector2(0.1f, 0.3f);
        storyText.rectTransform.anchorMax = new Vector2(0.9f, 0.6f);
        storyText.rectTransform.sizeDelta = Vector2.zero;

        // Boss name text
        GameObject bossNameObj = CreateUIElement(canvasObj, "BossNameText");
        bossNameText = bossNameObj.AddComponent<Text>();
        bossNameText.text = "";
        bossNameText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        bossNameText.fontSize = 56;
        bossNameText.color = new Color(0.886f, 0.447f, 0.357f); // Official Terracotta #E2725B (danger)
        bossNameText.alignment = TextAnchor.MiddleCenter;
        bossNameText.rectTransform.anchorMin = new Vector2(0, 0.15f);
        bossNameText.rectTransform.anchorMax = new Vector2(1, 0.35f);
        bossNameText.rectTransform.sizeDelta = Vector2.zero;

        // Boss portrait
        GameObject portraitObj = CreateUIElement(canvasObj, "BossPortrait");
        bossPortrait = portraitObj.AddComponent<Image>();
        bossPortrait.color = Color.white;
        bossPortrait.rectTransform.anchorMin = new Vector2(0.35f, 0.4f);
        bossPortrait.rectTransform.anchorMax = new Vector2(0.65f, 0.9f);
        bossPortrait.rectTransform.sizeDelta = Vector2.zero;
        bossPortrait.enabled = false;

        // Skip prompt
        GameObject skipObj = CreateUIElement(canvasObj, "SkipPrompt");
        skipPrompt = skipObj.AddComponent<Text>();
        skipPrompt.text = "Press any key to skip...";
        skipPrompt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        skipPrompt.fontSize = 24;
        skipPrompt.color = new Color(1, 1, 1, 0.5f);
        skipPrompt.alignment = TextAnchor.LowerRight;
        skipPrompt.rectTransform.anchorMin = new Vector2(0.7f, 0);
        skipPrompt.rectTransform.anchorMax = new Vector2(1f, 0.1f);
        skipPrompt.rectTransform.sizeDelta = Vector2.zero;
        skipPrompt.rectTransform.offsetMin = new Vector2(-20, 20);

        // Initially hide everything
        introCanvas.gameObject.SetActive(false);
    }

    private GameObject CreateUIElement(GameObject parent, string name)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent.transform, false);
        RectTransform rect = obj.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.sizeDelta = Vector2.zero;
        rect.anchoredPosition = Vector2.zero;
        return obj;
    }

    /// <summary>
    /// Play the full intro sequence
    /// </summary>
    public void PlayIntro(string bossName = "The Bronze Mask", Sprite bossSpritePortrait = null)
    {
        if (isPlaying) return;

        introCanvas.gameObject.SetActive(true);
        skipRequested = false;
        sequenceCoroutine = StartCoroutine(IntroCoroutine(bossName, bossSpritePortrait));
    }

    /// <summary>
    /// Skip the intro immediately
    /// </summary>
    public void SkipIntro()
    {
        skipRequested = true;
    }

    private IEnumerator IntroCoroutine(string bossName, Sprite bossSpritePortrait)
    {
        isPlaying = true;

        // Pause game during intro
        Time.timeScale = 0;

        // Reset all
        titleText.color = new Color(titleText.color.r, titleText.color.g, titleText.color.b, 0);
        storyText.text = "";
        bossNameText.text = "";
        bossPortrait.enabled = false;
        fadePanel.color = Color.black;

        // Play intro music if available
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayMusic(MusicType.Menu);
        }

        // PHASE 1: Title card
        yield return StartCoroutine(FadeText(titleText, 0, 1, fadeSpeed));
        if (!skipRequested) yield return WaitForSecondsUnscaled(titleDuration);
        yield return StartCoroutine(FadeText(titleText, 1, 0, fadeSpeed));

        if (skipRequested) { EndIntro(); yield break; }

        // PHASE 2: Story text (typewriter effect)
        foreach (string line in storyLines)
        {
            if (skipRequested) { EndIntro(); yield break; }

            storyText.text = "";
            yield return StartCoroutine(TypeText(storyText, line));
            if (!skipRequested) yield return WaitForSecondsUnscaled(1.5f);
            yield return StartCoroutine(FadeText(storyText, 1, 0, fadeSpeed * 2));
        }

        if (skipRequested) { EndIntro(); yield break; }

        // PHASE 3: Boss introduction
        if (bossSpritePortrait != null)
        {
            bossPortrait.sprite = bossSpritePortrait;
            bossPortrait.enabled = true;
            bossPortrait.color = new Color(1, 1, 1, 0);
            yield return StartCoroutine(FadeImage(bossPortrait, 0, 1, fadeSpeed));
        }

        bossNameText.text = bossName;
        bossNameText.color = new Color(bossNameText.color.r, bossNameText.color.g, bossNameText.color.b, 0);
        yield return StartCoroutine(FadeText(bossNameText, 0, 1, fadeSpeed));

        // Boss roar
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayBossAttack(BossAttackSoundType.Roar);
        }

        // Shake effect
        if (CameraShake.Instance != null)
        {
            CameraShake.Instance.ShakeHeavy();
        }

        if (!skipRequested) yield return WaitForSecondsUnscaled(bossIntroDuration);

        // PHASE 4: Fade out to battle
        yield return StartCoroutine(FadePanel(fadePanel, 0, 1, fadeSpeed));

        EndIntro();
    }

    private void EndIntro()
    {
        isPlaying = false;
        Time.timeScale = 1;

        // Switch to battle music
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayMusic(MusicType.Battle);
        }

        // Hide intro canvas
        introCanvas.gameObject.SetActive(false);

        OnIntroComplete?.Invoke();
    }

    private IEnumerator TypeText(Text textComponent, string fullText)
    {
        textComponent.text = "";
        textComponent.color = Color.white;

        foreach (char c in fullText)
        {
            if (skipRequested) break;

            textComponent.text += c;

            // Play typing sound occasionally
            if (c != ' ' && AudioManager.Instance != null && Random.value > 0.7f)
            {
                AudioManager.Instance.PlayUISound(UISoundType.Click);
            }

            yield return WaitForSecondsUnscaled(textTypeSpeed);
        }
    }

    private IEnumerator FadeText(Text textComponent, float from, float to, float speed)
    {
        float t = 0;
        Color color = textComponent.color;

        while (t < 1)
        {
            if (skipRequested && to == 0) break; // Allow skip during fade out

            t += Time.unscaledDeltaTime * speed;
            color.a = Mathf.Lerp(from, to, t);
            textComponent.color = color;
            yield return null;
        }

        color.a = to;
        textComponent.color = color;
    }

    private IEnumerator FadeImage(Image image, float from, float to, float speed)
    {
        float t = 0;
        Color color = image.color;

        while (t < 1)
        {
            if (skipRequested && to == 0) break;

            t += Time.unscaledDeltaTime * speed;
            color.a = Mathf.Lerp(from, to, t);
            image.color = color;
            yield return null;
        }

        color.a = to;
        image.color = color;
    }

    private IEnumerator FadePanel(Image panel, float from, float to, float speed)
    {
        float t = 0;
        Color color = panel.color;

        while (t < 1)
        {
            t += Time.unscaledDeltaTime * speed;
            color.a = Mathf.Lerp(from, to, t);
            panel.color = color;
            yield return null;
        }

        color.a = to;
        panel.color = color;
    }

    private IEnumerator WaitForSecondsUnscaled(float duration)
    {
        float elapsed = 0;
        while (elapsed < duration)
        {
            if (skipRequested) yield break;
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }
    }
}
