using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Pause menu UI controller
/// </summary>
public class PauseMenuUI : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] private GameObject pausePanel;

    [Header("Buttons")]
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button menuButton;

    [Header("Text")]
    [SerializeField] private Text pauseText;
    [SerializeField] private string pauseMessage = "PAUSED";

    private void Start()
    {
        // Set text
        if (pauseText != null)
        {
            pauseText.text = pauseMessage;
        }

        // Hide panel initially
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }

        // Setup buttons
        if (resumeButton != null)
        {
            resumeButton.onClick.AddListener(OnResumeClicked);
        }

        if (restartButton != null)
        {
            restartButton.onClick.AddListener(OnRestartClicked);
        }

        if (menuButton != null)
        {
            menuButton.onClick.AddListener(OnMenuClicked);
        }

        // Subscribe to game state changes
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameStateChanged += OnGameStateChanged;
        }
    }

    private void OnGameStateChanged(GameManager.GameState state)
    {
        if (pausePanel != null)
        {
            pausePanel.SetActive(state == GameManager.GameState.Paused);
        }
    }

    private void OnResumeClicked()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResumeGame();
        }
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
