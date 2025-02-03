using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Boxes/Spawn Settings")]
public class BoxSpawnSettings : ScriptableObject
{
    [System.Serializable]
    public struct TierEntry
    {
        public BoxTierSO tier;
        public int weight;
    }

    public TierEntry[] tiers;
}