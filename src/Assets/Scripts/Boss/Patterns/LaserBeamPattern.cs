using UnityEngine;
using System.Collections;

/// <summary>
/// Cuphead-style sweeping laser beam attack
/// Telegraphs with a thin line, then fires a thick damaging beam
/// </summary>
public class LaserBeamPattern : BossAttackPattern
{
    [Header("Laser Settings")]
    [SerializeField] private float laserWidth = 0.5f;
    [SerializeField] private float laserLength = 20f;
    [SerializeField] private float sweepAngle = 90f;      // Total sweep arc
    [SerializeField] private float sweepDuration = 2f;
    [SerializeField] private bool sweepClockwise = true;

    [Header("Visuals")]
    [SerializeField] private Color telegraphColor = new Color(1f, 0f, 0f, 0.3f);
    [SerializeField] private Color laserColor = new Color(1f, 0.3f, 0.1f, 1f);
    [SerializeField] private Color coreColor = new Color(1f, 1f, 0.8f, 1f);

    [Header("Damage")]
    [SerializeField] private float damageInterval = 0.3f; // Minimum time between damage ticks

    private GameObject laserObject;
    private LineRenderer lineRenderer;
    private LineRenderer coreRenderer;
    private float lastDamageTime;

    private void Awake()
    {
        patternName = "Laser Beam";
        damage = 35f;
        telegraphDuration = 1f;
        selectionWeight = 0.8f;
        minPhaseRequired = 2;
    }

    public override IEnumerator Telegraph(float speedMultiplier = 1f)
    {
        isCancelled = false;

        // Create laser object
        CreateLaserVisual();

        // Calculate initial angle (towards player)
        float startAngle = 0;
        if (player != null)
        {
            Vector2 toPlayer = player.position - transform.position;
            startAngle = Mathf.Atan2(toPlayer.y, toPlayer.x) * Mathf.Rad2Deg;
        }
        startAngle += sweepClockwise ? sweepAngle / 2 : -sweepAngle / 2;

        // Show thin telegraph line
        float duration = telegraphDuration / speedMultiplier;
        float elapsed = 0;

        while (elapsed < duration && !isCancelled)
        {
            // Pulse the telegraph
            float alpha = Mathf.PingPong(elapsed * 4f, 0.5f) + 0.2f;
            if (lineRenderer != null)
            {
                Color c = telegraphColor;
                c.a = alpha;
                lineRenderer.startColor = c;
                lineRenderer.endColor = c;
                lineRenderer.startWidth = 0.1f;
                lineRenderer.endWidth = 0.1f;
            }

            // Point at start position
            UpdateLaserDirection(startAngle);

            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    public override IEnumerator Execute(float speedMultiplier = 1f)
    {
        if (isCancelled)
        {
            Cleanup();
            yield break;
        }

        // Reset damage timer
        lastDamageTime = -damageInterval;

        // Calculate sweep
        float startAngle = 0;
        if (player != null)
        {
            Vector2 toPlayer = player.position - transform.position;
            startAngle = Mathf.Atan2(toPlayer.y, toPlayer.x) * Mathf.Rad2Deg;
        }

        float sweepStart = startAngle + (sweepClockwise ? sweepAngle / 2 : -sweepAngle / 2);
        float sweepEnd = startAngle + (sweepClockwise ? -sweepAngle / 2 : sweepAngle / 2);

        // Fire laser!
        if (lineRenderer != null)
        {
            lineRenderer.startColor = laserColor;
            lineRenderer.endColor = laserColor;
            lineRenderer.startWidth = laserWidth;
            lineRenderer.endWidth = laserWidth * 0.8f;
        }

        // Show core
        if (coreRenderer != null)
        {
            coreRenderer.enabled = true;
        }

        // Play sound
        if (attackSound != null)
        {
            AudioSource.PlayClipAtPoint(attackSound, transform.position);
        }

        // Screen shake
        if (CameraShake.Instance != null)
        {
            CameraShake.Instance.ShakeMedium();
        }

        // Sweep the laser
        float duration = sweepDuration / speedMultiplier;
        float elapsed = 0;

        while (elapsed < duration && !isCancelled)
        {
            float t = elapsed / duration;
            float currentAngle = Mathf.Lerp(sweepStart, sweepEnd, t);

            UpdateLaserDirection(currentAngle);

            // Check for hits along the laser
            CheckLaserHits(currentAngle);

            // Flicker effect
            if (lineRenderer != null)
            {
                float flicker = 0.8f + Random.Range(0f, 0.2f);
                lineRenderer.startWidth = laserWidth * flicker;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        Cleanup();
    }

    private void CreateLaserVisual()
    {
        laserObject = new GameObject("Laser");
        laserObject.transform.SetParent(transform);
        laserObject.transform.localPosition = Vector3.zero;

        // Main laser line
        lineRenderer = laserObject.AddComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.startColor = telegraphColor;
        lineRenderer.endColor = telegraphColor;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.sortingOrder = 15;

        // Core (brighter center)
        GameObject core = new GameObject("LaserCore");
        core.transform.SetParent(laserObject.transform);
        coreRenderer = core.AddComponent<LineRenderer>();
        coreRenderer.positionCount = 2;
        coreRenderer.startWidth = laserWidth * 0.3f;
        coreRenderer.endWidth = laserWidth * 0.2f;
        coreRenderer.startColor = coreColor;
        coreRenderer.endColor = coreColor;
        coreRenderer.material = new Material(Shader.Find("Sprites/Default"));
        coreRenderer.sortingOrder = 16;
        coreRenderer.enabled = false;
    }

    private void UpdateLaserDirection(float angle)
    {
        Vector2 direction = new Vector2(
            Mathf.Cos(angle * Mathf.Deg2Rad),
            Mathf.Sin(angle * Mathf.Deg2Rad)
        );

        Vector3 start = transform.position;
        Vector3 end = start + (Vector3)(direction * laserLength);

        if (lineRenderer != null)
        {
            lineRenderer.SetPosition(0, start);
            lineRenderer.SetPosition(1, end);
        }

        if (coreRenderer != null)
        {
            coreRenderer.SetPosition(0, start);
            coreRenderer.SetPosition(1, end);
        }
    }

    private void CheckLaserHits(float angle)
    {
        // Check damage interval cooldown
        if (Time.time - lastDamageTime < damageInterval)
            return;

        Vector2 direction = new Vector2(
            Mathf.Cos(angle * Mathf.Deg2Rad),
            Mathf.Sin(angle * Mathf.Deg2Rad)
        );

        // Raycast for player
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, direction, laserLength, LayerMask.GetMask("Player"));

        foreach (var hit in hits)
        {
            var playerHealth = hit.collider.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage); // Damage per tick (interval-based)
                lastDamageTime = Time.time;
                return; // Only damage once per interval
            }
        }
    }

    public override void Cancel()
    {
        base.Cancel();
        Cleanup();
    }

    private void Cleanup()
    {
        if (laserObject != null)
        {
            Destroy(laserObject);
            laserObject = null;
            lineRenderer = null;
            coreRenderer = null;
        }
    }
}
