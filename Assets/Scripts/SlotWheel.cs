using UnityEngine;
using System.Collections;

public class SlotWheel : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float spinSpeed = 720f;
    [SerializeField] private float acceleration = 180f;
    [SerializeField] private float deceleration = 360f;
    [SerializeField] private int slotsCount = 8;
    [SerializeField] private Transform itemsContainer;
    [SerializeField] private ItemCore[] fakeItemsPool;

    public bool IsSpinning { get; private set; }
    private float currentSpeed;
    private ItemCore realItem;
    private bool isTargetWheel;
    private float anglePerSlot;

    public void PrepareSpin(ItemCore targetItem, bool isTarget)
    {
        realItem = isTarget ? targetItem : null;
        isTargetWheel = isTarget;
        anglePerSlot = 360f / slotsCount;

        // Clear previous items
        foreach (Transform child in itemsContainer)
        {
            Destroy(child.gameObject);
        }

        // Create slot items
        for (int i = 0; i < slotsCount; i++)
        {
            ItemCore item = isTarget && i == 0 ? realItem :
                Instantiate(fakeItemsPool[Random.Range(0, fakeItemsPool.Length)]);

            item.transform.SetParent(itemsContainer);
            item.transform.localPosition = Vector3.zero;
            item.transform.localRotation = Quaternion.Euler(0, i * anglePerSlot, 0);
        }
    }

    public void StartSpin()
    {
        if (IsSpinning) return;

        StartCoroutine(SpinRoutine());
    }

    public void StopSpin(bool fake)
    {
        StartCoroutine(DecelerateRoutine(fake));
    }

    private IEnumerator SpinRoutine()
    {
        IsSpinning = true;
        currentSpeed = 0;

        while (currentSpeed < spinSpeed)
        {
            currentSpeed += acceleration * Time.deltaTime;
            itemsContainer.Rotate(Vector3.up, currentSpeed * Time.deltaTime);
            yield return null;
        }

        while (IsSpinning)
        {
            itemsContainer.Rotate(Vector3.up, currentSpeed * Time.deltaTime);
            yield return null;
        }
    }

    private IEnumerator DecelerateRoutine(bool fake)
    {
        float targetRotation = itemsContainer.localEulerAngles.y;
        if (!fake && isTargetWheel)
        {
            // Align to real item position
            targetRotation = Mathf.Round(targetRotation / anglePerSlot) * anglePerSlot;
        }
        else
        {
            // Random fake position
            targetRotation = Random.Range(0, 360);
        }

        while (Mathf.Abs(Mathf.DeltaAngle(itemsContainer.eulerAngles.y, targetRotation)) > 0.1f)
        {
            itemsContainer.rotation = Quaternion.RotateTowards(
                itemsContainer.rotation,
                Quaternion.Euler(0, targetRotation, 0),
                deceleration * Time.deltaTime
            );
            yield return null;
        }

        IsSpinning = false;
    }
}