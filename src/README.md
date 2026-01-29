# Mask of the Bronze God

A Cuphead-inspired 2D boss fight game with bronze era totem aesthetics.

## Quick Start (5 minutes)

### Step 1: Create Unity Project
1. Open **Unity Hub**
2. Click **New Project**
3. Select **2D (URP)** or **2D Core**
4. Name it `MaskOfTheBronzeGod`
5. Click **Create Project**

### Step 2: Import Scripts
1. Copy the entire `Assets/Scripts/` folder from this directory
2. Paste it into your Unity project's `Assets/` folder
3. Wait for Unity to compile (bottom right progress bar)

### Step 3: Run Setup Wizard
1. In Unity, go to menu: **Game Jam → Setup Wizard**
2. Click **"Setup Layers & Tags"**
3. Click **"Create Game Scene"**
4. Click **"Create Main Menu Scene"**

### Step 4: Configure Build Settings
1. Go to **File → Build Settings**
2. Click **Add Open Scenes** (or drag scenes from Assets/Scenes)
3. Make sure **MainMenu** is first (index 0)
4. Make sure **Game** is second (index 1)

### Step 5: Play!
1. Open the **Game** scene (Assets/Scenes/Game.unity)
2. Press **Play** button (or Ctrl+P)

---

## Controls

| Key | Action |
|-----|--------|
| **WASD / Arrows** | Move |
| **Space** | Dodge Roll / Parry |
| **Mouse1 / J** | Attack (3-hit combo) |
| **Escape** | Pause |

---

## Game Features

### Player
- Responsive movement with acceleration
- Dodge roll with invincibility frames
- 3-hit attack combo with input buffering
- **Parry** pink projectiles for super meter

### Boss (Cuphead-style)
- **Multiple phases** with visual transformations
- Attack patterns:
  - **Sweep** - Horizontal slash across arena
  - **Slam** - Overhead strike at player position
  - **Bullet Circle** - Expanding bullet patterns (includes pink parryable bullets)
  - **Laser Beam** - Sweeping laser (Phase 2+)
  - **Minion Spawn** - Summons smaller enemies (Phase 2+)

### Juice
- Screen shake on hits
- Hitstop on impacts
- Damage flash effects
- Phase transition effects

---

## Project Structure

```
Assets/
├── Scripts/
│   ├── Core/           # GameManager, CameraShake
│   ├── Player/         # Controller, Combat, Health, Parry
│   ├── Boss/           # Controller, Health, Phases
│   │   └── Patterns/   # Attack patterns
│   ├── Combat/         # Hitbox, Hurtbox, Feedback
│   ├── UI/             # Health bars, Menus, Super meter
│   └── Editor/         # Setup Wizard (auto-setup tool)
├── Scenes/
│   ├── MainMenu.unity
│   └── Game.unity
└── Prefabs/
    ├── Player.prefab
    └── Boss.prefab
```

---

## Adding Art

Replace the colored placeholder sprites:

1. **Player**: 32x64 pixels recommended
2. **Boss**: 128x128 or larger for each phase/form
3. **Bullets**: 16x16 or 32x32

### Color Palette
| Use | Hex | Color |
|-----|-----|-------|
| Boss/Primary | #CD7F32 | Bronze |
| Danger/Attacks | #E2725B | Terracotta |
| Highlights | #CC7722 | Ochre |
| Background | #3D2914 | Dark Bronze |
| UI/Text | #C2B280 | Sand |
| Parry objects | #FF80AB | Pink |

---

## Adding Boss Phases (Cuphead-style transformations)

The boss uses `BossControllerMultiPhase` which supports multiple forms:

```csharp
// In Unity Inspector, add BossPhaseData entries:
// Phase 1: healthPercentStart=1.0, healthPercentEnd=0.5
// Phase 2: healthPercentStart=0.5, healthPercentEnd=0.25
// Phase 3: healthPercentStart=0.25, healthPercentEnd=0
```

Each phase can have:
- Different sprite/form
- Different animator
- Different available attack patterns
- Different speed multiplier

---

## Building for Web (itch.io)

1. **File → Build Settings**
2. Select **WebGL**
3. Click **Switch Platform**
4. Click **Build**
5. Upload the build folder to itch.io

---

## Troubleshooting

### "Script not found" errors
- Make sure all scripts compiled (no red errors in Console)
- Check that scripts are in `Assets/Scripts/` folder

### Player/Boss don't appear
- Check they have SpriteRenderer components
- Assign a sprite or the placeholder will be invisible

### Attacks don't hit
- Check layer setup: Player on "Player" layer, Boss on "Enemy" layer
- Check tags: Player tagged "Player", Boss tagged "Boss"

### Parry doesn't work
- Pink bullets have `isParryable = true`
- Press Space while near a pink projectile
- Timing window is 0.2 seconds

---

## Credits

Game Design & Code: Created with Claude Code
Theme: Chaos and Masks (Game Jam)
Style Reference: Cuphead

---

## Files Reference

| File | Purpose |
|------|---------|
| `PlayerController.cs` | Movement, dodge roll |
| `PlayerCombat.cs` | Attack combo |
| `PlayerParry.cs` | Parry mechanic, super meter |
| `BossControllerMultiPhase.cs` | Multi-form boss state machine |
| `BossPhaseData.cs` | Phase/form data container |
| `SweepAttack.cs` | Horizontal sweep pattern |
| `SlamAttack.cs` | Overhead slam pattern |
| `BulletCirclePattern.cs` | Circular bullet hell |
| `LaserBeamPattern.cs` | Sweeping laser |
| `MinionSpawnPattern.cs` | Summon minions |
| `GameSetupWizard.cs` | Editor auto-setup tool |
