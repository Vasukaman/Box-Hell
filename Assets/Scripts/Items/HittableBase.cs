// DamageableBase.cs
using UnityEngine;
using System;
public abstract class HittableBase : MonoBehaviour, IHittable
{
    [SerializeField] protected float maxHealth = 100f;
    [SerializeField] protected bool destroyOnBreak = true;
    [SerializeField] protected Rigidbody rigidBody;


    public event Action<ItemCore> OnBreak;

    protected float currentHealth;
    protected ItemCore lastDamageSource;

    protected virtual void Awake()
    {
        currentHealth = maxHealth;
        rigidBody = GetComponent<Rigidbody>();
    }

    public virtual void TakeHit(HitData hitData)
    {
        TakeDamage(hitData.damage, hitData.sourseItem, hitData.position);
        TakePush(hitData.position, hitData.hitForce);
    }

    public virtual void TakeDamage(float damage, ItemCore damageSource, Vector3 hitPoint)
    {
        currentHealth = Mathf.Max(currentHealth - damage, 0);
        lastDamageSource = damageSource;

        if (currentHealth <= 0)
        {
            Break(hitPoint);
        }
    }


    public virtual void TakePush(Vector3 position, Vector3 force)
    {

        if (!rigidBody) return;
        rigidBody.AddForceAtPosition(force, position, ForceMode.Impulse);

    }

    protected virtual void Break(Vector3 hitPoint)
    {
        OnBreak?.Invoke(lastDamageSource);

        if (destroyOnBreak)
        {
            Destroy(gameObject);
        }
        else
        {
            // Optionally reset state
            currentHealth = maxHealth;
        }
    }

    public void Heal(float amount) => currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
}