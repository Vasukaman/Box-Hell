using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq; // Add this for LINQ support

// 5. Spawner Component
[RequireComponent(typeof(BoxSpawnSettings))]
public class BoxSpawner : MonoBehaviour
{
    [SerializeField] private BoxSpawnSettings settings;

    public Box SpawnBox()
    {
        if (settings.tiers.Length == 0)
        {
            Debug.LogError("No tiers defined in spawn settings!");
            return null;
        }

        // Select tier
        var tierEntry = WeightedRandom.Select(settings.tiers.Select(t => new WeightedTier(t)));
        if (tierEntry.tier == null) return null;

        // Select box from tier
        var boxEntry = WeightedRandom.Select(tierEntry.tier.boxes.Select(b => new WeightedBox(b)));
        if (boxEntry.prefab == null) return null;

        // Instantiate box
        return Instantiate(boxEntry.prefab, transform.position, transform.rotation);
    }

    // Helper structs for weighted selection
    private struct WeightedTier : WeightedRandom.IWeighted
    {
        public BoxTierSO tier;
        public int Weight { get; }

        public WeightedTier(BoxSpawnSettings.TierEntry entry)
        {
            tier = entry.tier;
            Weight = entry.weight;
        }
    }

    private struct WeightedBox : WeightedRandom.IWeighted
    {
        public Box prefab;
        public int Weight { get; }

        public WeightedBox(BoxTierSO.BoxEntry entry)
        {
            prefab = entry.prefab;
            Weight = entry.weight;
        }
    }
}