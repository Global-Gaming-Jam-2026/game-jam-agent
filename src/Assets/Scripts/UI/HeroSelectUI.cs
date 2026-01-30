using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Hero selection UI for the main menu.
/// Displays available heroes and allows player to select one before starting.
/// </summary>
public class HeroSelectUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Transform heroButtonContainer;
    [SerializeField] private GameObject heroButtonPrefab;
    [SerializeField] private Image heroPreviewImage;
    [SerializeField] private Text heroNameText;
    [SerializeField] private Text heroDescriptionText;
    [SerializeField] private Text heroStatsText;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button backButton;

    [Header("Preview Colors")]
    [SerializeField] private Image heroColorPreview;

    private int selectedHeroIndex = 0;
    private GameConfig gameConfig;

    private void Awake()
    {
        gameConfig = GameConfig.Instance;
    }

    private void Start()
    {
        if (gameConfig == null)
        {
            Debug.LogWarning("GameConfig not found. Hero selection disabled.");
            return;
        }

        PopulateHeroButtons();
        SelectHero(0);

        if (confirmButton != null)
        {
            confirmButton.onClick.AddListener(ConfirmSelection);
        }

        if (backButton != null)
        {
            backButton.onClick.AddListener(GoBack);
        }
    }

    private void PopulateHeroButtons()
    {
        if (heroButtonContainer == null || gameConfig.availableHeroes == null) return;

        // Clear existing buttons
        foreach (Transform child in heroButtonContainer)
        {
            Destroy(child.gameObject);
        }

        // Create button for each hero
        for (int i = 0; i < gameConfig.availableHeroes.Count; i++)
        {
            var hero = gameConfig.availableHeroes[i];
            int heroIndex = i; // Capture for closure

            GameObject button;
            if (heroButtonPrefab != null)
            {
                button = Instantiate(heroButtonPrefab, heroButtonContainer);
            }
            else
            {
                button = CreateDefaultHeroButton(hero);
                button.transform.SetParent(heroButtonContainer);
            }

            // Setup button
            var buttonComponent = button.GetComponent<Button>();
            if (buttonComponent != null)
            {
                buttonComponent.onClick.AddListener(() => SelectHero(heroIndex));
            }

            // Set button color to hero's primary color
            var image = button.GetComponent<Image>();
            if (image != null)
            {
                image.color = hero.primaryColor;
            }

            // Set button text
            var text = button.GetComponentInChildren<Text>();
            if (text != null)
            {
                text.text = hero.heroName;
            }
        }
    }

    private GameObject CreateDefaultHeroButton(HeroData hero)
    {
        GameObject button = new GameObject(hero.heroName + "_Button");

        var rect = button.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(150, 60);

        var image = button.AddComponent<Image>();
        image.color = hero.primaryColor;

        var buttonComp = button.AddComponent<Button>();
        buttonComp.targetGraphic = image;

        // Add text child
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(button.transform);

        var textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        var text = textObj.AddComponent<Text>();
        text.text = hero.heroName;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.white;
        text.fontSize = 18;

        return button;
    }

    public void SelectHero(int index)
    {
        if (gameConfig == null || index < 0 || index >= gameConfig.availableHeroes.Count) return;

        selectedHeroIndex = index;
        var hero = gameConfig.availableHeroes[index];

        // Update preview
        UpdateHeroPreview(hero);

        // Update GameConfig selection
        gameConfig.SelectHero(index);
    }

    private void UpdateHeroPreview(HeroData hero)
    {
        // Update name
        if (heroNameText != null)
        {
            heroNameText.text = hero.heroName;
        }

        // Update description
        if (heroDescriptionText != null)
        {
            heroDescriptionText.text = hero.description;
        }

        // Update stats
        if (heroStatsText != null)
        {
            heroStatsText.text = $"HP: {hero.maxHealth}\n" +
                                 $"Speed: {hero.moveSpeed}\n" +
                                 $"Damage: {hero.attackDamage}\n" +
                                 $"Ability: {GetAbilityName(hero.specialAbility)}";
        }

        // Update preview image
        if (heroPreviewImage != null)
        {
            if (hero.portrait != null)
            {
                heroPreviewImage.sprite = hero.portrait;
            }
            heroPreviewImage.color = hero.primaryColor;
        }

        // Update color preview
        if (heroColorPreview != null)
        {
            heroColorPreview.color = hero.primaryColor;
        }
    }

    private string GetAbilityName(HeroAbilityType ability)
    {
        return ability switch
        {
            HeroAbilityType.DoubleJump => "Double Jump",
            HeroAbilityType.QuickDash => "Quick Dash",
            HeroAbilityType.HeavyHitter => "Heavy Hitter",
            HeroAbilityType.ParryMaster => "Parry Master",
            HeroAbilityType.Berserker => "Berserker",
            HeroAbilityType.Lifesteal => "Lifesteal",
            HeroAbilityType.CounterAttack => "Counter Attack",
            _ => "None"
        };
    }

    private void ConfirmSelection()
    {
        // Start the game with selected hero
        if (UnityEngine.SceneManagement.SceneManager.GetSceneByName("Game") != null)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
        }
    }

    private void GoBack()
    {
        // Return to main menu
        gameObject.SetActive(false);

        // Or load main menu scene
        // UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

    /// <summary>
    /// Cycle to next hero (for controller/keyboard navigation)
    /// </summary>
    public void NextHero()
    {
        int newIndex = (selectedHeroIndex + 1) % gameConfig.availableHeroes.Count;
        SelectHero(newIndex);
    }

    /// <summary>
    /// Cycle to previous hero (for controller/keyboard navigation)
    /// </summary>
    public void PreviousHero()
    {
        int newIndex = selectedHeroIndex - 1;
        if (newIndex < 0) newIndex = gameConfig.availableHeroes.Count - 1;
        SelectHero(newIndex);
    }

    private void Update()
    {
        // Keyboard navigation
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            NextHero();
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            PreviousHero();
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
