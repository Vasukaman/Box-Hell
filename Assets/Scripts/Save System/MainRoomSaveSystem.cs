using UnityEngine;
using CI.QuickSave;
using System.Collections.Generic;
public struct SentItemData
{
    public string itemName;
    public int durability;
    public int currentPrice;
}

public class MainRoomSaveSystem : MonoBehaviour
{
    [SerializeField] private Transform itemsParent; // Parent for respawned items
    [SerializeField] private ItemDatabase itemDatabase;
    
    public struct ItemSaveData
    {
        public string itemName;
        public Vector3 position;
        public Quaternion rotation;
        public int durability;
        public int currentPrice;
        public bool isActive;
    }

    private void Awake()
    {
        LoadRoomState();
    }



    private void OnDestroy()
    {
       // SaveRoomState();
    }

    public void SaveRoomState()
    {
        // Get all item cores first
        var allItems = new List<ItemCore>(FindObjectsOfType<ItemCore>(true));

        // Add missing items to loot group
        itemDatabase.AddMissingToLootGroup(allItems);

        // Rest of save logic...
        var itemsToSave = new List<ItemSaveData>();

        foreach (var itemCore in allItems)
        {
            // Skip items in Tool state
            if (itemCore.currentState == ItemState.Tool || itemCore.currentState == ItemState.InSpawnHolder) continue;

            itemsToSave.Add(new ItemSaveData
            {
                itemName = itemCore.item.itemName,
                position = itemCore.transform.position,
                rotation = itemCore.transform.rotation,
                durability = itemCore.tool.currentDurability,
                currentPrice = itemCore.price,
                isActive = itemCore.gameObject.activeSelf // Save active state
            });
        }

        QuickSaveWriter.Create("MainRoom")
            .Write("Items", itemsToSave)
            .Commit();
    }

    public void LoadRoomState()
    {
      //  ClearAllItems();

        if (QuickSaveReader.RootExists("MainRoom"))
        {
            var reader = QuickSaveReader.Create("MainRoom");

            if (reader.Exists("Items"))
            {
                var savedItems = reader.Read<List<ItemSaveData>>("Items");
                SpawnItems(savedItems);
            }
        }
    }

    private void SpawnItems(List<ItemSaveData> items)
    {
        foreach (var itemData in items)
        {
            var itemPrefab = itemDatabase.GetItemByName(itemData.itemName);
            if (itemPrefab == null) continue;

            var newItem = Instantiate(itemPrefab.itemCore, itemsParent);

            // Set world position/rotation
            newItem.transform.SetPositionAndRotation(
                itemData.position,
                itemData.rotation
            );

            // Restore state
            newItem.price = itemData.currentPrice;
            newItem.tool.currentDurability = itemData.durability;
            newItem.gameObject.SetActive(itemData.isActive);

            // Ensure correct initial state
            newItem.MakeItWorldItem();
        }
    }

    private void ClearAllItems()
    {
        // Destroy only non-tool items
        foreach (var itemCore in FindObjectsOfType<ItemCore>())
        {
            if (itemCore.currentState != ItemState.Tool)
            {
                Destroy(itemCore.gameObject);
            }
        }
    }

    public void DeleteSaveData()
    {
        QuickSaveWriter.DeleteRoot("MainRoom");
    }




    // 2) Call this to enqueue a sent item
    public void AddSentItem(ItemCore item)
    {
        var writer = QuickSaveWriter.Create("MainRoom");
        var reader = QuickSaveReader.Create("MainRoom");

        List<SentItemData> list = reader.Exists("SentItems")
            ? reader.Read<List<SentItemData>>("SentItems")
            : new List<SentItemData>();

        list.Add(new SentItemData
        {
            itemName = item.item.itemName,
            durability = item.tool.currentDurability,
            currentPrice = item.price
        });

        writer.Write("SentItems", list).Commit();
    }

    // 3) Pop the next queued item (returns null if none)
    public SentItemData? PopNextSentItem()
    {
        var writer = QuickSaveWriter.Create("MainRoom");
        var reader = QuickSaveReader.Create("MainRoom");

        if (!reader.Exists("SentItems")) return null;
        var list = reader.Read<List<SentItemData>>("SentItems");
        if (list.Count == 0) return null;

        var next = list[0];
        list.RemoveAt(0);
        writer.Write("SentItems", list).Commit();
        return next;
    }
}