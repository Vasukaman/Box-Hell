using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIItemPrice : MonoBehaviour
{

    [SerializeField] private InteractionHandler interactionHandler;
    [SerializeField] private TMP_Text priceText;
    [SerializeField] private Transform pricePanel;

    private void Awake()
    {
        interactionHandler.OnItemSelected += ShowPrice;
        interactionHandler.OnItemLost += HidePrice;

    }
    void ShowPrice(ItemCore item)
    {
        pricePanel.gameObject.SetActive(true);
        priceText.text = item.price.ToString() + "$";
    }

    void HidePrice()
    {
        pricePanel.gameObject.SetActive(false);
    }
}
