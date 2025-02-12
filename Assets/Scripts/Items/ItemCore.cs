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
    [SerializeField] private ItemState currentState;
    [SerializeField] public int price;
    public PlayerCore owner;


    public event Action OnItemEquipped;
    public event Action OnItemThrowed;

    protected void Start()
    {
        UpdatePrice();
    }

    private void UpdatePrice()
    {
        price = item.price;
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



    private void OnValidate()
    {

        SetState(currentState);

    }
}
