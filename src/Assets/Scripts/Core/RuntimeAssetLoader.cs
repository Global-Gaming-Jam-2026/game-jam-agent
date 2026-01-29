using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Generates sprites and data assets at runtime so the game has visuals
/// without requiring manual Unity Editor setup.
/// </summary>
public static class RuntimeAssetLoader
{
    private static Dictionary<string, Sprite> heroSprites = new Dictionary<string, Sprite>();
    private static Dictionary<string, Sprite> bossSprites = new Dictionary<string, Sprite>();
    private static Dictionary<string, Sprite> effectSprites = new Dictionary<string, Sprite>();
    private static Dictionary<string, Sprite> arenaSprites = new Dictionary<string, Sprite>();

    private static Dictionary<string, HeroData> heroData = new Dictionary<string, HeroData>();
    private static Dictionary<string, BossData> bossData = new Dictionary<string, BossData>();

    private static bool initialized = false;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        if (initialized) return;

        Debug.Log("[RuntimeAssetLoader] Generating runtime sprites and data...");

        // Generate all sprites
        GenerateHeroSprites();
        GenerateBossSprites();
        GenerateEffectSprites();
        GenerateArenaSprites();

        // Generate data assets
        GenerateHeroData();
        GenerateBossData();

        initialized = true;
        Debug.Log("[RuntimeAssetLoader] Runtime assets ready.");
    }

    #region Sprite Generation

    private static void GenerateHeroSprites()
    {
        // Bronze Warrior - warm bronze tones
        heroSprites["BronzeWarrior"] = CreateHeroSprite(
            new Color(0.8f, 0.5f, 0.2f),   // Bronze body
            new Color(0.6f, 0.35f, 0.15f), // Darker trim
            new Color(1f, 0.9f, 0.7f)      // Light highlights
        );

        // Shadow Dancer - purple/dark tones
        heroSprites["ShadowDancer"] = CreateHeroSprite(
            new Color(0.3f, 0.2f, 0.4f),   // Dark purple body
            new Color(0.2f, 0.1f, 0.3f),   // Darker trim
            new Color(0.6f, 0.4f, 0.8f)    // Light purple highlights
        );

        // Flame Bearer - red/orange tones
        heroSprites["FlameBearer"] = CreateHeroSprite(
            new Color(0.9f, 0.3f, 0.1f),   // Red-orange body
            new Color(0.7f, 0.2f, 0.05f),  // Darker trim
            new Color(1f, 0.6f, 0.2f)      // Orange highlights
        );

        Debug.Log($"[RuntimeAssetLoader] Generated {heroSprites.Count} hero sprites");
    }

    private static Sprite CreateHeroSprite(Color bodyColor, Color trimColor, Color highlightColor)
    {
        int size = 64;
        Texture2D tex = new Texture2D(size, size);
        tex.filterMode = FilterMode.Point;

        // Clear to transparent
        Color[] pixels = new Color[size * size];
        for (int i = 0; i < pixels.Length; i++)
            pixels[i] = Color.clear;
        tex.SetPixels(pixels);

        // Draw body (rectangle 16-48 x 8-40)
        DrawFilledRect(tex, 20, 8, 24, 32, bodyColor);

        // Draw head (circle at center top)
        DrawFilledCircle(tex, 32, 48, 12, bodyColor);

        // Draw arms (rectangles on sides)
        DrawFilledRect(tex, 8, 20, 12, 16, bodyColor);
        DrawFilledRect(tex, 44, 20, 12, 16, bodyColor);

        // Draw legs (rectangles at bottom)
        DrawFilledRect(tex, 22, 0, 8, 12, trimColor);
        DrawFilledRect(tex, 34, 0, 8, 12, trimColor);

        // Draw eyes (white with black pupils)
        DrawFilledCircle(tex, 28, 50, 3, Color.white);
        DrawFilledCircle(tex, 36, 50, 3, Color.white);
        DrawFilledCircle(tex, 28, 50, 1, Color.black);
        DrawFilledCircle(tex, 36, 50, 1, Color.black);

        // Highlight on head
        DrawFilledCircle(tex, 35, 54, 2, highlightColor);

        // Body trim/belt
        DrawFilledRect(tex, 20, 18, 24, 4, trimColor);

        tex.Apply();

        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 64f);
    }

    private static void GenerateBossSprites()
    {
        // Bronze Mask - large imposing mask
        bossSprites["BronzeMask"] = CreateBossSprite(
            new Color(0.8f, 0.5f, 0.2f),   // Bronze body
            new Color(1f, 0.9f, 0.5f),     // Golden eyes
            new Color(0.5f, 0.3f, 0.1f)    // Dark accents
        );

        // Chaos Totem - purple chaos entity
        bossSprites["ChaosTotem"] = CreateBossSprite(
            new Color(0.5f, 0.2f, 0.4f),   // Purple body
            new Color(1f, 0.3f, 0.5f),     // Pink glowing eyes
            new Color(0.3f, 0.1f, 0.2f)    // Dark purple accents
        );

        Debug.Log($"[RuntimeAssetLoader] Generated {bossSprites.Count} boss sprites");
    }

    private static Sprite CreateBossSprite(Color bodyColor, Color eyeColor, Color accentColor)
    {
        int size = 128;
        Texture2D tex = new Texture2D(size, size);
        tex.filterMode = FilterMode.Point;

        // Clear to transparent
        Color[] pixels = new Color[size * size];
        for (int i = 0; i < pixels.Length; i++)
            pixels[i] = Color.clear;
        tex.SetPixels(pixels);

        // Draw main body (large oval/mask shape)
        DrawFilledOval(tex, 64, 64, 50, 55, bodyColor);

        // Draw inner mask detail
        DrawFilledOval(tex, 64, 64, 40, 45, accentColor);
        DrawFilledOval(tex, 64, 64, 35, 40, bodyColor);

        // Draw glowing eyes (large and menacing)
        DrawFilledOval(tex, 44, 75, 12, 8, eyeColor);
        DrawFilledOval(tex, 84, 75, 12, 8, eyeColor);

        // Black pupils
        DrawFilledCircle(tex, 44, 75, 4, Color.black);
        DrawFilledCircle(tex, 84, 75, 4, Color.black);

        // Eye glow effect
        DrawFilledOval(tex, 44, 75, 14, 10, new Color(eyeColor.r, eyeColor.g, eyeColor.b, 0.3f));
        DrawFilledOval(tex, 84, 75, 14, 10, new Color(eyeColor.r, eyeColor.g, eyeColor.b, 0.3f));

        // Mouth/markings
        DrawFilledRect(tex, 50, 40, 28, 6, accentColor);

        // Decorative horns/protrusions at top
        DrawFilledTriangle(tex, 30, 115, 20, 90, 40, 90, accentColor);
        DrawFilledTriangle(tex, 98, 115, 88, 90, 108, 90, accentColor);

        // Side decorations
        DrawFilledRect(tex, 10, 55, 8, 30, accentColor);
        DrawFilledRect(tex, 110, 55, 8, 30, accentColor);

        tex.Apply();

        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 64f);
    }

    private static void GenerateEffectSprites()
    {
        // Projectile sprite (glowing orb)
        effectSprites["Projectile"] = CreateProjectileSprite(new Color(1f, 0.4f, 0.2f));
        effectSprites["ProjectileParryable"] = CreateProjectileSprite(new Color(1f, 0.8f, 0.2f));

        // Hit effect (starburst)
        effectSprites["HitEffect"] = CreateHitEffectSprite(Color.white);

        // Dodge trail
        effectSprites["DodgeTrail"] = CreateTrailSprite(new Color(0.8f, 0.8f, 1f, 0.5f));

        Debug.Log($"[RuntimeAssetLoader] Generated {effectSprites.Count} effect sprites");
    }

    private static void GenerateArenaSprites()
    {
        // Arena floor - textured ground
        arenaSprites["Floor"] = CreateArenaFloorSprite(
            new Color(0.3f, 0.2f, 0.1f),   // Base brown
            new Color(0.25f, 0.15f, 0.08f) // Darker lines
        );

        // Arena wall/boundary
        arenaSprites["Wall"] = CreateArenaWallSprite(
            new Color(0.4f, 0.25f, 0.15f), // Wall color
            new Color(0.5f, 0.35f, 0.2f)   // Highlight
        );

        // Background
        arenaSprites["Background"] = CreateBackgroundSprite(
            new Color(0.15f, 0.1f, 0.08f), // Dark brown background
            new Color(0.2f, 0.12f, 0.1f)   // Slightly lighter variation
        );

        Debug.Log($"[RuntimeAssetLoader] Generated {arenaSprites.Count} arena sprites");
    }

    private static Sprite CreateArenaFloorSprite(Color baseColor, Color lineColor)
    {
        int width = 128;
        int height = 32;
        Texture2D tex = new Texture2D(width, height);
        tex.filterMode = FilterMode.Point;
        tex.wrapMode = TextureWrapMode.Repeat;

        // Fill with base color
        Color[] pixels = new Color[width * height];
        for (int i = 0; i < pixels.Length; i++)
            pixels[i] = baseColor;
        tex.SetPixels(pixels);

        // Add horizontal lines for texture
        for (int y = 0; y < height; y += 8)
        {
            for (int x = 0; x < width; x++)
            {
                tex.SetPixel(x, y, lineColor);
            }
        }

        // Add vertical brick-like pattern
        for (int y = 0; y < height; y++)
        {
            int offset = (y / 8) % 2 == 0 ? 0 : 32;
            for (int x = offset; x < width; x += 64)
            {
                tex.SetPixel(x, y, lineColor);
                if (x + 1 < width) tex.SetPixel(x + 1, y, lineColor);
            }
        }

        // Add some surface detail
        for (int i = 0; i < 20; i++)
        {
            int x = Random.Range(0, width);
            int y = Random.Range(0, height);
            Color detailColor = Color.Lerp(baseColor, lineColor, 0.5f);
            if (x < width && y < height)
                tex.SetPixel(x, y, detailColor);
        }

        tex.Apply();

        return Sprite.Create(tex, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f), 32f);
    }

    private static Sprite CreateArenaWallSprite(Color wallColor, Color highlightColor)
    {
        int size = 64;
        Texture2D tex = new Texture2D(size, size);
        tex.filterMode = FilterMode.Point;

        // Fill with wall color
        Color[] pixels = new Color[size * size];
        for (int i = 0; i < pixels.Length; i++)
            pixels[i] = wallColor;
        tex.SetPixels(pixels);

        // Add column details
        for (int y = 0; y < size; y++)
        {
            // Left edge highlight
            tex.SetPixel(0, y, highlightColor);
            tex.SetPixel(1, y, highlightColor);

            // Right edge shadow
            tex.SetPixel(size - 1, y, Color.Lerp(wallColor, Color.black, 0.3f));
            tex.SetPixel(size - 2, y, Color.Lerp(wallColor, Color.black, 0.2f));
        }

        // Add horizontal bands
        for (int bandY = 8; bandY < size; bandY += 16)
        {
            for (int x = 0; x < size; x++)
            {
                tex.SetPixel(x, bandY, highlightColor);
            }
        }

        tex.Apply();

        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 32f);
    }

    private static Sprite CreateBackgroundSprite(Color baseColor, Color variantColor)
    {
        int size = 256;
        Texture2D tex = new Texture2D(size, size);
        tex.filterMode = FilterMode.Point;

        // Create gradient background
        for (int y = 0; y < size; y++)
        {
            float t = (float)y / size;
            Color rowColor = Color.Lerp(baseColor, variantColor, t * 0.5f);

            for (int x = 0; x < size; x++)
            {
                tex.SetPixel(x, y, rowColor);
            }
        }

        // Add some atmospheric particles/dust
        for (int i = 0; i < 50; i++)
        {
            int x = Random.Range(0, size);
            int y = Random.Range(0, size);
            Color dustColor = new Color(1f, 0.9f, 0.7f, 0.1f);
            tex.SetPixel(x, y, Color.Lerp(tex.GetPixel(x, y), dustColor, 0.3f));
        }

        tex.Apply();

        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 64f);
    }

    private static Sprite CreateProjectileSprite(Color color)
    {
        int size = 16;
        Texture2D tex = new Texture2D(size, size);
        tex.filterMode = FilterMode.Point;

        Color[] pixels = new Color[size * size];
        for (int i = 0; i < pixels.Length; i++)
            pixels[i] = Color.clear;
        tex.SetPixels(pixels);

        // Glowing orb
        DrawFilledCircle(tex, 8, 8, 6, color);
        DrawFilledCircle(tex, 8, 8, 4, Color.Lerp(color, Color.white, 0.5f));
        DrawFilledCircle(tex, 8, 8, 2, Color.white);

        tex.Apply();

        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 16f);
    }

    private static Sprite CreateHitEffectSprite(Color color)
    {
        int size = 32;
        Texture2D tex = new Texture2D(size, size);
        tex.filterMode = FilterMode.Point;

        Color[] pixels = new Color[size * size];
        for (int i = 0; i < pixels.Length; i++)
            pixels[i] = Color.clear;
        tex.SetPixels(pixels);

        // Starburst pattern
        int cx = size / 2;
        int cy = size / 2;

        // Draw rays
        for (int angle = 0; angle < 360; angle += 45)
        {
            float rad = angle * Mathf.Deg2Rad;
            int endX = cx + Mathf.RoundToInt(Mathf.Cos(rad) * 14);
            int endY = cy + Mathf.RoundToInt(Mathf.Sin(rad) * 14);
            DrawLine(tex, cx, cy, endX, endY, color);
        }

        // Center glow
        DrawFilledCircle(tex, cx, cy, 4, color);
        DrawFilledCircle(tex, cx, cy, 2, Color.white);

        tex.Apply();

        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 32f);
    }

    private static Sprite CreateTrailSprite(Color color)
    {
        int size = 16;
        Texture2D tex = new Texture2D(size, size);
        tex.filterMode = FilterMode.Point;

        Color[] pixels = new Color[size * size];
        for (int i = 0; i < pixels.Length; i++)
            pixels[i] = Color.clear;
        tex.SetPixels(pixels);

        // Gradient fade trail
        for (int x = 0; x < size; x++)
        {
            float alpha = 1f - (float)x / size;
            Color c = new Color(color.r, color.g, color.b, color.a * alpha);
            for (int y = 4; y < 12; y++)
            {
                tex.SetPixel(x, y, c);
            }
        }

        tex.Apply();

        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 16f);
    }

    #endregion

    #region Drawing Helpers

    private static void DrawFilledRect(Texture2D tex, int x, int y, int width, int height, Color color)
    {
        for (int px = x; px < x + width && px < tex.width; px++)
        {
            for (int py = y; py < y + height && py < tex.height; py++)
            {
                if (px >= 0 && py >= 0)
                    tex.SetPixel(px, py, color);
            }
        }
    }

    private static void DrawFilledCircle(Texture2D tex, int cx, int cy, int radius, Color color)
    {
        for (int x = -radius; x <= radius; x++)
        {
            for (int y = -radius; y <= radius; y++)
            {
                if (x * x + y * y <= radius * radius)
                {
                    int px = cx + x;
                    int py = cy + y;
                    if (px >= 0 && px < tex.width && py >= 0 && py < tex.height)
                        tex.SetPixel(px, py, color);
                }
            }
        }
    }

    private static void DrawFilledOval(Texture2D tex, int cx, int cy, int radiusX, int radiusY, Color color)
    {
        for (int x = -radiusX; x <= radiusX; x++)
        {
            for (int y = -radiusY; y <= radiusY; y++)
            {
                float nx = (float)x / radiusX;
                float ny = (float)y / radiusY;
                if (nx * nx + ny * ny <= 1)
                {
                    int px = cx + x;
                    int py = cy + y;
                    if (px >= 0 && px < tex.width && py >= 0 && py < tex.height)
                        tex.SetPixel(px, py, color);
                }
            }
        }
    }

    private static void DrawFilledTriangle(Texture2D tex, int x1, int y1, int x2, int y2, int x3, int y3, Color color)
    {
        int minX = Mathf.Min(x1, Mathf.Min(x2, x3));
        int maxX = Mathf.Max(x1, Mathf.Max(x2, x3));
        int minY = Mathf.Min(y1, Mathf.Min(y2, y3));
        int maxY = Mathf.Max(y1, Mathf.Max(y2, y3));

        for (int x = minX; x <= maxX; x++)
        {
            for (int y = minY; y <= maxY; y++)
            {
                if (PointInTriangle(x, y, x1, y1, x2, y2, x3, y3))
                {
                    if (x >= 0 && x < tex.width && y >= 0 && y < tex.height)
                        tex.SetPixel(x, y, color);
                }
            }
        }
    }

    private static bool PointInTriangle(int px, int py, int x1, int y1, int x2, int y2, int x3, int y3)
    {
        float d1 = Sign(px, py, x1, y1, x2, y2);
        float d2 = Sign(px, py, x2, y2, x3, y3);
        float d3 = Sign(px, py, x3, y3, x1, y1);

        bool hasNeg = (d1 < 0) || (d2 < 0) || (d3 < 0);
        bool hasPos = (d1 > 0) || (d2 > 0) || (d3 > 0);

        return !(hasNeg && hasPos);
    }

    private static float Sign(int p1x, int p1y, int p2x, int p2y, int p3x, int p3y)
    {
        return (p1x - p3x) * (p2y - p3y) - (p2x - p3x) * (p1y - p3y);
    }

    private static void DrawLine(Texture2D tex, int x1, int y1, int x2, int y2, Color color)
    {
        int dx = Mathf.Abs(x2 - x1);
        int dy = Mathf.Abs(y2 - y1);
        int sx = x1 < x2 ? 1 : -1;
        int sy = y1 < y2 ? 1 : -1;
        int err = dx - dy;

        while (true)
        {
            if (x1 >= 0 && x1 < tex.width && y1 >= 0 && y1 < tex.height)
                tex.SetPixel(x1, y1, color);

            if (x1 == x2 && y1 == y2) break;

            int e2 = 2 * err;
            if (e2 > -dy) { err -= dy; x1 += sx; }
            if (e2 < dx) { err += dx; y1 += sy; }
        }
    }

    #endregion

    #region Data Generation

    private static void GenerateHeroData()
    {
        // Bronze Warrior
        var bronzeWarrior = ScriptableObject.CreateInstance<HeroData>();
        bronzeWarrior.heroName = "Bronze Warrior";
        bronzeWarrior.description = "A balanced fighter with quick attacks.";
        bronzeWarrior.primaryColor = new Color(0.8f, 0.5f, 0.2f);
        bronzeWarrior.secondaryColor = new Color(0.6f, 0.35f, 0.15f);
        bronzeWarrior.attackColor = new Color(1f, 0.8f, 0.3f);
        bronzeWarrior.damageFlashColor = Color.white;
        bronzeWarrior.maxHealth = 100f;
        bronzeWarrior.moveSpeed = 6f;
        bronzeWarrior.dodgeSpeed = 12f;
        bronzeWarrior.attackDamage = 25f;
        bronzeWarrior.idleSprite = heroSprites["BronzeWarrior"];
        bronzeWarrior.portrait = heroSprites["BronzeWarrior"];
        heroData["BronzeWarrior"] = bronzeWarrior;

        // Shadow Dancer
        var shadowDancer = ScriptableObject.CreateInstance<HeroData>();
        shadowDancer.heroName = "Shadow Dancer";
        shadowDancer.description = "Fast and evasive with quick dodges.";
        shadowDancer.primaryColor = new Color(0.3f, 0.2f, 0.4f);
        shadowDancer.secondaryColor = new Color(0.2f, 0.1f, 0.3f);
        shadowDancer.attackColor = new Color(0.6f, 0.4f, 0.8f);
        shadowDancer.damageFlashColor = new Color(0.8f, 0.6f, 1f);
        shadowDancer.maxHealth = 80f;
        shadowDancer.moveSpeed = 8f;
        shadowDancer.dodgeSpeed = 15f;
        shadowDancer.attackDamage = 20f;
        shadowDancer.specialAbility = HeroAbilityType.QuickDash;
        shadowDancer.idleSprite = heroSprites["ShadowDancer"];
        shadowDancer.portrait = heroSprites["ShadowDancer"];
        heroData["ShadowDancer"] = shadowDancer;

        // Flame Bearer
        var flameBearer = ScriptableObject.CreateInstance<HeroData>();
        flameBearer.heroName = "Flame Bearer";
        flameBearer.description = "Slow but powerful with heavy attacks.";
        flameBearer.primaryColor = new Color(0.9f, 0.3f, 0.1f);
        flameBearer.secondaryColor = new Color(0.7f, 0.2f, 0.05f);
        flameBearer.attackColor = new Color(1f, 0.6f, 0.2f);
        flameBearer.damageFlashColor = new Color(1f, 0.8f, 0.6f);
        flameBearer.maxHealth = 120f;
        flameBearer.moveSpeed = 5f;
        flameBearer.dodgeSpeed = 10f;
        flameBearer.attackDamage = 35f;
        flameBearer.specialAbility = HeroAbilityType.HeavyHitter;
        flameBearer.idleSprite = heroSprites["FlameBearer"];
        flameBearer.portrait = heroSprites["FlameBearer"];
        heroData["FlameBearer"] = flameBearer;

        Debug.Log($"[RuntimeAssetLoader] Generated {heroData.Count} hero data assets");
    }

    private static void GenerateBossData()
    {
        // Bronze Mask
        var bronzeMask = ScriptableObject.CreateInstance<BossData>();
        bronzeMask.bossName = "The Bronze Mask";
        bronzeMask.description = "An ancient totemic guardian awakened by chaos.";
        bronzeMask.primaryColor = new Color(0.8f, 0.5f, 0.2f);
        bronzeMask.secondaryColor = new Color(0.5f, 0.3f, 0.1f);
        bronzeMask.eyeColor = new Color(1f, 0.9f, 0.5f);
        bronzeMask.attackColor = new Color(1f, 0.4f, 0.2f);
        bronzeMask.phase2TintColor = new Color(0.9f, 0.2f, 0.3f);
        bronzeMask.maxHealth = 500f;
        bronzeMask.staggerThreshold = 100f;
        bronzeMask.portrait = bossSprites["BronzeMask"];
        bronzeMask.arenaPosition = new Vector2(3, 1);
        bronzeMask.bossScale = new Vector2(2, 2);

        // Phase 1
        var phase1 = new BossPhaseConfig
        {
            phaseName = "Awakening",
            healthPercentStart = 1f,
            healthPercentEnd = 0.5f,
            phaseSprite = bossSprites["BronzeMask"],
            attackCooldown = 2f,
            useSweepAttack = true,
            useSlamAttack = true,
            useBulletPattern = false,
            useSpiritProjectiles = true
        };

        // Phase 2
        var phase2 = new BossPhaseConfig
        {
            phaseName = "Fury",
            healthPercentStart = 0.5f,
            healthPercentEnd = 0f,
            phaseSprite = bossSprites["BronzeMask"],
            phaseTint = new Color(0.9f, 0.2f, 0.3f),
            attackCooldown = 1.5f,
            patternSpeedMultiplier = 1.3f,
            useSweepAttack = true,
            useSlamAttack = true,
            useBulletPattern = true,
            useSpiritProjectiles = true,
            useAggresivePatterns = true
        };

        bronzeMask.phases.Add(phase1);
        bronzeMask.phases.Add(phase2);
        bossData["BronzeMask"] = bronzeMask;

        // Chaos Totem
        var chaosTotem = ScriptableObject.CreateInstance<BossData>();
        chaosTotem.bossName = "Chaos Totem";
        chaosTotem.description = "A manifestation of pure chaos energy.";
        chaosTotem.primaryColor = new Color(0.5f, 0.2f, 0.4f);
        chaosTotem.secondaryColor = new Color(0.3f, 0.1f, 0.2f);
        chaosTotem.eyeColor = new Color(1f, 0.3f, 0.5f);
        chaosTotem.attackColor = new Color(0.8f, 0.2f, 0.6f);
        chaosTotem.phase2TintColor = new Color(0.6f, 0.1f, 0.4f);
        chaosTotem.maxHealth = 600f;
        chaosTotem.staggerThreshold = 120f;
        chaosTotem.portrait = bossSprites["ChaosTotem"];
        chaosTotem.arenaPosition = new Vector2(3, 1);
        chaosTotem.bossScale = new Vector2(2.2f, 2.2f);

        var chaosPhase1 = new BossPhaseConfig
        {
            phaseName = "Emergence",
            healthPercentStart = 1f,
            healthPercentEnd = 0.5f,
            phaseSprite = bossSprites["ChaosTotem"],
            attackCooldown = 1.8f,
            useSweepAttack = true,
            useBulletPattern = true,
            useSpiritProjectiles = true
        };

        var chaosPhase2 = new BossPhaseConfig
        {
            phaseName = "Chaos Unleashed",
            healthPercentStart = 0.5f,
            healthPercentEnd = 0f,
            phaseSprite = bossSprites["ChaosTotem"],
            phaseTint = new Color(0.6f, 0.1f, 0.4f),
            attackCooldown = 1.2f,
            patternSpeedMultiplier = 1.5f,
            useSweepAttack = true,
            useSlamAttack = true,
            useBulletPattern = true,
            useLaserBeam = true,
            useSpiritProjectiles = true,
            useAggresivePatterns = true
        };

        chaosTotem.phases.Add(chaosPhase1);
        chaosTotem.phases.Add(chaosPhase2);
        bossData["ChaosTotem"] = chaosTotem;

        Debug.Log($"[RuntimeAssetLoader] Generated {bossData.Count} boss data assets");
    }

    #endregion

    #region Public Access Methods

    public static Sprite GetHeroSprite(string heroName)
    {
        if (!initialized) Initialize();

        string key = heroName.Replace(" ", "");
        if (heroSprites.TryGetValue(key, out Sprite sprite))
            return sprite;

        // Return first available as fallback
        foreach (var kvp in heroSprites)
            return kvp.Value;

        return null;
    }

    public static Sprite GetBossSprite(string bossName)
    {
        if (!initialized) Initialize();

        string key = bossName.Replace(" ", "").Replace("The", "");
        if (bossSprites.TryGetValue(key, out Sprite sprite))
            return sprite;

        // Return first available as fallback
        foreach (var kvp in bossSprites)
            return kvp.Value;

        return null;
    }

    public static Sprite GetEffectSprite(string effectName)
    {
        if (!initialized) Initialize();

        if (effectSprites.TryGetValue(effectName, out Sprite sprite))
            return sprite;

        return null;
    }

    public static Sprite GetArenaSprite(string spriteName)
    {
        if (!initialized) Initialize();

        if (arenaSprites.TryGetValue(spriteName, out Sprite sprite))
            return sprite;

        return null;
    }

    public static HeroData GetHeroData(string heroName)
    {
        if (!initialized) Initialize();

        string key = heroName.Replace(" ", "");
        if (heroData.TryGetValue(key, out HeroData data))
            return data;

        // Return first available as fallback
        foreach (var kvp in heroData)
            return kvp.Value;

        return null;
    }

    public static BossData GetBossData(string bossName)
    {
        if (!initialized) Initialize();

        string key = bossName.Replace(" ", "").Replace("The", "");
        if (bossData.TryGetValue(key, out BossData data))
            return data;

        // Return first available as fallback
        foreach (var kvp in bossData)
            return kvp.Value;

        return null;
    }

    public static HeroData GetDefaultHero()
    {
        if (!initialized) Initialize();
        return heroData.ContainsKey("BronzeWarrior") ? heroData["BronzeWarrior"] : null;
    }

    public static BossData GetDefaultBoss()
    {
        if (!initialized) Initialize();
        return bossData.ContainsKey("BronzeMask") ? bossData["BronzeMask"] : null;
    }

    public static List<HeroData> GetAllHeroes()
    {
        if (!initialized) Initialize();
        return new List<HeroData>(heroData.Values);
    }

    public static List<BossData> GetAllBosses()
    {
        if (!initialized) Initialize();
        return new List<BossData>(bossData.Values);
    }

    #endregion
}
