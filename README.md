# Semi-Autonomous Game Jam Agent

A collaborative game development agent for Claude Code. You guide the vision, Claude builds the game.

---

## Quick Start

```bash
# 1. Extract this folder anywhere
# 2. Install MCP dependencies
cd GameJamAgent/mcp
pip install -r requirements.txt

# 3. Start Claude Code in the folder
cd ..
claude

# 4. Tell it to make a game
> make a game about space cats
```

Claude will:
- Present 3 game concepts → **Wait for your pick**
- Recommend engine → **Wait for confirmation**
- Build core mechanic → **Wait for feedback**
- Continue iterating → **With your guidance**

---

## How Collaboration Works

```
┌─────────────────────────────────────────────────────────────┐
│  You: "make a game about wizards"                           │
└─────────────────────────────────────────────────────────────┘
                           ↓
┌─────────────────────────────────────────────────────────────┐
│  Claude PROPOSES:                                           │
│  "Here are 3 concepts: A) Tower defense B) Puzzle C) RPG"   │
│  "I recommend A because... Which do you prefer?"            │
└─────────────────────────────────────────────────────────────┘
                           ↓
┌─────────────────────────────────────────────────────────────┐
│  You DECIDE: "Let's do the puzzle one"                      │
└─────────────────────────────────────────────────────────────┘
                           ↓
┌─────────────────────────────────────────────────────────────┐
│  Claude EXECUTES:                                           │
│  - Creates project structure                                │
│  - Implements core mechanics                                │
│  - Reports what was built + how to test                     │
└─────────────────────────────────────────────────────────────┘
                           ↓
┌─────────────────────────────────────────────────────────────┐
│  Claude VERIFIES: "Core mechanic done. Try it. What next?"  │
└─────────────────────────────────────────────────────────────┘
```

---

## Verification Checkpoints

Claude stops and asks at these points:

| Phase | What Claude Presents | You Decide |
|-------|---------------------|------------|
| **Brainstorm** | 3 game ideas with pros/cons | Pick one |
| **Engine** | Recommended engine + reasoning | Confirm |
| **Setup** | Project structure | Go ahead |
| **Core Mechanic** | Working demo + what's next | Feedback |
| **Each Feature** | What was built, how to test | Review |
| **Art Direction** | Style guide, colors | Approve |
| **Polish** | Priority list | Selection |
| **Ship** | Final checklist, known issues | Green light |

---

## What's Included

```
GameJamAgent/
├── CLAUDE.md              ← Agent brain (semi-autonomous behavior)
├── README.md              ← You are here
│
├── .claude/
│   ├── mcp.json           ← MCP server config
│   ├── hooks/             ← Automation scripts
│   └── commands/          ← Slash commands
│
├── mcp/
│   ├── gamedev_server.py  ← MCP server with real tools
│   └── requirements.txt   ← Python dependencies
│
├── skills/                ← Game dev knowledge (25 skills)
│   ├── game-engines/      ← Godot, Unity, Phaser patterns
│   ├── gameplay-mechanics/← Combat, movement, progression
│   ├── art-direction/     ← Color, style, assets
│   ├── audio-direction/   ← Music, SFX, mood
│   ├── game-feel/         ← Juice, feedback, polish
│   ├── ux-design/         ← Menus, accessibility
│   └── ... (19 more)
│
├── docs/                  ← Design doc & task board
├── src/                   ← Game code goes here
├── assets/                ← Art, audio, fonts
└── builds/                ← Exported builds
```

---

## MCP Tools Available

Claude has REAL tools, not just text knowledge:

| Tool | What it does |
|------|--------------|
| `godot_create_project` | Creates full Godot 4 project |
| `godot_create_script` | Creates GDScript with boilerplate |
| `godot_run` | Launches the game |
| `unity_create_project` | Creates Unity folder structure |
| `unity_create_script` | Creates C# scripts |
| `phaser_create_project` | Creates Phaser.js with npm |
| `phaser_dev_server` | Starts dev server |
| `project_structure` | Shows file tree |
| `project_todo` | Manages TASKS.md |
| `project_design` | Manages GAME_DESIGN.md |
| `run_command` | Runs any shell command |

---

## Slash Commands

| Command | What it does |
|---------|--------------|
| `/jam-start <theme>` | Brainstorm → Pick idea → Create project |
| `/jam-feature <feature>` | Implement a feature end-to-end |
| `/jam-debug <problem>` | Find and fix bugs |
| `/jam-playtest <notes>` | Analyze feedback, prioritize fixes |
| `/jam-ship <platform>` | Build and prepare submission |

---

## Uncertainty Handling

Claude uses smart judgment:

| Situation | Claude's Action |
|-----------|-----------------|
| **Obvious** (naming, formatting) | Does it, reports briefly |
| **Uncertain but safe** (approach choice) | Tries it, reports what happened |
| **Uncertain and risky** (architecture) | Detailed report → Asks first |
| **Could break code** (deleting, refactoring) | Never attempts, asks with full context |

---

## Team Setup

### Requirements
- Python 3.8+ (for MCP server)
- Claude Code (`npm install -g @anthropic-ai/claude-code`)
- Node.js (for Phaser projects)
- Godot 4 (optional, for Godot projects)
- Unity (optional, for Unity projects)

### Each team member:
1. Get this folder (zip or git clone)
2. Run `pip install -r mcp/requirements.txt`
3. Run `claude` from inside the folder
4. Start building

---

## Engine Recommendations

| Situation | Recommended Engine |
|-----------|-------------------|
| 2D game, quick iteration | Godot |
| 3D game, team knows C# | Unity |
| Web-only, instant sharing | Phaser |
| CPU-only machine | Godot or Phaser |
| GPU available | Any |

---

## Troubleshooting

### MCP server not connecting
```bash
# Check Python and mcp package
python --version  # Should be 3.8+
pip show mcp      # Should show version

# Reinstall if needed
pip install --upgrade mcp
```

### Godot not found
- Install Godot 4 from https://godotengine.org
- Add to PATH, or open project.godot manually

### Unity not found
- Open Unity Hub and create project at src/ folder
- Scripts are already created, just open in Unity

### Phaser not starting
```bash
cd src
npm install
npm run dev
# Open http://localhost:5173
```

---

## Tips for Jam Success

1. **Provide clear direction** - Claude works best with your vision
2. **Test often** - When Claude says "try it", actually test it
3. **Give feedback** - "Add more juice" or "too hard" guides iteration
4. **Cut scope early** - Small finished > ambitious broken
5. **Check TASKS.md** - Progress is tracked automatically
6. **Use the skills** - Ask about art direction, game feel, etc.

---

## Creative Guidance Available

Ask Claude about:

| Topic | Skill Reference |
|-------|-----------------|
| Color palettes, art style | `art-direction/SKILL.md` |
| Music mood, SFX lists | `audio-direction/SKILL.md` |
| Screen shake, particles | `game-feel/SKILL.md` |
| Menus, accessibility | `ux-design/SKILL.md` |
| Engine patterns | `game-engines/SKILL.md` |
| Mechanics design | `gameplay-mechanics/SKILL.md` |

---

## License

Do whatever you want with this. Go win your jam.

---

**COLLABORATE. BUILD. SHIP. WIN.**
