using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DestractableObject : MonoBehaviour, IHittable, IDamageableByExplosion
{
    [SerializeField] private int _health;
    [SerializeField] private ParticleSystem _breakEffectPrefab;
    [SerializeField] public UnityEvent OnTakeDamage; 
    [SerializeField] public UnityEvent OnTakeHit; 
    [SerializeField] public UnityEvent OnBreak;
    [SerializeField] private bool _takeDamageByHit = true;
    [SerializeField] private bool _takeDamageByExplosion = true;
    public void TakeHit(HitData hitData)
    {
        OnTakeHit.Invoke();
        if (_takeDamageByHit)
        TakeDamage(hitData.damage);
    }

    private void Break()
    {
        Destroy(this.gameObject);
    }

    private void TakeDamage(int damage)
    {
        OnTakeDamage.Invoke();

        _health -= damage;

        if (_health <= 0)
            Break();
    }

    public void TakeExplosionDamage(ExplosionData explosionData, Vector3 explosionOrigin)
    {
        if (_takeDamageByExplosion)
        TakeDamage((int)explosionData.baseDamage);
    }


}
