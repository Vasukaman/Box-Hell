// GamblingMachine.cs
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;

public class GamblingMachine : MonoBehaviour
{
    [Header("Slot Generators")]
    [Tooltip("Define each pool: its LootGroup, how many slots, and weight")]
    public List<SlotGeneratorConfig> generators;

    [Header("Pricing")]
    [SerializeField] private int basePrice = 100;
    [SerializeField] private int priceMultiplier = 2;
    [Tooltip("World‐space Text for showing the current price")]
    [SerializeField] private TMP_Text priceText;

    [Header("Spawn")]
    [SerializeField] private Transform spawnPoint;

    [Header("UI Prefabs & Parents")]
    [SerializeField] private GamblingMachineUISlot slotPrefab;
    [SerializeField] private Transform slotsContainer;
    [SerializeField] private DamageToActivate button;

    [Header("Shuffle Settings")]
    [Tooltip("Total duration of the shuffle animation (seconds)")]
    [SerializeField] private float shuffleDuration = 1f;
    [Tooltip("Curve mapping normalized time [0→1] to interval between pulses")]
    [SerializeField] private AnimationCurve shuffleIntervalCurve = AnimationCurve.EaseInOut(0, 0.15f, 1, 0.05f);

    [Header("Final Highlight")]
    [Tooltip("Duration (seconds) to highlight the final chosen slot")]
    [SerializeField] private float finalHighlightDuration = 0.5f;

    [Header("Sounds")]
    [SerializeField] private AudioSource audioSource;


    // Internal state
    private List<GamblingMachineUISlot> slots = new List<GamblingMachineUISlot>();
    private List<SlotGeneratorConfig> slotSources = new List<SlotGeneratorConfig>();
    private int currentPrice;

    private void Start()
    {
        currentPrice = basePrice;
        UpdatePriceDisplay();
        BuildAndPopulateSlots();
        button.OnActivateEvent += TryActivating;
    }

    private void OnDestroy()
    {
        button.OnActivateEvent -= TryActivating;
    }
    /// <summary>
    /// Flattens generators into individual slots and populates their icons.
    /// </summary>
    private void BuildAndPopulateSlots()
    {
        // collapse each generator into slotSources
        slotSources.Clear();
        foreach (var gen in generators)
        {
            for (int i = 0; i < gen.slotCount; i++)
            {
                slotSources.Add(gen);
            }
        }

        // spawn & setup UI slots
        foreach (var source in slotSources)
        {
            var di = PickFromLootGroup(source.lootGroup);
            var slot = Instantiate(slotPrefab, slotsContainer);
            var tint = di.item.raritySO.slotTintColor;
            slot.Setup(di, tint, source.slotDropWeight);
            slots.Add(slot);
        }
    }


    public void TryActivating(PlayerCore player)
    {
 
        if (player.TryBuying(currentPrice))
        {
            Activate();
        }

    }

    /// <summary>
    /// Called by your button in the Editor.
    /// </summary>
    public void Activate()
    {
        if (slots.All(s => s.IsUsed())) return;
        StartCoroutine(RunMachine());
    }

    private IEnumerator RunMachine()
    {
        // 1) Shuffle phase: pulse random unused slots over shuffleDuration
        float elapsed = 0f;
        while (elapsed < shuffleDuration)
        {
            float t = Mathf.Clamp01(elapsed / shuffleDuration);
            float interval = shuffleIntervalCurve.Evaluate(t);

            // pick one random unused slot
            var unused = slots.Where(s => !s.IsUsed()).ToList();
            if (unused.Count > 0)
            {
                int pick = Random.Range(0, unused.Count);
                var chosen = unused[pick];

                // deselect ALL unused slots (they’ll tween back to scale=1 over interval)
                foreach (var s in unused)
                {
                    s.Deselect(interval);
                }

                // pulse only the chosen one
                chosen.Pulse(interval);
                //Play slot sound
                audioSource.PlayOneShot(chosen.assignedItem.item.raritySO.slotShuffleSound.sound, chosen.assignedItem.item.raritySO.slotShuffleSound.volume);
            }

            yield return new WaitForSeconds(interval);
            elapsed += interval;
        }

        // 2) Selection phase: pick a winner among the still-unused slots
        var alive = slots.Where(s => !s.IsUsed()).ToList();
        float totalW = alive.Sum(s => s.slotWeight);
        float roll = Random.value * totalW;
        var winner = alive[0];
        foreach (var s in alive)
        {
            if (roll < s.slotWeight)
            {
                winner = s;
                break;
            }
            roll -= s.slotWeight;
        }

        // 3) Final highlight animation: over finalHighlightDuration
        foreach (var s in alive)
        {
            if (s == winner)
            {
                audioSource.PlayOneShot(winner.assignedItem.item.raritySO.slotShuffleSound.sound, winner.assignedItem.item.raritySO.slotShuffleSound.volume);
                s.Pulse(finalHighlightDuration);
            }
            else
            {
                s.Deselect(finalHighlightDuration);
            }
        }

        // wait for that final highlight to complete
        yield return new WaitForSeconds(finalHighlightDuration);

        // 4) Dispense item
        var di = winner.GetAssignedItem();
        var corePrefab = di.item.itemCore;
        var inst = Instantiate(corePrefab, spawnPoint.position, spawnPoint.rotation);

        audioSource.PlayOneShot(di.item.raritySO.slotPickSound.sound, di.item.raritySO.slotPickSound.volume);


        inst.MakeItWorldItem();

        // 5) Grey out winner
        winner.MarkUsed();

        // 6) Update price text
        currentPrice *= priceMultiplier;
        UpdatePriceDisplay();
    }

    /// <summary>
    /// Updates the on-machine price text.
    /// </summary>
    private void UpdatePriceDisplay()
    {
        if (priceText != null)
        {
            priceText.text = $"{currentPrice}$";
        }
    }

    /// <summary>
    /// Picks one DropScoreItem from the given LootGroup (weighted by dropScore).
    /// </summary>
    private DropScoreItem PickFromLootGroup(LootGroup group)
    {
        int total = group.items.Sum(x => x.dropScore);
        int roll = Random.Range(0, total);
        foreach (var di in group.items)
        {
            if (roll < di.dropScore) return di;
            roll -= di.dropScore;
        }
        return group.items.Last();
    }
}
