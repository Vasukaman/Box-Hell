// AutosellerLock.cs
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using System.Collections.Generic;

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

    // NEW: events for external save system
    public event System.Action<int> OnProgressChanged;
    public event System.Action<int> OnCompleted;

    private void Awake()
    {
        // Start with defaults; external loader may override via SetState(...)
        _currentProgress = targetPrice;
        UpdateDisplay();
        UpdateVisualState();
    }

    private void Update()
    {
        
        CheckCompletion();
    }

    private void LateUpdate() => _processedItems.Clear();

    private void OnTriggerEnter(Collider other)
    {
        if (_isCompleted) return;

        var item = other.GetComponentInParent<ItemCore>();
        if (item != null && !_processedItems.Contains(item))
        {
            _processedItems.Add(item);
            ProcessItem(item);
        }
    }

    private void ProcessItem(ItemCore item)
    {
        int contribution = Mathf.Min(item.price, _currentProgress);
        if (contribution <= 0) return;

        _currentProgress -= contribution;
        Destroy(item.gameObject);

        UpdateDisplay();
        OnProgressChanged?.Invoke(_currentProgress);

        CheckCompletion();
    }

    private void UpdateDisplay()
    {
        if (priceDisplay) priceDisplay.text = $"{_currentProgress}$";
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
            OnCompleted?.Invoke(0);

            // Optional: disable collider so no more processing
            var col = GetComponent<Collider>();
            if (col) col.enabled = false;
        }
    }

    /// <summary>
    /// Allows external code to override both progress & completed flags,
    /// and immediately update visuals (and fire completion if already done).
    /// </summary>
    public void SetState(int loadedProgress, bool loadedCompleted)
    {
        _currentProgress = loadedProgress;

        CheckCompletion();
        //_isCompleted = loadedCompleted;

        UpdateDisplay();
        UpdateVisualState();

        if (_isCompleted)
            onPriceReached.Invoke();

       
    }

    /// <summary>
    /// For debug: reset to a fresh lock.
    /// </summary>
    public void ResetLock(int newTargetPrice)
    {
        _currentProgress = newTargetPrice;
        targetPrice = newTargetPrice;
        _isCompleted = false;
        var col = GetComponent<Collider>();
        if (col) col.enabled = true;

        UpdateDisplay();
        UpdateVisualState();
        OnProgressChanged?.Invoke(_currentProgress);
    }
}
