// DamageableBase.cs
using UnityEngine;
using System;
public abstract class DamageableBase : MonoBehaviour, IDamageable
{
    [SerializeField] protected float maxHealth = 100f;
    [SerializeField] protected bool destroyOnBreak = true;

    public event Action<Item> OnBreak;

    protected float currentHealth;
    protected Item lastDamageSource;

    protected virtual void Awake()
    {
        currentHealth = maxHealth;
    }

    public virtual void TakeDamage(float damage, Item damageSource)
    {
        currentHealth = Mathf.Max(currentHealth - damage, 0);
        lastDamageSource = damageSource;

        if (currentHealth <= 0)
        {
            Break();
        }
    }


    public virtual void TakePush(Vector3 position, Vector3 force)
    {


    }

    protected virtual void Break()
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