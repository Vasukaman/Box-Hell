using UnityEngine;

[CreateAssetMenu(menuName = "Loot/Loot Configuration")]
public class LootConfiguration : ScriptableObject
{
    public DropScoreEntry<LootPack>[] lootPacks;
    public int noDropScore = 50;

    public int minCoins = 1;
    public int maxCoins = 5;
}