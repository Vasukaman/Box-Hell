using UnityEngine;
using System.Collections.Generic;

public class LootManager : MonoBehaviour
{
    public static LootManager Instance { get; private set; }

    [SerializeField] private GameData _gameData;

    public static GameData GameData => Instance._gameData;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
  
    public static void GenerateBoxLoot(LootConfiguration config, int tier, float luck, Vector3 position)
    {
        if (config == null) return;

        int coinAmmount = Random.Range(config.minCoins, config.maxCoins);

        AddRandomCoins(position, coinAmmount);

        // Roll for loot pack
        var selectedPack = SelectFromEntries(config.lootPacks, config.noDropScore, luck);
        if (selectedPack == null) return;

        // Roll for loot group
        var selectedGroup = SelectFromEntries(selectedPack.lootGroups, selectedPack.noDropScore, luck);
        if (selectedGroup == null) return;

        // Roll for final item
        var selectedItem = SelectFromItems(selectedGroup.items, selectedGroup.noDropScore, luck);
        if (selectedItem == null) return;

        SpawnItem(selectedItem, position);
    }

    static T SelectFromEntries<T>(DropScoreEntry<T>[] entries, int noDropScore, float luck) where T : UnityEngine.Object
    {
        int totalScore = CalculateTotalScore(entries, noDropScore);
        if (totalScore <= 0) return null;

        int roll = Random.Range(0, totalScore) + Mathf.FloorToInt(luck);
        int current = 0;

        foreach (var entry in entries)
        {
            current += entry.dropScore;
            if (roll < current) return entry.value;
        }

        return null; // No drop
    }

    static Item SelectFromItems(DropScoreItem[] items, int noDropScore, float luck)
    {
        int totalScore = CalculateTotalScore(items, noDropScore);
        if (totalScore <= 0) return null;

        int roll = Random.Range(0, totalScore) + Mathf.FloorToInt(luck);
        int current = 0;

        foreach (var item in items)
        {
            current += item.dropScore;
            if (roll < current) return item.item;
        }

        return null; // No drop
    }

    static int CalculateTotalScore<T>(DropScoreEntry<T>[] entries, int noDropScore) where T : UnityEngine.Object
    {
        int total = noDropScore;
        foreach (var entry in entries) total += entry.dropScore;
        return total;
    }

    static int CalculateTotalScore(DropScoreItem[] items, int noDropScore)
    {
        int total = noDropScore;
        foreach (var item in items) total += item.dropScore;
        return total;
    }

    static void SpawnItem(Item item, Vector3 position)
    {
        if (item?.itemCore != null)
            Instantiate(item.itemCore.gameObject, position, Quaternion.identity);
    }

    static void AddRandomCoins(Vector3 position, int ammount)
    {
        GameData.playerCore.ModifyCoins(ammount);   
    }
}