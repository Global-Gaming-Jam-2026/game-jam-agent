#!/usr/bin/env python3
"""
Game Development MCP Server
Provides tools for Godot, Unity, and Phaser game development.
"""

import asyncio
import json
import os
import shutil
import subprocess
import sys
from pathlib import Path
from typing import Any

from mcp.server import Server
from mcp.server.stdio import stdio_server
from mcp.types import Tool, TextContent

# Get the project root (parent of mcp folder)
PROJECT_ROOT = Path(__file__).parent.parent.absolute()

server = Server("gamedev-server")


# =============================================================================
# UTILITY FUNCTIONS
# =============================================================================

def get_project_root() -> Path:
    """Get the project root directory."""
    return PROJECT_ROOT


def run_shell(cmd: str, cwd: str = None) -> dict:
    """Run a shell command and return result."""
    try:
        result = subprocess.run(
            cmd,
            shell=True,
            cwd=cwd or str(PROJECT_ROOT),
            capture_output=True,
            text=True,
            timeout=60
        )
        return {
            "success": result.returncode == 0,
            "stdout": result.stdout,
            "stderr": result.stderr,
            "returncode": result.returncode
        }
    except subprocess.TimeoutExpired:
        return {"success": False, "error": "Command timed out after 60s"}
    except Exception as e:
        return {"success": False, "error": str(e)}


# =============================================================================
# TOOL DEFINITIONS
# =============================================================================

@server.list_tools()
async def list_tools() -> list[Tool]:
    """List all available tools."""
    return [
        # Project Tools
        Tool(
            name="project_structure",
            description="Get the current project file/folder structure as a tree",
            inputSchema={"type": "object", "properties": {}, "required": []}
        ),
        Tool(
            name="project_todo",
            description="Read or update the TASKS.md file",
            inputSchema={
                "type": "object",
                "properties": {
                    "action": {"type": "string", "enum": ["read", "write", "append"]},
                    "content": {"type": "string", "description": "Content to write (for write/append)"}
                },
                "required": ["action"]
            }
        ),
        Tool(
            name="project_design",
            description="Read or update the GAME_DESIGN.md file",
            inputSchema={
                "type": "object",
                "properties": {
                    "action": {"type": "string", "enum": ["read", "write"]},
                    "content": {"type": "string", "description": "Content to write"}
                },
                "required": ["action"]
            }
        ),
        Tool(
            name="run_command",
            description="Execute a shell command in the project directory",
            inputSchema={
                "type": "object",
                "properties": {
                    "command": {"type": "string", "description": "The command to run"}
                },
                "required": ["command"]
            }
        ),

        # Godot Tools
        Tool(
            name="godot_create_project",
            description="Create a new Godot 4 project with starter template",
            inputSchema={
                "type": "object",
                "properties": {
                    "name": {"type": "string", "description": "Project name"}
                },
                "required": ["name"]
            }
        ),
        Tool(
            name="godot_create_script",
            description="Create a new GDScript file with boilerplate",
            inputSchema={
                "type": "object",
                "properties": {
                    "name": {"type": "string", "description": "Script name (without .gd)"},
                    "type": {"type": "string", "enum": ["node", "player", "enemy", "ui", "manager"], "description": "Script type for boilerplate"}
                },
                "required": ["name", "type"]
            }
        ),
        Tool(
            name="godot_run",
            description="Launch the Godot project for testing",
            inputSchema={"type": "object", "properties": {}, "required": []}
        ),

        # Unity Tools
        Tool(
            name="unity_create_project",
            description="Create a new Unity project",
            inputSchema={
                "type": "object",
                "properties": {
                    "name": {"type": "string", "description": "Project name"}
                },
                "required": ["name"]
            }
        ),
        Tool(
            name="unity_create_script",
            description="Create a new C# script with boilerplate",
            inputSchema={
                "type": "object",
                "properties": {
                    "name": {"type": "string", "description": "Script name (without .cs)"},
                    "type": {"type": "string", "enum": ["monobehaviour", "scriptableobject", "manager"], "description": "Script type"}
                },
                "required": ["name", "type"]
            }
        ),

        # Phaser Tools
        Tool(
            name="phaser_create_project",
            description="Create a new Phaser.js project with npm",
            inputSchema={
                "type": "object",
                "properties": {
                    "name": {"type": "string", "description": "Project name"}
                },
                "required": ["name"]
            }
        ),
        Tool(
            name="phaser_dev_server",
            description="Start or stop the Phaser development server",
            inputSchema={
                "type": "object",
                "properties": {
                    "action": {"type": "string", "enum": ["start", "stop", "status"]}
                },
                "required": ["action"]
            }
        ),
    ]


# =============================================================================
# TOOL IMPLEMENTATIONS
# =============================================================================

@server.call_tool()
async def call_tool(name: str, arguments: dict[str, Any]) -> list[TextContent]:
    """Handle tool calls."""

    result = None

    # ----- PROJECT TOOLS -----
    if name == "project_structure":
        result = get_project_structure()

    elif name == "project_todo":
        result = handle_todo(arguments)

    elif name == "project_design":
        result = handle_design(arguments)

    elif name == "run_command":
        result = run_shell(arguments["command"])

    # ----- GODOT TOOLS -----
    elif name == "godot_create_project":
        result = create_godot_project(arguments["name"])

    elif name == "godot_create_script":
        result = create_godot_script(arguments["name"], arguments["type"])

    elif name == "godot_run":
        result = run_godot()

    # ----- UNITY TOOLS -----
    elif name == "unity_create_project":
        result = create_unity_project(arguments["name"])

    elif name == "unity_create_script":
        result = create_unity_script(arguments["name"], arguments["type"])

    # ----- PHASER TOOLS -----
    elif name == "phaser_create_project":
        result = create_phaser_project(arguments["name"])

    elif name == "phaser_dev_server":
        result = handle_phaser_server(arguments["action"])

    else:
        result = {"error": f"Unknown tool: {name}"}

    return [TextContent(type="text", text=json.dumps(result, indent=2))]


# =============================================================================
# PROJECT TOOL IMPLEMENTATIONS
# =============================================================================

def get_project_structure() -> dict:
    """Get project file tree."""
    def tree(path: Path, prefix: str = "") -> list[str]:
        lines = []
        items = sorted(path.iterdir(), key=lambda x: (x.is_file(), x.name))

        # Skip certain directories
        skip = {'.git', 'node_modules', '__pycache__', '.godot', 'Library', 'Temp', 'Logs'}
        items = [i for i in items if i.name not in skip]

        for i, item in enumerate(items):
            is_last = i == len(items) - 1
            connector = "└── " if is_last else "├── "
            lines.append(f"{prefix}{connector}{item.name}")

            if item.is_dir():
                extension = "    " if is_last else "│   "
                lines.extend(tree(item, prefix + extension))

        return lines

    try:
        lines = [str(PROJECT_ROOT.name) + "/"]
        lines.extend(tree(PROJECT_ROOT))
        return {"success": True, "tree": "\n".join(lines)}
    except Exception as e:
        return {"success": False, "error": str(e)}


def handle_todo(args: dict) -> dict:
    """Handle TASKS.md operations."""
    todo_path = PROJECT_ROOT / "docs" / "TASKS.md"

    if args["action"] == "read":
        if todo_path.exists():
            return {"success": True, "content": todo_path.read_text()}
        return {"success": False, "error": "TASKS.md not found"}

    elif args["action"] == "write":
        todo_path.parent.mkdir(parents=True, exist_ok=True)
        todo_path.write_text(args.get("content", ""))
        return {"success": True, "message": "TASKS.md updated"}

    elif args["action"] == "append":
        todo_path.parent.mkdir(parents=True, exist_ok=True)
        existing = todo_path.read_text() if todo_path.exists() else ""
        todo_path.write_text(existing + "\n" + args.get("content", ""))
        return {"success": True, "message": "Appended to TASKS.md"}

    return {"success": False, "error": "Invalid action"}


def handle_design(args: dict) -> dict:
    """Handle GAME_DESIGN.md operations."""
    design_path = PROJECT_ROOT / "docs" / "GAME_DESIGN.md"

    if args["action"] == "read":
        if design_path.exists():
            return {"success": True, "content": design_path.read_text()}
        return {"success": False, "error": "GAME_DESIGN.md not found"}

    elif args["action"] == "write":
        design_path.parent.mkdir(parents=True, exist_ok=True)
        design_path.write_text(args.get("content", ""))
        return {"success": True, "message": "GAME_DESIGN.md updated"}

    return {"success": False, "error": "Invalid action"}


# =============================================================================
# GODOT TOOL IMPLEMENTATIONS
# =============================================================================

def create_godot_project(name: str) -> dict:
    """Create a Godot 4 project."""
    src_dir = PROJECT_ROOT / "src"
    src_dir.mkdir(exist_ok=True)

    # Create project.godot
    project_file = src_dir / "project.godot"
    project_content = f'''[gd_resource type="Resource" script_class="ProjectSettings" load_steps=1 format=3]

config_version=5

[application]
config/name="{name}"
run/main_scene="res://scenes/main.tscn"
config/features=PackedStringArray("4.2", "Forward Plus")

[display]
window/size/viewport_width=1280
window/size/viewport_height=720

[input]
move_left={{
"deadzone": 0.5,
"events": [Object(InputEventKey,"resource_local_to_scene":false,"resource_name":"","device":-1,"window_id":0,"alt_pressed":false,"shift_pressed":false,"ctrl_pressed":false,"meta_pressed":false,"pressed":false,"keycode":65,"physical_keycode":0,"key_label":0,"unicode":97,"echo":false,"script":null), Object(InputEventKey,"resource_local_to_scene":false,"resource_name":"","device":-1,"window_id":0,"alt_pressed":false,"shift_pressed":false,"ctrl_pressed":false,"meta_pressed":false,"pressed":false,"keycode":4194319,"physical_keycode":0,"key_label":0,"unicode":0,"echo":false,"script":null)]
}}
move_right={{
"deadzone": 0.5,
"events": [Object(InputEventKey,"resource_local_to_scene":false,"resource_name":"","device":-1,"window_id":0,"alt_pressed":false,"shift_pressed":false,"ctrl_pressed":false,"meta_pressed":false,"pressed":false,"keycode":68,"physical_keycode":0,"key_label":0,"unicode":100,"echo":false,"script":null), Object(InputEventKey,"resource_local_to_scene":false,"resource_name":"","device":-1,"window_id":0,"alt_pressed":false,"shift_pressed":false,"ctrl_pressed":false,"meta_pressed":false,"pressed":false,"keycode":4194321,"physical_keycode":0,"key_label":0,"unicode":0,"echo":false,"script":null)]
}}
move_up={{
"deadzone": 0.5,
"events": [Object(InputEventKey,"resource_local_to_scene":false,"resource_name":"","device":-1,"window_id":0,"alt_pressed":false,"shift_pressed":false,"ctrl_pressed":false,"meta_pressed":false,"pressed":false,"keycode":87,"physical_keycode":0,"key_label":0,"unicode":119,"echo":false,"script":null), Object(InputEventKey,"resource_local_to_scene":false,"resource_name":"","device":-1,"window_id":0,"alt_pressed":false,"shift_pressed":false,"ctrl_pressed":false,"meta_pressed":false,"pressed":false,"keycode":4194320,"physical_keycode":0,"key_label":0,"unicode":0,"echo":false,"script":null)]
}}
move_down={{
"deadzone": 0.5,
"events": [Object(InputEventKey,"resource_local_to_scene":false,"resource_name":"","device":-1,"window_id":0,"alt_pressed":false,"shift_pressed":false,"ctrl_pressed":false,"meta_pressed":false,"pressed":false,"keycode":83,"physical_keycode":0,"key_label":0,"unicode":115,"echo":false,"script":null), Object(InputEventKey,"resource_local_to_scene":false,"resource_name":"","device":-1,"window_id":0,"alt_pressed":false,"shift_pressed":false,"ctrl_pressed":false,"meta_pressed":false,"pressed":false,"keycode":4194322,"physical_keycode":0,"key_label":0,"unicode":0,"echo":false,"script":null)]
}}
jump={{
"deadzone": 0.5,
"events": [Object(InputEventKey,"resource_local_to_scene":false,"resource_name":"","device":-1,"window_id":0,"alt_pressed":false,"shift_pressed":false,"ctrl_pressed":false,"meta_pressed":false,"pressed":false,"keycode":32,"physical_keycode":0,"key_label":0,"unicode":32,"echo":false,"script":null)]
}}
'''
    project_file.write_text(project_content)

    # Create directories
    (src_dir / "scenes").mkdir(exist_ok=True)
    (src_dir / "scripts").mkdir(exist_ok=True)
    (src_dir / "assets").mkdir(exist_ok=True)

    # Create main scene
    main_scene = src_dir / "scenes" / "main.tscn"
    main_scene.write_text('''[gd_scene load_steps=1 format=3]

[node name="Main" type="Node2D"]
''')

    # Create game manager script
    game_manager = src_dir / "scripts" / "game_manager.gd"
    game_manager.write_text('''extends Node

## Game Manager - Central game state and logic

var score: int = 0
var is_paused: bool = false

func _ready() -> void:
    print("Game started!")

func add_score(amount: int) -> void:
    score += amount
    print("Score: ", score)

func pause_game() -> void:
    is_paused = true
    get_tree().paused = true

func resume_game() -> void:
    is_paused = false
    get_tree().paused = false
''')

    return {
        "success": True,
        "message": f"Created Godot project '{name}'",
        "files_created": [
            "src/project.godot",
            "src/scenes/main.tscn",
            "src/scripts/game_manager.gd"
        ]
    }


def create_godot_script(name: str, script_type: str) -> dict:
    """Create a GDScript file with boilerplate."""
    scripts_dir = PROJECT_ROOT / "src" / "scripts"
    scripts_dir.mkdir(parents=True, exist_ok=True)

    script_path = scripts_dir / f"{name}.gd"

    templates = {
        "node": '''extends Node

func _ready() -> void:
    pass

func _process(delta: float) -> void:
    pass
''',
        "player": '''extends CharacterBody2D

@export var speed: float = 200.0
@export var jump_force: float = 400.0

var gravity: float = ProjectSettings.get_setting("physics/2d/default_gravity")

func _physics_process(delta: float) -> void:
    # Gravity
    if not is_on_floor():
        velocity.y += gravity * delta

    # Jump
    if Input.is_action_just_pressed("jump") and is_on_floor():
        velocity.y = -jump_force

    # Movement
    var direction := Input.get_axis("move_left", "move_right")
    velocity.x = direction * speed

    move_and_slide()
''',
        "enemy": '''extends CharacterBody2D

@export var speed: float = 100.0
@export var health: int = 3

signal died

func _physics_process(delta: float) -> void:
    # Basic patrol or chase logic here
    move_and_slide()

func take_damage(amount: int) -> void:
    health -= amount
    if health <= 0:
        die()

func die() -> void:
    died.emit()
    queue_free()
''',
        "ui": '''extends Control

func _ready() -> void:
    pass

func update_display() -> void:
    pass
''',
        "manager": '''extends Node

## Manager for handling game subsystem

func _ready() -> void:
    pass

func initialize() -> void:
    pass

func cleanup() -> void:
    pass
'''
    }

    content = templates.get(script_type, templates["node"])
    script_path.write_text(content)

    return {
        "success": True,
        "message": f"Created {script_type} script: {name}.gd",
        "path": str(script_path)
    }


def run_godot() -> dict:
    """Run the Godot project."""
    # Try to find Godot executable
    godot_paths = [
        "godot",
        "godot4",
        r"C:\Program Files\Godot\Godot.exe",
        r"C:\Godot\Godot_v4.2-stable_win64.exe",
        "/Applications/Godot.app/Contents/MacOS/Godot"
    ]

    project_file = PROJECT_ROOT / "src" / "project.godot"
    if not project_file.exists():
        return {"success": False, "error": "No Godot project found in src/"}

    for godot_path in godot_paths:
        result = run_shell(f'"{godot_path}" --path "{PROJECT_ROOT / "src"}"')
        if result.get("success") or "not found" not in result.get("stderr", "").lower():
            return {"success": True, "message": "Godot launched", "output": result}

    return {
        "success": False,
        "error": "Godot not found. Install Godot 4 and add to PATH, or open project manually."
    }


# =============================================================================
# UNITY TOOL IMPLEMENTATIONS
# =============================================================================

def create_unity_project(name: str) -> dict:
    """Create a Unity project structure (manual Unity opening required)."""
    src_dir = PROJECT_ROOT / "src"
    src_dir.mkdir(exist_ok=True)

    # Create Unity-style folder structure
    (src_dir / "Assets" / "Scripts").mkdir(parents=True, exist_ok=True)
    (src_dir / "Assets" / "Scenes").mkdir(parents=True, exist_ok=True)
    (src_dir / "Assets" / "Prefabs").mkdir(parents=True, exist_ok=True)
    (src_dir / "Assets" / "Materials").mkdir(parents=True, exist_ok=True)

    # Create a basic GameManager script
    game_manager = src_dir / "Assets" / "Scripts" / "GameManager.cs"
    game_manager.write_text(f'''using UnityEngine;

public class GameManager : MonoBehaviour
{{
    public static GameManager Instance {{ get; private set; }}

    public int Score {{ get; private set; }}
    public bool IsPaused {{ get; private set; }}

    void Awake()
    {{
        if (Instance == null)
        {{
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }}
        else
        {{
            Destroy(gameObject);
        }}
    }}

    void Start()
    {{
        Debug.Log("{name} started!");
    }}

    public void AddScore(int amount)
    {{
        Score += amount;
        Debug.Log($"Score: {{Score}}");
    }}

    public void TogglePause()
    {{
        IsPaused = !IsPaused;
        Time.timeScale = IsPaused ? 0f : 1f;
    }}
}}
''')

    # Create PlayerController
    player_controller = src_dir / "Assets" / "Scripts" / "PlayerController.cs"
    player_controller.write_text('''using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;

    private Rigidbody2D rb;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Horizontal movement
        float horizontal = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(horizontal * moveSpeed, rb.velocity.y);

        // Jump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}
''')

    return {
        "success": True,
        "message": f"Created Unity project structure for '{name}'",
        "note": "Open Unity Hub and create new project at src/ folder, or open existing project there",
        "files_created": [
            "src/Assets/Scripts/GameManager.cs",
            "src/Assets/Scripts/PlayerController.cs"
        ]
    }


def create_unity_script(name: str, script_type: str) -> dict:
    """Create a C# script for Unity."""
    scripts_dir = PROJECT_ROOT / "src" / "Assets" / "Scripts"
    scripts_dir.mkdir(parents=True, exist_ok=True)

    script_path = scripts_dir / f"{name}.cs"

    templates = {
        "monobehaviour": f'''using UnityEngine;

public class {name} : MonoBehaviour
{{
    void Start()
    {{

    }}

    void Update()
    {{

    }}
}}
''',
        "scriptableobject": f'''using UnityEngine;

[CreateAssetMenu(fileName = "{name}", menuName = "Game/{name}")]
public class {name} : ScriptableObject
{{
    public string displayName;
    public int value;
}}
''',
        "manager": f'''using UnityEngine;

public class {name} : MonoBehaviour
{{
    public static {name} Instance {{ get; private set; }}

    void Awake()
    {{
        if (Instance == null)
        {{
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }}
        else
        {{
            Destroy(gameObject);
        }}
    }}

    void Start()
    {{
        Initialize();
    }}

    void Initialize()
    {{

    }}
}}
'''
    }

    content = templates.get(script_type, templates["monobehaviour"])
    script_path.write_text(content)

    return {
        "success": True,
        "message": f"Created {script_type} script: {name}.cs",
        "path": str(script_path)
    }


# =============================================================================
# PHASER TOOL IMPLEMENTATIONS
# =============================================================================

def create_phaser_project(name: str) -> dict:
    """Create a Phaser.js project."""
    src_dir = PROJECT_ROOT / "src"
    src_dir.mkdir(exist_ok=True)

    # Create package.json
    package_json = src_dir / "package.json"
    package_json.write_text(json.dumps({
        "name": name.lower().replace(" ", "-"),
        "version": "1.0.0",
        "description": f"{name} - Game Jam Project",
        "scripts": {
            "dev": "npx vite",
            "build": "npx vite build",
            "preview": "npx vite preview"
        },
        "dependencies": {
            "phaser": "^3.70.0"
        },
        "devDependencies": {
            "vite": "^5.0.0"
        }
    }, indent=2))

    # Create index.html
    index_html = src_dir / "index.html"
    index_html.write_text(f'''<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>{name}</title>
    <style>
        body {{
            margin: 0;
            padding: 0;
            display: flex;
            justify-content: center;
            align-items: center;
            min-height: 100vh;
            background: #1a1a2e;
        }}
    </style>
</head>
<body>
    <script type="module" src="/main.js"></script>
</body>
</html>
''')

    # Create main.js
    main_js = src_dir / "main.js"
    main_js.write_text('''import Phaser from 'phaser';
import { GameScene } from './scenes/GameScene.js';

const config = {
    type: Phaser.AUTO,
    width: 800,
    height: 600,
    backgroundColor: '#2d2d2d',
    physics: {
        default: 'arcade',
        arcade: {
            gravity: { y: 300 },
            debug: false
        }
    },
    scene: [GameScene]
};

const game = new Phaser.Game(config);
''')

    # Create scenes folder and GameScene
    scenes_dir = src_dir / "scenes"
    scenes_dir.mkdir(exist_ok=True)

    game_scene = scenes_dir / "GameScene.js"
    game_scene.write_text('''import Phaser from 'phaser';

export class GameScene extends Phaser.Scene {
    constructor() {
        super({ key: 'GameScene' });
        this.score = 0;
    }

    preload() {
        // Load assets here
        // this.load.image('player', 'assets/player.png');
    }

    create() {
        // Create game objects
        this.add.text(400, 300, 'Game Jam!', {
            fontSize: '48px',
            fill: '#fff'
        }).setOrigin(0.5);

        // Create player (placeholder rectangle)
        this.player = this.add.rectangle(400, 500, 32, 32, 0x00ff00);
        this.physics.add.existing(this.player);
        this.player.body.setCollideWorldBounds(true);

        // Setup controls
        this.cursors = this.input.keyboard.createCursorKeys();

        // Score text
        this.scoreText = this.add.text(16, 16, 'Score: 0', {
            fontSize: '24px',
            fill: '#fff'
        });
    }

    update() {
        const speed = 200;

        // Horizontal movement
        if (this.cursors.left.isDown) {
            this.player.body.setVelocityX(-speed);
        } else if (this.cursors.right.isDown) {
            this.player.body.setVelocityX(speed);
        } else {
            this.player.body.setVelocityX(0);
        }

        // Jump
        if (this.cursors.up.isDown && this.player.body.touching.down) {
            this.player.body.setVelocityY(-330);
        }
    }

    addScore(amount) {
        this.score += amount;
        this.scoreText.setText('Score: ' + this.score);
    }
}
''')

    # Create assets folder
    (src_dir / "assets").mkdir(exist_ok=True)

    return {
        "success": True,
        "message": f"Created Phaser project '{name}'",
        "next_steps": [
            "cd src && npm install",
            "npm run dev",
            "Open http://localhost:5173 in browser"
        ],
        "files_created": [
            "src/package.json",
            "src/index.html",
            "src/main.js",
            "src/scenes/GameScene.js"
        ]
    }


def handle_phaser_server(action: str) -> dict:
    """Handle Phaser dev server."""
    src_dir = PROJECT_ROOT / "src"

    if action == "start":
        # Check if package.json exists
        if not (src_dir / "package.json").exists():
            return {"success": False, "error": "No Phaser project found. Run phaser_create_project first."}

        # Check if node_modules exists
        if not (src_dir / "node_modules").exists():
            return {
                "success": False,
                "error": "Dependencies not installed. Run: cd src && npm install"
            }

        # Start dev server in background
        result = run_shell("npm run dev", cwd=str(src_dir))
        return {
            "success": True,
            "message": "Dev server starting...",
            "url": "http://localhost:5173",
            "output": result
        }

    elif action == "status":
        return {"success": True, "message": "Check terminal for server status"}

    elif action == "stop":
        return {"success": True, "message": "Stop the server with Ctrl+C in terminal"}

    return {"success": False, "error": "Invalid action"}


# =============================================================================
# MAIN
# =============================================================================

async def main():
    """Run the MCP server."""
    async with stdio_server() as (read_stream, write_stream):
        await server.run(
            read_stream,
            write_stream,
            server.create_initialization_options()
        )


if __name__ == "__main__":
    asyncio.run(main())
