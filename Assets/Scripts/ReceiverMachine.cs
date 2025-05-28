// ReceiverMachine.cs
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class ReceiverMachine : MonoBehaviour
{
    [Tooltip("Your save system in the scene")]
    public MainRoomSaveSystem saveSystem;

    [Tooltip("Spawn location/parent for received items")]
    public Transform itemsParent;

    [Tooltip("Database to look up prefabs by name")]
    public ItemDatabase itemDatabase;

    [Tooltip("Seconds between spawns")]
    public float spawnInterval = 1f;

    private void OnEnable()
    {
        StartCoroutine(SpawnRoutine());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            var data = saveSystem.PopNextSentItem();
            if (data.HasValue)
            {
                // lookup prefab
                var prefab = itemDatabase.GetItemByName(data.Value.itemName);
                if (prefab != null)
                {
                    // instantiate
                    var core = Instantiate(prefab.itemCore, itemsParent);
                    core.transform.position = transform.position;
                    core.transform.rotation = transform.rotation;

                    // restore state
                    core.price = data.Value.currentPrice;
                    core.tool.currentDurability = data.Value.durability;
                    core.MakeItWorldItem();
                }
            }
        }
    }
}
