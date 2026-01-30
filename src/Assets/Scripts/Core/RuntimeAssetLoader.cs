using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Generates sprites and data assets at runtime so the game has visuals
/// without requiring manual Unity Editor setup.
/// Uses DetailedSpriteGenerator for high-quality character art with animations.
/// </summary>
public static class RuntimeAssetLoader
{
    private static Dictionary<string, Sprite> heroSprites = new Dictionary<string, Sprite>();
    private static Dictionary<string, Sprite> bossSprites = new Dictionary<string, Sprite>();
    private static Dictionary<string, Sprite> effectSprites = new Dictionary<string, Sprite>();
    private static Dictionary<string, Sprite> arenaSprites = new Dictionary<string, Sprite>();

    // Animation frame caches
    private static Dictionary<string, Sprite[]> heroIdleFrames = new Dictionary<string, Sprite[]>();
    private static Dictionary<string, Sprite[]> heroAttackFrames = new Dictionary<string, Sprite[]>();
    private static Dictionary<string, Sprite[]> heroDodgeFrames = new Dictionary<string, Sprite[]>();
    private static Dictionary<string, Sprite[]> heroHurtFrames = new Dictionary<string, Sprite[]>();

    private static Dictionary<string, Sprite[]> bossIdleFrames = new Dictionary<string, Sprite[]>();
    private static Dictionary<string, Sprite[]> bossAttackFrames = new Dictionary<string, Sprite[]>();
    private static Dictionary<string, Sprite[]> bossHurtFrames = new Dictionary<string, Sprite[]>();
    private static Dictionary<string, Sprite[]> bossTransitionFrames = new Dictionary<string, Sprite[]>();

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
        // Bronze Warrior - warm bronze tones (Warrior type)
        Color bronzeBody = new Color(0.8f, 0.5f, 0.2f);
        Color bronzeArmor = new Color(0.6f, 0.35f, 0.15f);
        Color bronzeSkin = new Color(0.95f, 0.8f, 0.7f);
        Color bronzeWeapon = new Color(0.7f, 0.7f, 0.75f);

        heroIdleFrames["BronzeWarrior"] = DetailedSpriteGenerator.GenerateHeroIdleFrames(bronzeBody, bronzeArmor, bronzeSkin, bronzeWeapon, DetailedSpriteGenerator.HeroType.Warrior);
        heroAttackFrames["BronzeWarrior"] = DetailedSpriteGenerator.GenerateHeroAttackFrames(bronzeBody, bronzeArmor, bronzeSkin, bronzeWeapon, DetailedSpriteGenerator.HeroType.Warrior);
        heroDodgeFrames["BronzeWarrior"] = DetailedSpriteGenerator.GenerateHeroDodgeFrames(bronzeBody, bronzeArmor, bronzeSkin, bronzeWeapon, DetailedSpriteGenerator.HeroType.Warrior);
        heroHurtFrames["BronzeWarrior"] = DetailedSpriteGenerator.GenerateHeroHurtFrames(bronzeBody, bronzeArmor, bronzeSkin, bronzeWeapon, DetailedSpriteGenerator.HeroType.Warrior);
        heroSprites["BronzeWarrior"] = heroIdleFrames["BronzeWarrior"][0];

        // Shadow Dancer - purple/dark tones (Rogue type)
        Color shadowBody = new Color(0.3f, 0.2f, 0.4f);
        Color shadowArmor = new Color(0.2f, 0.15f, 0.25f);
        Color shadowSkin = new Color(0.85f, 0.75f, 0.85f);
        Color shadowWeapon = new Color(0.5f, 0.3f, 0.6f);

        heroIdleFrames["ShadowDancer"] = DetailedSpriteGenerator.GenerateHeroIdleFrames(shadowBody, shadowArmor, shadowSkin, shadowWeapon, DetailedSpriteGenerator.HeroType.Rogue);
        heroAttackFrames["ShadowDancer"] = DetailedSpriteGenerator.GenerateHeroAttackFrames(shadowBody, shadowArmor, shadowSkin, shadowWeapon, DetailedSpriteGenerator.HeroType.Rogue);
        heroDodgeFrames["ShadowDancer"] = DetailedSpriteGenerator.GenerateHeroDodgeFrames(shadowBody, shadowArmor, shadowSkin, shadowWeapon, DetailedSpriteGenerator.HeroType.Rogue);
        heroHurtFrames["ShadowDancer"] = DetailedSpriteGenerator.GenerateHeroHurtFrames(shadowBody, shadowArmor, shadowSkin, shadowWeapon, DetailedSpriteGenerator.HeroType.Rogue);
        heroSprites["ShadowDancer"] = heroIdleFrames["ShadowDancer"][0];

        // Flame Bearer - red/orange tones (Mage type)
        Color flameBody = new Color(0.9f, 0.3f, 0.1f);
        Color flameArmor = new Color(0.7f, 0.2f, 0.1f);
        Color flameSkin = new Color(0.95f, 0.85f, 0.75f);
        Color flameWeapon = new Color(1f, 0.5f, 0.2f);

        heroIdleFrames["FlameBearer"] = DetailedSpriteGenerator.GenerateHeroIdleFrames(flameBody, flameArmor, flameSkin, flameWeapon, DetailedSpriteGenerator.HeroType.Mage);
        heroAttackFrames["FlameBearer"] = DetailedSpriteGenerator.GenerateHeroAttackFrames(flameBody, flameArmor, flameSkin, flameWeapon, DetailedSpriteGenerator.HeroType.Mage);
        heroDodgeFrames["FlameBearer"] = DetailedSpriteGenerator.GenerateHeroDodgeFrames(flameBody, flameArmor, flameSkin, flameWeapon, DetailedSpriteGenerator.HeroType.Mage);
        heroHurtFrames["FlameBearer"] = DetailedSpriteGenerator.GenerateHeroHurtFrames(flameBody, flameArmor, flameSkin, flameWeapon, DetailedSpriteGenerator.HeroType.Mage);
        heroSprites["FlameBearer"] = heroIdleFrames["FlameBearer"][0];

        Debug.Log($"[RuntimeAssetLoader] Generated {heroSprites.Count} detailed hero sprites with animations");
    }

    private static void GenerateBossSprites()
    {
        // Bronze Mask - large imposing mask
        Color bronzeBody = new Color(0.8f, 0.5f, 0.2f);
        Color bronzeEye = new Color(1f, 0.9f, 0.5f);
        Color bronzeAccent = new Color(0.5f, 0.3f, 0.1f);
        Color bronzeGlow = new Color(1f, 0.7f, 0.3f);
        Color bronzePhase2 = new Color(0.9f, 0.2f, 0.2f);

        bossIdleFrames["BronzeMask"] = DetailedSpriteGenerator.GenerateBossIdleFrames(bronzeBody, bronzeEye, bronzeAccent, bronzeGlow);
        bossAttackFrames["BronzeMask"] = DetailedSpriteGenerator.GenerateBossAttackFrames(bronzeBody, bronzeEye, bronzeAccent, bronzeGlow);
        bossHurtFrames["BronzeMask"] = DetailedSpriteGenerator.GenerateBossHurtFrames(bronzeBody, bronzeEye, bronzeAccent, bronzeGlow);
        bossTransitionFrames["BronzeMask"] = DetailedSpriteGenerator.GenerateBossTransitionFrames(bronzeBody, bronzeEye, bronzeAccent, bronzeGlow, bronzePhase2);
        bossSprites["BronzeMask"] = bossIdleFrames["BronzeMask"][0];

        // Chaos Totem - purple chaos entity
        Color chaosBody = new Color(0.5f, 0.2f, 0.4f);
        Color chaosEye = new Color(1f, 0.3f, 0.5f);
        Color chaosAccent = new Color(0.3f, 0.1f, 0.2f);
        Color chaosGlow = new Color(0.8f, 0.3f, 0.6f);
        Color chaosPhase2 = new Color(0.6f, 0.1f, 0.4f);

        bossIdleFrames["ChaosTotem"] = DetailedSpriteGenerator.GenerateBossIdleFrames(chaosBody, chaosEye, chaosAccent, chaosGlow);
        bossAttackFrames["ChaosTotem"] = DetailedSpriteGenerator.GenerateBossAttackFrames(chaosBody, chaosEye, chaosAccent, chaosGlow);
        bossHurtFrames["ChaosTotem"] = DetailedSpriteGenerator.GenerateBossHurtFrames(chaosBody, chaosEye, chaosAccent, chaosGlow);
        bossTransitionFrames["ChaosTotem"] = DetailedSpriteGenerator.GenerateBossTransitionFrames(chaosBody, chaosEye, chaosAccent, chaosGlow, chaosPhase2);
        bossSprites["ChaosTotem"] = bossIdleFrames["ChaosTotem"][0];

        Debug.Log($"[RuntimeAssetLoader] Generated {bossSprites.Count} detailed boss sprites with animations");
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

    #region Animation Frame Access

    public static Sprite[] GetHeroIdleFrames(string heroName)
    {
        if (!initialized) Initialize();
        string key = heroName.Replace(" ", "");
        return heroIdleFrames.TryGetValue(key, out Sprite[] frames) ? frames : null;
    }

    public static Sprite[] GetHeroAttackFrames(string heroName)
    {
        if (!initialized) Initialize();
        string key = heroName.Replace(" ", "");
        return heroAttackFrames.TryGetValue(key, out Sprite[] frames) ? frames : null;
    }

    public static Sprite[] GetHeroDodgeFrames(string heroName)
    {
        if (!initialized) Initialize();
        string key = heroName.Replace(" ", "");
        return heroDodgeFrames.TryGetValue(key, out Sprite[] frames) ? frames : null;
    }

    public static Sprite[] GetHeroHurtFrames(string heroName)
    {
        if (!initialized) Initialize();
        string key = heroName.Replace(" ", "");
        return heroHurtFrames.TryGetValue(key, out Sprite[] frames) ? frames : null;
    }

    public static Sprite[] GetBossIdleFrames(string bossName)
    {
        if (!initialized) Initialize();
        string key = bossName.Replace(" ", "").Replace("The", "");
        return bossIdleFrames.TryGetValue(key, out Sprite[] frames) ? frames : null;
    }

    public static Sprite[] GetBossAttackFrames(string bossName)
    {
        if (!initialized) Initialize();
        string key = bossName.Replace(" ", "").Replace("The", "");
        return bossAttackFrames.TryGetValue(key, out Sprite[] frames) ? frames : null;
    }

    public static Sprite[] GetBossHurtFrames(string bossName)
    {
        if (!initialized) Initialize();
        string key = bossName.Replace(" ", "").Replace("The", "");
        return bossHurtFrames.TryGetValue(key, out Sprite[] frames) ? frames : null;
    }

    public static Sprite[] GetBossTransitionFrames(string bossName)
    {
        if (!initialized) Initialize();
        string key = bossName.Replace(" ", "").Replace("The", "");
        return bossTransitionFrames.TryGetValue(key, out Sprite[] frames) ? frames : null;
    }

    #endregion
}
