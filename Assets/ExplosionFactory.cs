using UnityEngine;

public class ExplosionFactory : MonoBehaviour
{
    public static ExplosionFactory Instance { get; private set; }

    [Header("Configuration")]
    [SerializeField] private GameObject _explosionPrefab;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void CreateExplosion(Vector3 position, ExplosionData data)
    {
        if (_explosionPrefab == null)
        {
            Debug.LogError("Explosion prefab not assigned in factory!");
            return;
        }

        GameObject explosionInstance = Instantiate(_explosionPrefab, position, Quaternion.identity);
        Explosion explosionComponent = explosionInstance.GetComponent<Explosion>();

        if (explosionComponent != null)
        {
            explosionComponent.SetExplosionData(data);
        }
        else
        {
            Debug.LogWarning("Explosion prefab is missing Explosion component!");
        }
    }

    // Optional: Preload explosions using object pooling
    public void WarmPool(int count)
    {
        // Implement object pooling here if needed
    }
}