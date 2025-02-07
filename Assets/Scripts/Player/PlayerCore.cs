using UnityEngine;
using System;

public class PlayerCore : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private int _currentHealth = 100;
    [SerializeField] private int _currentCoins;

    [Header("Extra")]
    [SerializeField] private GameData gameData;

    // Events with amount parameters
    public event Action<int> onHealthChanged;
    public event Action<int> onCoinsChanged;
    public event Action onDeath;

    public int Health => _currentHealth;
    public int Coins => _currentCoins;

    private void Awake()
    {
        gameData.playerCore = this;
    }


    private RoomManager currentRoomManager;


    public  void Respawn()
    {
        Transform respawnPoint = currentRoomManager.GetRespawnTransform();
        transform.position = respawnPoint.position;
        transform.rotation = respawnPoint.rotation;
    }

    public void SetCurrentRoomManager(RoomManager newRoomManager)
    {
        currentRoomManager = newRoomManager;
    }

    public void ModifyHealth(int amount)
    {
        _currentHealth = Mathf.Max(0, _currentHealth + amount);
        onHealthChanged?.Invoke(amount);

        if (_currentHealth <= 0)
            HandleDeath();
    }

    public void ModifyCoins(int amount)
    {
        _currentCoins = Mathf.Max(0, _currentCoins + amount);
        onCoinsChanged?.Invoke(amount);
    }

    void HandleDeath()
    {
        onDeath?.Invoke();
        // Add custom death logic later
    }
}