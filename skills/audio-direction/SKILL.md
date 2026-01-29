# AUDIO DIRECTION FOR GAME JAMS

Expert guidance for sound design, music direction, and audio implementation.

---

## AUDIO PRIORITY FOR JAMS

| Priority | What | Why |
|----------|------|-----|
| 1 | Core feedback sounds | Players need feedback |
| 2 | Menu/UI sounds | Polish perception |
| 3 | Background music | Sets mood |
| 4 | Ambient sounds | Immersion (time permitting) |

---

## ESSENTIAL SFX BY GAME TYPE

### Platformer
- [ ] Jump
- [ ] Land
- [ ] Collect item
- [ ] Hit enemy
- [ ] Take damage
- [ ] Death
- [ ] Level complete

### Shooter
- [ ] Player shoot
- [ ] Enemy shoot
- [ ] Explosion
- [ ] Hit marker
- [ ] Power-up
- [ ] Player death
- [ ] Enemy death

### Puzzle
- [ ] Piece select
- [ ] Piece place
- [ ] Match/clear
- [ ] Invalid move
- [ ] Level complete
- [ ] Combo bonus

### Top-Down/RPG
- [ ] Footsteps (optional)
- [ ] Attack/action
- [ ] Damage taken
- [ ] Item pickup
- [ ] Menu navigation
- [ ] Door/transition
- [ ] NPC interaction

---

## MUSIC MOOD GUIDE

### By Game Feel

| Mood | Tempo (BPM) | Key | Instruments |
|------|-------------|-----|-------------|
| Action/Intense | 140-180 | Minor | Synths, drums, bass |
| Peaceful/Calm | 60-80 | Major | Piano, pads, strings |
| Mysterious | 70-100 | Minor/Modal | Pads, bells, low drones |
| Playful/Fun | 120-140 | Major | Chiptune, bouncy bass |
| Horror | 50-80 | Chromatic | Drones, dissonance, silence |
| Epic/Boss | 150-170 | Minor | Orchestra, heavy drums |
| Retro/8-bit | 120-150 | Any | Chiptune, square waves |

### Music Looping Tips
- End on same chord as beginning
- Crossfade 0.1-0.5 seconds
- 30-90 second loops work well
- Avoid recognizable melodic endings

---

## SFX DESIGN PRINCIPLES

### Feedback Timing
| Action | Sound Timing |
|--------|--------------|
| Jump | Instant on button press |
| Land | On ground contact |
| Hit | On collision frame |
| Collect | On pickup trigger |
| UI | On input, not on release |

### Sound Layering
```
Impact sound = Attack + Body + Tail

Attack: Sharp transient (click, snap)
Body: Character of sound (punch, slice)
Tail: Decay/reverb (room, echo)
```

### Volume Balancing (Relative)
```
SFX (player actions): 100%
SFX (environment): 70-80%
Music: 40-60%
Ambient: 30-50%
UI sounds: 80-90%
```

---

## QUICK SFX CREATION

### Using SFXR/BFXR (Free Tools)
| Sound Type | Preset to Start |
|------------|-----------------|
| Jump | Jump |
| Shoot | Laser/Shoot |
| Explosion | Explosion |
| Pickup | Coin/Pickup |
| Hit | Hit/Hurt |
| Powerup | Powerup |
| Blip/UI | Blip/Select |

### Parameter Tips
- **Frequency**: Higher = smaller object
- **Decay**: Longer = more reverb/echo
- **Noise**: More = grittier/explosive
- **Square wave**: Classic 8-bit sound
- **Saw wave**: Buzzy, aggressive
- **Sine wave**: Pure, gentle

---

## FREE AUDIO SOURCES

### SFX
| Source | Notes |
|--------|-------|
| Freesound.org | Huge library, check licenses |
| OpenGameArt.org | Game-ready, usually CC0 |
| Kenney.nl | Clean UI and game sounds |
| BFXR (tool) | Generate retro SFX |
| sfxr.me (web) | Browser-based BFXR |
| ZapSplat.com | Free with attribution |

### Music
| Source | Notes |
|--------|-------|
| Kevin MacLeod | Royalty-free, attribution |
| OpenGameArt.org | Various styles |
| FreeMusicArchive | Check licenses carefully |
| CC Mixter | Creative Commons music |

### Tools
| Tool | Use |
|------|-----|
| Audacity | Edit, trim, convert audio |
| BFXR | Generate retro SFX |
| BeepBox | Browser chiptune music |
| Bosca Ceoil | Simple music maker |
| LMMS | Full DAW (free) |

---

## AUDIO IMPLEMENTATION

### Volume Settings (Code)
```
Master Volume: 1.0 (user adjustable)
├── Music: 0.5
├── SFX: 0.8
│   ├── Player: 1.0
│   ├── Environment: 0.7
│   └── UI: 0.9
└── Ambient: 0.4
```

### When to Play Sounds
```
PLAY sound when:
- Player does action (immediate feedback)
- Important event happens (damage, collect)
- State changes (menu open, level start)

DON'T PLAY sound when:
- Too frequent (limit to every X ms)
- Player isn't causing it
- It would be annoying on repeat
```

### Sound Pooling (Performance)
- Pre-load all sounds at start
- Limit simultaneous instances
- Stop old sounds before starting new (same type)

---

## MUSIC BRIEFS

Use these to describe what you want to a composer or for searching:

### Template
```
MUSIC BRIEF: [Track Name]

Mood: [happy/sad/tense/peaceful/action/mysterious]
Energy: [low/medium/high]
Tempo: [slow/medium/fast] ([X] BPM)
Reference: [Similar game or song]
Duration: [X] seconds, looping
Instruments: [suggestions]

Notes: [Any specific requirements]
```

### Examples

**Main Theme - Platformer**
```
Mood: Playful, adventurous
Energy: Medium-high
Tempo: Fast (140 BPM)
Reference: Celeste, Shovel Knight
Duration: 60 seconds, looping
Instruments: Chiptune, energetic drums
Notes: Should feel encouraging, not stressful
```

**Puzzle Theme**
```
Mood: Calm, focused
Energy: Low
Tempo: Slow (70 BPM)
Reference: Tetris Effect (chill version)
Duration: 90 seconds, looping
Instruments: Piano, soft pads
Notes: Non-distracting, good for thinking
```

**Boss Battle**
```
Mood: Intense, epic
Energy: Very high
Tempo: Fast (160 BPM)
Reference: Undertale boss fights
Duration: 60 seconds, looping
Instruments: Heavy drums, aggressive synths
Notes: Should feel dangerous and exciting
```

---

## COMMON AUDIO MISTAKES

### Avoid These
| Mistake | Fix |
|---------|-----|
| Sounds too loud | Lower to 70-80%, boost later if needed |
| Music drowns SFX | Duck music during important SFX |
| No audio options | Add volume sliders (music/SFX separate) |
| Annoying loops | Longer loops, musical endings |
| Clipping/distortion | Lower master volume, check peaks |
| Delayed feedback | Play sounds instantly, not after animation |

### Clipping Prevention
- Keep master volume at -3dB to -6dB
- Use limiter on master bus
- Test on multiple devices

---

## ADAPTIVE AUDIO (IF TIME)

### Simple Implementation
```
IF game_state == "calm":
    play music_calm
ELSE IF game_state == "danger":
    crossfade to music_danger
ELSE IF game_state == "boss":
    play music_boss
```

### Music Layers (Advanced)
```
Layer 1: Ambient pad (always playing)
Layer 2: Drums (add when action)
Layer 3: Lead melody (add at high intensity)

Fade layers in/out based on gameplay
```

---

## PRE-SUBMISSION CHECKLIST

- [ ] All player actions have audio feedback?
- [ ] Music loops seamlessly?
- [ ] Volume balance feels right?
- [ ] No clipping or distortion?
- [ ] Audio settings accessible?
- [ ] Mute works correctly?
- [ ] Sound doesn't play when paused?
- [ ] Credits include audio attribution?

---

## WHEN TO STOP

**Good enough for jam:**
- Core gameplay has feedback sounds
- Background music exists and loops
- Nothing is painfully loud or jarring

**Skip if running low on time:**
- Ambient soundscapes
- Dynamic music systems
- Positional audio
- Footstep variations
- Voice acting

---

*Remember: Simple, well-timed sound beats complex audio nobody notices.*
