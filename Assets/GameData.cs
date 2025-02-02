using UnityEngine;

[CreateAssetMenu(menuName = "Game/Game Data")]
public class GameData : ScriptableObject
{
    [Header("Player References")]
    public PlayerCore playerCore;
    public InventorySystem inventorySystem;

    [Header("Systems")]
    public LootManager lootManager;

}