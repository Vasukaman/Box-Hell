using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class GamblingMachineUISlot : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Transform slotTransform;
    [SerializeField] private Image iconImage;
    [SerializeField] private Image overlay;
    [SerializeField] private Image rarityBackground;

    [Header("Animation")]
    [SerializeField] private float pulseScale = 1.2f;
    [SerializeField] private Ease pulseEase = Ease.OutBack;
    [SerializeField] private Ease deselectEase = Ease.InOutSine;

    [HideInInspector] public float slotWeight;

    public DropScoreItem assignedItem;
    private bool isUsed = false;
    private Tween currentTween;

    public void Setup(DropScoreItem item, Color tintColor, float weight)
    {
        assignedItem = item;
        slotWeight = weight;
        iconImage.sprite = item.item.icon;
        rarityBackground.color = tintColor;
        overlay.color = Color.clear;
        transform.localScale = Vector3.one;
        isUsed = false;
    }

    public void Pulse(float duration)
    {
        if (isUsed || duration <= 0f) return;

        currentTween?.Kill();
        transform.localScale = Vector3.one;

        currentTween = slotTransform.DOScale(pulseScale, duration)
            .SetEase(pulseEase)
            .OnComplete(() => {
                currentTween = slotTransform.DOScale(1f, duration)
                    .SetEase(deselectEase);
            });
    }

    public void Deselect(float duration)
    {
        if (isUsed || duration <= 0f) return;

        currentTween?.Kill();
        currentTween = slotTransform.DOScale(1f, duration)
            .SetEase(deselectEase);
    }

    public void MarkUsed()
    {
        isUsed = true;
        overlay.color = new Color(0, 0, 0, 0.6f);
        transform.DOScale(1f, 0.2f).SetEase(Ease.InOutSine);
    }

    public bool IsUsed() => isUsed;

    public DropScoreItem GetAssignedItem() => assignedItem;
}
