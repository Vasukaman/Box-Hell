using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Boxes/Box Tier")]
public class BoxTierSO : ScriptableObject
{
    [System.Serializable]
    public struct BoxEntry
    {
        public Box prefab;
        public int weight;
    }

    public BoxEntry[] boxes;
}

