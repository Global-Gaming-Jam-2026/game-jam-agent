using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

/// <summary>
/// UI effects for buttons and interactive elements.
/// Provides hover, click, and transition animations.
/// </summary>
public class UIEffects : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("Scale Effects")]
    [SerializeField] private bool useScaleEffect = true;
    [SerializeField] private float hoverScale = 1.1f;
    [SerializeField] private float clickScale = 0.95f;
    [SerializeField] private float scaleSpeed = 10f;

    [Header("Color Effects")]
    [SerializeField] private bool useColorEffect = true;
    [SerializeField] private Color hoverTint = new Color(1.2f, 1.2f, 1.2f, 1f);
    [SerializeField] private Color clickTint = new Color(0.8f, 0.8f, 0.8f, 1f);
    [SerializeField] private float colorSpeed = 8f;

    [Header("Glow Effect")]
    [SerializeField] private bool useGlowEffect = false;
    [SerializeField] private Image glowImage;
    [SerializeField] private float glowIntensity = 0.5f;

    [Header("Audio")]
    [SerializeField] private bool playHoverSound = true;
    [SerializeField] private bool playClickSound = true;

    private RectTransform rectTransform;
    private Graphic targetGraphic;
    private Vector3 originalScale;
    private Color originalColor;
    private float targetScale = 1f;
    private Color targetColor;
    private bool isHovered = false;
    private bool isPressed = false;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        targetGraphic = GetComponent<Graphic>();

        if (rectTransform != null)
        {
            originalScale = rectTransform.localScale;
        }

        if (targetGraphic != null)
        {
            originalColor = targetGraphic.color;
            targetColor = originalColor;
        }

        if (glowImage != null)
        {
            glowImage.color = new Color(glowImage.color.r, glowImage.color.g, glowImage.color.b, 0);
        }
    }

    private void Update()
    {
        // Smooth scale transition
        if (useScaleEffect && rectTransform != null)
        {
            Vector3 currentScale = rectTransform.localScale;
            Vector3 target = originalScale * targetScale;
            rectTransform.localScale = Vector3.Lerp(currentScale, target, Time.unscaledDeltaTime * scaleSpeed);
        }

        // Smooth color transition
        if (useColorEffect && targetGraphic != null)
        {
            targetGraphic.color = Color.Lerp(targetGraphic.color, targetColor, Time.unscaledDeltaTime * colorSpeed);
        }

        // Glow effect
        if (useGlowEffect && glowImage != null)
        {
            float targetAlpha = isHovered ? glowIntensity : 0;
            Color glowColor = glowImage.color;
            glowColor.a = Mathf.Lerp(glowColor.a, targetAlpha, Time.unscaledDeltaTime * colorSpeed);
            glowImage.color = glowColor;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
        targetScale = hoverScale;
        targetColor = MultiplyColor(originalColor, hoverTint);

        if (playHoverSound && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayUISound(UISoundType.Hover);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
        isPressed = false;
        targetScale = 1f;
        targetColor = originalColor;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;
        targetScale = clickScale;
        targetColor = MultiplyColor(originalColor, clickTint);

        if (playClickSound && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayUISound(UISoundType.Click);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;
        if (isHovered)
        {
            targetScale = hoverScale;
            targetColor = MultiplyColor(originalColor, hoverTint);
        }
        else
        {
            targetScale = 1f;
            targetColor = originalColor;
        }
    }

    private Color MultiplyColor(Color a, Color b)
    {
        return new Color(
            Mathf.Clamp01(a.r * b.r),
            Mathf.Clamp01(a.g * b.g),
            Mathf.Clamp01(a.b * b.b),
            a.a * b.a
        );
    }

    /// <summary>
    /// Pulse animation for attention
    /// </summary>
    public void Pulse(float intensity = 1.2f, float duration = 0.3f)
    {
        StartCoroutine(PulseCoroutine(intensity, duration));
    }

    private IEnumerator PulseCoroutine(float intensity, float duration)
    {
        float elapsed = 0;
        Vector3 startScale = rectTransform.localScale;
        Vector3 peakScale = originalScale * intensity;

        // Scale up
        while (elapsed < duration * 0.5f)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / (duration * 0.5f);
            rectTransform.localScale = Vector3.Lerp(startScale, peakScale, t);
            yield return null;
        }

        // Scale down
        elapsed = 0;
        while (elapsed < duration * 0.5f)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / (duration * 0.5f);
            rectTransform.localScale = Vector3.Lerp(peakScale, originalScale * targetScale, t);
            yield return null;
        }
    }

    /// <summary>
    /// Shake animation for error feedback
    /// </summary>
    public void Shake(float intensity = 10f, float duration = 0.3f)
    {
        StartCoroutine(ShakeCoroutine(intensity, duration));
    }

    private IEnumerator ShakeCoroutine(float intensity, float duration)
    {
        Vector3 startPos = rectTransform.anchoredPosition;
        float elapsed = 0;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float decay = 1 - (elapsed / duration);
            float offsetX = Random.Range(-intensity, intensity) * decay;
            rectTransform.anchoredPosition = startPos + new Vector3(offsetX, 0, 0);
            yield return null;
        }

        rectTransform.anchoredPosition = startPos;
    }
}

/// <summary>
/// Screen transition effects
/// </summary>
public class UIScreenTransition : MonoBehaviour
{
    public static UIScreenTransition Instance { get; private set; }

    [SerializeField] private Image fadePanel;
    [SerializeField] private float fadeSpeed = 2f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (fadePanel == null)
        {
            CreateFadePanel();
        }
    }

    private void CreateFadePanel()
    {
        // Find or create canvas
        Canvas canvas = FindAnyObjectByType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("TransitionCanvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 9999;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
        }

        GameObject panelObj = new GameObject("FadePanel");
        panelObj.transform.SetParent(canvas.transform, false);
        fadePanel = panelObj.AddComponent<Image>();
        fadePanel.color = new Color(0, 0, 0, 0);
        fadePanel.raycastTarget = false;

        RectTransform rect = fadePanel.rectTransform;
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.sizeDelta = Vector2.zero;
    }

    /// <summary>
    /// Fade to black
    /// </summary>
    public void FadeOut(System.Action onComplete = null)
    {
        StartCoroutine(FadeCoroutine(0, 1, onComplete));
    }

    /// <summary>
    /// Fade from black
    /// </summary>
    public void FadeIn(System.Action onComplete = null)
    {
        StartCoroutine(FadeCoroutine(1, 0, onComplete));
    }

    /// <summary>
    /// Fade out, execute action, fade in
    /// </summary>
    public void FadeTransition(System.Action midAction, System.Action onComplete = null)
    {
        StartCoroutine(TransitionCoroutine(midAction, onComplete));
    }

    private IEnumerator FadeCoroutine(float from, float to, System.Action onComplete)
    {
        fadePanel.raycastTarget = true;
        float elapsed = 0;
        Color color = fadePanel.color;

        while (elapsed < 1f / fadeSpeed)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed * fadeSpeed;
            color.a = Mathf.Lerp(from, to, t);
            fadePanel.color = color;
            yield return null;
        }

        color.a = to;
        fadePanel.color = color;
        fadePanel.raycastTarget = to > 0.5f;

        onComplete?.Invoke();
    }

    private IEnumerator TransitionCoroutine(System.Action midAction, System.Action onComplete)
    {
        // Fade out
        yield return FadeCoroutine(0, 1, null);

        // Execute mid action
        midAction?.Invoke();

        // Brief pause
        yield return new WaitForSecondsRealtime(0.2f);

        // Fade in
        yield return FadeCoroutine(1, 0, onComplete);
    }
}

/// <summary>
/// Combo counter UI display
/// </summary>
public class ComboCounterUI : MonoBehaviour
{
    public static ComboCounterUI Instance { get; private set; }

    [SerializeField] private Text comboText;
    [SerializeField] private Text comboMultiplierText;
    [SerializeField] private float displayDuration = 2f;
    [SerializeField] private float punchScale = 1.3f;

    private RectTransform comboRect;
    private int currentCombo = 0;
    private float hideTimer = 0;
    private bool isVisible = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (comboText == null)
        {
            CreateComboUI();
        }
    }

    private void CreateComboUI()
    {
        // Find or create canvas
        Canvas canvas = FindAnyObjectByType<Canvas>();
        if (canvas == null) return;

        GameObject container = new GameObject("ComboContainer");
        container.transform.SetParent(canvas.transform, false);
        RectTransform containerRect = container.AddComponent<RectTransform>();
        containerRect.anchorMin = new Vector2(0.8f, 0.7f);
        containerRect.anchorMax = new Vector2(1f, 0.9f);
        containerRect.sizeDelta = Vector2.zero;

        // Combo number
        GameObject comboObj = new GameObject("ComboText");
        comboObj.transform.SetParent(container.transform, false);
        comboText = comboObj.AddComponent<Text>();
        comboText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        comboText.fontSize = 72;
        comboText.color = Color.yellow;
        comboText.alignment = TextAnchor.MiddleCenter;
        comboText.text = "";
        comboRect = comboText.rectTransform;
        comboRect.anchorMin = new Vector2(0, 0.5f);
        comboRect.anchorMax = new Vector2(1, 1);
        comboRect.sizeDelta = Vector2.zero;

        // Multiplier text
        GameObject multObj = new GameObject("MultiplierText");
        multObj.transform.SetParent(container.transform, false);
        comboMultiplierText = multObj.AddComponent<Text>();
        comboMultiplierText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        comboMultiplierText.fontSize = 36;
        comboMultiplierText.color = Color.white;
        comboMultiplierText.alignment = TextAnchor.MiddleCenter;
        comboMultiplierText.text = "";
        RectTransform multRect = comboMultiplierText.rectTransform;
        multRect.anchorMin = new Vector2(0, 0);
        multRect.anchorMax = new Vector2(1, 0.5f);
        multRect.sizeDelta = Vector2.zero;

        container.SetActive(false);
        this.gameObject.SetActive(true);
    }

    private void Update()
    {
        if (isVisible)
        {
            hideTimer -= Time.deltaTime;
            if (hideTimer <= 0)
            {
                HideCombo();
            }
        }
    }

    public void AddHit()
    {
        currentCombo++;
        UpdateDisplay();
        hideTimer = displayDuration;

        if (!isVisible)
        {
            ShowCombo();
        }

        // Punch effect
        if (comboRect != null)
        {
            StartCoroutine(PunchScale());
        }
    }

    public void ResetCombo()
    {
        currentCombo = 0;
        HideCombo();
    }

    private void UpdateDisplay()
    {
        if (comboText != null)
        {
            comboText.text = currentCombo.ToString();

            // Color based on combo
            if (currentCombo >= 10)
                comboText.color = Color.red;
            else if (currentCombo >= 5)
                comboText.color = new Color(1f, 0.5f, 0f);
            else
                comboText.color = Color.yellow;
        }

        if (comboMultiplierText != null && currentCombo >= 2)
        {
            comboMultiplierText.text = "COMBO!";
        }
    }

    private void ShowCombo()
    {
        isVisible = true;
        if (comboText != null && comboText.transform.parent != null)
        {
            comboText.transform.parent.gameObject.SetActive(true);
        }
    }

    private void HideCombo()
    {
        isVisible = false;
        currentCombo = 0;
        if (comboText != null && comboText.transform.parent != null)
        {
            comboText.transform.parent.gameObject.SetActive(false);
        }
    }

    private IEnumerator PunchScale()
    {
        Vector3 originalScale = Vector3.one;
        Vector3 punchedScale = Vector3.one * punchScale;

        float elapsed = 0;
        float duration = 0.15f;

        // Scale up
        while (elapsed < duration * 0.5f)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / (duration * 0.5f);
            comboRect.localScale = Vector3.Lerp(originalScale, punchedScale, t);
            yield return null;
        }

        // Scale down
        elapsed = 0;
        while (elapsed < duration * 0.5f)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / (duration * 0.5f);
            comboRect.localScale = Vector3.Lerp(punchedScale, originalScale, t);
            yield return null;
        }

        comboRect.localScale = originalScale;
    }
}
