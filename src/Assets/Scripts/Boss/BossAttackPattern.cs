using UnityEngine;
using System.Collections;

/// <summary>
/// Base class for all boss attack patterns
/// </summary>
public abstract class BossAttackPattern : MonoBehaviour
{
    [Header("Pattern Settings")]
    [SerializeField] protected string patternName = "Attack";
    [SerializeField] protected float damage = 25f;
    [SerializeField] protected float telegraphDuration = 0.5f;
    [SerializeField] protected float attackDuration = 0.3f;
    [SerializeField] protected float selectionWeight = 1f;
    [SerializeField] protected int minPhaseRequired = 1;

    [Header("Audio")]
    [SerializeField] protected AudioClip telegraphSound;
    [SerializeField] protected AudioClip attackSound;

    protected BossControllerMultiPhase boss;
    protected Transform player;
    protected bool isCancelled;

    public string PatternName => patternName;
    public float SelectionWeight => selectionWeight;
    public int MinPhaseRequired => minPhaseRequired;

    public virtual void Initialize(BossControllerMultiPhase bossController)
    {
        boss = bossController;
        player = boss.Player;
    }

    /// <summary>
    /// Telegraph phase - shows warning before attack
    /// </summary>
    public virtual IEnumerator Telegraph(float speedMultiplier = 1f)
    {
        isCancelled = false;
        float duration = telegraphDuration / speedMultiplier;

        // Play telegraph sound
        if (telegraphSound != null)
        {
            AudioSource.PlayClipAtPoint(telegraphSound, transform.position);
        }

        yield return new WaitForSeconds(duration);
    }

    /// <summary>
    /// Execute the attack
    /// </summary>
    public abstract IEnumerator Execute(float speedMultiplier = 1f);

    /// <summary>
    /// Cancel the attack (when staggered)
    /// </summary>
    public virtual void Cancel()
    {
        isCancelled = true;
        StopAllCoroutines();
    }

    /// <summary>
    /// Deal damage to player if in range
    /// </summary>
    protected void DealDamageToPlayer(Vector2 hitboxCenter, Vector2 hitboxSize, float damageAmount)
    {
        Collider2D[] hits = Physics2D.OverlapBoxAll(hitboxCenter, hitboxSize, 0, LayerMask.GetMask("Player"));

        foreach (var hit in hits)
        {
            var playerHealth = hit.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damageAmount);

                // Play attack sound
                if (attackSound != null)
                {
                    AudioSource.PlayClipAtPoint(attackSound, transform.position);
                }

                // Screen shake
                if (CameraShake.Instance != null)
                {
                    CameraShake.Instance.ShakeBossAttack();
                }
            }
        }
    }

    /// <summary>
    /// Get direction towards player
    /// </summary>
    protected int GetDirectionToPlayer()
    {
        if (player == null) return 1;
        return player.position.x > transform.position.x ? 1 : -1;
    }

    /// <summary>
    /// Get distance to player
    /// </summary>
    protected float GetDistanceToPlayer()
    {
        if (player == null) return float.MaxValue;
        return Vector2.Distance(transform.position, player.position);
    }
}
