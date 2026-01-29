using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Settings menu for game options.
/// Persists settings using PlayerPrefs.
/// </summary>
public class SettingsUI : MonoBehaviour
{
    [Header("Audio Settings")]
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private Text masterVolumeText;
    [SerializeField] private Text musicVolumeText;
    [SerializeField] private Text sfxVolumeText;

    [Header("Gameplay Settings")]
    [SerializeField] private Toggle screenShakeToggle;
    [SerializeField] private Toggle tutorialToggle;
    [SerializeField] private Slider difficultySlider;
    [SerializeField] private Text difficultyText;

    [Header("Display Settings")]
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private Toggle vsyncToggle;

    [Header("Controls Display")]
    [SerializeField] private GameObject controlsPanel;

    [Header("Buttons")]
    [SerializeField] private Button applyButton;
    [SerializeField] private Button resetButton;
    [SerializeField] private Button backButton;

    // Default values
    private const float DEFAULT_MASTER_VOLUME = 1f;
    private const float DEFAULT_MUSIC_VOLUME = 0.7f;
    private const float DEFAULT_SFX_VOLUME = 1f;
    private const bool DEFAULT_SCREEN_SHAKE = true;
    private const bool DEFAULT_TUTORIAL = true;
    private const float DEFAULT_DIFFICULTY = 1f;

    // PlayerPrefs keys
    private const string KEY_MASTER_VOLUME = "MasterVolume";
    private const string KEY_MUSIC_VOLUME = "MusicVolume";
    private const string KEY_SFX_VOLUME = "SFXVolume";
    private const string KEY_SCREEN_SHAKE = "ScreenShake";
    private const string KEY_TUTORIAL = "Tutorial";
    private const string KEY_DIFFICULTY = "Difficulty";
    private const string KEY_FULLSCREEN = "Fullscreen";
    private const string KEY_VSYNC = "VSync";

    // Static access for other scripts
    public static float MasterVolume { get; private set; } = DEFAULT_MASTER_VOLUME;
    public static float MusicVolume { get; private set; } = DEFAULT_MUSIC_VOLUME;
    public static float SFXVolume { get; private set; } = DEFAULT_SFX_VOLUME;
    public static bool ScreenShakeEnabled { get; private set; } = DEFAULT_SCREEN_SHAKE;
    public static bool TutorialEnabled { get; private set; } = DEFAULT_TUTORIAL;
    public static float DifficultyMultiplier { get; private set; } = DEFAULT_DIFFICULTY;

    private void Awake()
    {
        LoadSettings();
    }

    private void Start()
    {
        SetupSliders();
        SetupToggles();
        SetupButtons();
        UpdateUI();
    }

    private void SetupSliders()
    {
        if (masterVolumeSlider != null)
        {
            masterVolumeSlider.minValue = 0f;
            masterVolumeSlider.maxValue = 1f;
            masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
        }

        if (musicVolumeSlider != null)
        {
            musicVolumeSlider.minValue = 0f;
            musicVolumeSlider.maxValue = 1f;
            musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        }

        if (sfxVolumeSlider != null)
        {
            sfxVolumeSlider.minValue = 0f;
            sfxVolumeSlider.maxValue = 1f;
            sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
        }

        if (difficultySlider != null)
        {
            difficultySlider.minValue = 0.5f;
            difficultySlider.maxValue = 2f;
            difficultySlider.onValueChanged.AddListener(OnDifficultyChanged);
        }
    }

    private void SetupToggles()
    {
        if (screenShakeToggle != null)
        {
            screenShakeToggle.onValueChanged.AddListener(OnScreenShakeChanged);
        }

        if (tutorialToggle != null)
        {
            tutorialToggle.onValueChanged.AddListener(OnTutorialChanged);
        }

        if (fullscreenToggle != null)
        {
            fullscreenToggle.onValueChanged.AddListener(OnFullscreenChanged);
        }

        if (vsyncToggle != null)
        {
            vsyncToggle.onValueChanged.AddListener(OnVSyncChanged);
        }
    }

    private void SetupButtons()
    {
        if (applyButton != null)
        {
            applyButton.onClick.AddListener(ApplySettings);
        }

        if (resetButton != null)
        {
            resetButton.onClick.AddListener(ResetToDefaults);
        }

        if (backButton != null)
        {
            backButton.onClick.AddListener(GoBack);
        }
    }

    private void UpdateUI()
    {
        if (masterVolumeSlider != null) masterVolumeSlider.value = MasterVolume;
        if (musicVolumeSlider != null) musicVolumeSlider.value = MusicVolume;
        if (sfxVolumeSlider != null) sfxVolumeSlider.value = SFXVolume;
        if (screenShakeToggle != null) screenShakeToggle.isOn = ScreenShakeEnabled;
        if (tutorialToggle != null) tutorialToggle.isOn = TutorialEnabled;
        if (difficultySlider != null) difficultySlider.value = DifficultyMultiplier;
        if (fullscreenToggle != null) fullscreenToggle.isOn = Screen.fullScreen;
        if (vsyncToggle != null) vsyncToggle.isOn = QualitySettings.vSyncCount > 0;

        UpdateVolumeTexts();
        UpdateDifficultyText();
    }

    private void OnMasterVolumeChanged(float value)
    {
        MasterVolume = value;
        AudioListener.volume = value;
        UpdateVolumeTexts();
    }

    private void OnMusicVolumeChanged(float value)
    {
        MusicVolume = value;
        UpdateVolumeTexts();
        // Apply to music audio source if exists
    }

    private void OnSFXVolumeChanged(float value)
    {
        SFXVolume = value;
        UpdateVolumeTexts();
    }

    private void OnScreenShakeChanged(bool value)
    {
        ScreenShakeEnabled = value;
        if (GameConfig.Instance != null)
        {
            GameConfig.Instance.screenShakeEnabled = value;
        }
    }

    private void OnTutorialChanged(bool value)
    {
        TutorialEnabled = value;
        if (GameConfig.Instance != null)
        {
            GameConfig.Instance.tutorialEnabled = value;
        }
    }

    private void OnDifficultyChanged(float value)
    {
        DifficultyMultiplier = value;
        if (GameConfig.Instance != null)
        {
            GameConfig.Instance.difficultyMultiplier = value;
        }
        UpdateDifficultyText();
    }

    private void OnFullscreenChanged(bool value)
    {
        Screen.fullScreen = value;
    }

    private void OnVSyncChanged(bool value)
    {
        QualitySettings.vSyncCount = value ? 1 : 0;
    }

    private void UpdateVolumeTexts()
    {
        if (masterVolumeText != null) masterVolumeText.text = $"{Mathf.RoundToInt(MasterVolume * 100)}%";
        if (musicVolumeText != null) musicVolumeText.text = $"{Mathf.RoundToInt(MusicVolume * 100)}%";
        if (sfxVolumeText != null) sfxVolumeText.text = $"{Mathf.RoundToInt(SFXVolume * 100)}%";
    }

    private void UpdateDifficultyText()
    {
        if (difficultyText != null)
        {
            string label;
            if (DifficultyMultiplier <= 0.6f) label = "Easy";
            else if (DifficultyMultiplier <= 1.1f) label = "Normal";
            else if (DifficultyMultiplier <= 1.5f) label = "Hard";
            else label = "Extreme";

            difficultyText.text = label;
        }
    }

    public void ApplySettings()
    {
        SaveSettings();
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetFloat(KEY_MASTER_VOLUME, MasterVolume);
        PlayerPrefs.SetFloat(KEY_MUSIC_VOLUME, MusicVolume);
        PlayerPrefs.SetFloat(KEY_SFX_VOLUME, SFXVolume);
        PlayerPrefs.SetInt(KEY_SCREEN_SHAKE, ScreenShakeEnabled ? 1 : 0);
        PlayerPrefs.SetInt(KEY_TUTORIAL, TutorialEnabled ? 1 : 0);
        PlayerPrefs.SetFloat(KEY_DIFFICULTY, DifficultyMultiplier);
        PlayerPrefs.SetInt(KEY_FULLSCREEN, Screen.fullScreen ? 1 : 0);
        PlayerPrefs.SetInt(KEY_VSYNC, QualitySettings.vSyncCount);
        PlayerPrefs.Save();
    }

    public static void LoadSettings()
    {
        MasterVolume = PlayerPrefs.GetFloat(KEY_MASTER_VOLUME, DEFAULT_MASTER_VOLUME);
        MusicVolume = PlayerPrefs.GetFloat(KEY_MUSIC_VOLUME, DEFAULT_MUSIC_VOLUME);
        SFXVolume = PlayerPrefs.GetFloat(KEY_SFX_VOLUME, DEFAULT_SFX_VOLUME);
        ScreenShakeEnabled = PlayerPrefs.GetInt(KEY_SCREEN_SHAKE, 1) == 1;
        TutorialEnabled = PlayerPrefs.GetInt(KEY_TUTORIAL, 1) == 1;
        DifficultyMultiplier = PlayerPrefs.GetFloat(KEY_DIFFICULTY, DEFAULT_DIFFICULTY);

        // Apply loaded settings
        AudioListener.volume = MasterVolume;
        Screen.fullScreen = PlayerPrefs.GetInt(KEY_FULLSCREEN, 0) == 1;
        QualitySettings.vSyncCount = PlayerPrefs.GetInt(KEY_VSYNC, 1);
    }

    public void ResetToDefaults()
    {
        MasterVolume = DEFAULT_MASTER_VOLUME;
        MusicVolume = DEFAULT_MUSIC_VOLUME;
        SFXVolume = DEFAULT_SFX_VOLUME;
        ScreenShakeEnabled = DEFAULT_SCREEN_SHAKE;
        TutorialEnabled = DEFAULT_TUTORIAL;
        DifficultyMultiplier = DEFAULT_DIFFICULTY;

        UpdateUI();
        ApplySettings();
    }

    public void ShowControls()
    {
        if (controlsPanel != null)
        {
            controlsPanel.SetActive(true);
        }
    }

    public void HideControls()
    {
        if (controlsPanel != null)
        {
            controlsPanel.SetActive(false);
        }
    }

    private void GoBack()
    {
        SaveSettings();
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GoBack();
        }
    }
}
