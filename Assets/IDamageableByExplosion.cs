
using UnityEngine;
using System;


public interface IDamageableByExplosion
{
    void TakeExplosionDamage(ExplosionData explosionData, Vector3 explosionOrigin);
}
