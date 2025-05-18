using UnityEngine;

[CreateAssetMenu(menuName = "Dungeon/Room Configuration")]
public class RoomConfiguration : WeightedConfig
{
    public string roomName;
    public RoomType roomType;
    public GameObject roomPrefab;
    public int minTier = 0;
    public int maxTier = 99;


    public bool hasSellMachine = false;
    // Inherits weight from WeightedConfig
}