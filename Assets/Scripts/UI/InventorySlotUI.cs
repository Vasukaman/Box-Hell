using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class InventorySlotUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private RectTransform selectionTransform;
    [SerializeField] private CanvasGroup textCanvasGroup;

    private Item currentItem;
    private bool isSelected;
    private float selectionSpeed = 0.2f;
    private Vector3 baseScale;

    void Awake()
    {
        baseScale = selectionTransform.localScale;
        textCanvasGroup.alpha = 0;
        Deselect();
    }

    public void SetItem(Item item)
    {
        currentItem = item;
        iconImage.enabled = item != null;

        if (item != null)
        {
            iconImage.sprite = item.icon;
            nameText.text = item.itemName;
        }
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