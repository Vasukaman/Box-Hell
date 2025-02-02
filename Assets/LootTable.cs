using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Loot/Loot Table")]
public class LootTable : ScriptableObject
{
    public List<LootEntry> lootEntries;
}