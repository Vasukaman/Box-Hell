// Modified WorldItem.cs
using UnityEngine;

public class WorldItem : MonoBehaviour
{
    [SerializeField] private float throwForce = 5f;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Collider col;

    [SerializeField] private Item item;

    public Item ContainedItem => item;

    public void Initialize(Item newItem, bool applyThrowForce = false)
    {
        item = newItem;
        gameObject.name = $"{item.itemName}_World";

        if (applyThrowForce && rb != null)
        {
            rb.AddForce(transform.forward * throwForce, ForceMode.Impulse);
        }
    }

    // Optional: Add visual effects or custom behavior here
}