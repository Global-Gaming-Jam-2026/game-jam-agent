# Game Design Document

## Title
**Mask of the Bronze God**

## Theme
Chaos and Masks

## Description
A 2D pattern-based boss fight where the player faces an ancient bronze sarcophagus mask come to life. Learn the boss's attack patterns, dodge with precise timing, and strike during recovery windows. Dark Souls-inspired methodical combat with a Bronze Era totem aesthetic.

## Core Mechanic
**Pattern Recognition + Timing**
- Watch for telegraph animations
- Dodge at the right moment
- Punish during recovery windows
- Repeat until victory

## MVP Features (Must Have)
1. [x] Player movement (WASD horizontal)
2. [x] Player dodge roll with i-frames
3. [x] Player 3-hit attack combo
4. [x] Boss with state machine
5. [x] Pattern 1: Sweep Attack
6. [x] Pattern 2: Slam Attack
7. [x] Health bars for player and boss
8. [x] Win/Lose conditions
9. [x] Game restart flow

## Stretch Goals (If Time)
- [ ] Pattern 3: Spirit Projectiles (Phase 2)
- [ ] Boss stagger mechanic
- [ ] Phase 2 speed increase
- [ ] Damage numbers
- [ ] Particle effects

## Controls
| Input | Action |
|-------|--------|
| WASD / Arrows | Move horizontally |
| Space | Dodge roll (i-frames) |
| Mouse1 / J | Attack (3-hit combo) |
| Escape | Pause |

## Art Style
**Bronze Era Totem Aesthetic**
- Warm earth tones palette
- Geometric patterns on boss
- Simple but impactful silhouettes
- Clear visual hierarchy

### Color Palette
| Role | Color | Hex |
|------|-------|-----|
| Primary (Boss) | Bronze | #CD7F32 |
| Danger | Terracotta | #E2725B |
| Highlights | Ochre | #CC7722 |
| Background | Dark Bronze | #3D2914 |
| UI/Text | Sand | #C2B280 |

## Audio
**SFX (via sfxr.me):**
- Player: attack whoosh, dodge swoosh, hit grunt, death
- Boss: telegraph tone, sweep, slam, hit crack, death shatter
- UI: menu clicks

**Music:**
- Single epic loop
- 150-160 BPM, D minor
- Heavy drums + low brass
- ~90 seconds, seamless loop

## Technical Notes
- Engine: Unity 2D (URP)
- Target: WebGL (itch.io) + Windows backup
- Resolution: 1920x1080
- Physics: Rigidbody2D Kinematic, trigger colliders for hitboxes

## Boss Design

### The Mask
A floating bronze sarcophagus mask with glowing eyes that telegraph attacks.

**Stats:**
- HP: 500
- Phases: 2 (100-50%, 50-0%)

**Attack Patterns:**

| Pattern | Telegraph | Attack | Counter |
|---------|-----------|--------|---------|
| Sweep | Tilt + charge glow (0.5s) | Wide horizontal slash | Roll through |
| Slam | Rise + shadow below (0.7s) | Vertical slam at player | Sidestep, punish recovery |
| Spirits | Eyes glow, mini masks (0.4s) | 3 homing projectiles | Dodge timing |

## Player Design

**Stats:**
- HP: 100
- I-frames after hit: 0.5s
- Dodge i-frames: 0.2s

**Combat Values:**
- Move speed: 6 units/sec
- Dodge duration: 0.4s
- Attack wind-up: 0.1s
- Attack active: 0.05s
- Attack recovery: 0.3s
