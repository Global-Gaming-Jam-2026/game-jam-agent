using UnityEngine;
using System.Collections;

/// <summary>
/// Pattern 1: Wide horizontal sweep across the arena
/// Counter: Roll through the sweep
/// </summary>
public class SweepAttack : BossAttackPattern
{
    [Header("Sweep Settings")]
    [SerializeField] private float sweepWidth = 8f;
    [SerializeField] private float sweepHeight = 2f;
    [SerializeField] private float sweepSpeed = 15f;
    [SerializeField] private float startOffset = 5f;

    [Header("Visuals")]
    [SerializeField] private GameObject sweepVisual;
    [SerializeField] private Color telegraphColor = new Color(1f, 0.5f, 0, 0.5f); // Orange telegraph

    private SpriteRenderer telegraphRenderer;
    private Vector3 sweepStartPos;
    private Vector3 sweepEndPos;
    private int sweepDirection;

    private void Awake()
    {
        patternName = "Sweep";
        damage = 30f;
        telegraphDuration = 0.5f;
        attackDuration = 0.4f;
        selectionWeight = 2f;
        minPhaseRequired = 1;

        // Create telegraph visual if not assigned
        if (sweepVisual == null)
        {
            sweepVisual = new GameObject("SweepVisual");
            sweepVisual.transform.SetParent(transform);
            var sr = sweepVisual.AddComponent<SpriteRenderer>();
            sr.sprite = CreateSquareSprite();
            sr.color = telegraphColor;
            telegraphRenderer = sr;
            sweepVisual.SetActive(false);
        }
        else
        {
            telegraphRenderer = sweepVisual.GetComponent<SpriteRenderer>();
        }
    }

    public override IEnumerator Telegraph(float speedMultiplier = 1f)
    {
        isCancelled = false;
        float duration = telegraphDuration / speedMultiplier;

        // Determine sweep direction (towards player, then continue through)
        sweepDirection = GetDirectionToPlayer();

        // Calculate sweep path
        sweepStartPos = transform.position + new Vector3(-startOffset * sweepDirection, 0, 0);
        sweepEndPos = transform.position + new Vector3(startOffset * sweepDirection, 0, 0);

        // Show telegraph
        if (sweepVisual != null)
        {
            sweepVisual.SetActive(true);
            sweepVisual.transform.localScale = new Vector3(sweepWidth * 2, sweepHeight, 1);
            sweepVisual.transform.position = transform.position;

            // Pulse telegraph
            float elapsed = 0;
            while (elapsed < duration && !isCancelled)
            {
                float alpha = Mathf.PingPong(elapsed * 4f, 0.5f) + 0.3f;
                if (telegraphRenderer != null)
                {
                    Color c = telegraphColor;
                    c.a = alpha;
                    telegraphRenderer.color = c;
                }
                elapsed += Time.deltaTime;
                yield return null;
            }
        }
        else
        {
            yield return new WaitForSeconds(duration);
        }
    }

    public override IEnumerator Execute(float speedMultiplier = 1f)
    {
        if (isCancelled) yield break;

        // Hide telegraph, prepare attack
        if (sweepVisual != null)
        {
            sweepVisual.SetActive(true);
            telegraphRenderer.color = new Color(1f, 0.3f, 0.1f, 0.8f); // More solid for attack
            sweepVisual.transform.localScale = new Vector3(sweepWidth, sweepHeight, 1);
        }

        // Play attack sound
        if (attackSound != null)
        {
            AudioSource.PlayClipAtPoint(attackSound, transform.position);
        }

        // Sweep across
        float sweepTime = (startOffset * 2) / (sweepSpeed * speedMultiplier);
        float elapsed = 0;

        while (elapsed < sweepTime && !isCancelled)
        {
            float t = elapsed / sweepTime;
            Vector3 currentPos = Vector3.Lerp(sweepStartPos, sweepEndPos, t);

            if (sweepVisual != null)
            {
                sweepVisual.transform.position = currentPos;
            }

            // Check for hits along the sweep
            DealDamageToPlayer(currentPos, new Vector2(sweepWidth, sweepHeight), damage);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Cleanup
        if (sweepVisual != null)
        {
            sweepVisual.SetActive(false);
        }

        // Screen shake on completion
        if (CameraShake.Instance != null)
        {
            CameraShake.Instance.ShakeMedium();
        }
    }

    public override void Cancel()
    {
        base.Cancel();
        if (sweepVisual != null)
        {
            sweepVisual.SetActive(false);
        }
    }

    private Sprite CreateSquareSprite()
    {
        Texture2D tex = new Texture2D(4, 4);
        Color[] colors = new Color[16];
        for (int i = 0; i < 16; i++) colors[i] = Color.white;
        tex.SetPixels(colors);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, 4, 4), new Vector2(0.5f, 0.5f), 4);
    }
}
