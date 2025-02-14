using UnityEngine;
using System;

public abstract class Tool : MonoBehaviour
{
    public event Action OnToolUsed;
    public event Action OnDurabilityChanged;
    public event Action OnToolStopped;
    public event Action OnToolThrown;
    public event Action<ItemCore> OnToolBreak; // New event for when tool breaks

    [Header("Durability Settings")]
    [SerializeField] protected int maxDurability = 100;
    [SerializeField] protected int durabilityLossPerUse = 1;
    [SerializeField] protected int throwDurabilityCost = 0;

    [Header("References")]
    [SerializeField] protected Item item;
    [SerializeField] protected ItemCore itemCore;

    [SerializeField] public int currentDurability;
    private bool isBroken = false;
    public float DurabilityPercentage => currentDurability / maxDurability;

    protected virtual void Awake()
    {
        itemCore.OnItemThrowed += ToolThrown;
        SetDurability(maxDurability); // Initialize durability
    }

    public virtual void Use()
    {
        if (currentDurability <= 0)
        {
            BreakTool();
            return;
        }

        // Handle durability loss


        OnToolUsed?.Invoke();
    }

    public virtual void StopUsing()
    {
        OnToolStopped?.Invoke();
    }

    public virtual void ToolThrown()
    {
        if (!isBroken)
        {
            ReduceDurability(throwDurabilityCost);
        }
        OnToolThrown?.Invoke();
    }

    protected virtual void ReduceDurability(int amount)
    {
        if (isBroken) return;

        SetDurability(Mathf.Max(0, currentDurability - amount));

    
    }
    protected virtual void ReduceDurabilityUse()
    {
        ReduceDurability(durabilityLossPerUse);
    }


    protected virtual void BreakTool()
    {
        isBroken = true;
        OnToolBreak?.Invoke(itemCore);

        // Automatically stop using when broken
        StopUsing();
        Destroy(itemCore.gameObject);
        // Add any visual/audio effects here
        Debug.Log($"{gameObject.name} broke!");
    }

    public void RepairTool(int repairAmount)
    {
        SetDurability(Mathf.Min(maxDurability, currentDurability + repairAmount));
        isBroken = false;
    }

    public void FullRepair()
    {
        SetDurability(maxDurability);
        isBroken = false;
    }

    private void SetDurability(int newDurability)
    {
        if (maxDurability == -1) currentDurability = 100;
        currentDurability = newDurability;
        OnDurabilityChanged?.Invoke();
    }
    protected virtual void Update() { }
}