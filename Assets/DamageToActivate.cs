using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

public class DamageToActivate : MonoBehaviour, IDamageable
{
    [SerializeField] UnityEvent OnActivate;
    public void TakeDamage(float damage, Item damageSource)
    {
        OnActivate.Invoke();
    }
    public void TakePush(Vector3 position, Vector3 force)
    {

    }


     public event Action<Item> OnBreak;
}
