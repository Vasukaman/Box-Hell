// InventorySystem.cs
using UnityEngine;
using System;



public class InventorySystem : MonoBehaviour
{
    public event Action OnInventoryUpdated;
    public event Action<int> OnSelectedSlotChanged;

    [SerializeField] private int slotCount = 4;
    [SerializeField] private Item[] items;
    private int selectedSlot = -1;

    public int SlotCount => slotCount;
    public int SelectedSlot => selectedSlot;
    public Item[] Items => items;

    [SerializeField] private Transform itemHolder;

    private Tool currentTool;

    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
        items = new Item[slotCount];


       

    }



    public bool AddItem(Item item)
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] == null)
            {
                items[i] = item;
                OnInventoryUpdated?.Invoke();
                SelectSlot(i);
                return true;
            }
        }
        return false;
    }

    public void RemoveItem(int index)
    {
        if (index < 0 || index >= items.Length) return;
        items[index] = null;
        OnInventoryUpdated?.Invoke();
    }

    private void ClearCurrentTool()
    {
        if (currentTool != null)
        {
            Destroy(currentTool.gameObject);
            currentTool = null;
        }
    }

    public void SelectSlot(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= items.Length) return;

        selectedSlot = slotIndex;


        Debug.Log($"Slot changed");
        ClearCurrentTool();

        Tool selectedTool = GetSelectedItem().linkedTool;
        if (GetSelectedItem() != null && selectedTool != null)
        {
            currentTool = Instantiate(selectedTool, itemHolder);
            currentTool.transform.localPosition = Vector3.zero;
            currentTool.transform.localRotation = Quaternion.identity;
        }


        OnSelectedSlotChanged?.Invoke(slotIndex);
    }



    public Item GetSelectedItem()
    {
        if (selectedSlot >= 0 && selectedSlot < items.Length)
        {
            Debug.Log(items[selectedSlot]);
            return items[selectedSlot];
        }
        return null;
    }



    public bool TryRemoveSelectedItem(out Item removedItem)
    {
        removedItem = null;
        if (selectedSlot < 0 || selectedSlot >= items.Length) return false;
        if (items[selectedSlot] == null) return false;

        removedItem = items[selectedSlot];

        ClearCurrentTool();

        items[selectedSlot] = null;


        OnInventoryUpdated?.Invoke();
        return true;
    }

    public void ThrowItem()
    {
 
     

        if (TryRemoveSelectedItem(out Item thrownItem))
        {
            Debug.Log(thrownItem);
            Vector3 throwDirection = mainCamera.transform.forward;

         WorldItemFactory.CreateWorldItem(
         thrownItem,
         itemHolder.position,
         Quaternion.LookRotation(throwDirection),
         true
     );
        }
    }

    public void UseCurrentTool()
    {
        currentTool.Use();
    }

}