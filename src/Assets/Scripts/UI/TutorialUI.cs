using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Tutorial/How to Play UI screen.
/// Can be shown before game starts or from pause menu.
/// </summary>
public class TutorialUI : MonoBehaviour
{
    [Header("Pages")]
    [SerializeField] private GameObject[] tutorialPages;
    [SerializeField] private int currentPage = 0;

    [Header("Navigation")]
    [SerializeField] private Button nextButton;
    [SerializeField] private Button prevButton;
    [SerializeField] private Button skipButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private Text pageIndicatorText;

    [Header("Content Panels")]
    [SerializeField] private GameObject movementPanel;
    [SerializeField] private GameObject combatPanel;
    [SerializeField] private GameObject parryPanel;
    [SerializeField] private GameObject bossPanel;

    [Header("Animation")]
    [SerializeField] private float fadeTime = 0.3f;

    [Header("Auto-create content")]
    [SerializeField] private bool autoCreateContent = true;

    private CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    private void Start()
    {
        if (autoCreateContent && (tutorialPages == null || tutorialPages.Length == 0))
        {
            CreateDefaultPages();
        }

        SetupButtons();
        ShowPage(0);
    }

    private void CreateDefaultPages()
    {
        // Create container for pages
        tutorialPages = new GameObject[4];

        // Page 1: Movement
        tutorialPages[0] = CreateTutorialPage("Movement",
            "MOVEMENT\n\n" +
            "WASD or Arrow Keys - Move\n\n" +
            "SPACE - Dodge Roll\n" +
            "(Invincible while rolling!)\n\n" +
            "Dodge through attacks to avoid damage.");

        // Page 2: Combat
        tutorialPages[1] = CreateTutorialPage("Combat",
            "COMBAT\n\n" +
            "Left Mouse Button - Attack\n\n" +
            "Chain up to 3 hits for a combo!\n" +
            "Final hit deals bonus damage.\n\n" +
            "Attack the boss to deal damage.");

        // Page 3: Parry
        tutorialPages[2] = CreateTutorialPage("Parry",
            "PARRY SYSTEM\n\n" +
            "Look for PINK projectiles!\n\n" +
            "Press SPACE near pink bullets\n" +
            "to slap them away.\n\n" +
            "Successful parries fill your\n" +
            "SUPER METER for bonus damage!");

        // Page 4: Boss Fight
        tutorialPages[3] = CreateTutorialPage("Boss Fight",
            "BOSS MECHANICS\n\n" +
            "Watch for TELEGRAPH warnings\n" +
            "before each attack!\n\n" +
            "Boss has 2 PHASES:\n" +
            "- Phase 1: Basic attacks\n" +
            "- Phase 2: More patterns, faster!\n\n" +
            "Deplete boss HP to WIN!");

        // Hide all initially
        foreach (var page in tutorialPages)
        {
            page.SetActive(false);
        }
    }

    private GameObject CreateTutorialPage(string title, string content)
    {
        GameObject page = new GameObject(title + "_Page");
        page.transform.SetParent(transform);

        var rect = page.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.1f, 0.1f);
        rect.anchorMax = new Vector2(0.9f, 0.85f);
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        // Background
        var bg = page.AddComponent<Image>();
        bg.color = new Color(0.15f, 0.1f, 0.05f, 0.95f);

        // Text
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(page.transform);

        var textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = new Vector2(0.05f, 0.05f);
        textRect.anchorMax = new Vector2(0.95f, 0.95f);
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        var text = textObj.AddComponent<Text>();
        text.text = content;
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = 28;
        text.color = new Color(1f, 0.95f, 0.85f);
        text.alignment = TextAnchor.MiddleCenter;
        text.lineSpacing = 1.2f;

        return page;
    }

    private void SetupButtons()
    {
        if (nextButton != null)
        {
            nextButton.onClick.AddListener(NextPage);
        }

        if (prevButton != null)
        {
            prevButton.onClick.AddListener(PrevPage);
        }

        if (skipButton != null)
        {
            skipButton.onClick.AddListener(CloseTutorial);
        }

        if (closeButton != null)
        {
            closeButton.onClick.AddListener(CloseTutorial);
        }
    }

    public void ShowPage(int index)
    {
        if (tutorialPages == null || tutorialPages.Length == 0) return;

        currentPage = Mathf.Clamp(index, 0, tutorialPages.Length - 1);

        // Hide all pages
        foreach (var page in tutorialPages)
        {
            if (page != null) page.SetActive(false);
        }

        // Show current page
        if (tutorialPages[currentPage] != null)
        {
            tutorialPages[currentPage].SetActive(true);
        }

        UpdatePageIndicator();
        UpdateNavigationButtons();
    }

    private void UpdatePageIndicator()
    {
        if (pageIndicatorText != null && tutorialPages != null)
        {
            pageIndicatorText.text = $"{currentPage + 1} / {tutorialPages.Length}";
        }
    }

    private void UpdateNavigationButtons()
    {
        if (prevButton != null)
        {
            prevButton.interactable = currentPage > 0;
        }

        if (nextButton != null)
        {
            // Change to "Start" on last page
            var text = nextButton.GetComponentInChildren<Text>();
            if (text != null)
            {
                text.text = currentPage >= tutorialPages.Length - 1 ? "Start!" : "Next";
            }
        }
    }

    public void NextPage()
    {
        if (currentPage >= tutorialPages.Length - 1)
        {
            CloseTutorial();
        }
        else
        {
            ShowPage(currentPage + 1);
        }
    }

    public void PrevPage()
    {
        ShowPage(currentPage - 1);
    }

    public void CloseTutorial()
    {
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        float elapsed = 0;
        while (elapsed < fadeTime)
        {
            canvasGroup.alpha = 1 - (elapsed / fadeTime);
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        gameObject.SetActive(false);
        canvasGroup.alpha = 1;

        // Check if we should start the game
        if (GameManager.Instance != null && GameManager.Instance.CurrentState != GameManager.GameState.Playing)
        {
            // Resume or start game
        }
    }

    public void Show()
    {
        gameObject.SetActive(true);
        ShowPage(0);
        StartCoroutine(FadeIn());
    }

    private IEnumerator FadeIn()
    {
        canvasGroup.alpha = 0;
        float elapsed = 0;
        while (elapsed < fadeTime)
        {
            canvasGroup.alpha = elapsed / fadeTime;
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }
        canvasGroup.alpha = 1;
    }

    private void Update()
    {
        // Keyboard navigation
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.Space))
        {
            NextPage();
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            PrevPage();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseTutorial();
        }
    }

    /// <summary>
    /// Check if tutorial should be shown (based on settings)
    /// </summary>
    public static bool ShouldShowTutorial()
    {
        return SettingsUI.TutorialEnabled && PlayerPrefs.GetInt("TutorialShown", 0) == 0;
    }

    /// <summary>
    /// Mark tutorial as shown
    /// </summary>
    public static void MarkTutorialShown()
    {
        PlayerPrefs.SetInt("TutorialShown", 1);
        PlayerPrefs.Save();
    }
}
