using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles win/lose screen UI
/// </summary>
public class GameOverUI : MonoBehaviour
{
    [Header("Win Screen")]
    [SerializeField] private GameObject winPanel;
    [SerializeField] private Text winText;
    [SerializeField] private Button winRestartButton;
    [SerializeField] private Button winMenuButton;

    [Header("Lose Screen")]
    [SerializeField] private GameObject losePanel;
    [SerializeField] private Text loseText;
    [SerializeField] private Button loseRestartButton;
    [SerializeField] private Button loseMenuButton;

    [Header("Text Content")]
    [SerializeField] private string winMessage = "VICTORY";
    [SerializeField] private string loseMessage = "YOU DIED";

    [Header("Animation")]
    [SerializeField] private float fadeInDuration = 0.5f;

    private CanvasGroup winCanvasGroup;
    private CanvasGroup loseCanvasGroup;

    private void Start()
    {
        // Setup canvas groups for fade
        if (winPanel != null)
        {
            winCanvasGroup = winPanel.GetComponent<CanvasGroup>();
            if (winCanvasGroup == null)
            {
                winCanvasGroup = winPanel.AddComponent<CanvasGroup>();
            }
            winPanel.SetActive(false);
        }

        if (losePanel != null)
        {
            loseCanvasGroup = losePanel.GetComponent<CanvasGroup>();
            if (loseCanvasGroup == null)
            {
                loseCanvasGroup = losePanel.AddComponent<CanvasGroup>();
            }
            losePanel.SetActive(false);
        }

        // Set text
        if (winText != null) winText.text = winMessage;
        if (loseText != null) loseText.text = loseMessage;

        // Setup buttons
        SetupButtons();

        // Subscribe to game state changes
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameStateChanged += OnGameStateChanged;
        }
    }

    private void SetupButtons()
    {
        if (winRestartButton != null)
        {
            winRestartButton.onClick.AddListener(OnRestartClicked);
        }
        if (winMenuButton != null)
        {
            winMenuButton.onClick.AddListener(OnMenuClicked);
        }
        if (loseRestartButton != null)
        {
            loseRestartButton.onClick.AddListener(OnRestartClicked);
        }
        if (loseMenuButton != null)
        {
            loseMenuButton.onClick.AddListener(OnMenuClicked);
        }
    }

    private void OnGameStateChanged(GameManager.GameState state)
    {
        switch (state)
        {
            case GameManager.GameState.Won:
                ShowWinScreen();
                break;
            case GameManager.GameState.Lost:
                ShowLoseScreen();
                break;
        }
    }

    private void ShowWinScreen()
    {
        if (winPanel != null)
        {
            winPanel.SetActive(true);
            StartCoroutine(FadeIn(winCanvasGroup));
        }
    }

    private void ShowLoseScreen()
    {
        if (losePanel != null)
        {
            losePanel.SetActive(true);
            StartCoroutine(FadeIn(loseCanvasGroup));
        }
    }

    private System.Collections.IEnumerator FadeIn(CanvasGroup canvasGroup)
    {
        if (canvasGroup == null) yield break;

        canvasGroup.alpha = 0;
        float elapsed = 0;

        while (elapsed < fadeInDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(0, 1, elapsed / fadeInDuration);
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        canvasGroup.alpha = 1;
    }

    private void OnRestartClicked()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RestartGame();
        }
    }

    private void OnMenuClicked()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.LoadMainMenu();
        }
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameStateChanged -= OnGameStateChanged;
        }
    }
}
