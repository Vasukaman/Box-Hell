using UnityEngine;

[CreateAssetMenu(menuName = "Loot/Loot Pack")]
public class LootPack : ScriptableObject
{
    public DropScoreEntry<LootGroup>[] lootGroups;
    public int noDropScore = 30;
}