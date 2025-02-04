// Modified Item.cs
using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    // Add this
    [Header("UI Settings")]
    public Sprite icon;
    public string itemName;

    // Rest of your existing fields
    [SerializeField] private ItemType type;
    [SerializeField] public ItemCore itemCore;
}