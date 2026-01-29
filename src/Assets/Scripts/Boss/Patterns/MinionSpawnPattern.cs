using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Cuphead-style minion spawn attack
/// Boss summons smaller enemies that the player must deal with
/// </summary>
public class MinionSpawnPattern : BossAttackPattern
{
    [Header("Spawn Settings")]
    [SerializeField] private int minionCount = 3;
    [SerializeField] private float spawnRadius = 3f;
    [SerializeField] private float timeBetweenSpawns = 0.3f;

    [Header("Minion Settings")]
    [SerializeField] private float minionHealth = 30f;
    [SerializeField] private float minionSpeed = 3f;
    [SerializeField] private float minionDamage = 10f;
    [SerializeField] private float minionSize = 0.6f;
    [SerializeField] private Color minionColor = new Color(0.6f, 0.4f, 0.2f);

    [Header("Behavior")]
    [SerializeField] private MinionBehaviorType behaviorType = MinionBehaviorType.Chase;

    private List<GameObject> activeMinions = new List<GameObject>();

    public enum MinionBehaviorType
    {
        Chase,      // Chase player
        Orbit,      // Orbit around boss
        Stationary  // Stay in place, shoot
    }

    private void Awake()
    {
        patternName = "Summon Minions";
        damage = minionDamage;
        telegraphDuration = 0.8f;
        selectionWeight = 0.6f;
        minPhaseRequired = 2;
    }

    public override IEnumerator Telegraph(float speedMultiplier = 1f)
    {
        isCancelled = false;
        float duration = telegraphDuration / speedMultiplier;

        // Visual: summon circles appear at spawn points
        List<GameObject> spawnIndicators = new List<GameObject>();

        for (int i = 0; i < minionCount; i++)
        {
            float angle = (360f / minionCount) * i;
            Vector3 spawnPos = transform.position + new Vector3(
                Mathf.Cos(angle * Mathf.Deg2Rad) * spawnRadius,
                Mathf.Sin(angle * Mathf.Deg2Rad) * spawnRadius,
                0
            );

            var indicator = CreateSpawnIndicator(spawnPos);
            spawnIndicators.Add(indicator);
        }

        // Animate indicators
        float elapsed = 0;
        while (elapsed < duration && !isCancelled)
        {
            float scale = Mathf.Lerp(0, 1, elapsed / duration);
            foreach (var ind in spawnIndicators)
            {
                if (ind != null)
                {
                    ind.transform.localScale = Vector3.one * scale * minionSize * 2;
                }
            }
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Cleanup indicators
        foreach (var ind in spawnIndicators)
        {
            if (ind != null) Destroy(ind);
        }
    }

    public override IEnumerator Execute(float speedMultiplier = 1f)
    {
        if (isCancelled) yield break;

        // Spawn minions
        for (int i = 0; i < minionCount && !isCancelled; i++)
        {
            float angle = (360f / minionCount) * i;
            Vector3 spawnPos = transform.position + new Vector3(
                Mathf.Cos(angle * Mathf.Deg2Rad) * spawnRadius,
                Mathf.Sin(angle * Mathf.Deg2Rad) * spawnRadius,
                0
            );

            SpawnMinion(spawnPos);

            // Only play sound for first minion to avoid audio stacking
            if (attackSound != null && i == 0)
            {
                AudioSource.PlayClipAtPoint(attackSound, transform.position, 0.7f);
            }

            yield return new WaitForSeconds(timeBetweenSpawns / speedMultiplier);
        }

        // Wait for minions to be defeated or timeout
        float timeout = 10f;
        float elapsed = 0;

        while (elapsed < timeout && !isCancelled)
        {
            activeMinions.RemoveAll(m => m == null);
            if (activeMinions.Count == 0) break;
            elapsed += Time.deltaTime;
            yield return null;
        }

        CleanupMinions();
    }

    private GameObject CreateSpawnIndicator(Vector3 position)
    {
        GameObject indicator = new GameObject("SpawnIndicator");
        indicator.transform.position = position;

        var sr = indicator.AddComponent<SpriteRenderer>();
        sr.sprite = CreateCircleSprite();
        sr.color = new Color(minionColor.r, minionColor.g, minionColor.b, 0.5f);
        sr.sortingOrder = 5;

        return indicator;
    }

    private void SpawnMinion(Vector3 position)
    {
        GameObject minion = new GameObject("Minion");
        minion.transform.position = position;
        minion.transform.localScale = Vector3.one * minionSize;
        minion.layer = LayerMask.NameToLayer("Enemy");

        // Sprite
        var sr = minion.AddComponent<SpriteRenderer>();
        sr.sprite = CreateMinionSprite();
        sr.color = minionColor;
        sr.sortingOrder = 8;

        // Collider
        var col = minion.AddComponent<CircleCollider2D>();
        col.radius = 0.5f;

        // Rigidbody
        var rb = minion.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.bodyType = RigidbodyType2D.Kinematic;

        // Minion AI
        var ai = minion.AddComponent<MinionAI>();
        ai.Initialize(player, minionHealth, minionSpeed, minionDamage, behaviorType, transform);

        activeMinions.Add(minion);
    }

    public override void Cancel()
    {
        base.Cancel();
        CleanupMinions();
    }

    private void CleanupMinions()
    {
        foreach (var minion in activeMinions)
        {
            if (minion != null) Destroy(minion);
        }
        activeMinions.Clear();
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
                colors[y * size + x] = dist < radius ? Color.white : Color.clear;
            }
        }

        tex.SetPixels(colors);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
    }

    private Sprite CreateMinionSprite()
    {
        // Small mask-like sprite
        int size = 32;
        Texture2D tex = new Texture2D(size, size);
        Color[] colors = new Color[size * size];

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                Vector2 pos = new Vector2(x, y);
                Vector2 center = new Vector2(size / 2f, size / 2f);
                float dist = Vector2.Distance(pos, center);

                if (dist < size / 2f - 2)
                {
                    // Eyes
                    Vector2 leftEye = new Vector2(size * 0.35f, size * 0.6f);
                    Vector2 rightEye = new Vector2(size * 0.65f, size * 0.6f);

                    if (Vector2.Distance(pos, leftEye) < 3 || Vector2.Distance(pos, rightEye) < 3)
                    {
                        colors[y * size + x] = Color.clear; // Eye holes
                    }
                    else
                    {
                        colors[y * size + x] = Color.white;
                    }
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
/// Simple minion AI behavior
/// </summary>
public class MinionAI : MonoBehaviour
{
    private Transform target;
    private Transform boss;
    private float health;
    private float speed;
    private float damage;
    private MinionSpawnPattern.MinionBehaviorType behavior;
    private float orbitAngle;
    private SpriteRenderer spriteRenderer;

    public void Initialize(Transform target, float health, float speed, float damage,
        MinionSpawnPattern.MinionBehaviorType behavior, Transform boss)
    {
        this.target = target;
        this.health = health;
        this.speed = speed;
        this.damage = damage;
        this.behavior = behavior;
        this.boss = boss;
        this.orbitAngle = Random.Range(0f, 360f);
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (target == null) return;

        switch (behavior)
        {
            case MinionSpawnPattern.MinionBehaviorType.Chase:
                ChasePlayer();
                break;
            case MinionSpawnPattern.MinionBehaviorType.Orbit:
                OrbitBoss();
                break;
            case MinionSpawnPattern.MinionBehaviorType.Stationary:
                // Stay in place
                break;
        }
    }

    private void ChasePlayer()
    {
        Vector2 direction = ((Vector2)(target.position - transform.position)).normalized;
        transform.position += (Vector3)(direction * speed * Time.deltaTime);
    }

    private void OrbitBoss()
    {
        if (boss == null) return;
        orbitAngle += speed * 8f * Time.deltaTime; // Reduced from 20f for smoother orbit
        float radius = 2f;
        Vector3 offset = new Vector3(
            Mathf.Cos(orbitAngle * Mathf.Deg2Rad) * radius,
            Mathf.Sin(orbitAngle * Mathf.Deg2Rad) * radius,
            0
        );
        transform.position = boss.position + offset;
    }

    public void TakeDamage(float amount)
    {
        health -= amount;

        // Flash
        if (spriteRenderer != null)
        {
            StartCoroutine(FlashDamage());
        }

        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator FlashDamage()
    {
        Color original = spriteRenderer.color;
        spriteRenderer.color = Color.white;
        yield return new WaitForSeconds(0.1f);
        if (spriteRenderer != null) spriteRenderer.color = original;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var playerHealth = other.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage);
        }

        // Can be damaged by player attacks
        if (other.CompareTag("PlayerAttack"))
        {
            var hitbox = other.GetComponent<Hitbox>();
            if (hitbox != null)
            {
                TakeDamage(hitbox.Damage);
            }
        }
    }
}
