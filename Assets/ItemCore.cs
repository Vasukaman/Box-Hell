using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum ItemState { WorldItem, Tool }
public class ItemCore : MonoBehaviour
{
    [SerializeField] public Tool tool;
    [SerializeField] public WorldItem worldItem;
    [SerializeField] public Item item;
    [SerializeField] private GameObject visual;
    [SerializeField] private ItemState currentState;

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

    }
    public void MakeItWorldItem()
    {
        SetState(ItemState.WorldItem);
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

    } 
    
    private void TurnIntoWorldItem()
    {
        
        tool.gameObject.SetActive(false);
        worldItem.gameObject.SetActive(true);
        worldItem.EnableWorldItemExtra();
        visual.transform.localPosition = Vector3.zero;
        visual.transform.localRotation = Quaternion.identity;
    }


    private void OnValidate()
    {

        SetState(currentState);

    }
}
