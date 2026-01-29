# Mask of the Bronze God - COMPREHENSIVE COMPLETION PLAN

## Current Status
- **Unity Project**: `C:\Users\User\My project`
- **Scripts**: 37 C# scripts compiled
- **Scene**: Basic setup exists
- **Issue**: Player not moving despite input working

---

## PHASE 1: FIX CRITICAL BLOCKER (Player Movement)

### Diagnosis
The PlayerController uses `transform.position +=` which should work. Likely issues:
1. GameManager not in scene (Instance is null)
2. GameManager state not set to Playing
3. Script not attached to Player object

### Fix Steps
1. Open Unity project `C:\Users\User\My project`
2. In Hierarchy, check if GameManager exists
3. If not, create empty GameObject "GameManager" and add GameManager.cs
4. Verify Player has PlayerController component attached
5. Press Play and test WASD movement
6. If still not working, add debug log to PlayerController.Update():
   ```csharp
   Debug.Log($"State: {GameManager.Instance?.CurrentState}, Input: {horizontalInput}");
   ```

---

## PHASE 2: COMPLETE CORE GAMEPLAY

### 2.1 Verify All Systems Work
- [ ] Player moves with WASD
- [ ] Player dodges with Space
- [ ] Player attacks with Mouse1/J
- [ ] Boss attacks player (SweepAttack, SlamAttack)
- [ ] Player takes damage
- [ ] Boss takes damage
- [ ] Win condition (boss HP = 0)
- [ ] Lose condition (player HP = 0)
- [ ] Restart flow works

### 2.2 Fix Known Script Issues
From previous debug session:
- [x] Player Hurtbox (fixed in GameSetupWizard)
- [x] LaserBeamPattern damage interval (fixed)
- [x] SweepAttack single hit per sweep (fixed)
- [x] MinionSpawnPattern audio stacking (fixed)
- [x] MinionSpawnPattern orbit speed (fixed)

---

## PHASE 3: MULTIPLE HEROES (Day 2)

### Hero 1: Bronze Warrior (Default)
- HP: 100
- Speed: 6
- Damage: 25
- Dodge Speed: 12

### Hero 2: Swift Shadow
- HP: 70
- Speed: 9
- Damage: 20
- Dodge Speed: 15

### Hero 3: Iron Guardian
- HP: 150
- Speed: 4
- Damage: 30
- Dodge Speed: 10

### Implementation
1. Create HeroData ScriptableObjects in `Assets/Resources/Heroes/`
2. Connect HeroSelectUI to load selected hero
3. HeroInitializer applies stats from selected HeroData

---

## PHASE 4: MULTIPLE BOSSES (Day 2)

### Boss 1: Mask of the Bronze God (Current)
- HP: 500
- 2 Phases (100-50%, 50-0%)
- Patterns: Sweep, Slam (Phase 1), BulletCircle, Laser, Spirits, Minions (Phase 2)

### Boss 2: Chaos Herald (Harder)
- HP: 750
- 3 Phases (100-66%, 66-33%, 33-0%)
- Patterns: All of above + new variants

### Implementation
1. Create BossData ScriptableObjects in `Assets/Resources/Bosses/`
2. Connect BossSelectUI to load selected boss
3. BossInitializer applies stats from selected BossData

---

## PHASE 5: MENU SYSTEM (Day 2)

### Scenes Required
1. **MainMenu** - Start, Settings, Quit
2. **HeroSelect** - Pick hero
3. **BossSelect** - Pick boss (or auto)
4. **Game** - Main gameplay
5. **Victory** / **Defeat** - End screens (can be overlays)

### UI Scripts Already Exist
- MainMenuUI.cs
- HeroSelectUI.cs
- BossSelectUI.cs
- SettingsUI.cs
- PauseMenuUI.cs
- GameOverUI.cs
- TutorialUI.cs

### Implementation
1. Create MainMenu scene with UI Canvas
2. Wire up buttons to SceneTransition
3. Add Build Settings: MainMenu (0), HeroSelect (1), Game (2)

---

## PHASE 6: ART PASS (Day 2/3)

### Minimum Art Needed
| Asset | Size | Description |
|-------|------|-------------|
| Player Idle | 32x64 | Standing pose |
| Player Walk | 32x64 x4 | Walk cycle |
| Player Attack | 48x64 x3 | 3-hit combo |
| Player Dodge | 48x32 | Roll animation |
| Boss Phase 1 | 128x128 | Bronze mask |
| Boss Phase 2 | 128x128 | Cracked/glowing mask |
| Bullet | 16x16 | Bronze/pink projectiles |
| Arena | 1920x1080 | Background gradient |
| Health Bar | 200x20 | Player health |
| Boss Health | 400x30 | Boss health |
| Super Meter | 100x100 | Card icons |

### Art Sources
- Use existing PSD: `Untitled_Artwork (1).psd`
- Generate placeholders with solid colors
- Import from itch.io free assets if needed

---

## PHASE 7: AUDIO PASS (Day 3)

### Sound Effects (29 total)
Reference: `SOUND_LIST_FOR_AUDIO_TECH.md`

| Category | Sounds |
|----------|--------|
| Player | footstep, dodge, attack_1/2/3, hurt, death |
| Boss | telegraph, sweep, slam, bullet_fire, laser, hurt, death, phase_transition |
| UI | menu_click, menu_hover, victory, defeat, pause |
| Environment | ambient |

### Music
- One loop ~90 seconds
- 150 BPM, D minor
- Heavy drums + low brass

### Implementation
1. Generate SFX with sfxr.me or use free sounds
2. Create AudioManager or use Unity Audio Mixer
3. Assign clips to AudioSource components

---

## PHASE 8: POLISH (Day 3)

### Game Feel (from skills/game-feel/SKILL.md)
- [ ] Hitstop: 0.05s on hit, 0.1s on heavy hit
- [ ] Screen shake: light (2px), medium (5px), heavy (10px)
- [ ] Damage flash: white 0.1s
- [ ] Attack telegraph: 0.3-0.5s warning
- [ ] Particle effects on impact

### Balance Tuning
- Player HP: 100 (3-4 hits to die)
- Boss attack damage: 25-35
- Player attack damage: 15-25
- Boss HP: 500 (20-30 hits to kill)
- I-frames: 0.2s dodge, 0.5s after damage

---

## PHASE 9: BUILD & SHIP (Day 3)

### Pre-Build Checklist
- [ ] All scenes in Build Settings
- [ ] Player Preferences saved
- [ ] Volume settings work
- [ ] No console errors
- [ ] Game completes start-to-finish

### WebGL Build
1. File → Build Settings → WebGL
2. Player Settings:
   - Compression: Gzip
   - Memory Size: 256MB
   - Enable Exceptions: Full
3. Build and Run locally
4. Test in browser (Chrome/Firefox)

### itch.io Submission
1. Create game page
2. Upload WebGL build as ZIP
3. Set embed dimensions: 960x540 or 1280x720
4. Add screenshots/GIF
5. Write description
6. Publish

---

## IMMEDIATE NEXT ACTIONS

### Right Now:
1. **Open Unity project**: `C:\Users\User\My project`
2. **Add GameManager** to scene if missing
3. **Test player movement**
4. **Report back** what happens

### If Movement Works:
1. Test full gameplay loop
2. Create HeroData assets
3. Build menu scenes

### If Movement Fails:
1. Add debug logs
2. Check Console for errors
3. Verify component attachments

---

## TIME ESTIMATES

| Phase | Tasks | Priority |
|-------|-------|----------|
| 1. Fix Movement | 15 min | CRITICAL |
| 2. Verify Core | 30 min | HIGH |
| 3. Heroes | 1 hour | MEDIUM |
| 4. Bosses | 1 hour | MEDIUM |
| 5. Menus | 1 hour | HIGH |
| 6. Art | 2 hours | MEDIUM |
| 7. Audio | 1 hour | LOW |
| 8. Polish | 1 hour | LOW |
| 9. Ship | 30 min | HIGH |

**Total: ~8-9 hours of focused work**

---

## Files to Modify/Create

### ScriptableObjects to Create
- `Assets/Resources/GameConfig.asset`
- `Assets/Resources/Heroes/BronzeWarrior.asset`
- `Assets/Resources/Heroes/SwiftShadow.asset`
- `Assets/Resources/Heroes/IronGuardian.asset`
- `Assets/Resources/Bosses/BronzeMask.asset`
- `Assets/Resources/Bosses/ChaosHerald.asset`

### Scenes to Create
- `Assets/Scenes/MainMenu.unity`
- `Assets/Scenes/HeroSelect.unity` (optional, can be part of MainMenu)
- `Assets/Scenes/Game.unity` (exists)

### Audio to Add
- `Assets/Audio/SFX/` (29 sound effects)
- `Assets/Audio/Music/` (1 background loop)
