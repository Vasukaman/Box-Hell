// IDamageable.cs
using UnityEngine;
using System;

public interface IDamageable
{
    void TakeDamage(float damage, Item damageSource);
    event Action<Item> OnBreak;
}