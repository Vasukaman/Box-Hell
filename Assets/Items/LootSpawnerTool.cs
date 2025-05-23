using UnityEngine;

public class LootSpawnerTool : Tool
{
    [Header("Loot Spawning")]
    [SerializeField] private LootConfiguration lootConfig;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float spawnForce = 5f;
    [SerializeField] private Vector3 spawnTorque = new Vector3(0, 100f, 0);

    [Header("Effects")]
    [SerializeField] private ParticleSystem spawnEffect;
    [SerializeField] private AudioClip spawnSound;
    [SerializeField] private float destroyDelay = 2f;
    [SerializeField] private float checkDistance = 1f;

    [SerializeField] public LayerMask wallsLayers;
    protected override void Awake()
    {
        base.Awake();
        // Set up for single use
        maxDurability = 1;
        currentDurability = 1;
        durabilityLossPerUse = 1;

        // Default to self if no spawn point assigned
        if (spawnPoint == null) spawnPoint = transform;
    }

    public override void Use()
    {
        if (isBroken) return;

        base.Use();

        // Get spawn position with ground check
        Vector3 spawnPosition = GetSpawnPosition();

        // Generate loot
        Item spawnedItem = LootManager.GenerateBoxLoot(lootConfig, 1, 0, spawnPosition);
        if (spawnedItem != null)
        {
            SpawnItemVisuals(spawnedItem, spawnPosition);
        }

        //// Apply physics force to spawned object
        //if (spawnedItem != null && spawnedItem.worldPrefab != null)
        //{
        // /   Rigidbody rb = spawnedItem.worldPrefab.GetComponent<Rigidbody>();
        //    if (rb != null)
        //    {
        //        rb.AddForce(Random.insideUnitSphere * spawnForce, ForceMode.Impulse);
        //        rb.AddTorque(spawnTorque * Random.Range(0.8f, 1.2f));
        //    }
        //}

        // Break the tool after use
        BreakTool();
    }

    private Vector3 GetSpawnPosition()
    {
        RaycastHit hit;
        Vector3 checkPosition = spawnPoint.position;// + Vector3.up * 0.5f;


        if (Physics.Raycast(checkPosition, Vector3.down, out hit, checkDistance, wallsLayers))
        {
            return hit.point + Vector3.up * 0.1f;
        }

        return spawnPoint.position;
    }

    private void SpawnItemVisuals(Item item, Vector3 position)
    {
        // Spawn effect
        if (spawnEffect != null)
        {
            ParticleSystem effect = Instantiate(spawnEffect, position, Quaternion.identity);
            effect.Play();
            Destroy(effect.gameObject, destroyDelay);
        }

        //// Play sound
        //if (spawnSound != null && audioSource != null)
        //{
        //    audioSource.PlayOneShot(spawnSound);
        //}

        // Rarity effects
        if (item.raritySO != null)
        {
            if (item.raritySO.dropEffect != null)
            {
                Instantiate(item.raritySO.dropEffect, position, Quaternion.identity);
            }
        }
    }

    protected override void BreakTool()
    {
        // Add any final break effects here
        base.BreakTool();
    }
}