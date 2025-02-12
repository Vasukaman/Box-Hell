using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

public class DamageToActivate : MonoBehaviour, IHittable
{
    [SerializeField] UnityEvent OnActivate;
    public event Action<PlayerCore> OnActivateEvent;
    public virtual void TakeHit(HitData hitData)
    {
        TakeDamage(hitData.damage, hitData.sourseItem, hitData.position);
        TakePush(hitData.position, hitData.hitForce);
    }
    public void TakeDamage(float damage, ItemCore damageSource, Vector3 hitPoint)
    {
        OnActivate.Invoke();
        OnActivateEvent?.Invoke(damageSource.owner);
    }
    public void TakePush(Vector3 position, Vector3 force)
    {

    }


     public event Action<Item> OnBreak;
}
