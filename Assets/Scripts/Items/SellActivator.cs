using UnityEngine;          // For MonoBehaviour, GameObject, Collider, etc.
using UnityEngine.Events;   // For UnityEvent
using TMPro;               // For TextMeshPro (TMP_Text)
using System.Collections.Generic; // For HashSet<T>, List<T>, etc

public class AutosellerLock : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private int targetPrice = 100;
    [SerializeField] private TMP_Text priceDisplay;
    [SerializeField] private UnityEvent onPriceReached;

    [Header("Visuals")]
    [SerializeField] private GameObject activeIndicator;
    [SerializeField] private GameObject completedIndicator;

    private int _currentProgress;
    private bool _isCompleted;
    private HashSet<ItemCore> _processedItems = new HashSet<ItemCore>();

    private void Awake()
    {
        _currentProgress = targetPrice;
        UpdateDisplay();
        UpdateVisualState();
    }

    private void LateUpdate()
    {
        _processedItems.Clear();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_isCompleted) return;

        ItemCore item = other.GetComponentInParent<ItemCore>();
        if (item != null && !_processedItems.Contains(item))
        {
            _processedItems.Add(item);
            ProcessItem(item);
        }
    }

    private void ProcessItem(ItemCore item)
    {
        // Calculate actual contribution (prevent negative values)
        int contribution = Mathf.Min(item.price, _currentProgress);
        _currentProgress -= contribution;

        // Handle oversell
        if (contribution > 0)
        {
            Destroy(item.gameObject);
            UpdateDisplay();
            CheckCompletion();
        }
    }

    private void UpdateDisplay()
    {
        if (priceDisplay != null)
        {
            priceDisplay.text = $"{_currentProgress}$";
        }
    }

    private void UpdateVisualState()
    {
        if (activeIndicator) activeIndicator.SetActive(!_isCompleted);
        if (completedIndicator) completedIndicator.SetActive(_isCompleted);
    }

    private void CheckCompletion()
    {
        if (_currentProgress <= 0 && !_isCompleted)
        {
            _isCompleted = true;
            onPriceReached.Invoke();
            UpdateVisualState();

            // Optional: Disable collider after completion
            GetComponent<Collider>().enabled = false;
        }
    }

    // For debug/reset purposes
    public void ResetLock(int newTargetPrice)
    {
        _currentProgress = newTargetPrice;
        targetPrice = newTargetPrice;
        _isCompleted = false;
        GetComponent<Collider>().enabled = true;
        UpdateDisplay();
        UpdateVisualState();
    }
}