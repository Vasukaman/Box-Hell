using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ItemDatabase : MonoBehaviour
{
    [SerializeField] private LootGroup lootGroup;
    [SerializeField] private int defaultDropScore = 10;
    
    private Dictionary<string, Item> itemMap = new Dictionary<string, Item>();

    private void Awake()
    {
        InitializeItemMap();
     //   ValidateLootGroup();
    }

    private void InitializeItemMap()
    {
        // Initialize from loot group
        foreach (var dropItem in lootGroup.items)
        {
            if (dropItem.item != null && !itemMap.ContainsKey(dropItem.item.itemName))
            {
                itemMap.Add(dropItem.item.itemName, dropItem.item);
            }
        }
    }

    public Item GetItemByName(string name)
    {
        if (itemMap.TryGetValue(name, out Item item))
        {
            return item;
        }
        
        // Optional: Search all assets if not found in loot group
        return SearchAllItems(name);
    }

    private Item SearchAllItems(string name)
    {
        // Fallback search logic
        foreach (var dropItem in lootGroup.items)
        {
            if (dropItem.item.itemName == name)
            {
                return dropItem.item;
            }
        }
        return null;
    }

    // Call this before saving to ensure all items are in loot config
    public void AddMissingToLootGroup(List<ItemCore> sceneItems)
    {
        #if UNITY_EDITOR
        bool modified = false;
        var itemsList = new List<DropScoreItem>(lootGroup.items);

        foreach (var itemCore in sceneItems)
        {
            if (itemCore.item == null) continue;

            bool exists = false;
            foreach (var dropItem in itemsList)
            {
                if (dropItem.item == itemCore.item)
                {
                    exists = true;
                    break;
                }
            }

            if (!exists)
            {
                itemsList.Add(new DropScoreItem
                {
                    item = itemCore.item,
                    dropScore = defaultDropScore
                });
                modified = true;
            }
        }

        if (modified)
        {
            lootGroup.items = itemsList.ToArray();
            EditorUtility.SetDirty(lootGroup);
            AssetDatabase.SaveAssets();
        }
        #endif
    }
}