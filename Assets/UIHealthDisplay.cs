using UnityEngine;
using UnityEngine.UI;

public class UIHealthDisplay : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerCore _playerCore;
    [SerializeField] private GameObject _heartPrefab;
    [SerializeField] private Transform _heartsContainer;

    private void Start()
    {
        // Clear existing hearts (for editor setup)
        foreach (Transform child in _heartsContainer)
        {
            Destroy(child.gameObject);
        }

        // Initialize hearts based on starting health
        for (int i = 0; i < _playerCore.Health; i++)
        {
            Instantiate(_heartPrefab, _heartsContainer);
        }

        // Subscribe to health changes
        _playerCore.onHealthChanged += UpdateHealthDisplay;
    }

    private void UpdateHealthDisplay(int changeAmount)
    {
        // Enable/disable hearts based on current health
        for (int i = 0; i < _heartsContainer.childCount; i++)
        {
            _heartsContainer.GetChild(i).gameObject.SetActive(i < _playerCore.Health);
        }
    }

    private void OnDestroy()
    {
        if (_playerCore != null)
        {
            _playerCore.onHealthChanged -= UpdateHealthDisplay;
        }
    }
}