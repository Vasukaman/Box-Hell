// InventorySystem.cs
using UnityEngine;
using System;



public class InventorySystem : MonoBehaviour
{
    public event Action OnInventoryUpdated;
    public event Action<int> OnSelectedSlotChanged;

    [SerializeField] private int slotCount = 4;
    [SerializeField] private ItemCore[] items;
    private int selectedSlot = -1;

    public int SlotCount => slotCount;
    public int SelectedSlot => selectedSlot;
    public ItemCore[] Items => items;

    [SerializeField] private Transform itemHolder;



    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
        items = new ItemCore[slotCount];


       

    }



    public bool AddItem(ItemCore item)
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] == null)
            {
                items[i] = item;
                item.transform.SetParent(itemHolder);
                item.transform.localPosition = Vector3.zero;
                item.MakeItTool();
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
        if (GetSelectedItem() != null)
        {
            ItemCore currentItem = GetSelectedItem();
            currentItem.transform.SetParent(null);
            currentItem.MakeItWorldItem();
            items[selectedSlot] = null;
        }
    }

    public void SelectSlot(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= items.Length) return;

        GetSelectedItem()?.gameObject.SetActive(false);

        Debug.Log($"Slot changed");


        selectedSlot = slotIndex;


    

        ItemCore selectedItem = items[selectedSlot];
        if (GetSelectedItem() != null)
        {


            GetSelectedItem().gameObject.SetActive(true);
        }


        OnSelectedSlotChanged?.Invoke(slotIndex);
    }



    public ItemCore GetSelectedItem()
    {
        if (selectedSlot >= 0 && selectedSlot < items.Length)
        {
            Debug.Log(items[selectedSlot]); 
            return items[selectedSlot];
        }
        return null;
    }



    public bool TryRemoveSelectedItem(out ItemCore removedItem)
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
 
     

        if (TryRemoveSelectedItem(out ItemCore thrownItem))
        {
            Debug.Log(thrownItem);
            thrownItem.HandleItemThrowed();
            thrownItem.worldItem.Throw();
        }
    }

    public void UseCurrentTool()
    {
        GetSelectedItem()?.tool?.Use();
    }

}