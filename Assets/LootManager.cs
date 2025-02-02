using UnityEngine;
using System.Collections.Generic;

public class LootManager : MonoBehaviour
{
    public static LootManager Instance;
    public LootTable regularBoxLootTable;
    public float luckMultiplier = 0.5f;

    void Awake() => Instance = this;

    public static void GenerateBoxLoot(BoxType boxType, int tier, float luck, Vector3 position)
    {
        Debug.LogError("Generating loot");
        if (Instance == null)
        {
            Debug.LogError("LootManager instance not found!");
            return;
        }

        // Get appropriate loot table
        var lootTable = boxType switch
        {
            BoxType.Regular => Instance.regularBoxLootTable,
            _ => Instance.regularBoxLootTable
        };

        // Calculate drops
        var drops = CalculateDrops(lootTable, tier, luck);

        // Process drops
        foreach (var item in drops)
        {
            if (item.type == ItemType.Points)
            {
                // Empty for now - add later
            }
            else if (item.worldItemPrefab != null)
            {
                Instantiate(item.worldItemPrefab, position, Quaternion.identity);
            }
        }

        // Add Box Coins (implementation left empty)
        AddBoxCoins(tier);
    }

    static List<Item> CalculateDrops(LootTable table, int tier, float luck)
    {
        List<Item> drops = new List<Item>();

        foreach (var entry in table.lootEntries)
        {
            if (tier < entry.minTier || tier > entry.maxTier) continue;

            float effectiveChance = Mathf.Clamp(
                entry.baseDropChance + (luck * Instance.luckMultiplier),
                0f, 100f
            );

            if (Random.Range(0f, 100f) <= effectiveChance)
                drops.Add(entry.item);
        }

        return drops;
    }

    static void AddBoxCoins(int tier) { /* Empty */ }
}