// IDamageable.cs
using UnityEngine;
using System;

[Serializable]
public struct HitData
{
    public int damage;
    public ItemCore sourseItem;
    public Vector3 position;
    public Vector3 hitForce;
    public DecalTextureData decalData;

    public HitData(int _damage, ItemCore _sourseItem, Vector3 _position, Vector3 _hitforce, DecalTextureData _decalData)
    {
        damage = _damage;
        sourseItem = _sourseItem;
        position = _position;
        hitForce = _hitforce;
        decalData = _decalData;

    }
}

public interface IHittable //This is not good, there are too much stuff in IDAmagable!!!! //TODO
{
    void TakeHit(HitData hitData);

  
}