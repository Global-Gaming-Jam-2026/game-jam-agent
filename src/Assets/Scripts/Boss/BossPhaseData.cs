using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Data container for a single boss phase/form (Cuphead-style)
/// Each phase can have completely different visuals and attacks
/// </summary>
[System.Serializable]
public class BossPhaseData
{
    [Header("Phase Info")]
    public string phaseName = "Phase 1";
    public float healthPercentStart = 1f;    // Phase starts at this HP %
    public float healthPercentEnd = 0.5f;    // Phase ends at this HP %

    [Header("Visuals - The Form")]
    public Sprite formSprite;                 // Main sprite for this form
    public RuntimeAnimatorController animator; // Animator for this form
    public Vector2 formScale = Vector2.one;   // Scale of this form
    public Vector2 formOffset = Vector2.zero; // Position offset

    [Header("Attack Patterns")]
    public List<BossAttackPattern> patterns;  // Patterns available in this phase
    public float attackCooldown = 1.5f;       // Time between attacks
    public float patternSpeedMultiplier = 1f; // Speed modifier for patterns

    [Header("Phase Transition")]
    public float transitionDuration = 1.5f;   // How long transformation takes
    public bool screenShakeOnTransition = true;
    public bool invulnerableDuringTransition = true;

    [Header("Audio")]
    public AudioClip phaseMusic;              // Music for this phase
    public AudioClip transformSound;          // Sound when transforming TO this phase
    public AudioClip deathSound;              // If this is final phase, death sound
}
