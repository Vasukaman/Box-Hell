using UnityEngine;

[CreateAssetMenu(menuName = "Dungeon/Corridor Configuration")]
public class CorridorConfiguration : WeightedConfig
{
    public string corridorName;
    public GameObject corridorPrefab;
    // Inherits weight from WeightedConfig
}