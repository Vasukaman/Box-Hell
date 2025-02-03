using UnityEngine;

public class GameData : MonoBehaviour
{
    public static GameData Instance { get; private set; }

    [Header("Player References")]
    public PlayerCore playerCore;
    public InventorySystem inventorySystem;

    [Header("Coin Settings")]
    public int minCoinsPerBox = 1;
    public int maxCoinsPerBox = 5;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}