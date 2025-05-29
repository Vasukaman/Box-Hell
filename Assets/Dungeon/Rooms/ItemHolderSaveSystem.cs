// (2) New script: ItemHolderSaveSystem.cs
using UnityEngine;
using CI.QuickSave;
using System.Collections.Generic;

[RequireComponent(typeof(ItemHolder))]
public class ItemHolderSaveSystem : MonoBehaviour
{
    [Tooltip("Unique ID for this holder (e.g. ChestA)")]
    [SerializeField] private string saveKey = "DefaultHolder";

    [Tooltip("Needed to spawn the held item on load")]
    [SerializeField] private ItemDatabase itemDatabase;

    private ItemHolder holder;
    private const string ROOT = "ItemHolder";
    private const string ENTRY = "HeldItem";

    private void Awake()
    {
        holder = GetComponent<ItemHolder>();

        // 1) LOAD on start
        string fullRoot = $"{ROOT}_{saveKey}";
        if (QuickSaveReader.RootExists(fullRoot))
        {
            var reader = QuickSaveReader.Create(fullRoot);
            if (reader.Exists(ENTRY))
            {
                // read struct
                var data = reader.Read<SentItemData>(ENTRY);

                // spawn prefab
                var prefab = itemDatabase.GetItemByName(data.itemName);
                if (prefab != null)
                {
                    var inst = Instantiate(prefab.itemCore, holder.itemHoldPoint);
                    inst.transform.localPosition = Vector3.zero;
                    inst.price = data.currentPrice;
                    inst.tool.currentDurability = data.durability;
                    inst.MakeItHoldedByHolder();

                    // parent & float setup
                    holder.AddItem(inst);
                }
            }
        }

        // 2) SUBSCRIBE to save on changes
        holder.OnItemAdded += SaveHeldItem;
        holder.OnItemRemoved += RemoveSavedItem;
    }

    private void OnDestroy()
    {
        holder.OnItemAdded -= SaveHeldItem;
        holder.OnItemRemoved -= RemoveSavedItem;
    }

    // called when AddItem runs
    private void SaveHeldItem(ItemCore item)
    {
        var data = new SentItemData
        {
            itemName = item.item.itemName,
            durability = item.tool.currentDurability,
            currentPrice = item.price
        };

        string fullRoot = $"{ROOT}_{saveKey}";
        QuickSaveWriter.Create(fullRoot)
            .Write(ENTRY, data)
            .Commit();
    }

    // called when RemoveItem runs
    private void RemoveSavedItem(ItemCore item)
    {
        string fullRoot = $"{ROOT}_{saveKey}";
        QuickSaveWriter.DeleteRoot(fullRoot);
    }
}
