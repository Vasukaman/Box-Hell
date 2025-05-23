using UnityEngine;
using System.Collections.Generic;

public class LootManager : MonoBehaviour
{
    public static LootManager Instance { get; private set; }

    [SerializeField] private GameData _gameData;
    [SerializeField] private  WorldItemHolder _itemHolderPrefab;

    public static GameData GameData => Instance._gameData;
    public static WorldItemHolder itemholderPrefab => Instance._itemHolderPrefab;

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


    // Modified main loot generation method with holder option
    public static Item GenerateBoxLoot(LootConfiguration config, int tier, float luck, Vector3 position, bool spawnWithHolder = true)
    {
        if (config == null) return null;

        Item selectedItem = SelectLootItem(config, luck);
        if (selectedItem != null)
        {
            SpawnItem(selectedItem, position, spawnWithHolder);
        }
        return selectedItem;
    }

    // New method that just selects item without spawning
    public static Item SelectLootItem(LootConfiguration config, float luck)
    {
        if (config == null) return null;

        var selectedPack = SelectFromEntries(config.lootPacks, config.noDropScore, luck);
        if (selectedPack == null) return null;

        var selectedGroup = SelectFromEntries(selectedPack.lootGroups, selectedPack.noDropScore, luck);
        if (selectedGroup == null) return null;

        return SelectFromItems(selectedGroup.items, selectedGroup.noDropScore, luck);
    }

    // Modified spawn method with holder toggle
    static void SpawnItem(Item item, Vector3 position, bool useHolder = true)
    {
        if (item?.itemCore == null) return;

        ItemCore itemCore = Instantiate(item.itemCore, position, Quaternion.identity);

        if (useHolder && itemholderPrefab != null)
        {
            WorldItemHolder holder = Instantiate(itemholderPrefab, position, Quaternion.identity);
            holder.Initialize(itemCore);
        }

        Debug.Log(item + " spawned");
    }

    // New direct spawn method without holder
    public static void SpawnItemWithoutHolder(Item item, Vector3 position)
    {
        if (item?.itemCore == null) return;

        Instantiate(item.itemCore, position, Quaternion.identity);
        Debug.Log(item + " spawned without holder");
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
        if (item?.itemCore == null) return;

        ItemCore itemCore = Instantiate(item.itemCore, position, Quaternion.identity);
        if (itemholderPrefab!=null)
            {
               
                WorldItemHolder holder = Instantiate(itemholderPrefab, position, Quaternion.identity);
                holder.Initialize(itemCore);
            }

        Debug.Log(item+" spawned");

     
        }

        static void AddRandomCoins(Vector3 position, int ammount)
    {
        GameData.playerCore.ModifyCoins(ammount);   
    }
}