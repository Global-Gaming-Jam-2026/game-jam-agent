using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Main menu UI controller
/// </summary>
public class MainMenuUI : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button quitButton;

    [Header("Title")]
    [SerializeField] private Text titleText;
    [SerializeField] private string gameTitle = "MASK OF THE\nBRONZE GOD";

    [Header("Scene")]
    [SerializeField] private string gameSceneName = "Game";

    [Header("Animation")]
    [SerializeField] private float titlePulseSpeed = 1f;
    [SerializeField] private float titlePulseAmount = 0.05f;

    private Vector3 titleOriginalScale;

    private void Start()
    {
        // Set title
        if (titleText != null)
        {
            titleText.text = gameTitle;
            titleOriginalScale = titleText.transform.localScale;
        }

        // Setup buttons
        if (playButton != null)
        {
            playButton.onClick.AddListener(OnPlayClicked);
        }

        if (quitButton != null)
        {
            quitButton.onClick.AddListener(OnQuitClicked);
        }

        // Ensure time is running
        Time.timeScale = 1f;
    }

    private void Update()
    {
        // Title pulse animation
        if (titleText != null)
        {
            float pulse = 1f + Mathf.Sin(Time.time * titlePulseSpeed) * titlePulseAmount;
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
        SceneManager.LoadScene(gameSceneName);
    }

    private void OnQuitClicked()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}
