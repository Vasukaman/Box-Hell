using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public struct ExplosionData
{
    public float radius;
    public float baseDamage;
    public float force;
    public float forceUpwardsModifier;
    public LayerMask affectedLayers;
    public AnimationCurve damageFalloff;
    public bool applyForce;
    public bool useDamageFalloff;
}

