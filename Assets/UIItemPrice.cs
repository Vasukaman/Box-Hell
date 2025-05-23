using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIItemPrice : MonoBehaviour
{

    [SerializeField] private InteractionHandler interactionHandler;
    [SerializeField] private TMP_Text priceText;
    [SerializeField] private Transform pricePanel;
    [SerializeField] private GameObject higherPriceIcon;
    [SerializeField] private GameObject lowerPriceIcon;


    private void Awake()
    {
        interactionHandler.OnItemSelected += ShowPrice;
        interactionHandler.OnItemLost += HidePrice;

    }
    void ShowPrice(ItemCore item, ItemCore currentItem)
    {
        pricePanel.gameObject.SetActive(true);
        priceText.text = item.price.ToString() + "$";


        HidePriceDifferenceIcons();

        if (!currentItem && item.price > 0)
            ShowHigherPriceIcon();

        if (currentItem)
        {
            if (item.price > currentItem.price)
               ShowHigherPriceIcon();

            if (item.price < currentItem.price)
                ShowLowerPriceIcon();
        }

    }

    void HidePrice()
    {
        pricePanel.gameObject.SetActive(false);
        HidePriceDifferenceIcons();
    }

    void ShowHigherPriceIcon()
    {
        higherPriceIcon.SetActive(true);
        lowerPriceIcon.SetActive(false);
     
    }

    void ShowLowerPriceIcon()
    {
        higherPriceIcon.SetActive(false);
        lowerPriceIcon.SetActive(true);
    }

    void HidePriceDifferenceIcons()
    {
        higherPriceIcon.SetActive(false);
        lowerPriceIcon.SetActive(false);
    }
}
