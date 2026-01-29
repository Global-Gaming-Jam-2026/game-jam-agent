using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Pattern 3: Summon homing spirit projectiles (Phase 2 only)
/// Counter: Dodge timing, or destroy with attacks
/// </summary>
public class SpiritProjectileAttack : BossAttackPattern
{
    [Header("Projectile Settings")]
    [SerializeField] private int projectileCount = 3;
    [SerializeField] private float projectileSpeed = 6f;
    [SerializeField] private float projectileLifetime = 4f;
    [SerializeField] private float homingStrength = 2f;
    [SerializeField] private float spawnDelay = 0.3f;
    [SerializeField] private float projectileSize = 0.5f;

    [Header("Visuals")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Color projectileColor = new Color(0.8f, 0.6f, 0.2f, 1f); // Bronze/gold

    private List<GameObject> activeProjectiles = new List<GameObject>();

    private void Awake()
    {
        patternName = "Spirits";
        damage = 20f;
        telegraphDuration = 0.4f;
        attackDuration = 1f;
        selectionWeight = 1f;
        minPhaseRequired = 2; // Phase 2 only!
    }

    public override IEnumerator Telegraph(float speedMultiplier = 1f)
    {
        isCancelled = false;
        float duration = telegraphDuration / speedMultiplier;

        // Visual: Boss glows, eyes light up
        var sr = GetComponent<SpriteRenderer>();
        Color originalColor = sr != null ? sr.color : Color.white;

        float elapsed = 0;
        while (elapsed < duration && !isCancelled)
        {
            float t = elapsed / duration;

            // Pulsing glow
            if (sr != null)
            {
                float glow = Mathf.Sin(t * Mathf.PI * 4) * 0.3f + 0.7f;
                sr.color = Color.Lerp(originalColor, projectileColor, glow);
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Reset color
        if (sr != null)
        {
            sr.color = originalColor;
        }
    }

    public override IEnumerator Execute(float speedMultiplier = 1f)
    {
        if (isCancelled) yield break;

        // Spawn projectiles with delay between each
        for (int i = 0; i < projectileCount && !isCancelled; i++)
        {
            SpawnProjectile(i);

            if (i < projectileCount - 1)
            {
                yield return new WaitForSeconds(spawnDelay / speedMultiplier);
            }
        }

        // Wait for projectiles to resolve (or timeout)
        float timeout = projectileLifetime + 0.5f;
        float elapsed = 0;

        while (activeProjectiles.Count > 0 && elapsed < timeout && !isCancelled)
        {
            // Clean up destroyed projectiles
            activeProjectiles.RemoveAll(p => p == null);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Cleanup remaining projectiles
        CleanupProjectiles();
    }

    private void SpawnProjectile(int index)
    {
        // Calculate spawn position (orbit around boss)
        float angle = (360f / projectileCount) * index * Mathf.Deg2Rad;
        Vector3 offset = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * 1.5f;
        Vector3 spawnPos = transform.position + offset;

        // Create projectile
        GameObject projectile;
        if (projectilePrefab != null)
        {
            projectile = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
        }
        else
        {
            projectile = CreateDefaultProjectile(spawnPos);
        }

        activeProjectiles.Add(projectile);

        // Add homing behavior
        var homing = projectile.AddComponent<HomingProjectile>();
        homing.Initialize(player, projectileSpeed, homingStrength, damage, projectileLifetime);
    }

    private GameObject CreateDefaultProjectile(Vector3 position)
    {
        GameObject proj = new GameObject("SpiritProjectile");
        proj.transform.position = position;
        proj.layer = LayerMask.NameToLayer("EnemyProjectile");

        // Sprite
        var sr = proj.AddComponent<SpriteRenderer>();
        sr.sprite = CreateCircleSprite();
        sr.color = projectileColor;
        proj.transform.localScale = Vector3.one * projectileSize;

        // Collider
        var col = proj.AddComponent<CircleCollider2D>();
        col.isTrigger = true;
        col.radius = 0.5f;

        // Rigidbody
        var rb = proj.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.bodyType = RigidbodyType2D.Kinematic;

        return proj;
    }

    public override void Cancel()
    {
        base.Cancel();
        CleanupProjectiles();
    }

    private void CleanupProjectiles()
    {
        foreach (var proj in activeProjectiles)
        {
            if (proj != null)
            {
                Destroy(proj);
            }
        }
        activeProjectiles.Clear();
    }

    private Sprite CreateCircleSprite()
    {
        int size = 32;
        Texture2D tex = new Texture2D(size, size);
        Color[] colors = new Color[size * size];

        Vector2 center = new Vector2(size / 2f, size / 2f);
        float radius = size / 2f;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dist = Vector2.Distance(new Vector2(x, y), center);
                if (dist < radius)
                {
                    // Gradient from center
                    float alpha = 1f - (dist / radius) * 0.5f;
                    colors[y * size + x] = new Color(1, 1, 1, alpha);
                }
                else
                {
                    colors[y * size + x] = Color.clear;
                }
            }
        }

        tex.SetPixels(colors);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
    }
}

/// <summary>
/// Homing projectile behavior
/// </summary>
public class HomingProjectile : MonoBehaviour
{
    private Transform target;
    private float speed;
    private float homingStrength;
    private float damage;
    private float lifetime;
    private Vector2 velocity;
    private float elapsed;

    public void Initialize(Transform target, float speed, float homingStrength, float damage, float lifetime)
    {
        this.target = target;
        this.speed = speed;
        this.homingStrength = homingStrength;
        this.damage = damage;
        this.lifetime = lifetime;

        // Initial velocity towards target
        if (target != null)
        {
            velocity = ((Vector2)(target.position - transform.position)).normalized * speed;
        }
        else
        {
            velocity = Vector2.right * speed;
        }
    }

    private void Update()
    {
        elapsed += Time.deltaTime;

        if (elapsed >= lifetime)
        {
            Destroy(gameObject);
            return;
        }

        // Homing behavior
        if (target != null)
        {
            Vector2 desiredDirection = ((Vector2)(target.position - transform.position)).normalized;
            velocity = Vector2.Lerp(velocity.normalized, desiredDirection, homingStrength * Time.deltaTime).normalized * speed;
        }

        // Move
        transform.position += (Vector3)(velocity * Time.deltaTime);

        // Rotate to face movement direction
        float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if hit player
        var playerHealth = other.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        // Check if destroyed by player attack (optional)
        if (other.CompareTag("PlayerAttack"))
        {
            // Could add score or effect here
            Destroy(gameObject);
        }
    }
}
