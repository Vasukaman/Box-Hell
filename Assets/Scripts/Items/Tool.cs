// Tool.cs
using UnityEngine;
using System;
public abstract class Tool : MonoBehaviour
{
    public event Action OnToolUsed;
    public event Action OnToolStopped;
    public event Action OnToolThrown;
    

    [SerializeField] protected Item item;
    [SerializeField] protected ItemCore itemCore;

    protected void Start()
    {
        itemCore.OnItemThrowed += ToolThrown;
    }
    public virtual void Use()
    {

        OnToolUsed?.Invoke();
       
    }

    public virtual void StopUsing()
    {
        OnToolStopped?.Invoke();
    }
    
    public virtual void ToolThrown()
    {
        OnToolThrown?.Invoke();
    }


    protected virtual void Update() { }
}