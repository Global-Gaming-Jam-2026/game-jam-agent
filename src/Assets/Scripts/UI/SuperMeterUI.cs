using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Cuphead-style super meter UI
/// Shows cards/segments that fill up, glows when ready
/// </summary>
public class SuperMeterUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerParry playerParry;
    [SerializeField] private Image[] meterCards;  // 5 card segments like Cuphead
    [SerializeField] private Image meterBackground;
    [SerializeField] private GameObject superReadyEffect;

    [Header("Colors")]
    [SerializeField] private Color emptyColor = new Color(0.2f, 0.2f, 0.2f, 0.8f);
    [SerializeField] private Color filledColor = new Color(0.8f, 0.5f, 0.2f, 1f); // Bronze
    [SerializeField] private Color readyColor = new Color(1f, 0.8f, 0.3f, 1f);    // Gold

    [Header("Animation")]
    [SerializeField] private float pulseSpeed = 3f;

    private bool isReady;

    private void Start()
    {
        // Find player parry if not assigned
        if (playerParry == null)
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerParry = player.GetComponent<PlayerParry>();
            }
        }

        if (playerParry != null)
        {
            playerParry.OnSuperMeterChanged += UpdateMeter;
            playerParry.OnSuperReady += OnSuperReady;
            UpdateMeter(playerParry.SuperMeterPercent);
        }

        // Hide ready effect initially
        if (superReadyEffect != null)
        {
            superReadyEffect.SetActive(false);
        }
    }

    private void Update()
    {
        if (isReady)
        {
            // Pulse effect when ready
            float pulse = Mathf.Sin(Time.time * pulseSpeed) * 0.2f + 0.8f;
            foreach (var card in meterCards)
            {
                if (card != null)
                {
                    card.color = Color.Lerp(filledColor, readyColor, pulse);
                }
            }
        }
    }

    private void UpdateMeter(float percent)
    {
        if (meterCards == null || meterCards.Length == 0) return;

        int cardCount = meterCards.Length;
        float percentPerCard = 1f / cardCount;

        for (int i = 0; i < cardCount; i++)
        {
            if (meterCards[i] == null) continue;

            float cardThreshold = (i + 1) * percentPerCard;

            if (percent >= cardThreshold)
            {
                // Fully filled
                meterCards[i].color = filledColor;
                meterCards[i].fillAmount = 1f;
            }
            else if (percent > i * percentPerCard)
            {
                // Partially filled
                float cardPercent = (percent - i * percentPerCard) / percentPerCard;
                meterCards[i].fillAmount = cardPercent;
                meterCards[i].color = Color.Lerp(emptyColor, filledColor, cardPercent);
            }
            else
            {
                // Empty
                meterCards[i].color = emptyColor;
                meterCards[i].fillAmount = 1f;
            }
        }

        // Reset ready state if meter emptied
        if (percent < 1f)
        {
            isReady = false;
            if (superReadyEffect != null)
            {
                superReadyEffect.SetActive(false);
            }
        }
    }

    private void OnSuperReady()
    {
        isReady = true;

        if (superReadyEffect != null)
        {
            superReadyEffect.SetActive(true);
        }

        // Flash effect
        StartCoroutine(ReadyFlash());
    }

    private System.Collections.IEnumerator ReadyFlash()
    {
        for (int i = 0; i < 3; i++)
        {
            foreach (var card in meterCards)
            {
                if (card != null) card.color = Color.white;
            }
            yield return new WaitForSeconds(0.1f);

            foreach (var card in meterCards)
            {
                if (card != null) card.color = readyColor;
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void OnDestroy()
    {
        if (playerParry != null)
        {
            playerParry.OnSuperMeterChanged -= UpdateMeter;
            playerParry.OnSuperReady -= OnSuperReady;
        }
    }
}
