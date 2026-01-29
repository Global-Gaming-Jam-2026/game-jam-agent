# Unity Project Setup - Mask of the Bronze God

## Quick Start

### 1. Create Unity Project
1. Open Unity Hub
2. Click "New Project"
3. Select **2D (URP)** template
4. Name: `MaskOfTheBronzeGod`
5. Create in this `src/` folder (or copy Assets folder after creation)

### 2. Import Scripts
Copy the entire `Assets/` folder contents into your Unity project's Assets folder.

### 3. Setup Layers
Go to **Edit > Project Settings > Tags and Layers**

Create these layers:
- Layer 6: `Player`
- Layer 7: `Enemy`
- Layer 8: `PlayerAttack`
- Layer 9: `EnemyProjectile`

### 4. Setup Tags
Create these tags:
- `Player`
- `Boss`
- `PlayerAttack`

### 5. Setup Physics
Go to **Edit > Project Settings > Physics 2D**

Configure collision matrix:
- Player attacks Enemy: YES
- Enemy attacks Player: YES
- Player ignores PlayerAttack: YES
- Enemy ignores EnemyProjectile: YES

---

## Scene Setup

### Game Scene

1. **Create Scene**: File > New Scene > Save as "Game"

2. **Camera Setup**:
   - Select Main Camera
   - Add `CameraShake` component
   - Set Background color: `#3D2914` (Dark Bronze)

3. **Create Player**:
   - Create empty GameObject "Player"
   - Add `SpriteRenderer` (assign placeholder sprite)
   - Add `Rigidbody2D` (set to Kinematic)
   - Add `BoxCollider2D`
   - Add scripts: `PlayerController`, `PlayerCombat`, `PlayerHealth`
   - Add child "AttackPoint" (empty object for hitbox origin)
   - Set Layer: `Player`, Tag: `Player`

4. **Create Boss**:
   - Create empty GameObject "Boss"
   - Add `SpriteRenderer` (assign placeholder sprite)
   - Add `Rigidbody2D` (set to Kinematic)
   - Add `BoxCollider2D`
   - Add scripts: `BossController`, `BossHealth`
   - Add pattern scripts as children or components: `SweepAttack`, `SlamAttack`, `SpiritProjectileAttack`
   - Assign patterns to BossController's patterns list
   - Set Layer: `Enemy`, Tag: `Boss`

5. **Create GameManager**:
   - Create empty GameObject "GameManager"
   - Add `GameManager` component
   - Add `HitFeedback` component

6. **Create Arena Floor**:
   - Create Sprite (square)
   - Scale to arena size (20 x 1)
   - Add `BoxCollider2D`
   - Position below player

7. **Create UI Canvas**:
   - Create Canvas (Screen Space - Overlay)
   - Add player health bar (Image with fill)
   - Add boss health bar at top
   - Add UI scripts: `HealthBarUI`, `BossHealthBarUI`, `PauseMenuUI`, `GameOverUI`

### Main Menu Scene

1. Create Scene: "MainMenu"
2. Create Canvas with title and Play/Quit buttons
3. Add `MainMenuUI` script

---

## Component Configuration

### PlayerController
```
Move Speed: 6
Acceleration: 50
Dodge Speed: 12
Dodge Duration: 0.4
Dodge Cooldown: 0.6
I-Frame Duration: 0.2
```

### PlayerCombat
```
Attack Damage: 25
Attack Windup: 0.1
Attack Active: 0.05
Attack Recovery: 0.3
Max Combo Count: 3
Attack Size: (1.5, 1)
Enemy Layer: Enemy
```

### PlayerHealth
```
Max Health: 100
I-Frame Duration: 0.5
```

### BossController
```
Idle Duration: 1.5
Recovery Duration: 1
Stagger Duration: 2
Phase 2 Threshold: 0.5
Phase 2 Speed Multiplier: 1.25
Assign Player reference
Assign Patterns list
```

### BossHealth
```
Max Health: 500
Stagger Threshold: 100
Stagger Reset Time: 3
```

---

## Build Settings

1. Go to **File > Build Settings**
2. Add scenes: MainMenu, Game
3. Set MainMenu as first scene (index 0)
4. Target Platform: WebGL (or Windows)
5. Player Settings:
   - Resolution: 1920x1080
   - Company/Product Name
   - WebGL Memory: 256MB

---

## Testing Checklist

- [ ] Player moves with WASD
- [ ] Player dodge rolls with Space
- [ ] Player attacks with Mouse1/J
- [ ] Boss telegraphs attacks clearly
- [ ] Player can dodge boss attacks
- [ ] Health bars update correctly
- [ ] Player death triggers lose screen
- [ ] Boss death triggers win screen
- [ ] Restart works from both screens
- [ ] Pause menu works (Escape)

---

## Placeholder Art

Until you have real sprites, use colored squares:
- Player: 32x64 bronze rectangle
- Boss: 128x128 bronze/gold square with face features
- Attack effects: Semi-transparent colored shapes

Use the color palette from GAME_DESIGN.md:
- Bronze: #CD7F32
- Terracotta: #E2725B
- Ochre: #CC7722
- Dark Bronze: #3D2914
- Sand: #C2B280
