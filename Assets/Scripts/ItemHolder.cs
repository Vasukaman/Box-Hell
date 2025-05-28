// ItemHolder.cs
using UnityEngine;
using UnityEngine.Events;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class ItemHolder : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Where the item will be parented/held.")]
    public Transform itemHoldPoint;

    [Header("Float Animation")]
    public float floatAmplitude = 0.2f;
    public float floatFrequency = 1f;

    // Optional: if your items have rarity particle effects
    private ParticleSystem activeParticles;
    private Vector3 originalLocalPos;
    private float floatTimer = 0f;

    private ItemCore heldItem;

    /// <summary>
    /// Fired right after the item is removed (pulled out) of the holder.
    /// You can hook into this to play sounds, UI, etc.
    /// </summary>
    public UnityEvent<ItemCore> OnItemRemoved = new UnityEvent<ItemCore>();

    /// <summary>
    /// Returns true if no item is currently in the holder.
    /// </summary>
    public bool IsEmpty() => heldItem == null;

    /// <summary>
    /// Puts the given item into the holder.  Assumes you've already removed it
    /// from the inventory (e.g. via TryRemoveSelectedItem).
    /// </summary>
    public void AddItem(ItemCore item)
    {
        if (!IsEmpty() || item == null) return;
        heldItem = item;

        // parent & position
        heldItem.transform.SetParent(itemHoldPoint, true);
        heldItem.transform.localPosition = Vector3.zero;
        originalLocalPos = Vector3.zero;

        // disable physics
        var rb = heldItem.GetComponent<Rigidbody>();
        if (rb) rb.isKinematic = true;

        // custom hook on your ItemCore to switch state
        heldItem.MakeItHoldedByHolder();

        // optional sparkle
    //    var effect = heldItem.item.raritySO.holdEffect;
     //   if (effect != null)
      //      activeParticles = Instantiate(effect, itemHoldPoint.position, itemHoldPoint.rotation, transform);
    }

    /// <summary>
    /// Removes the held item, enables its physics, and fires OnItemRemoved.
    /// </summary>
    public ItemCore RemoveItem()
    {
        if (IsEmpty()) return null;

        // unparent
        heldItem.transform.SetParent(null, true);

        // restore physics
        var rb = heldItem.GetComponent<Rigidbody>();
        if (rb) rb.isKinematic = false;

        // custom hook to go back to world item
        heldItem.MakeItWorldItem();

        // cleanup
        if (activeParticles) Destroy(activeParticles.gameObject);
        var itemToReturn = heldItem;
        heldItem = null;

        OnItemRemoved.Invoke(itemToReturn);
        return itemToReturn;
    }

    private void Update()
    {
        if (heldItem != null)
        {
            floatTimer += Time.deltaTime;
            float y = Mathf.Sin(floatTimer * 2 * Mathf.PI * floatFrequency) * floatAmplitude;
            heldItem.transform.localPosition = originalLocalPos + Vector3.up * y;
        }
    }
}
