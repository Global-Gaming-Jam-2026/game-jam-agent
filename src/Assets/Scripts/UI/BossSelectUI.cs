using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Boss selection UI - allows player to pick which boss to fight.
/// Works similar to HeroSelectUI but for bosses.
/// </summary>
public class BossSelectUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Transform bossButtonContainer;
    [SerializeField] private GameObject bossButtonPrefab;
    [SerializeField] private Image bossPreviewImage;
    [SerializeField] private Text bossNameText;
    [SerializeField] private Text bossDescriptionText;
    [SerializeField] private Text bossStatsText;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button backButton;

    [Header("Preview")]
    [SerializeField] private Image bossColorPreview;

    private int selectedBossIndex = 0;
    private GameConfig gameConfig;

    private void Awake()
    {
        gameConfig = GameConfig.Instance;
    }

    private void Start()
    {
        if (gameConfig == null)
        {
            Debug.LogWarning("GameConfig not found. Boss selection disabled.");
            return;
        }

        PopulateBossButtons();
        SelectBoss(0);

        if (confirmButton != null)
        {
            confirmButton.onClick.AddListener(ConfirmSelection);
        }

        if (backButton != null)
        {
            backButton.onClick.AddListener(GoBack);
        }
    }

    private void PopulateBossButtons()
    {
        if (bossButtonContainer == null || gameConfig.availableBosses == null) return;

        // Clear existing buttons
        foreach (Transform child in bossButtonContainer)
        {
            Destroy(child.gameObject);
        }

        // Create button for each boss
        for (int i = 0; i < gameConfig.availableBosses.Count; i++)
        {
            var boss = gameConfig.availableBosses[i];
            int bossIndex = i;

            GameObject button;
            if (bossButtonPrefab != null)
            {
                button = Instantiate(bossButtonPrefab, bossButtonContainer);
            }
            else
            {
                button = CreateDefaultBossButton(boss);
                button.transform.SetParent(bossButtonContainer);
            }

            var buttonComponent = button.GetComponent<Button>();
            if (buttonComponent != null)
            {
                buttonComponent.onClick.AddListener(() => SelectBoss(bossIndex));
            }

            var image = button.GetComponent<Image>();
            if (image != null)
            {
                image.color = boss.primaryColor;
            }

            var text = button.GetComponentInChildren<Text>();
            if (text != null)
            {
                text.text = boss.bossName;
            }
        }
    }

    private GameObject CreateDefaultBossButton(BossData boss)
    {
        GameObject button = new GameObject(boss.bossName + "_Button");

        var rect = button.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(180, 80);

        var image = button.AddComponent<Image>();
        image.color = boss.primaryColor;

        var buttonComp = button.AddComponent<Button>();
        buttonComp.targetGraphic = image;

        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(button.transform);

        var textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;

        var text = textObj.AddComponent<Text>();
        text.text = boss.bossName;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.white;
        text.fontSize = 20;

        return button;
    }

    public void SelectBoss(int index)
    {
        if (gameConfig == null || index < 0 || index >= gameConfig.availableBosses.Count) return;

        selectedBossIndex = index;
        var boss = gameConfig.availableBosses[index];

        UpdateBossPreview(boss);
        gameConfig.SelectBoss(index);
    }

    private void UpdateBossPreview(BossData boss)
    {
        if (bossNameText != null)
        {
            bossNameText.text = boss.bossName;
        }

        if (bossDescriptionText != null)
        {
            bossDescriptionText.text = boss.description;
        }

        if (bossStatsText != null)
        {
            string phaseInfo = boss.phases != null ? $"{boss.phases.Count} phases" : "1 phase";
            bossStatsText.text = $"HP: {boss.maxHealth}\n" +
                                 $"Phases: {phaseInfo}\n" +
                                 $"Difficulty: {GetDifficultyRating(boss)}";
        }

        if (bossPreviewImage != null)
        {
            if (boss.portrait != null)
            {
                bossPreviewImage.sprite = boss.portrait;
            }
            bossPreviewImage.color = boss.primaryColor;
        }

        if (bossColorPreview != null)
        {
            bossColorPreview.color = boss.primaryColor;
        }
    }

    private string GetDifficultyRating(BossData boss)
    {
        // Simple difficulty calculation based on HP and phases
        float difficulty = boss.maxHealth / 100f;
        if (boss.phases != null) difficulty += boss.phases.Count * 0.5f;

        if (difficulty < 5) return "Easy";
        if (difficulty < 7) return "Medium";
        if (difficulty < 9) return "Hard";
        return "Extreme";
    }

    private void ConfirmSelection()
    {
        // Proceed to game or next selection screen
        UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
    }

    private void GoBack()
    {
        gameObject.SetActive(false);
    }

    public void NextBoss()
    {
        int newIndex = (selectedBossIndex + 1) % gameConfig.availableBosses.Count;
        SelectBoss(newIndex);
    }

    public void PreviousBoss()
    {
        int newIndex = selectedBossIndex - 1;
        if (newIndex < 0) newIndex = gameConfig.availableBosses.Count - 1;
        SelectBoss(newIndex);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            NextBoss();
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            PreviousBoss();
        }
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            ConfirmSelection();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GoBack();
        }
    }
}
