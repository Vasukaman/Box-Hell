using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class InventorySlotUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private RectTransform selectionTransform;
    [SerializeField] private CanvasGroup textCanvasGroup;
    [SerializeField] private TextMeshProUGUI durabilityText;

    private ItemCore currentItem;
    private bool isSelected;
    private float selectionSpeed = 0.2f;
    private Vector3 baseScale;

    void Awake()
    {
        baseScale = selectionTransform.localScale;
        textCanvasGroup.alpha = 0;
        Deselect();
    }

    public void SetItem(ItemCore item)
    {

        if (item==null)
        {
            currentItem = null;
            iconImage.enabled = false;
            durabilityText.enabled = false;

            iconImage.sprite = null;
            nameText.text = "";

            durabilityText.enabled = false;
            durabilityText.text = "";


            priceText.enabled = false;
            priceText.text = "";
            return;

        }

        if (currentItem!=null && currentItem.tool!=null)
        {
            currentItem.tool.OnDurabilityChanged -= UpdateDurability;
        }

        currentItem = item;
        iconImage.enabled = item != null;
        durabilityText.enabled = false;
        priceText.enabled = false;


        if (item != null)
        {
            iconImage.sprite = item.item.icon;
            nameText.text = item.item.itemName;
            priceText.text = item.price.ToString() + "$";
            priceText.enabled = true;


            if (currentItem.tool != null)
            {
                durabilityText.enabled = true;
                durabilityText.text = item.tool.currentDurability.ToString();

                currentItem.tool.OnDurabilityChanged += UpdateDurability;
            }
  
        }
    }

    public void UpdateDurability()
    {
        durabilityText.text = currentItem.tool.currentDurability.ToString();    
    }

    public void Select()
    {
        if (isSelected) return;

        isSelected = true;
        selectionTransform.DOScale(baseScale * 1.2f, selectionSpeed)
            .SetEase(Ease.OutBack);

        textCanvasGroup.DOFade(1, 0.3f)
            .OnComplete(() => textCanvasGroup.DOFade(0, 0.5f).SetDelay(1f));
    }

    public void Deselect()
    {
        isSelected = false;
        selectionTransform.DOScale(baseScale, selectionSpeed)
            .SetEase(Ease.InOutQuad);
    }
}