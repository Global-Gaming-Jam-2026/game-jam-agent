# UX DESIGN FOR GAME JAMS

Expert guidance for menus, accessibility, and player experience.

---

## UX PRIORITY FOR JAMS

| Priority | What | Why |
|----------|------|-----|
| 1 | Start playing fast | Don't lose players at menu |
| 2 | Clear controls | Players know what to do |
| 3 | Readable UI | Information is visible |
| 4 | Basic settings | Volume, restart at minimum |
| 5 | Polish | Transitions, animations |

---

## MENU FLOW

### Minimal (Jam Default)
```
TITLE SCREEN
├── Play → Start Game
├── Settings → Volume, Controls
└── Quit (optional on web)
```

### Standard
```
TITLE SCREEN
├── Play → Level Select OR Start
├── Settings
│   ├── Audio (Music/SFX volume)
│   ├── Controls (rebind/show)
│   └── Display (fullscreen)
├── Credits
└── Quit

PAUSE MENU (in-game)
├── Resume
├── Restart Level
├── Settings
└── Quit to Menu
```

### Time-Saving Tip
- Skip level select if only one level
- Skip credits until submission
- "Quit" not needed for web games

---

## FIRST-TIME EXPERIENCE

### Goal: Playing in < 30 seconds
```
1. Click "Play" (only obvious option)
2. Brief controls shown (or learn by doing)
3. Gameplay starts

AVOID:
- Long intros/cutscenes
- Mandatory tutorials
- Multiple menu clicks to start
- Loading screens without progress
```

### Teaching Controls
| Method | When |
|--------|------|
| On-screen prompts | Best default |
| Interactive tutorial | If mechanics complex |
| Separate tutorial level | If time allows |
| Trial and error | Simple games only |

### On-Screen Prompt Format
```
Simple: [SPACE] Jump
Better: [SPACE] Jump | [E] Interact
Visual: ⬆️ Move  |  🔘 Jump  |  ❌ Attack
```

---

## INFORMATION DISPLAY

### What Players Need to See
| Game Type | Essential Info |
|-----------|----------------|
| Platformer | Lives/health, collectibles |
| Shooter | Health, ammo, score |
| Puzzle | Moves/time, level number |
| Survival | Health, resources, threat |

### UI Placement
```
┌─────────────────────────────────┐
│ [Health]            [Score]     │  ← Top: Persistent info
│                                 │
│                                 │
│         GAMEPLAY AREA           │
│                                 │
│                                 │
│              [Item]  [Ability]  │  ← Bottom: Actions
└─────────────────────────────────┘
```

### Readability Rules
- Text: Minimum 16px, prefer 20-24px
- Contrast: Light text on dark, or vice versa
- Shadows/outlines: Help text stand out
- Don't cover gameplay with UI

---

## FEEDBACK PATTERNS

### Player Needs to Know
| Event | Feedback Type |
|-------|---------------|
| Action worked | Visual + sound |
| Action failed | Different sound + shake |
| Damage taken | Flash + sound + shake |
| Near death | Red vignette/pulse |
| Collected item | Particle + sound + UI update |
| Level complete | Big celebration |

### Progress Feedback
```
COLLECTING:
- Counter updates immediately
- Brief "pop" animation on counter
- Sound confirms collection

HEALTH CHANGE:
- Bar animates (not instant)
- Color shifts with amount
- Flash when critical

LOADING:
- Show progress if > 2 seconds
- Animate something (never freeze)
```

---

## PAUSE & SETTINGS

### Pause Menu Essentials
- [ ] Pause actually pauses everything
- [ ] Visual indication game is paused
- [ ] Resume option
- [ ] Restart level option
- [ ] Audio settings accessible

### Settings Options (Priority Order)
1. **Music volume** (slider or on/off)
2. **SFX volume** (slider or on/off)
3. **Fullscreen toggle** (desktop)
4. **Controls display** (not rebind)

### Settings That Can Wait
- Control rebinding
- Graphics quality
- Language selection
- Colorblind modes (see accessibility)

---

## ACCESSIBILITY BASICS

### Minimum Accessibility
- [ ] Can pause at any time
- [ ] Audio can be muted
- [ ] Text is readable size (16px+)
- [ ] Controls shown somewhere

### Better Accessibility
- [ ] High contrast mode option
- [ ] Colorblind palette consideration
- [ ] Scalable UI/text
- [ ] Alternative control schemes

### Color Accessibility
```
DON'T rely only on color for:
- Good vs bad (add symbols: ✓ vs ✗)
- Team indicators (add shapes)
- Status effects (add icons)

DO use:
- High contrast combinations
- Colorblind-safe palettes
- Redundant indicators (color + shape)
```

### Colorblind-Safe Colors
```
Instead of red/green:
- Blue (#0077BB) vs Orange (#EE7733)
- Purple (#AA3377) vs Yellow (#CCBB44)

Always combine color with:
- Different shapes
- Text labels
- Patterns/textures
```

---

## ERROR HANDLING

### When Things Go Wrong
| Situation | Good Response |
|-----------|---------------|
| Player dies | Quick restart, minimal interruption |
| Level failed | Clear reason, try again button |
| Soft lock | Restart level always available |
| Crash/error | Save progress if possible |

### Death Handling
```
QUICK RESTART (preferred):
1. Brief death animation (< 1s)
2. Fade/wipe transition (< 0.5s)
3. Respawn at checkpoint
4. Immediately playable

AVOID:
- Long death animations
- Multiple button presses to retry
- Returning to menu
- Losing all progress
```

---

## ONBOARDING PATTERNS

### Progressive Disclosure
```
Level 1: Just movement
Level 2: Introduce jump
Level 3: Introduce enemy
Level 4: Combine mechanics

NOT:
"Here are 8 controls. Good luck!"
```

### Teaching Through Design
```
FIRST ENEMY:
- Place where player can't miss it
- Make it slow/simple
- Safe space to learn

FIRST JUMP:
- Gap that's obviously a gap
- Short distance, low stakes
- Visual cue (platform edge)
```

### Control Prompts
```
SHOW prompt when:
- First encounter of mechanic
- After long gap without using
- Player seems stuck

HIDE prompt after:
- Player uses it successfully
- Enough time has passed
- Player disables in settings
```

---

## MENU UX DETAILS

### Button Design
| State | Visual Change |
|-------|---------------|
| Normal | Base appearance |
| Hover | Brighter, slightly larger |
| Pressed | Darker, slightly smaller |
| Disabled | Grayed out, no hover effect |

### Navigation
- Arrow keys / WASD to navigate
- Enter / Space to select
- Escape to go back
- Always show which option is selected

### Transitions
- Fade: Simple, safe (0.2-0.3s)
- Slide: Directional (0.3-0.4s)
- None: Fine for jam, saves time

---

## GAME OVER / WIN STATES

### Game Over Screen
```
SHOW:
- "Game Over" or equivalent
- Final score/progress
- "Retry" button (prominent)
- "Menu" button

DON'T:
- Force watching long animation
- Hide retry option
- Require multiple clicks
```

### Victory Screen
```
SHOW:
- "You Win!" celebration
- Final stats (time, score, etc.)
- "Play Again" button
- "Menu" button

CELEBRATE:
- Particles/confetti
- Triumphant sound
- Brief moment before input
```

---

## QUICK UX CHECKLIST

### Before Submission
- [ ] Can start game in < 3 clicks?
- [ ] Controls are shown/taught?
- [ ] Game can be paused?
- [ ] Volume can be adjusted?
- [ ] Can restart level easily?
- [ ] Game over has retry option?
- [ ] Win state is clear?
- [ ] Text is readable?

### Common Mistakes
| Mistake | Fix |
|---------|-----|
| Can't pause | Add pause menu |
| Stuck after death | Add retry button |
| No audio control | Add mute at minimum |
| Tiny text | Minimum 16px |
| Unclear objective | Add brief instruction |
| Soft locks | Always allow restart |

---

## MOBILE/TOUCH UX

### If Supporting Touch
- Touch targets: Minimum 44x44 pixels
- Important buttons: Not at screen edges
- Swipe: Clear visual indication
- Virtual joystick: Show on touch

### Touch Control Patterns
| Game Type | Control |
|-----------|---------|
| Platformer | Virtual buttons or tap-to-jump |
| Puzzle | Tap/drag pieces |
| Shooter | Joystick + auto-fire or tap |
| Runner | Tap to action |

---

## TIME-SAVING UX

### If Running Out of Time
```
ESSENTIAL (do these):
1. Play button that works
2. Game can restart
3. Controls shown once

SKIP IF NEEDED:
- Settings menu
- Multiple menu screens
- Animated transitions
- Credits
```

### Minimum Viable Menu
```
TITLE: Game Name
[PLAY] ← Big, centered, obvious

That's it. Add more later.
```

---

## TESTING CHECKLIST

### Watch Someone Play
- Where do they get stuck?
- What do they click first?
- Do they understand the goal?
- Do they know the controls?
- Are they frustrated or having fun?

### Common First-Player Issues
- Didn't see controls explanation
- Clicked wrong button to start
- Didn't know they could pause
- Couldn't figure out objective
- Got stuck with no way out

---

*Remember: If the player can't figure out how to play, they won't play.*
