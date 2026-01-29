using UnityEngine;

/// <summary>
/// Hurtbox component - receives damage from Hitboxes
/// Attach to entities that can take damage
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class Hurtbox : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool isPlayerOwned = true;
    [SerializeField] private float damageMultiplier = 1f;

    [Header("References")]
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private BossHealth bossHealth;

    private Collider2D hurtboxCollider;

    public bool IsPlayerOwned => isPlayerOwned;

    public event System.Action<float> OnDamageReceived;

    private void Awake()
    {
        hurtboxCollider = GetComponent<Collider2D>();
        hurtboxCollider.isTrigger = true;

        // Auto-find health components if not assigned
        if (isPlayerOwned && playerHealth == null)
        {
            playerHealth = GetComponentInParent<PlayerHealth>();
        }
        if (!isPlayerOwned && bossHealth == null)
        {
            bossHealth = GetComponentInParent<BossHealth>();
        }
    }

    /// <summary>
    /// Called when a Hitbox hits this Hurtbox
    /// </summary>
    public void ReceiveHit(Hitbox hitbox)
    {
        float finalDamage = hitbox.Damage * damageMultiplier;

        // Apply damage to appropriate health component
        if (isPlayerOwned && playerHealth != null)
        {
            playerHealth.TakeDamage(finalDamage);
        }
        else if (!isPlayerOwned && bossHealth != null)
        {
            bossHealth.TakeDamage(finalDamage);
        }

        OnDamageReceived?.Invoke(finalDamage);
    }

    /// <summary>
    /// Enable/disable the hurtbox
    /// </summary>
    public void SetActive(bool active)
    {
        hurtboxCollider.enabled = active;
    }
}
