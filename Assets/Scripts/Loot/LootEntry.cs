using UnityEngine;

[System.Serializable]
public class LootEntry
{
    public Item item;
    [Range(0, 100)] public float baseDropChance;
    public int minTier = 0;
    public int maxTier = int.MaxValue;
}