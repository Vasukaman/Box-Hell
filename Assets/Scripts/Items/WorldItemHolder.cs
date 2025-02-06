using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;
using System.Collections;

public class WorldItemHolder : MonoBehaviour
{
    [Header("References")]
    public Transform particleSpawnPoint;
    public Transform itemHoldPoint;

    [Header("Settings")]
    public float holdDuration = 3f;
    public float floatAmplitude = 0.2f;
    public float floatFrequency = 1f;

    private ItemCore heldItem;
    private Rigidbody itemRigidbody;
    private Vector3 originalLocalPosition;
    private ParticleSystem activeParticle;
    private float timer;


    public void Initialize(ItemCore item)
    {
        heldItem = item;

        // Parent the item to this holder
        heldItem.transform.SetParent(itemHoldPoint, true);

        // Store original position and disable physics
       
        heldItem.transform.localPosition = Vector3.zero;

        heldItem.MakeItHoldedByHolder();

        ParticleSystem holdEffect = heldItem.item.raritySO.holdEffect;

        // Create particle effect
        if (holdEffect != null && particleSpawnPoint != null)
        {
            activeParticle = Instantiate(holdEffect, particleSpawnPoint.position, particleSpawnPoint.rotation, transform);
        }

        heldItem.OnItemEquipped += ReleaseItemEquip;
        StartCoroutine(HoldItemRoutine());
    }

    private IEnumerator HoldItemRoutine()
    {
        yield return new WaitForSeconds(holdDuration);

        ReleaseItem();

    }

    private void ReleaseItem()
    {

        heldItem.transform.SetParent(null);

        heldItem.MakeItWorldItem();

        Destroy(gameObject);
    }

    private void ReleaseItemEquip()
    {
        //heldItem.transform.SetParent(null);
        heldItem.MakeItTool();

        if (gameObject)
        Destroy(gameObject);
    }

    private void Update()
    {
        if (heldItem != null)
        {
            // Calculate floating position using sine wave
            timer += Time.deltaTime;
            float verticalOffset = Mathf.Sin(timer * Mathf.PI * 2 * floatFrequency) * floatAmplitude;
            heldItem.transform.localPosition = originalLocalPosition + Vector3.up * verticalOffset;
        }
    }

    private void OnDestroy()
    {
        // Clean up particles if they exist
        if (activeParticle != null)
        {
            heldItem.OnItemEquipped -= ReleaseItemEquip;
            Destroy(activeParticle.gameObject);
        }
    }
}
