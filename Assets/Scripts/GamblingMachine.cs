using UnityEngine;
using TMPro;
using System.Collections;

public class GamblingMachine : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float baseRollPrice = 50f;
    [SerializeField] private Vector2 priceMultiplierRange = new Vector2(1.2f, 1.5f);
    [SerializeField] private float maxPriceMultiplier = 5f;
    [SerializeField] private ItemCore[] possibleItems;

    [Header("References")]
    [SerializeField] private TMP_Text priceText;
    [SerializeField] private SlotWheel[] wheels;
    [SerializeField] private Transform itemOutputPoint;
    [SerializeField] private DamageToActivate rollButton;

    private float currentMultiplier = 1f;
    private bool isRolling;
    private ItemCore currentRealItem;
    private int targetWheelIndex;

    private void Awake()
    {
        rollButton.OnActivateEvent += TryRoll;
        UpdatePriceDisplay();
    }

    private void UpdatePriceDisplay()
    {
        priceText.text = $"{(baseRollPrice * currentMultiplier):F1}$";
    }

    public void TryRoll(PlayerCore player)
    {
        if (isRolling || !player.TryBuying(Mathf.RoundToInt(baseRollPrice * currentMultiplier)))
            return;

        StartCoroutine(RollSequence(player));
    }

    private IEnumerator RollSequence(PlayerCore player)
    {
        isRolling = true;
        currentMultiplier = Mathf.Min(maxPriceMultiplier,
            currentMultiplier * Random.Range(priceMultiplierRange.x, priceMultiplierRange.y));

        // Select real item and target wheel
        currentRealItem = Instantiate(possibleItems[Random.Range(0, possibleItems.Length)]);
        targetWheelIndex = Random.Range(0, wheels.Length);

        // Configure wheels
        foreach (var wheel in wheels)
        {
            wheel.PrepareSpin(currentRealItem, wheel == wheels[targetWheelIndex]);
        }

        // Start spin
        foreach (var wheel in wheels)
        {
            wheel.StartSpin();
        }

        // Stop wheels with target wheel last
        for (int i = 0; i < wheels.Length; i++)
        {
            if (i == targetWheelIndex) continue;
            yield return new WaitForSeconds(Random.Range(0.5f, 1.2f));
            wheels[i].StopSpin(fake: true);
            yield return new WaitUntil(() => !wheels[i].IsSpinning);
        }

        // Stop target wheel with real item
        yield return new WaitForSeconds(1f);
        wheels[targetWheelIndex].StopSpin(fake: false);
        yield return new WaitUntil(() => !wheels[targetWheelIndex].IsSpinning);

        isRolling = false;
        UpdatePriceDisplay();
    }

    public void CollectItem()
    {
        if (currentRealItem != null)
        {
            currentRealItem.transform.SetParent(itemOutputPoint);
            currentRealItem.MakeItWorldItem();
            currentMultiplier = 1f;
            UpdatePriceDisplay();
        }
    }
}