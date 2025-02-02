using UnityEngine;

[CreateAssetMenu(menuName = "Loot/Loot Group")]
public class LootGroup : ScriptableObject
{
    public DropScoreItem[] items;
    public int noDropScore = 20;
}