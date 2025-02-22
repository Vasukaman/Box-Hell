using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections;

public class PlayerCore : MonoBehaviour, IDamageableByExplosion, IHittable
{
    [Header("Stats")]
    [SerializeField] private int _currentHealth = 6;
    [SerializeField] private int _currentCoins = 0;

    [Header("Invincibility")]
    [SerializeField] private float _invincibilityTime = 1f; // Serializable invincibility duration
    private bool _isInvincible = false; // Tracks invincibility state

    [Header("Extra")]
    [SerializeField] private GameData gameData;
    [SerializeField] private float _durationOfHitFreezeFrame = 0.1f;

    // Events with amount parameters
    public event Action<int> onHealthChanged;
    public event Action<int> onCoinsChanged;
    public event Action onDeath;
    public UnityEvent OnTakeDamage;

    public int Health => _currentHealth;
    public int Coins => _currentCoins;

    private void Awake()
    {
        TimeManager.Instance.UnfreezeGame();
        gameData.playerCore = this;
    }

    private RoomManager currentRoomManager;

    private void Start()
    {
        onHealthChanged?.Invoke(_currentHealth);
    }

    public void Respawn()
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
            StartCoroutine(WaitAndHandleDeath());
    }

    IEnumerator WaitAndHandleDeath()
    {
        yield return new WaitForSeconds(0.3f);
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
        GameStateManager.Instance.HandleDeath();
        // Add custom death logic later
    }

    public void TakeDamage()
    {
        if (_isInvincible) return; // Exit if invincible

        OnTakeDamage?.Invoke();

        ScreenEffectManager.Instance.HitEffect(_durationOfHitFreezeFrame * 2);
        TimeManager.Instance.FreezeFrame(_durationOfHitFreezeFrame);

        ModifyHealth(-1);
        StartCoroutine(InvincibilityCoroutine()); // Start invincibility after taking damage
    }

    // Handles invincibility duration
    private IEnumerator InvincibilityCoroutine()
    {
        _isInvincible = true;
        yield return new WaitForSeconds(_invincibilityTime);
        _isInvincible = false;
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