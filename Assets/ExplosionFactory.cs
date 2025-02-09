using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExplosionFactory
{
    private static GameObject _explosionPrefab;

    public static void Initialize(GameObject explosionPrefab)
    {
        _explosionPrefab = explosionPrefab;
    }

    public static void CreateExplosion(Vector3 position, ExplosionData data)
    {
        if (_explosionPrefab == null)
        {
            Debug.LogError("Explosion prefab not initialized!");
            return;
        }

        GameObject explosionInstance = Object.Instantiate(_explosionPrefab, position, Quaternion.identity);
        Explosion explosionComponent = explosionInstance.GetComponent<Explosion>();

        if (explosionComponent != null)
        {
            explosionComponent.SetExplosionData(data);
        }
    }
}
