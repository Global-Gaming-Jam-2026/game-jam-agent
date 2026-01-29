using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// Handles scene transitions with fade effects.
/// Singleton that persists across scene loads.
/// </summary>
public class SceneTransition : MonoBehaviour
{
    public static SceneTransition Instance { get; private set; }

    [Header("Fade Settings")]
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private Color fadeColor = Color.black;

    [Header("Optional Loading Screen")]
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private Slider loadingBar;
    [SerializeField] private Text loadingText;

    private Canvas canvas;
    private Image fadeImage;
    private bool isTransitioning;

    private void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SetupCanvas();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void SetupCanvas()
    {
        // Create canvas for fade overlay
        canvas = gameObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999; // Always on top

        gameObject.AddComponent<CanvasScaler>();
        gameObject.AddComponent<GraphicRaycaster>();

        // Create fade image
        GameObject fadeObj = new GameObject("FadeImage");
        fadeObj.transform.SetParent(transform);

        var rect = fadeObj.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        fadeImage = fadeObj.AddComponent<Image>();
        fadeImage.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, 0);
        fadeImage.raycastTarget = false;
    }

    /// <summary>
    /// Load a scene with fade transition
    /// </summary>
    public static void LoadScene(string sceneName)
    {
        if (Instance != null)
        {
            Instance.StartCoroutine(Instance.TransitionToScene(sceneName));
        }
        else
        {
            SceneManager.LoadScene(sceneName);
        }
    }

    /// <summary>
    /// Load a scene by index with fade transition
    /// </summary>
    public static void LoadScene(int sceneIndex)
    {
        if (Instance != null)
        {
            Instance.StartCoroutine(Instance.TransitionToSceneIndex(sceneIndex));
        }
        else
        {
            SceneManager.LoadScene(sceneIndex);
        }
    }

    /// <summary>
    /// Reload current scene
    /// </summary>
    public static void ReloadScene()
    {
        LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private IEnumerator TransitionToScene(string sceneName)
    {
        if (isTransitioning) yield break;
        isTransitioning = true;

        // Fade out
        yield return StartCoroutine(Fade(0, 1));

        // Load scene
        SceneManager.LoadScene(sceneName);

        // Wait a frame for scene to initialize
        yield return null;

        // Fade in
        yield return StartCoroutine(Fade(1, 0));

        isTransitioning = false;
    }

    private IEnumerator TransitionToSceneIndex(int index)
    {
        if (isTransitioning) yield break;
        isTransitioning = true;

        yield return StartCoroutine(Fade(0, 1));
        SceneManager.LoadScene(index);
        yield return null;
        yield return StartCoroutine(Fade(1, 0));

        isTransitioning = false;
    }

    /// <summary>
    /// Async load with loading screen
    /// </summary>
    public static void LoadSceneAsync(string sceneName)
    {
        if (Instance != null)
        {
            Instance.StartCoroutine(Instance.TransitionToSceneAsync(sceneName));
        }
        else
        {
            SceneManager.LoadScene(sceneName);
        }
    }

    private IEnumerator TransitionToSceneAsync(string sceneName)
    {
        if (isTransitioning) yield break;
        isTransitioning = true;

        // Fade out
        yield return StartCoroutine(Fade(0, 1));

        // Show loading screen
        if (loadingScreen != null)
        {
            loadingScreen.SetActive(true);
        }

        // Start async load
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;

        // Update loading bar
        while (!asyncLoad.isDone)
        {
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);

            if (loadingBar != null)
            {
                loadingBar.value = progress;
            }

            if (loadingText != null)
            {
                loadingText.text = $"Loading... {Mathf.RoundToInt(progress * 100)}%";
            }

            // When ready, activate scene
            if (asyncLoad.progress >= 0.9f)
            {
                asyncLoad.allowSceneActivation = true;
            }

            yield return null;
        }

        // Hide loading screen
        if (loadingScreen != null)
        {
            loadingScreen.SetActive(false);
        }

        // Wait a frame
        yield return null;

        // Fade in
        yield return StartCoroutine(Fade(1, 0));

        isTransitioning = false;
    }

    private IEnumerator Fade(float startAlpha, float endAlpha)
    {
        float elapsed = 0;
        Color color = fadeImage.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / fadeDuration;
            color.a = Mathf.Lerp(startAlpha, endAlpha, t);
            fadeImage.color = color;
            yield return null;
        }

        color.a = endAlpha;
        fadeImage.color = color;
    }

    /// <summary>
    /// Quick fade out (for dramatic moments)
    /// </summary>
    public static void FadeOut(System.Action onComplete = null)
    {
        if (Instance != null)
        {
            Instance.StartCoroutine(Instance.FadeOutCoroutine(onComplete));
        }
    }

    private IEnumerator FadeOutCoroutine(System.Action onComplete)
    {
        yield return StartCoroutine(Fade(0, 1));
        onComplete?.Invoke();
    }

    /// <summary>
    /// Quick fade in
    /// </summary>
    public static void FadeIn(System.Action onComplete = null)
    {
        if (Instance != null)
        {
            Instance.StartCoroutine(Instance.FadeInCoroutine(onComplete));
        }
    }

    private IEnumerator FadeInCoroutine(System.Action onComplete)
    {
        yield return StartCoroutine(Fade(1, 0));
        onComplete?.Invoke();
    }

    /// <summary>
    /// Set the fade color
    /// </summary>
    public static void SetFadeColor(Color color)
    {
        if (Instance != null && Instance.fadeImage != null)
        {
            Instance.fadeColor = color;
            Color c = Instance.fadeImage.color;
            Instance.fadeImage.color = new Color(color.r, color.g, color.b, c.a);
        }
    }
}
