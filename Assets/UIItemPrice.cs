using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIItemPrice : MonoBehaviour
{

    [SerializeField] private InteractionHandler interactionHandler;
    [SerializeField] private TMP_Text priceText;
    [SerializeField] private Transform pricePanel;
    [SerializeField] private Sprite littleBetterIcon;
    [SerializeField] private Sprite betterIcon;
    [SerializeField] private Color betterIconColour;
    [SerializeField] private Sprite littleWorseIcon;
    [SerializeField] private Sprite worseIcon;
    [SerializeField] private Color worseIconColour;
    [SerializeField] private Sprite sameIcon;

    [SerializeField] private Image priceChangeIcon;


    [SerializeField] private GameObject weaponInfoPanel;
    [SerializeField] private Image speedChangeIcon;
    [SerializeField] private Image damageChangeIcon;
    [SerializeField] private Image rangeChangeIcon;




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

            if (item.item.type == ItemType.Tool && currentItem.item.type == ItemType.Tool)
            {
                weaponInfoPanel.SetActive(true);

                if (item.tool.damage > currentItem.tool.damage)
                {
                    damageChangeIcon.sprite = betterIcon;
                    damageChangeIcon.color = betterIconColour;
                }
                else if (item.tool.damage < currentItem.tool.damage)
                {
                    damageChangeIcon.sprite = worseIcon;
                    damageChangeIcon.color = worseIconColour;
                }
                else
                {
                    damageChangeIcon.color = new Color(0, 0, 0, 0);
                }


                if (item.tool.speed > currentItem.tool.speed)
                {
                    speedChangeIcon.sprite = betterIcon;
                    speedChangeIcon.color = betterIconColour;
                }
                else if (item.tool.speed < currentItem.tool.speed)
                {
                    speedChangeIcon.sprite = worseIcon;
                    speedChangeIcon.color = worseIconColour;
                }
                else
                {
                    speedChangeIcon.color = new Color(0, 0, 0, 0);
                }


                if (item.tool.range > currentItem.tool.range)
                {
                    rangeChangeIcon.sprite = betterIcon;
                    rangeChangeIcon.color = betterIconColour;
                }
                else if (item.tool.range < currentItem.tool.range)
                {
                    rangeChangeIcon.sprite = worseIcon;
                    rangeChangeIcon.color = worseIconColour;
                }
                else
                {
                    rangeChangeIcon.color = new Color(0, 0, 0, 0);
                }


            }
            else
            {
                weaponInfoPanel.SetActive(false);
            }
        }
    }

    void HidePrice()
    {
        pricePanel.gameObject.SetActive(false);
        HidePriceDifferenceIcons();
        weaponInfoPanel.SetActive(false);
    }

    void ShowHigherPriceIcon()
    {
        priceChangeIcon.sprite = betterIcon;
        priceChangeIcon.gameObject.SetActive(true);
        priceChangeIcon.color = betterIconColour;

    }

    void ShowLowerPriceIcon()
    {
        priceChangeIcon.sprite = worseIcon;
        priceChangeIcon.color = worseIconColour;
        priceChangeIcon.gameObject.SetActive(true);

    }

    void HidePriceDifferenceIcons()
    {
        priceChangeIcon.gameObject.SetActive(false);
    }
}
