// Tool.cs
using UnityEngine;
using System;
public abstract class Tool : MonoBehaviour
{
    public event Action OnToolUsed;
    public event Action OnToolStopped;
    

    [SerializeField] protected Item item;

    public virtual void Use()
    {

        OnToolUsed?.Invoke();
       
    }

    public virtual void StopUsing()
    {
        OnToolStopped?.Invoke();
    }

    protected virtual void Update() { }
}