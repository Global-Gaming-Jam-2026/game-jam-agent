using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GameState { Playing, Paused, Won, Lost }
    public GameState CurrentState { get; private set; } = GameState.Playing;

    public event System.Action<GameState> OnGameStateChanged;

    [Header("References")]
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject winScreen;
    [SerializeField] private GameObject loseScreen;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
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
            case GameState.Playing:
                Time.timeScale = 1f;
                break;
            case GameState.Paused:
                Time.timeScale = 0f;
                if (pauseMenu != null) pauseMenu.SetActive(true);
                break;
            case GameState.Won:
                Time.timeScale = 0f;
                if (winScreen != null) winScreen.SetActive(true);
                break;
            case GameState.Lost:
                Time.timeScale = 0f;
                if (loseScreen != null) loseScreen.SetActive(true);
                break;
        }
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
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}
