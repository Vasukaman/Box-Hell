using UnityEngine;

[CreateAssetMenu(menuName = "Loot/Item Rarity")]
public class ItemRarityConfiguration : ScriptableObject
{
    public string name;
    public int rank;
    public ParticleSystem dropEffect;
    public ParticleSystem holdEffect;

}