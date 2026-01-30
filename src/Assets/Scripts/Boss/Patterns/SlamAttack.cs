using UnityEngine;
using System.Collections;

/// <summary>
/// Pattern 2: Overhead slam at player's position
/// Counter: Sidestep, then punish during recovery
/// </summary>
public class SlamAttack : BossAttackPattern
{
    [Header("Slam Settings")]
    [SerializeField] private float slamRadius = 2f;
    [SerializeField] private float riseHeight = 3f;
    [SerializeField] private float slamSpeed = 20f;

    [Header("Visuals")]
    [SerializeField] private GameObject shadowIndicator;
    [SerializeField] private GameObject impactEffect;
    [SerializeField] private Color telegraphColor = new Color(0.3f, 0, 0, 0.5f); // Dark red

    private SpriteRenderer shadowRenderer;
    private Vector3 originalPosition;
    private Vector3 targetPosition;

    private void Awake()
    {
        patternName = "Slam";
        damage = 20f;              // Balanced: 45 → 20 (not instant death)
        telegraphDuration = 1.5f;  // Balanced: 0.7 → 1.5 (readable telegraph)
        attackDuration = 0.2f;
        selectionWeight = 1.5f;
        minPhaseRequired = 1;

        // Create shadow indicator if not assigned
        if (shadowIndicator == null)
        {
            shadowIndicator = new GameObject("SlamShadow");
            shadowIndicator.transform.SetParent(transform.parent);
            var sr = shadowIndicator.AddComponent<SpriteRenderer>();
            sr.sprite = CreateCircleSprite();
            sr.color = telegraphColor;
            sr.sortingOrder = -1;
            shadowRenderer = sr;
            shadowIndicator.SetActive(false);
        }
        else
        {
            shadowRenderer = shadowIndicator.GetComponent<SpriteRenderer>();
        }
    }

    public override IEnumerator Telegraph(float speedMultiplier = 1f)
    {
        isCancelled = false;
        float duration = telegraphDuration / speedMultiplier;

        originalPosition = transform.position;

        // Target player's current position
        if (player != null)
        {
            targetPosition = new Vector3(player.position.x, originalPosition.y, originalPosition.z);
        }
        else
        {
            targetPosition = originalPosition;
        }

        // Show shadow indicator at target position
        if (shadowIndicator != null)
        {
            shadowIndicator.SetActive(true);
            shadowIndicator.transform.position = new Vector3(targetPosition.x, targetPosition.y - 1f, targetPosition.z);
            shadowIndicator.transform.localScale = Vector3.zero;
        }

        // Rise up
        Vector3 risePosition = originalPosition + Vector3.up * riseHeight;
        float riseTime = duration * 0.6f;
        float elapsed = 0;

        while (elapsed < riseTime && !isCancelled)
        {
            float t = elapsed / riseTime;
            transform.position = Vector3.Lerp(originalPosition, risePosition, EaseOutCubic(t));

            // Grow shadow indicator
            if (shadowIndicator != null)
            {
                float scale = Mathf.Lerp(0, slamRadius * 2, t);
                shadowIndicator.transform.localScale = new Vector3(scale, scale * 0.3f, 1);

                // Pulse shadow
                if (shadowRenderer != null)
                {
                    Color c = telegraphColor;
                    c.a = 0.3f + Mathf.PingPong(elapsed * 3f, 0.3f);
                    shadowRenderer.color = c;
                }
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Hold at top briefly
        float holdTime = duration * 0.4f;
        elapsed = 0;

        while (elapsed < holdTime && !isCancelled)
        {
            // Pulse more intensely
            if (shadowRenderer != null)
            {
                Color c = telegraphColor;
                c.a = 0.5f + Mathf.PingPong(elapsed * 6f, 0.4f);
                shadowRenderer.color = c;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    public override IEnumerator Execute(float speedMultiplier = 1f)
    {
        if (isCancelled) yield break;

        // Slam down fast
        Vector3 slamTarget = targetPosition;
        float slamDistance = Vector3.Distance(transform.position, slamTarget);
        float slamTime = slamDistance / (slamSpeed * speedMultiplier);

        Vector3 startPos = transform.position;
        float elapsed = 0;

        while (elapsed < slamTime && !isCancelled)
        {
            float t = elapsed / slamTime;
            transform.position = Vector3.Lerp(startPos, slamTarget, EaseInCubic(t));
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = slamTarget;

        // Impact!
        if (!isCancelled)
        {
            // Play attack sound
            if (attackSound != null)
            {
                AudioSource.PlayClipAtPoint(attackSound, transform.position);
            }

            // Deal damage in radius
            DealDamageToPlayer(slamTarget, new Vector2(slamRadius * 2, slamRadius), damage);

            // Heavy screen shake + zoom pulse for impact
            if (CameraShake.Instance != null)
            {
                CameraShake.Instance.BossSlamImpact();
            }

            // Impact effect
            if (impactEffect != null)
            {
                Instantiate(impactEffect, slamTarget, Quaternion.identity);
            }
        }

        // Hide shadow
        if (shadowIndicator != null)
        {
            shadowIndicator.SetActive(false);
        }

        // Return to original position slowly (recovery)
        yield return MoveToPosition(originalPosition, 0.5f / speedMultiplier);
    }

    private IEnumerator MoveToPosition(Vector3 target, float duration)
    {
        Vector3 start = transform.position;
        float elapsed = 0;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            transform.position = Vector3.Lerp(start, target, EaseOutCubic(t));
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = target;
    }

    public override void Cancel()
    {
        base.Cancel();
        if (shadowIndicator != null)
        {
            shadowIndicator.SetActive(false);
        }
        // Return to original position immediately
        transform.position = originalPosition;
    }

    private float EaseOutCubic(float t) => 1 - Mathf.Pow(1 - t, 3);
    private float EaseInCubic(float t) => t * t * t;

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
