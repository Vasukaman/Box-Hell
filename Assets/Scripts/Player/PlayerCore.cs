using UnityEngine;
using System;

public class PlayerCore : MonoBehaviour, IDamageableByExplosion, IHittable
{
    [Header("Stats")]
    [SerializeField] private int _currentHealth = 6;
    [SerializeField] private int _currentCoins = 0;

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

    private void Start()
    {
        onHealthChanged?.Invoke(_currentHealth);
    }
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

    private void ModifyHealth(int amount)
    {
        _currentHealth = Mathf.Max(0, _currentHealth + amount);
        onHealthChanged?.Invoke(_currentHealth);

        if (_currentHealth <= 0)
            HandleDeath();
    }

    public void ModifyCoins(int amount)
    {
        _currentCoins = Mathf.Max(0, _currentCoins + amount);
        onCoinsChanged?.Invoke(amount);
    }

    public void GiveCoins(int amount)
    {
        ModifyCoins(amount);
    }


    public bool TryBuying(int price)
    {
        if (_currentCoins < price) return false;

        ModifyCoins(-price);
        return true;
    }

    void HandleDeath()
    {
        onDeath?.Invoke();
        Application.LoadLevel(Application.loadedLevel);
        // Add custom death logic later
    }

    public void TakeDamage()
    {
        ModifyHealth(-1);
    }
    public void TakeExplosionDamage(ExplosionData explosionData, Vector3 explosionOrigin)
    {
        TakeDamage();
    }

    public void TakeHit(HitData hitData)
    {
        TakeDamage();
    }
}