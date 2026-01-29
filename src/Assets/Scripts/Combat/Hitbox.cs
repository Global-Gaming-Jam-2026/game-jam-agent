using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Hitbox component - deals damage when overlapping with Hurtboxes
/// Attach to attack objects or attack points
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class Hitbox : MonoBehaviour
{
    [Header("Damage Settings")]
    [SerializeField] private float damage = 10f;
    [SerializeField] private bool isPlayerOwned = true;
    [SerializeField] private bool canHitMultiple = false;

    [Header("Timing")]
    [SerializeField] private bool autoDisable = true;
    [SerializeField] private float activeTime = 0.1f;

    private Collider2D hitboxCollider;
    private HashSet<Hurtbox> alreadyHit = new HashSet<Hurtbox>();
    private float activeTimer;
    private bool isActive;

    public float Damage => damage;
    public bool IsPlayerOwned => isPlayerOwned;

    public event System.Action<Hurtbox> OnHit;

    private void Awake()
    {
        hitboxCollider = GetComponent<Collider2D>();
        hitboxCollider.isTrigger = true;
        hitboxCollider.enabled = false;
    }

    private void Update()
    {
        if (isActive && autoDisable)
        {
            activeTimer -= Time.deltaTime;
            if (activeTimer <= 0)
            {
                Deactivate();
            }
        }
    }

    /// <summary>
    /// Activate the hitbox for damage detection
    /// </summary>
    public void Activate()
    {
        isActive = true;
        activeTimer = activeTime;
        hitboxCollider.enabled = true;
        alreadyHit.Clear();
    }

    /// <summary>
    /// Activate with custom duration
    /// </summary>
    public void Activate(float duration)
    {
        activeTime = duration;
        Activate();
    }

    /// <summary>
    /// Deactivate the hitbox
    /// </summary>
    public void Deactivate()
    {
        isActive = false;
        hitboxCollider.enabled = false;
        alreadyHit.Clear();
    }

    /// <summary>
    /// Set damage value
    /// </summary>
    public void SetDamage(float newDamage)
    {
        damage = newDamage;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isActive) return;

        var hurtbox = other.GetComponent<Hurtbox>();
        if (hurtbox == null) return;

        // Check if we can hit this target
        if (isPlayerOwned && hurtbox.IsPlayerOwned) return; // Player can't hit self
        if (!isPlayerOwned && !hurtbox.IsPlayerOwned) return; // Enemy can't hit self

        // Check if already hit
        if (!canHitMultiple && alreadyHit.Contains(hurtbox)) return;

        // Register hit
        alreadyHit.Add(hurtbox);
        hurtbox.ReceiveHit(this);
        OnHit?.Invoke(hurtbox);
    }
}
