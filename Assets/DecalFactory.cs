using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DecalFactory : MonoBehaviour
{
    public static DecalFactory Instance { get; private set; }

    [Header("Settings")]
    [SerializeField] private DecalProjector _decalPrefab;
    [SerializeField] private int _initialPoolSize = 20;
    [SerializeField] private float _defaultDecalSize = 1f;

    private Queue<DecalProjector> _decalPool = new Queue<DecalProjector>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            InitializePool();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializePool()
    {
        for (int i = 0; i < _initialPoolSize; i++)
        {
            CreateDecalProjector();
        }
    }

    private DecalProjector CreateDecalProjector()
    {
        var decal = _decalPrefab != null ?
            Instantiate(_decalPrefab) :
            new GameObject("DecalProjector").AddComponent<DecalProjector>();

        decal.gameObject.SetActive(false);
        decal.transform.SetParent(transform);
        _decalPool.Enqueue(decal);
        return decal;
    }

    public void SpawnDecal(DecalTextureData decalData, Vector3 position, Vector3 normal, Transform parent ,float duration = 5f, float fadeTime = 0.5f)
    {
        if (decalData.sprite == null)
        {
            Debug.LogError("Cannot spawn decal - no sprite provided!");
            return;
        }

        var decal = GetAvailableDecal();
        if (decal == null) return;

        ConfigureDecal(decal, decalData, position, normal, parent);
        StartCoroutine(ManageDecalLifecycle(decal, duration, fadeTime));
    }

    private DecalProjector GetAvailableDecal()
    {
        if (_decalPool.Count == 0)
        {
            CreateDecalProjector();
        }

        if (_decalPool.Count < _initialPoolSize)
        {
            CreateDecalProjector();
        }

        var decal = _decalPool.Dequeue();
        decal.gameObject.SetActive(true);
        return decal;
    }

    private void ConfigureDecal(DecalProjector decal, DecalTextureData decalData, Vector3 position,
                              Vector3 normal, Transform parent)
    {
        decal.material.SetTexture("Base_Map",decalData.sprite.texture);
        decal.transform.position = position;
        decal.transform.rotation = Quaternion.LookRotation(-normal);
        decal.transform.SetParent(parent);

        // Configure decal size
        decal.size = decalData.scale;


    }

    private System.Collections.IEnumerator ManageDecalLifecycle(DecalProjector decal, float duration, float fadeTime)
    {
        // Fade in
        float timer = 0;
       
        decal.fadeFactor = 1;

        // Wait duration
        yield return new WaitForSeconds(duration);

        // Fade out
        timer = 0;
        while (timer < fadeTime)
        {
            decal.fadeFactor = Mathf.Lerp(1, 0, timer / fadeTime);
            timer += Time.deltaTime;
            yield return null;
        }

        ReleaseDecal(decal);
    }

    private void ReleaseDecal(DecalProjector decal)
    {
        decal.fadeFactor = 1;
        decal.gameObject.SetActive(false);
        _decalPool.Enqueue(decal);
    }

    // Example usage: DecalFactory.Instance.SpawnDecal(material, hit.point, hit.normal);
}