// Modified Item.cs
using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    public ItemType type;


    public string itemName;
    public Sprite icon;
    public Tool linkedTool;
    public WorldItem worldItemPrefab; // New reference
}