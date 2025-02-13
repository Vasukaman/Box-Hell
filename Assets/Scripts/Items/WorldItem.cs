// Modified WorldItem.cs
using UnityEngine;
using System;

public class WorldItem : MonoBehaviour
{
    [SerializeField] private float throwForce = 5f;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Collider col;

    [SerializeField] private Item item;

    public Item ContainedItem => item;

    public void Throw()
    {
      
     
            rb.AddForce(Camera.main.transform.forward * throwForce, ForceMode.Impulse);
        
    }

    public void DisableWorldItemExtra()
    {
        rb.isKinematic = true;
    }   
    
    public void EnableWorldItemExtra()
    {
        rb.isKinematic = false;
    }


    // Optional: Add visual effects or custom behavior here
}