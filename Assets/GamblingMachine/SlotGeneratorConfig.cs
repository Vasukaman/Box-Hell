// SlotGeneratorConfig.cs
using UnityEngine;

[System.Serializable]
public class SlotGeneratorConfig
{
    [Tooltip("Which LootGroup to draw items from")]
    public LootGroup lootGroup;

    [Min(1)]
    [Tooltip("How many UI slots in the machine are of this type")]
    public int slotCount = 1;

    [Tooltip("Relative weight of these slots when doing the final pick")]
    public float slotDropWeight = 1f;
}
