using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// Manages floating damage numbers.
/// Pools and spawns damage text for hits, healing, and combat events.
/// </summary>
public class DamageNumberUI : MonoBehaviour
{
    public static DamageNumberUI Instance { get; private set; }

    [Header("Pool Settings")]
    [SerializeField] private int poolSize = 30;

    [Header("Display Settings")]
    [SerializeField] private float floatSpeed = 2f;
    [SerializeField] private float lifetime = 1f;
    [SerializeField] private float scaleMultiplier = 1f;
    [SerializeField] private float criticalScaleBonus = 0.5f;

    [Header("Colors - Official Palette from GAME_DESIGN.md")]
    [SerializeField] private Color normalDamageColor = new Color(0.761f, 0.698f, 0.502f);  // Sand #C2B280
    [SerializeField] private Color criticalDamageColor = new Color(0.8f, 0.467f, 0.133f);  // Ochre #CC7722
    [SerializeField] private Color bossDamageColor = new Color(0.886f, 0.447f, 0.357f);    // Terracotta #E2725B
    [SerializeField] private Color healColor = Color.green;
    [SerializeField] private Color parryColor = new Color(1f, 0.843f, 0f);                  // Gold
    [SerializeField] private Color chaosColor = new Color(0.424f, 0.361f, 0.906f);          // Deep Purple #6C5CE7

    [Header("Font Settings")]
    [SerializeField] private int baseFontSize = 32;

    private List<DamageNumber> numberPool = new List<DamageNumber>();
    private int currentIndex = 0;
    private Canvas worldCanvas;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        SetupWorldCanvas();
        InitializePool();
    }

    private void SetupWorldCanvas()
    {
        GameObject canvasObj = new GameObject("DamageNumberCanvas");
        canvasObj.transform.SetParent(transform);

        worldCanvas = canvasObj.AddComponent<Canvas>();
        worldCanvas.renderMode = RenderMode.WorldSpace;
        worldCanvas.sortingOrder = 200;

        var scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.dynamicPixelsPerUnit = 100;

        canvasObj.AddComponent<GraphicRaycaster>();
        canvasObj.transform.localScale = Vector3.one * 0.01f;
    }

    private void InitializePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            var number = CreateDamageNumber();
            number.gameObject.SetActive(false);
            numberPool.Add(number);
        }
    }

    private DamageNumber CreateDamageNumber()
    {
        GameObject obj = new GameObject($"DamageNumber_{numberPool.Count}");
        obj.transform.SetParent(worldCanvas.transform);

        var text = obj.AddComponent<Text>();
        text.alignment = TextAnchor.MiddleCenter;
        text.fontSize = baseFontSize;
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.horizontalOverflow = HorizontalWrapMode.Overflow;
        text.verticalOverflow = VerticalWrapMode.Overflow;

        var rectTransform = obj.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(200, 50);

        var number = obj.AddComponent<DamageNumber>();
        return number;
    }

    private DamageNumber GetNumber()
    {
        DamageNumber number = numberPool[currentIndex];
        currentIndex = (currentIndex + 1) % numberPool.Count;
        return number;
    }

    #region Public Spawn Methods

    public void ShowDamage(Vector3 position, float damage, bool isCritical = false)
    {
        var number = GetNumber();
        Color color = isCritical ? criticalDamageColor : normalDamageColor;
        float scale = scaleMultiplier * (isCritical ? (1f + criticalScaleBonus) : 1f);

        number.Initialize(
            position + GetRandomOffset(),
            Mathf.RoundToInt(damage).ToString(),
            color,
            floatSpeed,
            lifetime,
            scale
        );

        if (isCritical && ParticleManager.Instance != null)
        {
            ParticleManager.Instance.SpawnSparkles(position, criticalDamageColor, 8);
        }
    }

    public void ShowPlayerDamage(Vector3 position, float damage)
    {
        var number = GetNumber();
        number.Initialize(
            position + GetRandomOffset(),
            "-" + Mathf.RoundToInt(damage).ToString(),
            bossDamageColor,
            floatSpeed * 1.2f,
            lifetime,
            scaleMultiplier * 1.2f
        );
    }

    public void ShowHeal(Vector3 position, float amount)
    {
        var number = GetNumber();
        number.Initialize(
            position + GetRandomOffset(),
            "+" + Mathf.RoundToInt(amount).ToString(),
            healColor,
            floatSpeed,
            lifetime,
            scaleMultiplier
        );
    }

    public void ShowParry(Vector3 position)
    {
        var number = GetNumber();
        number.Initialize(
            position + Vector3.up * 0.5f,
            "PARRY!",
            parryColor,
            floatSpeed * 0.8f,
            lifetime * 1.2f,
            scaleMultiplier * 1.3f
        );

        if (ParticleManager.Instance != null)
        {
            ParticleManager.Instance.SpawnSparkles(position, parryColor, 12);
        }
    }

    public void ShowDodge(Vector3 position)
    {
        var number = GetNumber();
        number.Initialize(
            position + Vector3.up * 0.3f,
            "DODGE!",
            new Color(0.635f, 0.608f, 0.996f), // Lavender #A29BFE - chaos accent
            floatSpeed * 1.5f,
            lifetime * 0.8f,
            scaleMultiplier * 0.9f
        );
    }

    public void ShowCombo(Vector3 position, int comboCount)
    {
        if (comboCount < 2) return;

        var number = GetNumber();
        // Lerp from Bronze to Ochre for increasing combo
        Color bronze = new Color(0.804f, 0.498f, 0.196f);
        Color ochre = new Color(0.8f, 0.467f, 0.133f);
        Color comboColor = Color.Lerp(bronze, ochre, Mathf.Min(comboCount / 5f, 1f));
        number.Initialize(
            position + Vector3.up * 0.8f + Vector3.right * 0.5f,
            $"{comboCount}x COMBO!",
            comboColor,
            floatSpeed * 0.6f,
            lifetime * 1.5f,
            scaleMultiplier * (1f + comboCount * 0.1f)
        );
    }

    public void ShowMiss(Vector3 position)
    {
        var number = GetNumber();
        number.Initialize(
            position + GetRandomOffset(),
            "MISS",
            Color.gray,
            floatSpeed,
            lifetime * 0.7f,
            scaleMultiplier * 0.8f
        );
    }

    public void ShowPhaseChange(Vector3 position, int phaseNumber)
    {
        var number = GetNumber();
        // Show "CHAOS UNLEASHED!" for phase 2 in chaos purple
        string text = phaseNumber >= 2 ? "CHAOS UNLEASHED!" : $"PHASE {phaseNumber}!";
        Color color = phaseNumber >= 2 ? chaosColor : new Color(0.804f, 0.498f, 0.196f); // Chaos purple or Bronze

        number.Initialize(
            position + Vector3.up * 2f,
            text,
            color,
            floatSpeed * 0.4f,
            lifetime * 2.5f,
            scaleMultiplier * 2.2f
        );
    }

    #endregion

    private Vector3 GetRandomOffset()
    {
        return new Vector3(
            Random.Range(-0.3f, 0.3f),
            Random.Range(0.2f, 0.5f),
            0
        );
    }
}

/// <summary>
/// Individual damage number that handles its own animation
/// </summary>
public class DamageNumber : MonoBehaviour
{
    private RectTransform rectTransform;
    private Text uiText;

    private Vector3 worldPosition;
    private float floatSpeed;
    private float lifetime;
    private float maxLifetime;
    private float startScale;
    private Color startColor;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        uiText = GetComponent<Text>();
    }

    public void Initialize(Vector3 position, string text, Color color, float floatSpeed, float lifetime, float scale)
    {
        worldPosition = position;
        this.floatSpeed = floatSpeed;
        this.lifetime = lifetime;
        this.maxLifetime = lifetime;
        this.startScale = scale;
        this.startColor = color;

        if (uiText != null)
        {
            uiText.text = text;
            uiText.color = color;
        }

        transform.localScale = Vector3.one * scale;
        UpdateScreenPosition();
        gameObject.SetActive(true);
    }

    private void Update()
    {
        lifetime -= Time.deltaTime;

        if (lifetime <= 0)
        {
            gameObject.SetActive(false);
            return;
        }

        worldPosition += Vector3.up * floatSpeed * Time.deltaTime;
        UpdateScreenPosition();

        float lifePercent = lifetime / maxLifetime;

        float scaleAnim;
        if (lifePercent > 0.8f)
        {
            float t = (lifePercent - 0.8f) / 0.2f;
            scaleAnim = Mathf.Lerp(1f, 1.3f, 1f - t);
        }
        else
        {
            scaleAnim = 1f;
        }
        transform.localScale = Vector3.one * startScale * scaleAnim;

        if (lifePercent < 0.3f)
        {
            float alpha = lifePercent / 0.3f;
            Color c = startColor;
            c.a = alpha;
            if (uiText != null)
                uiText.color = c;
        }
    }

    private void UpdateScreenPosition()
    {
        if (Camera.main == null) return;
        transform.position = worldPosition;
        transform.rotation = Camera.main.transform.rotation;
    }
}
