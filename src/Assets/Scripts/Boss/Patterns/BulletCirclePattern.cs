using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Cuphead-style circular bullet pattern
/// Fires bullets in expanding circles or spiral patterns
/// </summary>
public class BulletCirclePattern : BossAttackPattern
{
    [Header("Circle Settings")]
    [SerializeField] private int bulletsPerWave = 12;
    [SerializeField] private int waveCount = 3;
    [SerializeField] private float timeBetweenWaves = 0.5f;
    [SerializeField] private float bulletSpeed = 5f;
    [SerializeField] private float spiralOffset = 15f; // Rotation between waves

    [Header("Bullet Settings")]
    [SerializeField] private float bulletSize = 0.3f;
    [SerializeField] private float bulletLifetime = 5f;
    [SerializeField] private Color bulletColor = new Color(0.9f, 0.6f, 0.2f);
    [SerializeField] private bool hasParryBullet = true; // One pink bullet per wave

    [Header("Parry Bullet")]
    [SerializeField] private Color parryColor = new Color(1f, 0.5f, 0.7f); // Pink

    private List<GameObject> activeBullets = new List<GameObject>();

    private void Awake()
    {
        patternName = "Bullet Circle";
        damage = 15f;
        telegraphDuration = 0.6f;
        selectionWeight = 1f;
        minPhaseRequired = 1;
    }

    public override IEnumerator Telegraph(float speedMultiplier = 1f)
    {
        isCancelled = false;
        float duration = telegraphDuration / speedMultiplier;

        // Boss glows/pulses
        var sr = GetComponent<SpriteRenderer>();
        if (sr == null) sr = GetComponentInParent<SpriteRenderer>();

        Color originalColor = sr != null ? sr.color : Color.white;
        float elapsed = 0;

        while (elapsed < duration && !isCancelled)
        {
            if (sr != null)
            {
                float pulse = Mathf.Sin(elapsed * 20f) * 0.3f + 0.7f;
                sr.color = Color.Lerp(originalColor, bulletColor, pulse);
            }
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (sr != null) sr.color = originalColor;
    }

    public override IEnumerator Execute(float speedMultiplier = 1f)
    {
        if (isCancelled) yield break;

        float currentRotation = 0;
        int parryIndex = hasParryBullet ? Random.Range(0, bulletsPerWave) : -1;

        for (int wave = 0; wave < waveCount && !isCancelled; wave++)
        {
            // Fire a circle of bullets
            for (int i = 0; i < bulletsPerWave; i++)
            {
                float angle = (360f / bulletsPerWave) * i + currentRotation;
                Vector2 direction = new Vector2(
                    Mathf.Cos(angle * Mathf.Deg2Rad),
                    Mathf.Sin(angle * Mathf.Deg2Rad)
                );

                bool isParry = hasParryBullet && i == parryIndex;
                SpawnBullet(transform.position, direction, isParry);
            }

            // Play sound
            if (attackSound != null)
            {
                AudioSource.PlayClipAtPoint(attackSound, transform.position, 0.5f);
            }

            // Rotate for spiral effect
            currentRotation += spiralOffset;

            // Next parry bullet in different position
            parryIndex = hasParryBullet ? (parryIndex + 3) % bulletsPerWave : -1;

            yield return new WaitForSeconds(timeBetweenWaves / speedMultiplier);
        }

        // Wait for bullets to clear
        yield return new WaitForSeconds(1f);
        CleanupBullets();
    }

    private void SpawnBullet(Vector3 position, Vector2 direction, bool isParryable)
    {
        GameObject bullet = new GameObject(isParryable ? "ParryBullet" : "Bullet");
        bullet.transform.position = position;
        bullet.layer = LayerMask.NameToLayer("EnemyProjectile");

        // Sprite
        var sr = bullet.AddComponent<SpriteRenderer>();
        sr.sprite = CreateCircleSprite();
        sr.color = isParryable ? parryColor : bulletColor;
        sr.sortingOrder = 10;
        bullet.transform.localScale = Vector3.one * bulletSize;

        // Collider
        var col = bullet.AddComponent<CircleCollider2D>();
        col.isTrigger = true;
        col.radius = 0.5f;

        // Rigidbody
        var rb = bullet.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.linearVelocity = direction * bulletSpeed;

        // Bullet behavior
        var bulletScript = bullet.AddComponent<BulletBehavior>();
        bulletScript.Initialize(damage, bulletLifetime, isParryable);

        activeBullets.Add(bullet);
    }

    public override void Cancel()
    {
        base.Cancel();
        CleanupBullets();
    }

    private void CleanupBullets()
    {
        foreach (var bullet in activeBullets)
        {
            if (bullet != null) Destroy(bullet);
        }
        activeBullets.Clear();
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
}

/// <summary>
/// Simple bullet behavior - moves, damages player, can be parried
/// </summary>
public class BulletBehavior : MonoBehaviour
{
    private float damage;
    private float lifetime;
    private bool isParryable;
    private float elapsed;

    public bool IsParryable => isParryable;

    public void Initialize(float damage, float lifetime, bool parryable)
    {
        this.damage = damage;
        this.lifetime = lifetime;
        this.isParryable = parryable;
    }

    private void Update()
    {
        elapsed += Time.deltaTime;
        if (elapsed >= lifetime)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Hit player
        var playerHealth = other.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            var playerParry = other.GetComponent<PlayerParry>();

            // Check if player is parrying and bullet is parryable
            if (isParryable && playerParry != null && playerParry.IsParrying)
            {
                playerParry.SuccessfulParry();
                Destroy(gameObject);
                return;
            }

            // Deal damage
            playerHealth.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
