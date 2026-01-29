# Sound List for "Mask of the Bronze God"
## For Audio Technician

**Game Style:** Cuphead-inspired 2D boss fight
**Theme:** Bronze era, totemic masks, chaos
**Mood:** Intense but fun, cartoonish violence, satisfying feedback

---

## AUDIO FORMAT REQUIREMENTS

| Setting | Value |
|---------|-------|
| Format | `.wav` or `.ogg` (Unity accepts both) |
| Sample Rate | 44100 Hz (standard) |
| Bit Depth | 16-bit |
| Channels | Mono (for SFX), Stereo (for music) |
| Loudness | Normalized, no clipping |

**File Naming:** Use snake_case, e.g., `player_attack_01.wav`

---

## PLAYER SOUNDS (8 files)

| # | File Name | Description | Duration | Notes |
|---|-----------|-------------|----------|-------|
| 1 | `player_attack_01.wav` | First hit of combo | 0.1-0.2s | Light punch/swipe |
| 2 | `player_attack_02.wav` | Second hit of combo | 0.1-0.2s | Slightly heavier |
| 3 | `player_attack_03.wav` | Final hit of combo (big hit) | 0.2-0.3s | Powerful impact, most satisfying |
| 4 | `player_dodge.wav` | Dodge roll/dash | 0.2-0.4s | Whoosh, wind sound |
| 5 | `player_hurt.wav` | Player takes damage | 0.3-0.5s | Grunt + impact |
| 6 | `player_death.wav` | Player dies | 0.5-1.0s | Dramatic fall |
| 7 | `player_parry.wav` | Successful parry (slapping pink projectile) | 0.2-0.3s | Satisfying "slap" + ding, like deflecting |
| 8 | `player_super_ready.wav` | Super meter is full | 0.3-0.5s | Power-up chime, excitement |

---

## BOSS SOUNDS (12 files)

### General Boss Sounds
| # | File Name | Description | Duration | Notes |
|---|-----------|-------------|----------|-------|
| 9 | `boss_telegraph.wav` | Warning sound before any attack | 0.3-0.5s | Rising tension, "about to strike" |
| 10 | `boss_hurt.wav` | Boss takes damage | 0.1-0.2s | Heavy impact, stone/bronze hit |
| 11 | `boss_stagger.wav` | Boss is stunned (100+ damage) | 0.5-0.8s | Heavy stun, disoriented |
| 12 | `boss_transform.wav` | Boss changes to Phase 2 | 1.0-2.0s | Dramatic transformation, power surge |
| 13 | `boss_death.wav` | Boss defeated | 1.5-3.0s | Epic destruction, mask shattering |

### Attack Pattern Sounds
| # | File Name | Description | Duration | Notes |
|---|-----------|-------------|----------|-------|
| 14 | `attack_sweep.wav` | Horizontal sweep attack | 0.3-0.5s | Whooshing blade/arm swing |
| 15 | `attack_slam.wav` | Overhead slam (high damage) | 0.3-0.5s | Heavy impact, ground shake |
| 16 | `attack_bullet_fire.wav` | Bullets spawn (bullet hell) | 0.2-0.3s | Multiple projectiles launching |
| 17 | `attack_laser_charge.wav` | Laser beam charging | 0.5-1.0s | Building energy, rising pitch |
| 18 | `attack_laser_fire.wav` | Laser beam active | Loop 2.0s | Continuous beam, can loop |
| 19 | `attack_minion_spawn.wav` | Minions summoned | 0.5-0.8s | Dark magic summoning |
| 20 | `attack_spirit_projectile.wav` | Homing spirits launched | 0.3-0.5s | Ghostly whoosh |

---

## UI SOUNDS (6 files)

| # | File Name | Description | Duration | Notes |
|---|-----------|-------------|----------|-------|
| 21 | `ui_button_click.wav` | Menu button pressed | 0.05-0.1s | Clean click |
| 22 | `ui_button_hover.wav` | Mouse over button | 0.05s | Subtle tick |
| 23 | `ui_pause.wav` | Game paused | 0.2-0.3s | Time freeze sound |
| 24 | `ui_unpause.wav` | Game resumed | 0.2-0.3s | Time resume |
| 25 | `game_win.wav` | Player wins (boss defeated) | 2.0-4.0s | Victory fanfare, triumphant |
| 26 | `game_lose.wav` | Player loses (death) | 1.5-3.0s | Defeat jingle, somber |

---

## MUSIC (3 files)

| # | File Name | Description | Duration | Notes |
|---|-----------|-------------|----------|-------|
| 27 | `music_menu.ogg` | Main menu background | 30-60s loop | Bronze era vibe, mysterious |
| 28 | `music_boss_phase1.ogg` | Boss fight Phase 1 | 60-90s loop | Intense, action, faster tempo |
| 29 | `music_boss_phase2.ogg` | Boss fight Phase 2 (harder) | 60-90s loop | Even more intense, desperate |

---

## PRIORITY ORDER

**Must Have (game won't feel right without):**
1. player_attack_01, 02, 03
2. player_hurt
3. player_parry
4. boss_telegraph
5. attack_sweep, attack_slam
6. game_win, game_lose

**Should Have (adds polish):**
7. player_dodge
8. boss_hurt, boss_death
9. All other attack sounds
10. UI sounds

**Nice to Have (if time permits):**
11. Music tracks
12. boss_transform
13. player_super_ready

---

## STYLE REFERENCE

**Cuphead audio style:**
- Exaggerated, cartoonish impacts
- Satisfying "punch" to attacks
- Clear audio feedback for player actions
- Distinct warning sounds before boss attacks
- 1930s jazz-influenced music (optional for us)

**Our twist (Bronze Era):**
- Metallic, bronze-like impacts
- Ancient/totemic mystical elements
- Deep, resonant bass for boss
- Mask-like voice processing (optional)

---

## DELIVERY

Please deliver sounds in a folder structure:
```
sounds/
├── player/
│   ├── player_attack_01.wav
│   ├── player_attack_02.wav
│   └── ...
├── boss/
│   ├── boss_telegraph.wav
│   └── ...
├── attacks/
│   ├── attack_sweep.wav
│   └── ...
├── ui/
│   └── ...
└── music/
    └── ...
```

---

## QUICK GENERATION OPTION

If creating from scratch is too time-consuming, these free tools work:
- **SFXR/BFXR** (sfxr.me) - Retro game sound generator
- **Freesound.org** - Free sound library (check licenses)
- **Audacity** - Free editing software

---

**Total Files Needed: 29**
- Player: 8
- Boss: 12
- UI: 6
- Music: 3

Questions? Contact the development team.
