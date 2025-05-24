using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public enum ItemState { WorldItem, Tool, InSpawnHolder }
public class ItemCore : MonoBehaviour
{
    [SerializeField] public Tool tool;
    [SerializeField] public WorldItem worldItem;
    [SerializeField] public Item item;
    [SerializeField] private GameObject visual;
    [SerializeField] public ItemState currentState;
    [SerializeField] public int price;
    public PlayerCore owner;


    public event Action OnItemEquipped;
    public event Action OnItemThrowed;


    [Header("Movement Settings")]
    [SerializeField] private float movementSpeed = 5f;
    [SerializeField] private float parentingThreshold = 0.1f;

    private Coroutine movementCoroutine;
    private bool isMoving;

    protected void Start()
    {
        UpdatePrice();
    }

    private void UpdatePrice()
    {
        price = UnityEngine.Random.Range(item.priceRange.x, item.priceRange.y + 1);
    }

    public void SetOwner(PlayerCore newOwner)
    {
        owner = newOwner;
    }

    public void ClearOwner()
    {
        owner = null;
    }
    public void Equip()
    {
        OnItemEquipped?.Invoke();
    }

    private void HandleBeingSelected()
    {

    }

    private void OnSelected()
    {


    }

    private void OnDeselected()
    {


    }

    private void OnDestroy()
    {
     
    }
    private void SetState(ItemState newState)
    {
        currentState = newState;
        if (currentState==ItemState.Tool)
        {
            TurnIntoTool();
        }

        if (currentState == ItemState.WorldItem)
        {
            TurnIntoWorldItem();
        }

        if (currentState == ItemState.InSpawnHolder)
        {
            TurnIntoHoldable();
        }

    }
    public void HandleItemThrowed()
    {
        OnItemThrowed.Invoke();
    }

    public void HandleItemEquipped()
    {
        OnItemEquipped.Invoke();
    }
    public void MakeItWorldItem()
    {
        SetState(ItemState.WorldItem);
    }
    public void MakeItHoldedByHolder()
    {
        SetState(ItemState.InSpawnHolder);
    }

    public void MakeItTool()
    {
        SetState(ItemState.Tool);
    }

    private void TurnIntoTool()
    {
        worldItem.DisableWorldItemExtra();
        worldItem.gameObject.SetActive(false);
        tool.gameObject.SetActive(true);
        visual.transform.localPosition = Vector3.zero;
        visual.transform.localRotation = Quaternion.identity;

        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;


        visual.layer = 6;       
        foreach (Transform child in visual.transform.GetComponentsInChildren<Transform>())
        {
            child.gameObject.layer = 6;
        }
       

    } 
    
    private void TurnIntoWorldItem()
    {
        
        tool.gameObject.SetActive(false);
        worldItem.gameObject.SetActive(true);
        worldItem.EnableWorldItemExtra();
        visual.transform.localPosition = Vector3.zero;
        visual.transform.localRotation = Quaternion.identity;

        visual.layer = 0;
        foreach (Transform child in visual.transform.GetComponentsInChildren<Transform>())
        { 
            child.gameObject.layer = 0;
        }

    }


    private void TurnIntoHoldable()
    {

        TurnIntoWorldItem();
        worldItem.DisableWorldItemExtra();
        

    }



    public void MoveToTransform(Transform target, float duration)
    {
        if (currentState == ItemState.WorldItem) return;

        // Stop any existing movement
        StopMovement();

       // targetParent = target;
        movementCoroutine = StartCoroutine(MoveToTransformRoutine(target, duration));
    }

    private IEnumerator MoveToTransformRoutine(Transform target, float duration)
    {
        isMoving = true;
        float elapsedTime = 0f;
        Vector3 startPosition = transform.position;

        while (elapsedTime < duration &&
               currentState != ItemState.WorldItem &&
               target != null)
        {
            if (Vector3.Distance(transform.position, target.position) <= parentingThreshold)
            {
                AttachToParent(target);
                yield break;
            }

            // Smooth movement with dynamic target tracking
            transform.position = Vector3.Lerp(
                startPosition,
                target.position,
                elapsedTime / duration
            );

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Final check and attachment
        if (target != null &&
            Vector3.Distance(transform.position, target.position) <= parentingThreshold)
        {
            AttachToParent(target);
        }
        else
        {
            isMoving = false;
        }
    }

    private void AttachToParent(Transform parent)
    {
        transform.SetParent(parent);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        isMoving = false;

        // Optional: Add final adjustment
        StartCoroutine(FinalPositionAdjustment());
    }

    private IEnumerator FinalPositionAdjustment()
    {
        float adjustmentTime = 0.5f;
        float timer = 0f;
        Vector3 startPos = transform.localPosition;
        Quaternion startRot = transform.localRotation;

        while (timer < adjustmentTime)
        {
            transform.localPosition = Vector3.Lerp(startPos, Vector3.zero, timer / adjustmentTime);
            transform.localRotation = Quaternion.Lerp(startRot, Quaternion.identity, timer / adjustmentTime);
            timer += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    public void StopMovement()
    {
        if (movementCoroutine != null)
        {
            StopCoroutine(movementCoroutine);
            movementCoroutine = null;
        }
        isMoving = false;
     //   targetParent = null;
    }


    private void OnValidate()
    {

        SetState(currentState);

    }
}
