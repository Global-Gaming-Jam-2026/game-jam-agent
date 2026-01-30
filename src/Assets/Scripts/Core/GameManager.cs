using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GameState { Initializing, Playing, Paused, Won, Lost }
    public GameState CurrentState { get; private set; } = GameState.Initializing;

    public event System.Action<GameState> OnGameStateChanged;
    public event System.Action OnGameStart;

    [Header("References")]
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject winScreen;
    [SerializeField] private GameObject loseScreen;

    [Header("Settings")]
    [SerializeField] private bool autoStartGame = true;
    [SerializeField] private float autoStartDelay = 0.5f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        // Auto-start if no intro system or if configured to
        if (autoStartGame && IntroSequence.Instance == null)
        {
            Invoke(nameof(StartGame), autoStartDelay);
        }
        else if (autoStartGame)
        {
            // Wait for intro to complete
            CurrentState = GameState.Initializing;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && CurrentState == GameState.Playing)
        {
            PauseGame();
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && CurrentState == GameState.Paused)
        {
            ResumeGame();
        }
    }

    public void SetState(GameState newState)
    {
        CurrentState = newState;
        OnGameStateChanged?.Invoke(newState);

        switch (newState)
        {
            case GameState.Initializing:
                Time.timeScale = 1f;
                break;

            case GameState.Playing:
                Time.timeScale = 1f;
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlayMusic(MusicType.Battle);
                }
                break;

            case GameState.Paused:
                Time.timeScale = 0f;
                if (pauseMenu != null) pauseMenu.SetActive(true);
                break;

            case GameState.Won:
                Time.timeScale = 0.3f; // Slow-mo for victory
                if (winScreen != null) winScreen.SetActive(true);
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlayMusic(MusicType.Victory);
                }
                if (HitFeedback.Instance != null)
                {
                    HitFeedback.Instance.BossDeathFeedback();
                }
                break;

            case GameState.Lost:
                Time.timeScale = 0.5f; // Slow-mo for defeat
                if (loseScreen != null) loseScreen.SetActive(true);
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlayMusic(MusicType.Defeat);
                }
                break;
        }
    }

    /// <summary>
    /// Start the actual gameplay (called after intro or immediately)
    /// </summary>
    public void StartGame()
    {
        Debug.Log("[GameManager] Game Started!");
        SetState(GameState.Playing);
        OnGameStart?.Invoke();
    }

    public void PauseGame()
    {
        SetState(GameState.Paused);
    }

    public void ResumeGame()
    {
        if (pauseMenu != null) pauseMenu.SetActive(false);
        SetState(GameState.Playing);
    }

    public void PlayerWon()
    {
        SetState(GameState.Won);
    }

    public void PlayerLost()
    {
        SetState(GameState.Lost);
    }

    public void RestartGame()
    {
        // Use screen transition if available
        if (UIScreenTransition.Instance != null)
        {
            UIScreenTransition.Instance.FadeTransition(() =>
            {
                Time.timeScale = 1f;
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            });
        }
        else
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    public void LoadMainMenu()
    {
        if (UIScreenTransition.Instance != null)
        {
            UIScreenTransition.Instance.FadeTransition(() =>
            {
                Time.timeScale = 1f;
                SceneManager.LoadScene("MainMenu");
            });
        }
        else
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("MainMenu");
        }
    }

    public void QuitGame()
    {
        if (UIScreenTransition.Instance != null)
        {
            UIScreenTransition.Instance.FadeOut(() =>
            {
                #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
                #else
                Application.Quit();
                #endif
            });
        }
        else
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        }
    }
}
