// IDamageable.cs
using UnityEngine;
using System;

public interface IDamageable //This is not good, there are too much stuff in IDAmagable!!!! //TODO
{
    void TakeDamage(float damage, Item damageSource, Vector3 hitPoint);
    void TakePush(Vector3 position, Vector3 force);
    event Action<Item> OnBreak;
}