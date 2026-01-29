# ART DIRECTION FOR GAME JAMS

Expert guidance for visual style, color theory, and asset planning.

---

## QUICK STYLE SELECTOR

| Time Available | Recommended Style |
|----------------|-------------------|
| < 24 hours | Minimalist, solid colors, geometric shapes |
| 24-48 hours | Pixel art (low-res), limited palette |
| 48-72 hours | Pixel art (detailed) or hand-drawn |
| 72+ hours | Any style you're comfortable with |

---

## COLOR PALETTES BY MOOD

### Action/Intense
```
Primary: #FF4444 (red)
Secondary: #FF8800 (orange)
Accent: #FFDD00 (yellow)
Background: #1A1A2E (dark blue)
```

### Calm/Peaceful
```
Primary: #4ECDC4 (teal)
Secondary: #A8E6CF (mint)
Accent: #FFEAA7 (cream)
Background: #DFE6E9 (light gray)
```

### Mysterious/Dark
```
Primary: #6C5CE7 (purple)
Secondary: #A29BFE (lavender)
Accent: #00CEC9 (cyan)
Background: #2D3436 (charcoal)
```

### Playful/Fun
```
Primary: #FF6B9D (pink)
Secondary: #C44569 (magenta)
Accent: #F8B500 (gold)
Background: #FFF3E6 (cream)
```

### Retro/Nostalgic
```
Primary: #E17055 (coral)
Secondary: #FDCB6E (mustard)
Accent: #00B894 (green)
Background: #2D3436 (dark)
```

### Horror/Spooky
```
Primary: #B33939 (blood red)
Secondary: #6C3483 (dark purple)
Accent: #1ABC9C (sickly green)
Background: #0D0D0D (near black)
```

---

## RESOLUTION GUIDE

### Pixel Art Scales
| Target | Sprite Size | Screen Resolution |
|--------|-------------|-------------------|
| Tiny (Pico-8 style) | 8x8 to 16x16 | 128x128 → scale up |
| Small (NES style) | 16x16 to 32x32 | 256x240 → scale up |
| Medium (SNES style) | 32x32 to 64x64 | 320x240 → scale up |
| Large (GBA style) | 64x64+ | 480x320 → scale up |

### Scaling Rules
- ALWAYS use integer scaling (2x, 3x, 4x)
- NEVER use filtering on pixel art
- Set texture import to "Nearest" not "Linear"

---

## ASSET CHECKLIST BY GAME TYPE

### Platformer
- [ ] Player idle (1-4 frames)
- [ ] Player run (4-8 frames)
- [ ] Player jump (1-3 frames)
- [ ] Ground tiles (minimum 3: left, middle, right)
- [ ] Platform tiles
- [ ] Background (can be solid color)
- [ ] Collectible (1-2 frames shimmer)
- [ ] Enemy (2-4 frames)

### Top-Down
- [ ] Player (4 directions, 2-4 frames each)
- [ ] Floor tiles (2-3 variants)
- [ ] Wall tiles (corners + edges = 9 minimum)
- [ ] Objects/obstacles (3-5 types)
- [ ] Collectibles
- [ ] Enemy (4 directions or rotation)

### Puzzle
- [ ] Puzzle pieces/blocks (by type)
- [ ] Board/grid background
- [ ] Selection highlight
- [ ] Match/clear effect (particles ok)
- [ ] UI elements (score, timer)

### Shooter
- [ ] Player ship/character
- [ ] Bullets (player + enemy)
- [ ] Enemies (2-4 types)
- [ ] Background (scrolling layers)
- [ ] Explosions (4-8 frames)
- [ ] Power-ups (2-3 types)

---

## COMPOSITION PRINCIPLES

### Rule of Thirds
```
┌───┬───┬───┐
│   │   │   │  Place important elements
├───┼───┼───┤  at intersection points
│   │ X │   │  (marked X)
├───┼───┼───┤
│   │   │   │
└───┴───┴───┘
```

### Visual Hierarchy
1. **Player** - Most contrast, brightest colors
2. **Enemies/Hazards** - Warm colors (red, orange)
3. **Collectibles** - Shiny, animated, stands out
4. **Interactive** - Slightly different from background
5. **Background** - Lowest contrast, muted colors

### Readability Priority
```
HIGH CONTRAST: Player, hazards, collectibles
MEDIUM CONTRAST: Platforms, walls, interactive
LOW CONTRAST: Background, decorations
```

---

## ANIMATION FRAME COUNTS

| Animation Type | Minimum | Good | Great |
|----------------|---------|------|-------|
| Idle | 1 | 2-4 | 4-8 |
| Walk/Run | 2 | 4-6 | 6-12 |
| Jump | 1 | 2-3 | 4-6 |
| Attack | 2 | 3-5 | 6-10 |
| Death | 1 | 3-5 | 6-12 |
| Particle/FX | 3 | 4-6 | 8-16 |

### Frame Timing
- Walk cycle: 0.1s per frame (100ms)
- Run cycle: 0.08s per frame (80ms)
- Idle: 0.2-0.5s per frame (slow, relaxed)
- Attack: 0.05-0.08s per frame (fast, snappy)

---

## STYLE GUIDES

### Minimalist
- Solid shapes, no outlines
- 2-4 colors maximum
- Geometric forms (circles, squares, triangles)
- White space is your friend
- Focus on silhouette readability

### Pixel Art
- Consistent pixel size throughout
- Limited palette (8-32 colors)
- No anti-aliasing within sprites
- Outlines optional but consistent
- Dithering for gradients (use sparingly)

### Hand-Drawn
- Consistent line weight
- Wobble/imperfection is charm
- Can use sketchy shading
- Keep proportions intentional
- Limited color helps cohesion

### Low-Poly 3D
- Hard edges, flat shading
- Limited texture use
- Bright, saturated colors
- Consistent polygon density
- Strong silhouettes

---

## UI STYLE TIPS

### Fonts
| Game Mood | Font Style |
|-----------|------------|
| Retro | Pixel fonts (Press Start 2P, VT323) |
| Clean | Sans-serif (Roboto, Open Sans) |
| Fantasy | Serif with flourishes |
| Sci-Fi | Monospace, tech-looking |

### UI Color Rules
- **Background**: Semi-transparent dark (70-90% opacity)
- **Text**: High contrast to background
- **Buttons**: Distinguish idle/hover/pressed states
- **Health/Danger**: Red is universal for damage

### Readable Text Sizes
- Title: 48-72px
- Headers: 32-48px
- Body: 16-24px
- Small: 12-16px (sparingly)

---

## QUICK ASSET SOURCES (FREE)

| Source | Best For |
|--------|----------|
| OpenGameArt.org | 2D sprites, tiles, audio |
| Kenney.nl | Polished 2D/3D packs |
| itch.io (free assets) | Varied quality, check licenses |
| Lospec.com | Pixel art palettes |
| Coolors.co | Color palette generator |

---

## PRE-FLIGHT CHECKLIST

Before finalizing art:

- [ ] All sprites same resolution/scale?
- [ ] Color palette consistent across assets?
- [ ] Player clearly visible against backgrounds?
- [ ] Hazards/enemies clearly distinguishable?
- [ ] UI readable at target resolution?
- [ ] Animations smooth (no jarring frames)?
- [ ] Collectibles/pickups obvious?
- [ ] Interactive elements clearly different from static?

---

## WHEN TO STOP POLISHING

**Game jam priority order:**
1. Gameplay works and is fun
2. Everything is readable/understandable
3. Basic visual consistency
4. Polish animations
5. Extra details (time permitting)

**Stop polishing art when:**
- Core gameplay isn't done
- You're repeating minor adjustments
- It's already readable and consistent
- Time would be better spent elsewhere

---

*Remember: Shipped pixel art beats unfinished masterpiece.*
