// SenderMachine.cs
using UnityEngine;
using CI.QuickSave;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif


[RequireComponent(typeof(Collider), typeof(Animator))]
public class SenderMachine : MonoBehaviour
{
    [Tooltip("Trigger parameter name on your Animator")]
    public string sendTriggerName = "Send";

    private Animator anim;

    private const string ROOT = "MainRoom";
    private const string KEY = "SentItems";
    [SerializeField] private LootGroup lootGroup;
    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        var item = other.GetComponentInParent<ItemCore>();
        if (item == null || item.currentState == ItemState.Tool) return;
      
        
        AddMissingToLootGroup(item);

        // play send animation
        anim.SetTrigger(sendTriggerName);

        // build our SentItemData
        var data = new SentItemData
        {
            itemName = item.item.itemName,
            durability = item.tool.currentDurability,
            currentPrice = item.price
        };

        // load existing list
        var reader = QuickSaveReader.Create(ROOT);
        var list = reader.Exists(KEY)
            ? reader.Read<List<SentItemData>>(KEY)
            : new List<SentItemData>();



        // enqueue & save
        list.Add(data);
        QuickSaveWriter.Create(ROOT)
            .Write(KEY, list)
            .Commit();

    

        Destroy(item.gameObject);
    }

    public void AddMissingToLootGroup(ItemCore itemCore)
    {
#if UNITY_EDITOR
        bool modified = false;
        var itemsList = new List<DropScoreItem>(lootGroup.items);

      
            if (itemCore.item == null) return;

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
                    dropScore = 10
                });
                modified = true;
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
