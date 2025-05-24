using UnityEngine;
using System.Collections;

public class ItemInteractionHandler : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float movementDuration = 0.5f;
    [SerializeField] private float arrivalThreshold = 0.1f;
    [SerializeField] private Transform holderTransform;

    [Header("References")]
    [SerializeField] private LayerMask interactableLayers;
    [SerializeField] private Transform throwOrigin;

    [Header("Range")]
    [SerializeField] private float pickupRange = 3f;

    private Camera mainCamera;
    private InventorySystem inventory;
    private Coroutine currentMovement;
    private ItemCore movingItem;

    private void Awake()
    {
        mainCamera = Camera.main;
        inventory = GetComponent<InventorySystem>();
    }

    private void Update()
    {
        HandleItemPickup();
        HandleItemThrow();
    }

    private void HandleItemPickup()
    {
        if (!Input.GetKeyDown(KeyCode.E)) return;

        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (Physics.Raycast(ray, out RaycastHit hit, pickupRange, interactableLayers))
        {
            ItemCore item = hit.collider.GetComponentInParent<ItemCore>();
            if (item != null && item.currentState != ItemState.Tool)
            {
                StartItemMovement(item);
            }
        }
    }

    private void StartItemMovement(ItemCore item)
    {
        // Cancel previous movement if any
        if (currentMovement != null)
        {
            StopCoroutine(currentMovement);
            OnMovementInterrupted(movingItem);
        }

        item.transform.SetParent(null);

        movingItem = item;
        currentMovement = StartCoroutine(MoveItemToHolderRoutine(item));
    }

    private IEnumerator MoveItemToHolderRoutine(ItemCore item)
    {
        // Initialize movement
        Vector3 startPos = item.transform.position;
        Quaternion startRot = item.transform.rotation;
        float elapsed = 0f;

        item.Equip();

        item.MakeItTool(); // Change state if needed

        item.transform.SetParent(null,true);
        while (elapsed < movementDuration)
        {
            if (item == null || item.currentState == ItemState.WorldItem)
            {
                OnMovementInterrupted(item);
                yield break;
            }

            // Smooth movement to holder position
            item.transform.position = Vector3.Slerp(
                startPos,
                holderTransform.position,
                elapsed / movementDuration
            );

            item.transform.rotation = Quaternion.Slerp(
                startRot,
                holderTransform.rotation,
                elapsed / movementDuration
            );

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Finalize positioning
        if (item != null)
        {
       //     item.transform.SetParent(holderTransform);
       //     item.transform.localPosition = Vector3.zero;
            inventory.AddItem(item);
        }

        currentMovement = null;
        movingItem = null;
    }

    private void OnMovementInterrupted(ItemCore item)
    {
        if (item != null)
        {
            item.MakeItWorldItem();
            item.transform.SetParent(null, true);
        }
        currentMovement = null;
        movingItem = null;
    }

    public void CancelCurrentMovement()
    {
        if (currentMovement != null)
        {
            StopCoroutine(currentMovement);
            OnMovementInterrupted(movingItem);
        }
    }

    // Existing Throw method
    private void HandleItemThrow()
    {
        if (!Input.GetKeyDown(KeyCode.Q)) return;
        inventory.ThrowItem();
    }
}