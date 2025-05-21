using UnityEngine;

[CreateAssetMenu(menuName = "Dungeon/Room Configuration")]
public class RoomConfiguration : WeightedConfig
{
    public string roomName;
    public RoomType roomType;
    public GameObject roomPrefab;
    public int minTier = 0;
    public int maxTier = 99;
    public float priceMultiplier = 1;
    public int profitLevel = 0;
    public int dangerLevel = 0;
    public int quality = 0;

    public bool hasSellMachine = false;
    // Inherits weight from WeightedConfig
}