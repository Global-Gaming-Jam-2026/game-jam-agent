using UnityEngine;

/// <summary>
/// Applies runtime-generated sprites to arena elements.
/// Attach to any arena object (floor, walls, background) to get proper visuals.
/// </summary>
public class ArenaVisualSetup : MonoBehaviour
{
    public enum ArenaElementType
    {
        Floor,
        Wall,
        Background
    }

    [SerializeField] private ArenaElementType elementType = ArenaElementType.Floor;
    [SerializeField] private bool applyOnStart = true;

    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        if (applyOnStart)
        {
            ApplyVisuals();
        }
    }

    public void ApplyVisuals()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        }

        string spriteName = elementType switch
        {
            ArenaElementType.Floor => "Floor",
            ArenaElementType.Wall => "Wall",
            ArenaElementType.Background => "Background",
            _ => "Floor"
        };

        var sprite = RuntimeAssetLoader.GetArenaSprite(spriteName);
        if (sprite != null)
        {
            spriteRenderer.sprite = sprite;
            spriteRenderer.drawMode = SpriteDrawMode.Tiled;

            // Set sorting order based on type
            spriteRenderer.sortingOrder = elementType switch
            {
                ArenaElementType.Background => -10,
                ArenaElementType.Floor => -5,
                ArenaElementType.Wall => -3,
                _ => 0
            };

            Debug.Log($"[ArenaVisualSetup] Applied {spriteName} sprite to {gameObject.name}");
        }
        else
        {
            Debug.LogWarning($"[ArenaVisualSetup] Could not find {spriteName} sprite");
        }
    }
}

/// <summary>
/// Auto-setup for arena visuals at scene start.
/// Add this to the scene to ensure arena elements get their visuals.
/// </summary>
public class ArenaAutoSetup : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void AutoSetupArena()
    {
        // Find arena floor by name
        var floor = GameObject.Find("ArenaFloor");
        if (floor != null)
        {
            var sr = floor.GetComponent<SpriteRenderer>();
            if (sr != null && sr.sprite == null)
            {
                var sprite = RuntimeAssetLoader.GetArenaSprite("Floor");
                if (sprite != null)
                {
                    sr.sprite = sprite;
                    sr.drawMode = SpriteDrawMode.Tiled;
                    sr.sortingOrder = -5;
                    Debug.Log("[ArenaAutoSetup] Applied Floor sprite to ArenaFloor");
                }
            }
        }

        // Auto-apply to any SpriteRenderer that doesn't have a sprite
        AutoApplyMissingSprites();
    }

    private static void AutoApplyMissingSprites()
    {
        // Find player and boss objects
        var player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            var sr = player.GetComponent<SpriteRenderer>();
            if (sr != null && sr.sprite == null)
            {
                sr.sprite = RuntimeAssetLoader.GetHeroSprite("BronzeWarrior");
                Debug.Log("[ArenaAutoSetup] Applied fallback sprite to Player");
            }
        }

        var boss = GameObject.FindWithTag("Boss");
        if (boss != null)
        {
            var sr = boss.GetComponent<SpriteRenderer>();
            if (sr != null && sr.sprite == null)
            {
                sr.sprite = RuntimeAssetLoader.GetBossSprite("BronzeMask");
                Debug.Log("[ArenaAutoSetup] Applied fallback sprite to Boss");
            }
        }
    }
}
