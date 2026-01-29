# Visual Specification for "Mask of the Bronze God"
## For Visual Artist / Theme Designer

**Game Style:** Cuphead-inspired 2D boss fight
**Theme:** Bronze era, totemic masks, chaos
**Resolution:** 1920x1080 (16:9), scales down to mobile

---

## COLOR PALETTES

### Theme 1: Bronze Era (Default)
| Element | Hex | RGB | Usage |
|---------|-----|-----|-------|
| Background | `#3D2914` | (61, 41, 20) | Camera clear color |
| Arena Floor | `#4D331A` | (77, 51, 26) | Ground |
| UI Primary | `#CC8033` | (204, 128, 51) | Buttons, highlights |
| UI Accent | `#FFCC4D` | (255, 204, 77) | Important highlights |
| Text | `#FFF2D9` | (255, 242, 217) | All text |

### Theme 2: Chaos Realm
| Element | Hex | RGB | Usage |
|---------|-----|-----|-------|
| Background | `#26141F` | (38, 20, 31) | Camera clear color |
| Arena Floor | `#3D1F2E` | (61, 31, 46) | Ground |
| UI Primary | `#993366` | (153, 51, 102) | Buttons, highlights |
| UI Accent | `#FF4D99` | (255, 77, 153) | Important highlights |

---

## HEROES (3 Characters)

### Hero 1: Bronze Warrior
| Property | Value | Visual Notes |
|----------|-------|--------------|
| Primary Color | `#CC8033` | Bronze/gold body |
| Secondary Color | `#8F5923` | Darker bronze accents |
| Attack Color | `#FFE066` | Golden attack trail |
| Body Type | Balanced, medium build | Classic warrior proportions |
| Mask Style | Traditional bronze mask | Simple, symmetrical |

**Character Traits:**
- Balanced stance
- Standard warrior attire
- Bronze armor elements

### Hero 2: Shadow Dancer
| Property | Value | Visual Notes |
|----------|-------|--------------|
| Primary Color | `#4D3366` | Deep purple body |
| Secondary Color | `#362447` | Darker purple accents |
| Attack Color | `#9966CC` | Purple attack trail |
| Body Type | Slim, agile build | Dancer proportions |
| Mask Style | Sleek, angular mask | Sharp features |

**Character Traits:**
- Dynamic pose
- Flowing cloth elements
- Lighter armor, more mobility

### Hero 3: Flame Bearer
| Property | Value | Visual Notes |
|----------|-------|--------------|
| Primary Color | `#E64D1A` | Fiery orange body |
| Secondary Color | `#B33D14` | Darker red accents |
| Attack Color | `#FFCC33` | Fire/golden trail |
| Body Type | Large, heavy build | Tank proportions |
| Mask Style | Fierce, flame-inspired | Aggressive features |

**Character Traits:**
- Powerful stance
- Heavy armor
- Flame motifs

---

## BOSSES (2 Characters)

### Boss 1: The Bronze Mask
| Property | Value |
|----------|-------|
| Primary Color | `#CC8033` |
| Secondary Color | `#4D331A` |
| Eye Color | `#FFE680` (glowing) |
| Attack Color | `#FF6633` |
| Phase 2 Tint | `#E6334D` (red shift) |

**Visual Phases:**
- **Phase 1 (100%-50% HP):** Calm, stoic expression
- **Phase 2 (50%-0% HP):** Cracked, glowing red eyes, angrier expression

**Size:** 2x player scale
**Position:** Right side of arena

### Boss 2: Chaos Totem
| Property | Value |
|----------|-------|
| Primary Color | `#803350` |
| Secondary Color | `#4D1F30` |
| Eye Color | `#FF66B3` (glowing pink) |
| Attack Color | `#CC1A66` |
| Phase 2 Tint | `#CC1A80` (purple shift) |

**Visual Phases:**
- **Phase 1:** Multiple mask faces stacked
- **Phase 2:** Faces split apart, chaotic arrangement

---

## SPRITE REQUIREMENTS

### Player Sprites (per hero)
| Sprite | Size | Frames | Notes |
|--------|------|--------|-------|
| Idle | 64x96px | 4-6 | Breathing animation |
| Walk/Run | 64x96px | 6-8 | Loop |
| Attack 1 | 96x96px | 3-4 | Quick jab |
| Attack 2 | 96x96px | 3-4 | Second hit |
| Attack 3 | 128x96px | 4-5 | Heavy finisher |
| Dodge | 96x64px | 4-6 | Roll animation |
| Hurt | 64x96px | 2-3 | Flinch |
| Death | 96x96px | 5-8 | Fall animation |
| Parry | 80x96px | 3-4 | Slap motion |

**Total per hero:** ~36-52 frames

### Boss Sprites (per boss)
| Sprite | Size | Frames | Notes |
|--------|------|--------|-------|
| Idle Phase 1 | 192x192px | 4-6 | Floating/breathing |
| Idle Phase 2 | 192x192px | 4-6 | More aggressive idle |
| Telegraph | 192x192px | 3-4 | Windup warning |
| Attack | 256x192px | 4-6 | Generic attack |
| Stagger | 192x192px | 2-3 | Stunned |
| Transform | 256x256px | 6-10 | Phase transition |
| Death | 256x256px | 8-12 | Destruction |

**Total per boss:** ~31-47 frames

### Projectiles & Effects
| Sprite | Size | Color |
|--------|------|-------|
| Normal Bullet | 16x16px | `#E6B333` gold |
| Parryable Bullet | 16x16px | `#FF80B3` **PINK** (important!) |
| Danger Bullet | 16x16px | `#FF3333` red |
| Laser Beam | 32xVARIABLE | `#FF4D1A` orange core |
| Minion | 32x32px | `#996633` brown mask |
| Spirit Projectile | 24x24px | `#CC9933` gold/ghost |

### UI Elements
| Element | Size | Notes |
|---------|------|-------|
| Health Bar BG | 300x30px | Dark gray |
| Health Bar Fill | 296x26px | Green → Red gradient |
| Boss Health BG | 600x40px | Dark gray |
| Boss Health Fill | 596x36px | Bronze color |
| Super Meter Card | 40x60px | Cuphead-style card |
| Button | 200x60px | Bronze themed |

---

## ANIMATION TIMING

| Animation | Duration | Notes |
|-----------|----------|-------|
| Player Attack | 0.45s total | 0.1s windup, 0.05s active, 0.3s recovery |
| Dodge Roll | 0.4s | Fast roll |
| Boss Telegraph | 0.5-1.0s | Clear warning before attack |
| Boss Attack | 0.3-0.5s | Quick execution |
| Phase Transition | 1.5s | Dramatic transformation |
| Parry Flash | 0.1s | Brief white flash |

---

## VISUAL EFFECTS TO CREATE

### Particles (Optional but nice)
1. **Hit Sparks** - When player hits boss
2. **Dust Puff** - During dodge roll
3. **Parry Flash** - Pink starburst on successful parry
4. **Death Explosion** - Boss defeat

### Screen Effects
1. **Camera Shake** - On hits (code handles this)
2. **Flash White** - On damage (code handles this)
3. **Vignette** - Low health warning (optional)

---

## FILE NAMING CONVENTION

```
sprites/
├── heroes/
│   ├── bronze_warrior/
│   │   ├── bronze_warrior_idle_01.png
│   │   ├── bronze_warrior_idle_02.png
│   │   ├── bronze_warrior_attack1_01.png
│   │   └── ...
│   ├── shadow_dancer/
│   └── flame_bearer/
├── bosses/
│   ├── bronze_mask/
│   │   ├── bronze_mask_idle_p1_01.png  (p1 = phase 1)
│   │   ├── bronze_mask_idle_p2_01.png  (p2 = phase 2)
│   │   └── ...
│   └── chaos_totem/
├── projectiles/
│   ├── bullet_normal.png
│   ├── bullet_parryable.png  (MUST BE PINK!)
│   └── ...
└── ui/
    ├── health_bar_bg.png
    └── ...
```

---

## QUICK START (Minimal Viable Art)

If short on time, create these first:

1. **Player idle** (1 frame, colored rectangle is fine)
2. **Boss idle** (1 frame per phase)
3. **Pink parryable bullet** (critical for gameplay!)
4. **Health bar fill** (simple gradient)

The code generates procedural placeholders for everything else.

---

## TOOLS RECOMMENDED

- **Aseprite** - Best for pixel art animation
- **Piskel** (piskelapp.com) - Free browser-based
- **Photoshop/GIMP** - For larger assets
- **Unity Sprite Editor** - For slicing spritesheets

---

## EXPORT SETTINGS

| Setting | Value |
|---------|-------|
| Format | PNG (with transparency) |
| Color Mode | RGBA 32-bit |
| Compression | None (Unity handles this) |
| DPI | 72 (standard for games) |

---

**Questions? Contact the development team.**

The code is flexible - colors can be changed via ScriptableObjects in Unity.
Focus on shapes and animation first, colors can be adjusted later.
