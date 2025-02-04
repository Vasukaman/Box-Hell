using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private Transform slotsContainer;
    [SerializeField] private float slotSpacing = 10f;

    private InventorySystem inventory;
    private List<InventorySlotUI> slots = new List<InventorySlotUI>();
    private HorizontalLayoutGroup layoutGroup;

    void Awake()
    {
        layoutGroup = slotsContainer.GetComponent<HorizontalLayoutGroup>();
        layoutGroup.spacing = slotSpacing;
        // Very bad, //TODO FIX
        inventory = FindObjectOfType<InventorySystem>();
    
    }

    private void Start()
    {
        Initialize(inventory);
    }
    public void Initialize(InventorySystem inventorySystem)
    {
        inventory = inventorySystem;
        CreateSlots();
        UpdateAllSlots();

        inventory.OnInventoryUpdated += UpdateAllSlots;
        inventory.OnSelectedSlotChanged += OnSelectedSlotChanged;
    }

    void CreateSlots()
    {
        // Clear existing
        foreach (Transform child in slotsContainer)
            Destroy(child.gameObject);

        // Create new
        for (int i = 0; i < inventory.SlotCount; i++)
        {
            var slotObj = Instantiate(slotPrefab, slotsContainer);
            var slotUI = slotObj.GetComponent<InventorySlotUI>();
            slots.Add(slotUI);
        }
    }

    void UpdateAllSlots()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            var item = inventory.Items[i]?.item;    
            slots[i].SetItem(item);
            Debug.Log("Updated slot # " + i);
        }

    }

    void OnSelectedSlotChanged(int newSlot)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (i == newSlot) slots[i].Select();
            else slots[i].Deselect();
        }
    }

    void OnDestroy()
    {
        if (inventory != null)
        {
            inventory.OnInventoryUpdated -= UpdateAllSlots;
            inventory.OnSelectedSlotChanged -= OnSelectedSlotChanged;
        }
    }
}