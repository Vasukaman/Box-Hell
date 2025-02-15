// New ItemInteractionHandler.cs
using UnityEngine;

public class ItemInteractionHandler : MonoBehaviour
{
    [SerializeField] private float pickupRange = 3f;
    [SerializeField] private LayerMask interactableLayers;
    [SerializeField] private Transform throwOrigin;

    private Camera mainCamera;
    private InventorySystem inventory;

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
            if (item != null)
            {
              
                inventory.AddItem(item);
         
              
            }
        }
    }

    private void HandleItemThrow()
    {
        if (!Input.GetKeyDown(KeyCode.Q)) return;

       
            inventory.ThrowItem();
        
    }


}