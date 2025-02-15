// PlayerToolHandler.cs
using UnityEngine;

public class PlayerToolHandler : MonoBehaviour
{
    [SerializeField] private InventorySystem inventory;


    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
   
    }

    private void Update()
    {
        HandleInput();
      //  currentTool?.Update();
    }

    private void HandleInput()
    {
        HandleMouseScroll();
        HandleSlotSelection();
        HandleToolUsage();
     
    }

    private void HandleSlotSelection()
    {
       
        for (int i = 0; i < inventory.SlotCount; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
     
                inventory.SelectSlot(i);
            }
        }
    }

    private void HandleToolUsage()
    {
        if (Input.GetMouseButtonDown(0))
        {
            
           inventory.UseCurrentTool();
        }

        if (Input.GetMouseButtonUp(0))
        {
           // inventory.GetSelectedItem()?.linkedTool?.StopUsing();
        }
    }

    private void HandleMouseScroll()
    {
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");

        if (scrollInput > 0) // Scrolling up
        {
            inventory.SelectPrevSlot();
        }
        else if (scrollInput < 0) // Scrolling down
        {
            inventory.SelectNextSlot();
        }
    }
    private void OnDestroy()
    {
        if (inventory != null)
        {
      
        }
    }
}