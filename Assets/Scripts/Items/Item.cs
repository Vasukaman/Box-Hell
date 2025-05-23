// Modified Item.cs
using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    // Add this
    [Header("UI Settings")]
    public Sprite icon;
    public string itemName;
    public string description;
    public int price;
    public Vector2Int priceRange;



    // Rest of your existing fields
    [SerializeField] private ItemType type;
    [SerializeField] public ItemCore itemCore;
    [SerializeField] public ItemRarityConfiguration raritySO;
}