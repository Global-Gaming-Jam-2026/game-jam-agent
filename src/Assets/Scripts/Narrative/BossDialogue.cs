using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Manages boss dialogue and taunts during battle.
/// Displays text bubbles for boss personality moments.
/// </summary>
public class BossDialogue : MonoBehaviour
{
    public static BossDialogue Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private Canvas dialogueCanvas;
    [SerializeField] private Text dialogueText;
    [SerializeField] private Image bubbleBackground;

    [Header("Settings")]
    [SerializeField] private float displayDuration = 2.5f;
    [SerializeField] private float typeSpeed = 0.03f;
    [SerializeField] private float cooldownBetweenDialogues = 8f;
    [SerializeField] private Vector2 bubbleOffset = new Vector2(0, 2f);

    [Header("Dialogue Sets")]
    [SerializeField] private DialogueSet battleStartDialogue;
    [SerializeField] private DialogueSet phaseTransitionDialogue;
    [SerializeField] private DialogueSet attackDialogue;
    [SerializeField] private DialogueSet tauntDialogue;
    [SerializeField] private DialogueSet hurtDialogue;
    [SerializeField] private DialogueSet lowHealthDialogue;
    [SerializeField] private DialogueSet defeatDialogue;

    private Transform bossTransform;
    private Coroutine currentDialogue;
    private float dialogueCooldown = 0;
    private bool isShowingDialogue = false;
    private int currentPhase = 1;

    [System.Serializable]
    public class DialogueSet
    {
        public string[] lines;
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        InitializeDefaultDialogue();

        if (dialogueCanvas == null)
        {
            CreateDialogueUI();
        }
    }

    private void Start()
    {
        // Find boss
        var boss = GameObject.FindWithTag("Boss");
        if (boss != null)
        {
            bossTransform = boss.transform;
            SubscribeToBossEvents(boss);
        }

        HideDialogue();
    }

    private void Update()
    {
        // Update cooldown
        if (dialogueCooldown > 0)
        {
            dialogueCooldown -= Time.deltaTime;
        }

        // Update bubble position
        if (isShowingDialogue && bossTransform != null && dialogueCanvas != null)
        {
            UpdateBubblePosition();
        }
    }

    private void InitializeDefaultDialogue()
    {
        // Battle start
        battleStartDialogue = new DialogueSet
        {
            lines = new string[]
            {
                "You dare approach ME?",
                "The Bronze Mask recognizes no challenger!",
                "Your journey ends here, mortal.",
                "I have waited eons for worthy prey..."
            }
        };

        // Phase transition
        phaseTransitionDialogue = new DialogueSet
        {
            lines = new string[]
            {
                "ENOUGH! Face my TRUE POWER!",
                "You have awakened something ancient!",
                "The mask hungers for DESTRUCTION!",
                "NOW YOU WILL KNOW FEAR!"
            }
        };

        // Attack lines
        attackDialogue = new DialogueSet
        {
            lines = new string[]
            {
                "FALL!",
                "PERISH!",
                "KNEEL!",
                "SUFFER!",
                "CRUMBLE!"
            }
        };

        // Taunts
        tauntDialogue = new DialogueSet
        {
            lines = new string[]
            {
                "Is that all you have?",
                "Pathetic...",
                "You amuse me, insect.",
                "Your efforts are meaningless!",
                "Struggle all you want!"
            }
        };

        // When hurt
        hurtDialogue = new DialogueSet
        {
            lines = new string[]
            {
                "A lucky strike!",
                "You will PAY for that!",
                "Impressive... but futile!",
                "ARGH! Insolent worm!",
                "That... actually hurt?"
            }
        };

        // Low health
        lowHealthDialogue = new DialogueSet
        {
            lines = new string[]
            {
                "No... this cannot be!",
                "I will NOT be defeated!",
                "The mask... is eternal!",
                "You have not won yet!",
                "My power... fading...?"
            }
        };

        // Defeat
        defeatDialogue = new DialogueSet
        {
            lines = new string[]
            {
                "Impossible... defeated by a mortal...",
                "The mask... crumbles...",
                "You have... proven worthy...",
                "Perhaps... I can finally... rest..."
            }
        };
    }

    private void CreateDialogueUI()
    {
        // Create world-space canvas
        GameObject canvasObj = new GameObject("BossDialogueCanvas");
        canvasObj.transform.SetParent(transform);
        dialogueCanvas = canvasObj.AddComponent<Canvas>();
        dialogueCanvas.renderMode = RenderMode.WorldSpace;
        dialogueCanvas.sortingOrder = 150;

        // Set canvas scale for world space
        canvasObj.GetComponent<RectTransform>().sizeDelta = new Vector2(400, 100);
        canvasObj.transform.localScale = Vector3.one * 0.01f;

        // Background bubble
        GameObject bubbleObj = new GameObject("Bubble");
        bubbleObj.transform.SetParent(canvasObj.transform, false);
        bubbleBackground = bubbleObj.AddComponent<Image>();
        bubbleBackground.color = new Color(0.1f, 0.1f, 0.1f, 0.9f);
        var bubbleRect = bubbleBackground.rectTransform;
        bubbleRect.sizeDelta = new Vector2(400, 80);
        bubbleRect.anchoredPosition = Vector2.zero;

        // Dialogue text
        GameObject textObj = new GameObject("DialogueText");
        textObj.transform.SetParent(bubbleObj.transform, false);
        dialogueText = textObj.AddComponent<Text>();
        dialogueText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        dialogueText.fontSize = 24;
        dialogueText.color = Color.white;
        dialogueText.alignment = TextAnchor.MiddleCenter;
        var textRect = dialogueText.rectTransform;
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = new Vector2(-20, -10);
        textRect.anchoredPosition = Vector2.zero;

        dialogueCanvas.gameObject.SetActive(false);
    }

    private void SubscribeToBossEvents(GameObject boss)
    {
        var bossHealth = boss.GetComponent<BossHealth>();
        var bossController = boss.GetComponent<BossControllerMultiPhase>();

        if (bossHealth != null)
        {
            bossHealth.OnDamaged += OnBossDamaged;
            bossHealth.OnDeath += OnBossDefeated;
            bossHealth.OnHealthChanged += OnHealthChanged;
        }

        if (bossController != null)
        {
            bossController.OnPhaseChanged += OnPhaseChanged;
            bossController.OnStateChanged += OnStateChanged;
        }
    }

    private void UpdateBubblePosition()
    {
        if (bossTransform == null || dialogueCanvas == null) return;

        Vector3 worldPos = bossTransform.position + (Vector3)bubbleOffset;
        dialogueCanvas.transform.position = worldPos;

        // Face camera
        if (Camera.main != null)
        {
            dialogueCanvas.transform.rotation = Camera.main.transform.rotation;
        }
    }

    #region Public Methods

    /// <summary>
    /// Show battle start dialogue
    /// </summary>
    public void ShowBattleStart()
    {
        ShowRandomDialogue(battleStartDialogue, 3f);
    }

    /// <summary>
    /// Show phase transition dialogue
    /// </summary>
    public void ShowPhaseTransition(int newPhase)
    {
        currentPhase = newPhase;
        ShowRandomDialogue(phaseTransitionDialogue, 3f, true);
    }

    /// <summary>
    /// Show attack dialogue (brief)
    /// </summary>
    public void ShowAttack()
    {
        if (Random.value > 0.7f) // 30% chance
        {
            ShowRandomDialogue(attackDialogue, 1f);
        }
    }

    /// <summary>
    /// Show taunt dialogue
    /// </summary>
    public void ShowTaunt()
    {
        if (dialogueCooldown <= 0)
        {
            ShowRandomDialogue(tauntDialogue, 2.5f);
        }
    }

    /// <summary>
    /// Show dialogue when boss takes damage
    /// </summary>
    public void ShowHurt()
    {
        if (Random.value > 0.8f) // 20% chance
        {
            ShowRandomDialogue(hurtDialogue, 2f);
        }
    }

    /// <summary>
    /// Show low health dialogue
    /// </summary>
    public void ShowLowHealth()
    {
        ShowRandomDialogue(lowHealthDialogue, 3f, true);
    }

    /// <summary>
    /// Show defeat dialogue
    /// </summary>
    public void ShowDefeat()
    {
        ShowRandomDialogue(defeatDialogue, 4f, true);
    }

    /// <summary>
    /// Show a specific line of dialogue
    /// </summary>
    public void ShowDialogue(string text, float duration = -1)
    {
        if (currentDialogue != null)
        {
            StopCoroutine(currentDialogue);
        }

        float dur = duration > 0 ? duration : displayDuration;
        currentDialogue = StartCoroutine(DialogueCoroutine(text, dur));
    }

    #endregion

    #region Event Handlers

    private void OnBossDamaged()
    {
        ShowHurt();
    }

    private void OnBossDefeated()
    {
        ShowDefeat();
    }

    private void OnHealthChanged(float healthPercent)
    {
        // Low health warning at 25%
        if (healthPercent <= 0.25f && healthPercent > 0.2f)
        {
            ShowLowHealth();
        }
    }

    private void OnPhaseChanged(int phaseIndex, BossPhaseData phaseData)
    {
        ShowPhaseTransition(phaseIndex + 1);
    }

    private void OnStateChanged(BossControllerMultiPhase.BossState state)
    {
        if (state == BossControllerMultiPhase.BossState.Telegraph)
        {
            ShowAttack();
        }
        else if (state == BossControllerMultiPhase.BossState.Idle && Random.value > 0.9f)
        {
            ShowTaunt();
        }
    }

    #endregion

    #region Private Methods

    private void ShowRandomDialogue(DialogueSet set, float duration, bool ignoreCooldown = false)
    {
        if (set == null || set.lines == null || set.lines.Length == 0) return;
        if (!ignoreCooldown && dialogueCooldown > 0) return;

        string line = set.lines[Random.Range(0, set.lines.Length)];
        ShowDialogue(line, duration);
        dialogueCooldown = cooldownBetweenDialogues;
    }

    private IEnumerator DialogueCoroutine(string text, float duration)
    {
        isShowingDialogue = true;
        dialogueCanvas.gameObject.SetActive(true);
        dialogueText.text = "";

        UpdateBubblePosition();

        // Typewriter effect
        foreach (char c in text)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typeSpeed);
        }

        // Display
        yield return new WaitForSeconds(duration);

        // Fade out
        HideDialogue();
        isShowingDialogue = false;
        currentDialogue = null;
    }

    private void HideDialogue()
    {
        if (dialogueCanvas != null)
        {
            dialogueCanvas.gameObject.SetActive(false);
        }
    }

    #endregion

    private void OnDestroy()
    {
        var boss = GameObject.FindWithTag("Boss");
        if (boss != null)
        {
            var bossHealth = boss.GetComponent<BossHealth>();
            var bossController = boss.GetComponent<BossControllerMultiPhase>();

            if (bossHealth != null)
            {
                bossHealth.OnDamaged -= OnBossDamaged;
                bossHealth.OnDeath -= OnBossDefeated;
                bossHealth.OnHealthChanged -= OnHealthChanged;
            }

            if (bossController != null)
            {
                bossController.OnPhaseChanged -= OnPhaseChanged;
                bossController.OnStateChanged -= OnStateChanged;
            }
        }
    }
}
