using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Generates detailed pixel art sprites for heroes and bosses.
/// Creates proper character designs with weapons, armor, and animation frames.
/// </summary>
public static class DetailedSpriteGenerator
{
    #region Hero Sprites - 96x96

    public static Sprite[] GenerateHeroIdleFrames(Color bodyColor, Color armorColor, Color skinColor, Color weaponColor, HeroType type)
    {
        Sprite[] frames = new Sprite[4];
        for (int i = 0; i < 4; i++)
        {
            float breathOffset = Mathf.Sin(i * Mathf.PI * 0.5f) * 2f;
            frames[i] = CreateDetailedHeroSprite(bodyColor, armorColor, skinColor, weaponColor, type, 0, breathOffset);
        }
        return frames;
    }

    public static Sprite[] GenerateHeroAttackFrames(Color bodyColor, Color armorColor, Color skinColor, Color weaponColor, HeroType type)
    {
        Sprite[] frames = new Sprite[4];
        frames[0] = CreateDetailedHeroSprite(bodyColor, armorColor, skinColor, weaponColor, type, 1, 0); // Windup
        frames[1] = CreateDetailedHeroSprite(bodyColor, armorColor, skinColor, weaponColor, type, 2, 0); // Swing
        frames[2] = CreateDetailedHeroSprite(bodyColor, armorColor, skinColor, weaponColor, type, 3, 0); // Follow through
        frames[3] = CreateDetailedHeroSprite(bodyColor, armorColor, skinColor, weaponColor, type, 0, 0); // Return
        return frames;
    }

    public static Sprite[] GenerateHeroDodgeFrames(Color bodyColor, Color armorColor, Color skinColor, Color weaponColor, HeroType type)
    {
        Sprite[] frames = new Sprite[4];
        for (int i = 0; i < 4; i++)
        {
            frames[i] = CreateDetailedHeroSprite(bodyColor, armorColor, skinColor, weaponColor, type, 10 + i, 0);
        }
        return frames;
    }

    public static Sprite[] GenerateHeroHurtFrames(Color bodyColor, Color armorColor, Color skinColor, Color weaponColor, HeroType type)
    {
        Sprite[] frames = new Sprite[2];
        frames[0] = CreateDetailedHeroSprite(bodyColor, armorColor, skinColor, weaponColor, type, 20, 0);
        frames[1] = CreateDetailedHeroSprite(bodyColor, armorColor, skinColor, weaponColor, type, 21, 0);
        return frames;
    }

    private static Sprite CreateDetailedHeroSprite(Color bodyColor, Color armorColor, Color skinColor, Color weaponColor, HeroType type, int poseIndex, float breathOffset)
    {
        int size = 96;
        Texture2D tex = new Texture2D(size, size);
        tex.filterMode = FilterMode.Point;

        // Clear
        Color[] pixels = new Color[size * size];
        for (int i = 0; i < pixels.Length; i++) pixels[i] = Color.clear;
        tex.SetPixels(pixels);

        int centerX = size / 2;
        int baseY = 10;

        switch (type)
        {
            case HeroType.Warrior:
                DrawWarriorHero(tex, centerX, baseY, bodyColor, armorColor, skinColor, weaponColor, poseIndex, breathOffset);
                break;
            case HeroType.Rogue:
                DrawRogueHero(tex, centerX, baseY, bodyColor, armorColor, skinColor, weaponColor, poseIndex, breathOffset);
                break;
            case HeroType.Mage:
                DrawMageHero(tex, centerX, baseY, bodyColor, armorColor, skinColor, weaponColor, poseIndex, breathOffset);
                break;
        }

        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 64f);
    }

    private static void DrawWarriorHero(Texture2D tex, int cx, int by, Color body, Color armor, Color skin, Color weapon, int pose, float breath)
    {
        int headY = by + 55 + (int)breath;
        int bodyY = by + 25;

        // Legs
        DrawFilledRect(tex, cx - 12, by, 8, 20, body);
        DrawFilledRect(tex, cx + 4, by, 8, 20, body);
        // Boots
        DrawFilledRect(tex, cx - 14, by, 12, 6, armor);
        DrawFilledRect(tex, cx + 2, by, 12, 6, armor);

        // Body/Torso - bulky warrior
        DrawFilledRect(tex, cx - 18, bodyY, 36, 30, body);
        // Chest armor
        DrawFilledRect(tex, cx - 16, bodyY + 5, 32, 20, armor);
        // Belt
        DrawFilledRect(tex, cx - 18, bodyY, 36, 5, Color.Lerp(armor, Color.black, 0.3f));
        // Armor detail lines
        DrawFilledRect(tex, cx - 2, bodyY + 5, 4, 20, Color.Lerp(armor, Color.black, 0.2f));

        // Shoulder pauldrons
        DrawFilledOval(tex, cx - 22, bodyY + 22, 8, 10, armor);
        DrawFilledOval(tex, cx + 22, bodyY + 22, 8, 10, armor);

        // Arms
        int armPose = pose % 10;
        if (armPose == 0) // Idle
        {
            DrawFilledRect(tex, cx - 30, bodyY + 5, 10, 22, skin);
            DrawFilledRect(tex, cx + 20, bodyY + 5, 10, 22, skin);
            // Gauntlets
            DrawFilledRect(tex, cx - 32, bodyY + 2, 12, 8, armor);
            DrawFilledRect(tex, cx + 20, bodyY + 2, 12, 8, armor);
            // Sword at side
            DrawSword(tex, cx + 35, bodyY + 5, weapon, 0);
        }
        else if (armPose >= 1 && armPose <= 3) // Attack phases
        {
            // Attack animation - sword swing
            DrawFilledRect(tex, cx - 30, bodyY + 5, 10, 22, skin);
            int swordAngle = (armPose - 1) * 45 - 45;
            DrawSword(tex, cx + 25, bodyY + 30, weapon, swordAngle);
            // Right arm raised
            DrawFilledRect(tex, cx + 15, bodyY + 20 + (armPose * 5), 12, 18, skin);
        }
        else if (pose >= 10 && pose < 20) // Dodge roll
        {
            int rollPhase = pose - 10;
            // Tucked arms
            DrawFilledRect(tex, cx - 20, bodyY + 10, 8, 15, skin);
            DrawFilledRect(tex, cx + 12, bodyY + 10, 8, 15, skin);
        }
        else if (pose >= 20) // Hurt
        {
            DrawFilledRect(tex, cx - 28, bodyY + 8, 10, 18, skin);
            DrawFilledRect(tex, cx + 18, bodyY + 8, 10, 18, skin);
        }

        // Head
        DrawFilledOval(tex, cx, headY, 14, 16, skin);
        // Helmet
        DrawFilledOval(tex, cx, headY + 4, 16, 14, armor);
        // Helmet visor slit
        DrawFilledRect(tex, cx - 10, headY + 2, 20, 4, Color.black);
        // Eyes behind visor
        DrawFilledCircle(tex, cx - 4, headY + 3, 2, new Color(0.9f, 0.9f, 1f));
        DrawFilledCircle(tex, cx + 4, headY + 3, 2, new Color(0.9f, 0.9f, 1f));
        // Helmet crest
        DrawFilledTriangle(tex, cx, headY + 22, cx - 4, headY + 12, cx + 4, headY + 12, armor);
    }

    private static void DrawRogueHero(Texture2D tex, int cx, int by, Color body, Color armor, Color skin, Color weapon, int pose, float breath)
    {
        int headY = by + 52 + (int)breath;
        int bodyY = by + 22;

        // Legs - slim
        DrawFilledRect(tex, cx - 8, by, 6, 22, body);
        DrawFilledRect(tex, cx + 2, by, 6, 22, body);
        // Boots - light
        DrawFilledRect(tex, cx - 9, by, 8, 5, armor);
        DrawFilledRect(tex, cx + 1, by, 8, 5, armor);

        // Body/Torso - slim rogue
        DrawFilledRect(tex, cx - 12, bodyY, 24, 28, body);
        // Light leather armor
        DrawFilledRect(tex, cx - 10, bodyY + 8, 20, 16, armor);
        // Cross straps
        DrawLine(tex, cx - 10, bodyY + 8, cx + 10, bodyY + 24, Color.Lerp(armor, Color.black, 0.3f));
        DrawLine(tex, cx + 10, bodyY + 8, cx - 10, bodyY + 24, Color.Lerp(armor, Color.black, 0.3f));

        // Hood/Cloak collar
        DrawFilledTriangle(tex, cx, bodyY + 30, cx - 14, bodyY + 20, cx + 14, bodyY + 20, armor);

        // Arms - holding daggers
        int armPose = pose % 10;
        if (armPose == 0) // Idle
        {
            DrawFilledRect(tex, cx - 20, bodyY + 8, 8, 18, skin);
            DrawFilledRect(tex, cx + 12, bodyY + 8, 8, 18, skin);
            // Daggers
            DrawDagger(tex, cx - 24, bodyY + 5, weapon);
            DrawDagger(tex, cx + 24, bodyY + 5, weapon);
        }
        else if (armPose >= 1 && armPose <= 3)
        {
            // Fast slashing attack
            int offset = armPose * 8;
            DrawFilledRect(tex, cx - 25 + offset, bodyY + 10, 8, 16, skin);
            DrawFilledRect(tex, cx + 17 - offset, bodyY + 10, 8, 16, skin);
            DrawDagger(tex, cx - 20 + offset * 2, bodyY + 15, weapon);
            DrawDagger(tex, cx + 20 - offset * 2, bodyY + 15, weapon);
        }

        // Head - hooded
        DrawFilledOval(tex, cx, headY, 12, 14, skin);
        // Hood
        DrawFilledOval(tex, cx, headY + 2, 16, 16, armor);
        DrawFilledOval(tex, cx, headY - 2, 10, 12, skin); // Face showing
        // Eyes - sharp
        DrawFilledCircle(tex, cx - 4, headY, 2, Color.white);
        DrawFilledCircle(tex, cx + 4, headY, 2, Color.white);
        DrawFilledCircle(tex, cx - 4, headY, 1, Color.black);
        DrawFilledCircle(tex, cx + 4, headY, 1, Color.black);
        // Mask over lower face
        DrawFilledRect(tex, cx - 8, headY - 8, 16, 6, armor);
    }

    private static void DrawMageHero(Texture2D tex, int cx, int by, Color body, Color armor, Color skin, Color weapon, int pose, float breath)
    {
        int headY = by + 55 + (int)breath;
        int bodyY = by + 20;

        // Robe bottom
        DrawFilledTriangle(tex, cx, by, cx - 20, by + 25, cx + 20, by + 25, body);

        // Body/Torso - robed
        DrawFilledRect(tex, cx - 14, bodyY, 28, 35, body);
        // Robe trim
        DrawFilledRect(tex, cx - 14, bodyY + 30, 28, 4, armor);
        DrawFilledRect(tex, cx - 2, bodyY, 4, 35, armor);
        // Sash
        DrawFilledRect(tex, cx - 12, bodyY + 10, 24, 4, weapon);

        // Arms with wide sleeves
        int armPose = pose % 10;
        if (armPose == 0)
        {
            // Holding staff
            DrawFilledRect(tex, cx - 22, bodyY + 10, 10, 15, body);
            DrawFilledRect(tex, cx + 12, bodyY + 10, 10, 15, body);
            DrawFilledRect(tex, cx - 26, bodyY + 5, 6, 8, skin);
            DrawFilledRect(tex, cx + 20, bodyY + 5, 6, 8, skin);
            // Staff
            DrawStaff(tex, cx + 30, by + 5, weapon, armor);
        }
        else if (armPose >= 1 && armPose <= 3)
        {
            // Casting animation
            DrawFilledRect(tex, cx - 22, bodyY + 15 + armPose * 5, 10, 15, body);
            DrawFilledRect(tex, cx + 12, bodyY + 15 + armPose * 5, 10, 15, body);
            // Magic glow
            DrawFilledCircle(tex, cx, headY - 10 + armPose * 10, 8 + armPose * 3,
                new Color(weapon.r, weapon.g, weapon.b, 0.5f));
        }

        // Head
        DrawFilledOval(tex, cx, headY, 12, 14, skin);
        // Wizard hat base
        DrawFilledOval(tex, cx, headY + 8, 18, 6, armor);
        // Wizard hat cone
        DrawFilledTriangle(tex, cx, headY + 35, cx - 14, headY + 8, cx + 14, headY + 8, armor);
        // Hat band
        DrawFilledRect(tex, cx - 15, headY + 6, 30, 3, weapon);
        // Eyes - wise
        DrawFilledCircle(tex, cx - 4, headY + 2, 2, Color.white);
        DrawFilledCircle(tex, cx + 4, headY + 2, 2, Color.white);
        DrawFilledCircle(tex, cx - 4, headY + 2, 1, new Color(0.4f, 0.2f, 0.6f));
        DrawFilledCircle(tex, cx + 4, headY + 2, 1, new Color(0.4f, 0.2f, 0.6f));
        // Beard
        DrawFilledTriangle(tex, cx, headY - 15, cx - 8, headY - 4, cx + 8, headY - 4,
            Color.Lerp(skin, Color.white, 0.5f));
    }

    private static void DrawSword(Texture2D tex, int x, int y, Color color, int angle)
    {
        // Simple sword - handle and blade
        DrawFilledRect(tex, x - 2, y - 5, 4, 10, Color.Lerp(color, Color.black, 0.5f)); // Handle
        DrawFilledRect(tex, x - 6, y + 4, 12, 3, color); // Crossguard
        DrawFilledRect(tex, x - 2, y + 6, 4, 25, color); // Blade
        DrawFilledTriangle(tex, x, y + 33, x - 2, y + 30, x + 2, y + 30, color); // Tip
    }

    private static void DrawDagger(Texture2D tex, int x, int y, Color color)
    {
        DrawFilledRect(tex, x - 1, y - 3, 2, 6, Color.Lerp(color, Color.black, 0.5f)); // Handle
        DrawFilledRect(tex, x - 1, y + 2, 2, 12, color); // Blade
        DrawFilledCircle(tex, x, y + 14, 1, color); // Tip
    }

    private static void DrawStaff(Texture2D tex, int x, int y, Color gemColor, Color woodColor)
    {
        // Staff shaft
        DrawFilledRect(tex, x - 2, y, 4, 60, woodColor);
        // Staff head ornament
        DrawFilledOval(tex, x, y + 65, 8, 10, woodColor);
        // Gem
        DrawFilledCircle(tex, x, y + 68, 5, gemColor);
        DrawFilledCircle(tex, x + 1, y + 70, 2, Color.white); // Gem shine
    }

    #endregion

    #region Boss Sprites - 256x256

    public static Sprite[] GenerateBossIdleFrames(Color bodyColor, Color eyeColor, Color accentColor, Color glowColor)
    {
        Sprite[] frames = new Sprite[4];
        for (int i = 0; i < 4; i++)
        {
            float pulse = Mathf.Sin(i * Mathf.PI * 0.5f);
            frames[i] = CreateDetailedBossSprite(bodyColor, eyeColor, accentColor, glowColor, 0, pulse);
        }
        return frames;
    }

    public static Sprite[] GenerateBossAttackFrames(Color bodyColor, Color eyeColor, Color accentColor, Color glowColor)
    {
        Sprite[] frames = new Sprite[3];
        frames[0] = CreateDetailedBossSprite(bodyColor, eyeColor, accentColor, glowColor, 1, 0);
        frames[1] = CreateDetailedBossSprite(bodyColor, eyeColor, accentColor, glowColor, 2, 0);
        frames[2] = CreateDetailedBossSprite(bodyColor, eyeColor, accentColor, glowColor, 3, 0);
        return frames;
    }

    public static Sprite[] GenerateBossHurtFrames(Color bodyColor, Color eyeColor, Color accentColor, Color glowColor)
    {
        Sprite[] frames = new Sprite[2];
        frames[0] = CreateDetailedBossSprite(bodyColor, eyeColor, accentColor, Color.white, 10, 0);
        frames[1] = CreateDetailedBossSprite(bodyColor, eyeColor, accentColor, glowColor, 10, 0);
        return frames;
    }

    public static Sprite[] GenerateBossTransitionFrames(Color bodyColor, Color eyeColor, Color accentColor, Color glowColor, Color phase2Color)
    {
        Sprite[] frames = new Sprite[4];
        for (int i = 0; i < 4; i++)
        {
            Color transitionBody = Color.Lerp(bodyColor, phase2Color, i / 3f);
            Color transitionGlow = Color.Lerp(glowColor, Color.red, i / 3f);
            frames[i] = CreateDetailedBossSprite(transitionBody, eyeColor, accentColor, transitionGlow, 20 + i, 0);
        }
        return frames;
    }

    private static Sprite CreateDetailedBossSprite(Color bodyColor, Color eyeColor, Color accentColor, Color glowColor, int poseIndex, float pulse)
    {
        int size = 256;
        Texture2D tex = new Texture2D(size, size);
        tex.filterMode = FilterMode.Point;

        Color[] pixels = new Color[size * size];
        for (int i = 0; i < pixels.Length; i++) pixels[i] = Color.clear;
        tex.SetPixels(pixels);

        int cx = size / 2;
        int cy = size / 2;

        // Draw the menacing mask boss
        DrawBronzeMaskBoss(tex, cx, cy, bodyColor, eyeColor, accentColor, glowColor, poseIndex, pulse);

        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 64f);
    }

    private static void DrawBronzeMaskBoss(Texture2D tex, int cx, int cy, Color body, Color eye, Color accent, Color glow, int pose, float pulse)
    {
        int glowSize = (int)(pulse * 5);

        // Outer glow aura
        if (pose < 10) // Normal states
        {
            DrawFilledOval(tex, cx, cy, 95 + glowSize, 105 + glowSize, new Color(glow.r, glow.g, glow.b, 0.15f));
        }

        // Main mask body - large imposing shape
        DrawFilledOval(tex, cx, cy, 85, 95, body);

        // Inner face area - darker
        DrawFilledOval(tex, cx, cy - 5, 70, 75, accent);
        DrawFilledOval(tex, cx, cy - 5, 60, 65, body);

        // Forehead crest/crown
        int crownY = cy + 60;
        DrawFilledTriangle(tex, cx, crownY + 40, cx - 30, crownY, cx + 30, crownY, accent);
        DrawFilledTriangle(tex, cx - 25, crownY + 20, cx - 45, crownY - 10, cx - 5, crownY - 10, accent);
        DrawFilledTriangle(tex, cx + 25, crownY + 20, cx + 5, crownY - 10, cx + 45, crownY - 10, accent);
        // Crown jewels
        DrawFilledCircle(tex, cx, crownY + 30, 6, glow);
        DrawFilledCircle(tex, cx - 25, crownY + 10, 4, glow);
        DrawFilledCircle(tex, cx + 25, crownY + 10, 4, glow);

        // Eye sockets - dark recesses
        int eyeY = cy + 15;
        DrawFilledOval(tex, cx - 28, eyeY, 20, 16, Color.black);
        DrawFilledOval(tex, cx + 28, eyeY, 20, 16, Color.black);

        // Eyes - glowing and menacing
        int eyeGlow = (int)(Mathf.Abs(pulse) * 3);
        DrawFilledOval(tex, cx - 28, eyeY, 16 + eyeGlow, 12 + eyeGlow, eye);
        DrawFilledOval(tex, cx + 28, eyeY, 16 + eyeGlow, 12 + eyeGlow, eye);
        // Pupils - follow player (could be animated)
        int pupilOffset = (pose >= 1 && pose <= 3) ? 3 : 0;
        DrawFilledOval(tex, cx - 28 - pupilOffset, eyeY, 8, 10, Color.black);
        DrawFilledOval(tex, cx + 28 - pupilOffset, eyeY, 8, 10, Color.black);
        // Eye shine
        DrawFilledCircle(tex, cx - 32, eyeY + 4, 3, Color.white);
        DrawFilledCircle(tex, cx + 24, eyeY + 4, 3, Color.white);

        // Nose bridge
        DrawFilledRect(tex, cx - 4, cy - 5, 8, 25, accent);
        DrawFilledTriangle(tex, cx, cy - 20, cx - 8, cy - 5, cx + 8, cy - 5, accent);

        // Mouth - grimacing
        int mouthY = cy - 40;
        DrawFilledRect(tex, cx - 30, mouthY, 60, 20, Color.black);
        // Teeth
        for (int i = 0; i < 6; i++)
        {
            int toothX = cx - 25 + i * 10;
            DrawFilledRect(tex, toothX, mouthY + 12, 6, 8, Color.Lerp(body, Color.white, 0.8f));
        }

        // Cheek markings - tribal patterns
        DrawTribalMark(tex, cx - 55, cy, accent, glow);
        DrawTribalMark(tex, cx + 55, cy, accent, glow);

        // Side horns/decorations
        DrawFilledTriangle(tex, cx - 100, cy + 20, cx - 75, cy + 50, cx - 75, cy - 10, accent);
        DrawFilledTriangle(tex, cx + 100, cy + 20, cx + 75, cy + 50, cx + 75, cy - 10, accent);
        // Horn tips glow
        DrawFilledCircle(tex, cx - 95, cy + 20, 5, glow);
        DrawFilledCircle(tex, cx + 95, cy + 20, 5, glow);

        // Chin decoration
        int chinY = cy - 70;
        DrawFilledTriangle(tex, cx, chinY - 20, cx - 25, chinY + 10, cx + 25, chinY + 10, accent);
        DrawFilledCircle(tex, cx, chinY - 15, 5, glow);

        // Attack state modifications
        if (pose >= 1 && pose <= 3)
        {
            // Eyes glow more intensely
            DrawFilledOval(tex, cx - 28, eyeY, 22, 18, new Color(eye.r, eye.g, eye.b, 0.7f));
            DrawFilledOval(tex, cx + 28, eyeY, 22, 18, new Color(eye.r, eye.g, eye.b, 0.7f));
            // Mouth opens wider
            DrawFilledRect(tex, cx - 35, mouthY - 5, 70, 30, Color.black);
        }

        // Hurt state - flash white
        if (pose == 10)
        {
            // White flash overlay would be handled by color tinting
        }

        // Transition state - cracks appear
        if (pose >= 20)
        {
            int crackIntensity = pose - 20;
            DrawCracks(tex, cx, cy, crackIntensity, glow);
        }
    }

    private static void DrawTribalMark(Texture2D tex, int x, int y, Color lineColor, Color glowColor)
    {
        // Vertical lines
        DrawFilledRect(tex, x - 2, y - 20, 4, 40, lineColor);
        // Horizontal lines
        DrawFilledRect(tex, x - 10, y - 10, 20, 3, lineColor);
        DrawFilledRect(tex, x - 10, y + 7, 20, 3, lineColor);
        // Dots
        DrawFilledCircle(tex, x, y - 15, 3, glowColor);
        DrawFilledCircle(tex, x, y + 12, 3, glowColor);
    }

    private static void DrawCracks(Texture2D tex, int cx, int cy, int intensity, Color crackGlow)
    {
        // Draw crack lines emanating from center
        for (int i = 0; i < intensity + 1; i++)
        {
            int angle = i * 60;
            float rad = angle * Mathf.Deg2Rad;
            int endX = cx + (int)(Mathf.Cos(rad) * 50);
            int endY = cy + (int)(Mathf.Sin(rad) * 50);
            DrawLine(tex, cx, cy, endX, endY, crackGlow);
            // Branching cracks
            if (intensity > 1)
            {
                int branchX = cx + (int)(Mathf.Cos(rad + 0.3f) * 30);
                int branchY = cy + (int)(Mathf.Sin(rad + 0.3f) * 30);
                DrawLine(tex, (cx + endX) / 2, (cy + endY) / 2, branchX, branchY, crackGlow);
            }
        }
    }

    #endregion

    #region Drawing Utilities

    public static void DrawFilledRect(Texture2D tex, int x, int y, int width, int height, Color color)
    {
        for (int px = x; px < x + width && px < tex.width; px++)
        {
            for (int py = y; py < y + height && py < tex.height; py++)
            {
                if (px >= 0 && py >= 0)
                    BlendPixel(tex, px, py, color);
            }
        }
    }

    public static void DrawFilledCircle(Texture2D tex, int cx, int cy, int radius, Color color)
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
                        BlendPixel(tex, px, py, color);
                }
            }
        }
    }

    public static void DrawFilledOval(Texture2D tex, int cx, int cy, int radiusX, int radiusY, Color color)
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
                        BlendPixel(tex, px, py, color);
                }
            }
        }
    }

    public static void DrawFilledTriangle(Texture2D tex, int x1, int y1, int x2, int y2, int x3, int y3, Color color)
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
                        BlendPixel(tex, x, y, color);
                }
            }
        }
    }

    public static void DrawLine(Texture2D tex, int x1, int y1, int x2, int y2, Color color)
    {
        int dx = Mathf.Abs(x2 - x1);
        int dy = Mathf.Abs(y2 - y1);
        int sx = x1 < x2 ? 1 : -1;
        int sy = y1 < y2 ? 1 : -1;
        int err = dx - dy;

        while (true)
        {
            if (x1 >= 0 && x1 < tex.width && y1 >= 0 && y1 < tex.height)
            {
                BlendPixel(tex, x1, y1, color);
                // Thicker line
                if (x1 + 1 < tex.width) BlendPixel(tex, x1 + 1, y1, color);
                if (y1 + 1 < tex.height) BlendPixel(tex, x1, y1 + 1, color);
            }

            if (x1 == x2 && y1 == y2) break;

            int e2 = 2 * err;
            if (e2 > -dy) { err -= dy; x1 += sx; }
            if (e2 < dx) { err += dx; y1 += sy; }
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

    private static void BlendPixel(Texture2D tex, int x, int y, Color color)
    {
        if (color.a >= 1f)
        {
            tex.SetPixel(x, y, color);
        }
        else if (color.a > 0)
        {
            Color existing = tex.GetPixel(x, y);
            Color blended = Color.Lerp(existing, color, color.a);
            blended.a = Mathf.Max(existing.a, color.a);
            tex.SetPixel(x, y, blended);
        }
    }

    #endregion

    public enum HeroType
    {
        Warrior,
        Rogue,
        Mage
    }
}
