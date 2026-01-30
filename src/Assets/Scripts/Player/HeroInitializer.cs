using UnityEngine;

/// <summary>
/// Initializes the player with HeroData configuration.
/// Attach this to the Player GameObject alongside other player components.
/// Reads from GameConfig.selectedHero and applies stats/colors.
/// Sets up sprite animations using SpriteAnimator.
/// </summary>
public class HeroInitializer : MonoBehaviour
{
    [Header("Manual Override (optional)")]
    [Tooltip("If set, uses this instead of GameConfig")]
    [SerializeField] private HeroData heroOverride;

    [Header("Component References")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private PlayerCombat playerCombat;
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private PlayerParry playerParry;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private SpriteAnimator spriteAnimator;

    private HeroData currentHero;

    private void Awake()
    {
        // Auto-find components if not assigned
        if (playerController == null) playerController = GetComponent<PlayerController>();
        if (playerCombat == null) playerCombat = GetComponent<PlayerCombat>();
        if (playerHealth == null) playerHealth = GetComponent<PlayerHealth>();
        if (playerParry == null) playerParry = GetComponent<PlayerParry>();
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteAnimator == null) spriteAnimator = GetComponent<SpriteAnimator>();

        // Add SpriteAnimator if not present
        if (spriteAnimator == null)
        {
            spriteAnimator = gameObject.AddComponent<SpriteAnimator>();
        }
    }

    private void Start()
    {
        // Get hero data (override or from config)
        currentHero = heroOverride;
        if (currentHero == null && GameConfig.Instance != null)
        {
            currentHero = GameConfig.Instance.selectedHero;
        }

        // Fallback to RuntimeAssetLoader if no data found
        if (currentHero == null)
        {
            currentHero = RuntimeAssetLoader.GetDefaultHero();
            Debug.Log("[HeroInitializer] Using RuntimeAssetLoader default hero");
        }

        if (currentHero != null)
        {
            ApplyHeroData();
        }
        else
        {
            Debug.LogWarning("No HeroData found - applying fallback visuals");
            ApplyFallbackVisuals();
        }
    }

    private void ApplyFallbackVisuals()
    {
        // Apply runtime-generated sprite even without HeroData
        if (spriteRenderer != null)
        {
            var sprite = RuntimeAssetLoader.GetHeroSprite("BronzeWarrior");
            if (sprite != null)
            {
                spriteRenderer.sprite = sprite;
                Debug.Log("[HeroInitializer] Applied fallback sprite from RuntimeAssetLoader");
            }
            spriteRenderer.color = new Color(0.8f, 0.5f, 0.2f); // Bronze color
        }
    }

    /// <summary>
    /// Apply all hero data to player components
    /// </summary>
    public void ApplyHeroData()
    {
        if (currentHero == null) return;

        // Apply visuals
        ApplyVisuals();

        // Apply stats via reflection or direct access
        ApplyStats();

        Debug.Log($"Hero initialized: {currentHero.heroName}");
    }

    private void ApplyVisuals()
    {
        string heroKey = currentHero.heroName.Replace(" ", "");

        // Set up animation frames using SpriteAnimator
        if (spriteAnimator != null)
        {
            var idleFrames = RuntimeAssetLoader.GetHeroIdleFrames(heroKey);
            var attackFrames = RuntimeAssetLoader.GetHeroAttackFrames(heroKey);
            var dodgeFrames = RuntimeAssetLoader.GetHeroDodgeFrames(heroKey);
            var hurtFrames = RuntimeAssetLoader.GetHeroHurtFrames(heroKey);

            if (idleFrames != null)
            {
                spriteAnimator.SetupCharacterAnimations(idleFrames, idleFrames, attackFrames, dodgeFrames, hurtFrames);
                Debug.Log($"[HeroInitializer] Set up animations for {currentHero.heroName}");
            }
        }

        // Apply sprite (first frame of idle or fallback)
        if (spriteRenderer != null)
        {
            if (currentHero.idleSprite != null)
            {
                spriteRenderer.sprite = currentHero.idleSprite;
            }
            else
            {
                // Fallback to runtime-generated sprite
                var fallbackSprite = RuntimeAssetLoader.GetHeroSprite(heroKey);
                if (fallbackSprite != null)
                {
                    spriteRenderer.sprite = fallbackSprite;
                    Debug.Log($"[HeroInitializer] Using runtime sprite for {currentHero.heroName}");
                }
            }
            spriteRenderer.color = currentHero.primaryColor;
        }

        // Apply scale
        transform.localScale = Vector3.one * currentHero.spriteScale;

        // Apply animator if present (fallback to Unity Animator)
        var animator = GetComponent<Animator>();
        if (animator != null && currentHero.animatorController != null)
        {
            animator.runtimeAnimatorController = currentHero.animatorController;
        }

        // Subscribe to animation events
        SubscribeToAnimationEvents();
    }

    private void SubscribeToAnimationEvents()
    {
        // Subscribe to PlayerController events
        if (playerController != null)
        {
            playerController.OnDodgeStart += () =>
            {
                if (spriteAnimator != null && spriteAnimator.HasAnimation("dodge"))
                    spriteAnimator.Play("dodge");
            };

            playerController.OnDodgeEnd += () =>
            {
                if (spriteAnimator != null)
                    spriteAnimator.Play("idle");
            };
        }

        // Subscribe to PlayerCombat events
        if (playerCombat != null)
        {
            playerCombat.OnAttackStart += () =>
            {
                if (spriteAnimator != null && spriteAnimator.HasAnimation("attack"))
                    spriteAnimator.Play("attack");
            };

            playerCombat.OnAttackEnd += () =>
            {
                if (spriteAnimator != null)
                    spriteAnimator.Play("idle");
            };
        }

        // Subscribe to PlayerHealth events
        if (playerHealth != null)
        {
            playerHealth.OnDamaged += () =>
            {
                if (spriteAnimator != null && spriteAnimator.HasAnimation("hurt"))
                    spriteAnimator.Play("hurt");
            };
        }

        // Return to idle when hurt animation completes
        if (spriteAnimator != null)
        {
            spriteAnimator.OnAnimationComplete += (animName) =>
            {
                if (animName == "hurt" || animName == "attack")
                    spriteAnimator.Play("idle");
            };
        }
    }

    private void ApplyStats()
    {
        // Health
        if (playerHealth != null)
        {
            playerHealth.SetMaxHealth(currentHero.maxHealth);
            playerHealth.SetDamageFlashColor(currentHero.damageFlashColor);
        }

        // Movement speed
        if (playerController != null)
        {
            playerController.SetMoveSpeed(currentHero.moveSpeed);
            playerController.SetDodgeParams(currentHero.dodgeSpeed, currentHero.dodgeDuration);
        }

        // Attack damage
        if (playerCombat != null)
        {
            playerCombat.SetAttackDamage(currentHero.attackDamage);
        }

        Debug.Log($"Hero Stats Applied - HP: {currentHero.maxHealth}, Speed: {currentHero.moveSpeed}, Damage: {currentHero.attackDamage}");
    }

    /// <summary>
    /// Get the current hero data
    /// </summary>
    public HeroData GetHeroData()
    {
        return currentHero;
    }

    /// <summary>
    /// Change hero at runtime
    /// </summary>
    public void SetHero(HeroData newHero)
    {
        currentHero = newHero;
        ApplyHeroData();
    }

    /// <summary>
    /// Get hero color for external use (effects, particles, etc.)
    /// </summary>
    public Color GetPrimaryColor()
    {
        return currentHero != null ? currentHero.primaryColor : Color.white;
    }

    public Color GetAttackColor()
    {
        return currentHero != null ? currentHero.attackColor : Color.yellow;
    }
}
