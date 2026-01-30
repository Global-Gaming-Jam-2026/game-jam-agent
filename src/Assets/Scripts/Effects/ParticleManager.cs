using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manages sprite-based particle effects.
/// Pools and spawns visual effects for hits, impacts, and other game events.
/// </summary>
public class ParticleManager : MonoBehaviour
{
    public static ParticleManager Instance { get; private set; }

    [Header("Pool Settings")]
    [SerializeField] private int poolSize = 50;

    [Header("Effect Sprites")]
    [SerializeField] private Sprite hitSparkSprite;
    [SerializeField] private Sprite impactBurstSprite;
    [SerializeField] private Sprite dustPuffSprite;
    [SerializeField] private Sprite sparkleSprite;

    private List<ParticleObject> particlePool = new List<ParticleObject>();
    private int currentIndex = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        InitializePool();
        GenerateDefaultSprites();
    }

    private void InitializePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = new GameObject($"Particle_{i}");
            obj.transform.SetParent(transform);
            var particle = obj.AddComponent<ParticleObject>();
            particle.gameObject.SetActive(false);
            particlePool.Add(particle);
        }
    }

    private void GenerateDefaultSprites()
    {
        if (hitSparkSprite == null)
            hitSparkSprite = CreateSparkSprite();
        if (impactBurstSprite == null)
            impactBurstSprite = CreateBurstSprite();
        if (dustPuffSprite == null)
            dustPuffSprite = CreateDustSprite();
        if (sparkleSprite == null)
            sparkleSprite = CreateSparkleSprite();
    }

    private ParticleObject GetParticle()
    {
        ParticleObject particle = particlePool[currentIndex];
        currentIndex = (currentIndex + 1) % particlePool.Count;
        return particle;
    }

    #region Public Effect Methods

    /// <summary>
    /// Spawn hit sparks when player hits boss
    /// </summary>
    public void SpawnHitSparks(Vector3 position, Color color, int count = 8)
    {
        for (int i = 0; i < count; i++)
        {
            var particle = GetParticle();
            float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            float speed = Random.Range(3f, 8f);
            Vector2 velocity = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * speed;

            particle.Initialize(
                position + (Vector3)Random.insideUnitCircle * 0.2f,
                hitSparkSprite,
                color,
                velocity,
                0.3f,
                Random.Range(0.3f, 0.6f),
                true,
                0.8f
            );
        }
    }

    /// <summary>
    /// Spawn impact burst for heavy hits
    /// </summary>
    public void SpawnImpactBurst(Vector3 position, Color color, float scale = 1f)
    {
        var particle = GetParticle();
        particle.Initialize(
            position,
            impactBurstSprite,
            color,
            Vector2.zero,
            scale,
            0.4f,
            false,
            1f,
            true // Scale up then fade
        );
    }

    /// <summary>
    /// Spawn dust puff for dodges and landings
    /// </summary>
    public void SpawnDustPuff(Vector3 position, Color color, int count = 5)
    {
        for (int i = 0; i < count; i++)
        {
            var particle = GetParticle();
            float angle = Random.Range(60f, 120f) * Mathf.Deg2Rad; // Mostly upward
            float speed = Random.Range(1f, 3f);
            Vector2 velocity = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * speed;

            particle.Initialize(
                position + new Vector3(Random.Range(-0.3f, 0.3f), 0, 0),
                dustPuffSprite,
                color,
                velocity,
                Random.Range(0.3f, 0.6f),
                Random.Range(0.4f, 0.8f),
                false,
                0.5f
            );
        }
    }

    /// <summary>
    /// Spawn sparkles for parry success or power-ups
    /// </summary>
    public void SpawnSparkles(Vector3 position, Color color, int count = 12)
    {
        for (int i = 0; i < count; i++)
        {
            var particle = GetParticle();
            float angle = (360f / count) * i * Mathf.Deg2Rad;
            float speed = Random.Range(2f, 5f);
            Vector2 velocity = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * speed;

            particle.Initialize(
                position,
                sparkleSprite,
                color,
                velocity,
                Random.Range(0.2f, 0.4f),
                Random.Range(0.5f, 1f),
                true,
                0.9f
            );
        }
    }

    /// <summary>
    /// Spawn death explosion
    /// </summary>
    public void SpawnDeathExplosion(Vector3 position, Color primaryColor, Color secondaryColor)
    {
        // Large central burst
        SpawnImpactBurst(position, primaryColor, 2f);

        // Ring of sparks
        for (int i = 0; i < 16; i++)
        {
            var particle = GetParticle();
            float angle = (360f / 16) * i * Mathf.Deg2Rad;
            float speed = Random.Range(5f, 12f);
            Vector2 velocity = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * speed;

            particle.Initialize(
                position,
                hitSparkSprite,
                i % 2 == 0 ? primaryColor : secondaryColor,
                velocity,
                Random.Range(0.4f, 0.8f),
                Random.Range(0.6f, 1.2f),
                true,
                0.7f
            );
        }

        // Dust cloud
        SpawnDustPuff(position, Color.gray, 10);
    }

    /// <summary>
    /// Spawn trail effect (for dodge)
    /// </summary>
    public void SpawnTrailGhost(Vector3 position, Sprite characterSprite, Color color)
    {
        var particle = GetParticle();
        particle.Initialize(
            position,
            characterSprite,
            new Color(color.r, color.g, color.b, 0.5f),
            Vector2.zero,
            1f,
            0.3f,
            false,
            0.95f
        );
    }

    #endregion

    #region Chaos Theme Effects

    // Official colors from GAME_DESIGN.md
    private static readonly Color ChaosPurple = new Color(0.424f, 0.361f, 0.906f);  // #6C5CE7
    private static readonly Color ChaosLavender = new Color(0.635f, 0.608f, 0.996f); // #A29BFE
    private static readonly Color ChaosCrimson = new Color(0.863f, 0.078f, 0.235f);  // #DC143C
    private static readonly Color Bronze = new Color(0.804f, 0.498f, 0.196f);        // #CD7F32
    private static readonly Color Terracotta = new Color(0.886f, 0.447f, 0.357f);    // #E2725B
    private static readonly Color Ochre = new Color(0.8f, 0.467f, 0.133f);           // #CC7722

    /// <summary>
    /// Spawn spiraling chaos particles during Phase 2
    /// </summary>
    public void SpawnChaosSwirl(Vector3 position, int count = 8)
    {
        for (int i = 0; i < count; i++)
        {
            var particle = GetParticle();
            float angle = (360f / count) * i * Mathf.Deg2Rad + Time.time;
            float radius = Random.Range(0.5f, 1.5f);
            Vector3 offset = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * radius;

            // Spiral outward velocity
            float speed = Random.Range(1f, 3f);
            Vector2 velocity = new Vector2(Mathf.Cos(angle + 0.5f), Mathf.Sin(angle + 0.5f)) * speed;

            Color color = i % 2 == 0 ? ChaosPurple : ChaosLavender;

            particle.Initialize(
                position + offset,
                sparkleSprite,
                color,
                velocity,
                Random.Range(0.2f, 0.5f),
                Random.Range(0.8f, 1.5f),
                false,
                0.7f
            );
        }
    }

    /// <summary>
    /// Spawn corruption burst when chaos intensifies
    /// </summary>
    public void SpawnCorruptionBurst(Vector3 position, float scale = 1f)
    {
        // Central crimson burst
        SpawnImpactBurst(position, ChaosCrimson, scale);

        // Outer purple sparks
        for (int i = 0; i < 10; i++)
        {
            var particle = GetParticle();
            float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            float speed = Random.Range(4f, 10f);
            Vector2 velocity = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * speed;

            particle.Initialize(
                position,
                hitSparkSprite,
                i % 3 == 0 ? ChaosCrimson : ChaosPurple,
                velocity,
                Random.Range(0.3f, 0.6f),
                Random.Range(0.4f, 0.8f),
                true,
                0.8f
            );
        }
    }

    /// <summary>
    /// Spawn golden mask fragments when boss takes damage
    /// </summary>
    public void SpawnMaskFragment(Vector3 position, int count = 6)
    {
        for (int i = 0; i < count; i++)
        {
            var particle = GetParticle();
            float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            float speed = Random.Range(3f, 8f);
            Vector2 velocity = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * speed;

            Color color = i % 3 == 0 ? Ochre : Bronze;

            particle.Initialize(
                position + (Vector3)Random.insideUnitCircle * 0.3f,
                hitSparkSprite,
                color,
                velocity,
                Random.Range(0.4f, 0.8f),
                Random.Range(0.6f, 1.2f),
                true,
                0.6f
            );
        }
    }

    /// <summary>
    /// Spawn slow drifting ambient chaos particles
    /// </summary>
    public void SpawnChaosAmbient(Vector3 position)
    {
        var particle = GetParticle();
        float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        float speed = Random.Range(0.5f, 1.5f);
        Vector2 velocity = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * speed;

        Color color = Random.value > 0.5f ? ChaosPurple : new Color(ChaosPurple.r, ChaosPurple.g, ChaosPurple.b, 0.3f);

        particle.Initialize(
            position + (Vector3)Random.insideUnitCircle * 2f,
            dustPuffSprite,
            color,
            velocity,
            Random.Range(0.2f, 0.4f),
            Random.Range(2f, 4f),
            false,
            0.5f
        );
    }

    /// <summary>
    /// Spawn themed hit sparks using official Bronze palette
    /// </summary>
    public void SpawnBronzeHitSparks(Vector3 position, bool isCritical = false)
    {
        Color color = isCritical ? Ochre : Bronze;
        int count = isCritical ? 12 : 8;
        SpawnHitSparks(position, color, count);

        if (isCritical)
        {
            SpawnImpactBurst(position, Ochre, 0.8f);
        }
    }

    /// <summary>
    /// Spawn terracotta burst when boss damages player
    /// </summary>
    public void SpawnBossDamageEffect(Vector3 position)
    {
        SpawnHitSparks(position, Terracotta, 10);
        SpawnImpactBurst(position, Terracotta, 0.6f);
    }

    #endregion

    #region Sprite Generation

    private Sprite CreateSparkSprite()
    {
        int size = 16;
        Texture2D tex = new Texture2D(size, size);
        tex.filterMode = FilterMode.Point;

        Color[] pixels = new Color[size * size];
        for (int i = 0; i < pixels.Length; i++) pixels[i] = Color.clear;
        tex.SetPixels(pixels);

        // Diamond shape spark
        int cx = size / 2;
        int cy = size / 2;
        for (int i = 0; i < size / 2; i++)
        {
            int width = size / 2 - i;
            for (int x = -width; x <= width; x++)
            {
                float alpha = 1f - (float)i / (size / 2);
                tex.SetPixel(cx + x, cy + i, new Color(1, 1, 1, alpha));
                tex.SetPixel(cx + x, cy - i, new Color(1, 1, 1, alpha));
            }
        }

        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 16f);
    }

    private Sprite CreateBurstSprite()
    {
        int size = 32;
        Texture2D tex = new Texture2D(size, size);
        tex.filterMode = FilterMode.Point;

        Color[] pixels = new Color[size * size];
        for (int i = 0; i < pixels.Length; i++) pixels[i] = Color.clear;
        tex.SetPixels(pixels);

        int cx = size / 2;
        int cy = size / 2;

        // Starburst rays
        for (int angle = 0; angle < 360; angle += 30)
        {
            float rad = angle * Mathf.Deg2Rad;
            for (int r = 0; r < size / 2; r++)
            {
                int x = cx + (int)(Mathf.Cos(rad) * r);
                int y = cy + (int)(Mathf.Sin(rad) * r);
                float alpha = 1f - (float)r / (size / 2);
                if (x >= 0 && x < size && y >= 0 && y < size)
                    tex.SetPixel(x, y, new Color(1, 1, 1, alpha));
            }
        }

        // Center glow
        for (int x = -4; x <= 4; x++)
        {
            for (int y = -4; y <= 4; y++)
            {
                if (x * x + y * y <= 16)
                {
                    tex.SetPixel(cx + x, cy + y, Color.white);
                }
            }
        }

        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 32f);
    }

    private Sprite CreateDustSprite()
    {
        int size = 16;
        Texture2D tex = new Texture2D(size, size);
        tex.filterMode = FilterMode.Point;

        Color[] pixels = new Color[size * size];
        for (int i = 0; i < pixels.Length; i++) pixels[i] = Color.clear;
        tex.SetPixels(pixels);

        int cx = size / 2;
        int cy = size / 2;

        // Soft circle
        for (int x = -6; x <= 6; x++)
        {
            for (int y = -6; y <= 6; y++)
            {
                float dist = Mathf.Sqrt(x * x + y * y);
                if (dist <= 6)
                {
                    float alpha = 1f - (dist / 6f);
                    tex.SetPixel(cx + x, cy + y, new Color(1, 1, 1, alpha * 0.7f));
                }
            }
        }

        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 16f);
    }

    private Sprite CreateSparkleSprite()
    {
        int size = 8;
        Texture2D tex = new Texture2D(size, size);
        tex.filterMode = FilterMode.Point;

        Color[] pixels = new Color[size * size];
        for (int i = 0; i < pixels.Length; i++) pixels[i] = Color.clear;
        tex.SetPixels(pixels);

        int cx = size / 2;
        int cy = size / 2;

        // 4-point star
        tex.SetPixel(cx, cy, Color.white);
        tex.SetPixel(cx - 1, cy, Color.white);
        tex.SetPixel(cx + 1, cy, Color.white);
        tex.SetPixel(cx, cy - 1, Color.white);
        tex.SetPixel(cx, cy + 1, Color.white);
        tex.SetPixel(cx - 2, cy, new Color(1, 1, 1, 0.5f));
        tex.SetPixel(cx + 2, cy, new Color(1, 1, 1, 0.5f));
        tex.SetPixel(cx, cy - 2, new Color(1, 1, 1, 0.5f));
        tex.SetPixel(cx, cy + 2, new Color(1, 1, 1, 0.5f));

        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 8f);
    }

    #endregion
}

/// <summary>
/// Individual particle object that handles its own animation and lifecycle
/// </summary>
public class ParticleObject : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Vector2 velocity;
    private float lifetime;
    private float maxLifetime;
    private float startScale;
    private bool hasGravity;
    private float fadeSpeed;
    private bool scaleAnimation;
    private Color startColor;

    private void Awake()
    {
        spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sortingOrder = 100;
    }

    public void Initialize(Vector3 position, Sprite sprite, Color color, Vector2 velocity,
        float scale, float lifetime, bool hasGravity, float fadeSpeed, bool scaleAnimation = false)
    {
        transform.position = position;
        spriteRenderer.sprite = sprite;
        spriteRenderer.color = color;
        startColor = color;
        transform.localScale = Vector3.one * scale;
        startScale = scale;

        this.velocity = velocity;
        this.lifetime = lifetime;
        this.maxLifetime = lifetime;
        this.hasGravity = hasGravity;
        this.fadeSpeed = fadeSpeed;
        this.scaleAnimation = scaleAnimation;

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

        // Move
        transform.position += (Vector3)velocity * Time.deltaTime;

        // Gravity
        if (hasGravity)
        {
            velocity.y -= 15f * Time.deltaTime;
        }

        // Fade
        float lifePercent = lifetime / maxLifetime;
        Color c = startColor;
        c.a = Mathf.Lerp(0, startColor.a, Mathf.Pow(lifePercent, fadeSpeed));
        spriteRenderer.color = c;

        // Scale animation
        if (scaleAnimation)
        {
            float scaleMultiplier = 1f + (1f - lifePercent) * 2f;
            transform.localScale = Vector3.one * startScale * scaleMultiplier;
        }

        // Slow down
        velocity *= 0.98f;
    }
}
