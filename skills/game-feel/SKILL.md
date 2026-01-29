# GAME FEEL & JUICE FOR GAME JAMS

Expert guidance for making games feel satisfying and responsive.

---

## WHAT IS GAME FEEL?

Game feel (or "juice") = the tactile sensation of interacting with a game.

**Good game feel:**
- Inputs feel responsive
- Actions have weight and impact
- Feedback is immediate and clear
- Playing is satisfying even without goals

---

## THE JUICE HIERARCHY

| Priority | Effect | Impact |
|----------|--------|--------|
| 1 | Responsive controls | Foundation of feel |
| 2 | Visual feedback | Confirms actions |
| 3 | Audio feedback | Reinforces actions |
| 4 | Screen effects | Emphasizes impact |
| 5 | Particles | Polish layer |

**Rule:** Don't add juice to broken controls. Fix controls first.

---

## RESPONSIVE CONTROLS

### Input Response Times
| Quality | Response Time |
|---------|---------------|
| Snappy | 0-1 frames (0-16ms) |
| Good | 2-3 frames (17-50ms) |
| Sluggish | 4+ frames (67ms+) |

### Common Responsiveness Issues
```
❌ BAD: Wait for animation before action
✅ GOOD: Action happens instantly, animation follows

❌ BAD: Movement starts after button fully pressed
✅ GOOD: Movement starts on button down

❌ BAD: Fixed movement speed
✅ GOOD: Acceleration/deceleration curves
```

### Movement Feel
```
SNAPPY (platformer, action):
- High acceleration: 0.9-1.0
- Quick deceleration: 0.8-0.95
- Low air control: 0.3-0.5

FLOATY (exploration, puzzle):
- Low acceleration: 0.1-0.3
- Gradual deceleration: 0.95-0.99
- High air control: 0.7-0.9

WEIGHTY (horror, simulation):
- Medium acceleration: 0.3-0.5
- Long deceleration: 0.9-0.98
- Momentum-based turning
```

---

## VISUAL FEEDBACK

### Squash and Stretch
```
ON LAND:
- Squash: Scale to (1.3, 0.7) for 0.1s
- Return: Scale to (1.0, 1.0) over 0.1s

ON JUMP:
- Stretch: Scale to (0.7, 1.3) for 0.1s
- Return in air: Scale to (1.0, 1.0) over 0.2s

ON HIT:
- Squash toward impact direction
- Intensity based on damage
```

### Hit Feedback (Hitstop)
```
ON DAMAGE:
1. Freeze game for 2-5 frames (33-83ms)
2. Flash white for 1-2 frames
3. Knockback in direction of hit
4. Brief invincibility (flash/blink)
```

### Flash/Blink Effects
| Effect | Use Case | Duration |
|--------|----------|----------|
| White flash | Damage taken | 1-2 frames |
| Red tint | Low health | 0.5s pulse |
| Blink (disappear) | Invincibility | 0.1s on/off |
| Outline glow | Selection/highlight | Constant |

---

## SCREEN EFFECTS

### Screen Shake
```
LIGHT (small impacts):
- Magnitude: 2-5 pixels
- Duration: 0.1-0.15s
- Use: Landing, small hits

MEDIUM (significant events):
- Magnitude: 5-10 pixels
- Duration: 0.15-0.25s
- Use: Explosions, big hits

HEAVY (major events):
- Magnitude: 10-20 pixels
- Duration: 0.25-0.4s
- Use: Boss attacks, death
```

### Shake Implementation
```
EACH FRAME during shake:
  offset_x = random(-magnitude, magnitude)
  offset_y = random(-magnitude, magnitude)
  camera.position += (offset_x, offset_y)
  magnitude *= decay (0.9 typically)

END when magnitude < 0.5
```

### Hitstop (Frame Freeze)
```
ON MAJOR HIT:
1. Set timescale to 0 (or 0.1 for partial)
2. Wait 2-8 frames (33-133ms)
3. Resume normal speed
4. Often combined with screen shake
```

### Camera Effects
| Effect | When | How |
|--------|------|-----|
| Zoom punch | Big impact | Zoom in 5-10%, snap back |
| Slow zoom | Dramatic moment | Zoom in slowly over 0.5-1s |
| Lerp follow | Normal gameplay | Smooth camera follow |
| Snap follow | Fast action | Instant camera follow |

---

## PARTICLES

### Essential Particle Types
| Particle | Use Case |
|----------|----------|
| Dust/puff | Landing, running, stopping |
| Sparks | Hits, collisions, metal |
| Debris | Breaking objects |
| Trail | Fast movement, projectiles |
| Burst | Explosions, power-ups |
| Confetti | Victory, achievements |

### Particle Settings by Type
```
DUST PUFF (landing):
- Count: 5-15
- Lifetime: 0.2-0.5s
- Velocity: Outward and up
- Fade out: Yes
- Size: Small, varying

EXPLOSION:
- Count: 20-50
- Lifetime: 0.3-0.8s
- Velocity: All directions
- Colors: Orange → Red → Gray
- Size: Large → Small

TRAIL:
- Count: Continuous (1-3 per frame)
- Lifetime: 0.1-0.3s
- Velocity: Inherit from parent
- Fade: Yes
- Size: Shrink over time

COLLECT SPARKLE:
- Count: 10-20
- Lifetime: 0.3-0.6s
- Velocity: Upward + outward
- Colors: Yellow/white
- Size: Small, twinkling
```

---

## AUDIO FEEDBACK TIMING

### Sound Sync Points
```
ACTION → SOUND (immediate)
- Jump: Sound on button press
- Shoot: Sound on fire
- Hit: Sound on collision

ANIMATION → SOUND (synced)
- Footsteps: Sound on foot contact frames
- Swing: Sound on anticipation + hit
- Land: Sound on ground contact
```

### Sound Variations
- 2-4 variations per repeated sound
- Randomize pitch slightly (±5-10%)
- Randomize volume slightly (±10%)
- Prevents "machine gun" effect

---

## JUICE RECIPES

### Satisfying Jump
```
ON JUMP:
1. Squash player (0.7, 1.3)
2. Play jump sound
3. Spawn dust particles at feet
4. Snap to jump animation

ON LAND:
1. Squash player (1.3, 0.7)
2. Play land sound
3. Light screen shake (optional)
4. Spawn dust particles
5. Camera micro-dip (optional)
```

### Impactful Hit
```
ON HIT:
1. Hitstop (3-5 frames)
2. Screen shake (medium)
3. Flash target white
4. Spawn hit particles
5. Play hit sound
6. Knockback target
7. Number popup (damage)
```

### Collectible Pickup
```
ON COLLECT:
1. Play pickup sound
2. Spawn sparkle particles
3. Item scales up then disappears
4. UI counter pulses
5. Optional: Brief slow-mo
```

### Death/Explosion
```
ON DEATH:
1. Freeze frame (5-10 frames)
2. Heavy screen shake
3. Large explosion particles
4. Play explosion sound
5. Camera zoom punch (optional)
6. Screen flash white
7. Object disappears/shatters
```

### UI Button Press
```
ON HOVER:
1. Scale up 1.05-1.1
2. Color shift (brighter)
3. Play hover sound

ON PRESS:
1. Scale down 0.95
2. Color shift (darker)
3. Play click sound

ON RELEASE:
1. Scale back to 1.0
2. Trigger action
```

---

## TWEENING/EASING

### Common Easing Functions
| Ease | Feel | Use For |
|------|------|---------|
| Linear | Mechanical | Progress bars, timers |
| Ease Out | Satisfying | UI animations, landing |
| Ease In | Anticipation | Charging, wind-up |
| Ease In-Out | Smooth | Camera movement |
| Bounce | Playful | Popups, notifications |
| Elastic | Springy | UI elements, selections |
| Back | Overshoot | Dramatic entrances |

### Duration Guide
| Animation | Duration |
|-----------|----------|
| UI popup | 0.2-0.3s |
| Character land | 0.1-0.15s |
| Menu transition | 0.3-0.5s |
| Damage flash | 0.05-0.1s |
| Notification | 0.15-0.25s |

---

## QUICK JUICE CHECKLIST

### Minimum Viable Juice
- [ ] Responsive controls (no input lag)
- [ ] Jump has squash/stretch
- [ ] Landing has feedback (visual + audio)
- [ ] Hits have feedback (flash + sound)
- [ ] UI buttons react to hover/click

### Good Juice
- [ ] All above plus:
- [ ] Screen shake on impacts
- [ ] Particles on actions
- [ ] Hitstop on big hits
- [ ] Camera follows smoothly

### Great Juice
- [ ] All above plus:
- [ ] Anticipation frames on actions
- [ ] Particle trails on movement
- [ ] Camera punch on impacts
- [ ] UI animations on changes
- [ ] Sound variations

---

## COMMON MISTAKES

| Mistake | Fix |
|---------|-----|
| Too much shake | Less magnitude, shorter duration |
| Shake on everything | Reserve for impacts only |
| Particles everywhere | Subtle, purposeful particles |
| Slow-mo overuse | Brief and rare |
| Input lag for animation | Action first, animate after |
| No feedback at all | Add sound + visual minimum |

### The Juice Balance
```
TOO LITTLE: Game feels flat, unresponsive
JUST RIGHT: Game feels satisfying, clear
TOO MUCH: Game feels chaotic, exhausting
```

---

## IMPLEMENTATION PRIORITY

**For game jams, add juice in this order:**

1. **Controls** - Make them feel good FIRST
2. **Core action** - Juice the thing you do most
3. **Feedback sounds** - Quick win, big impact
4. **Hit/damage** - Critical for action games
5. **UI reactions** - Polish perception
6. **Particles** - Last, only if time

---

## JUICE WITHOUT CODE

Some juice is just animation:
- Idle breathing/bobbing
- Anticipation before action
- Follow-through after action
- Secondary motion (hair, clothes)
- Eye blinks and micro-expressions

---

*Remember: Juice is seasoning. The meal (gameplay) must be good first.*
