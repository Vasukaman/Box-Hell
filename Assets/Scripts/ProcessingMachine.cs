using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Collections;

public class ProcessingMachine : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int targetPrice = 100;
    [SerializeField] private float priceMultiplier = 1.2f; // Новое поле для множителя
    [SerializeField] private LootConfiguration outputLootConfig;
    [SerializeField] private Vector2 spawnForceRange = new Vector2(5f, 10f);

    [Header("References")]
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Collider processingCollider;
    [SerializeField] private TMP_Text progressText;
    [SerializeField] private ProgressIndicator progressIndicator;

    [Header("Effects")]
    [SerializeField] private ParticleSystem processingEffect;
    [SerializeField] private AudioClip processingSound;
    [SerializeField] private AudioClip itemProcessedSound; // Новый звук для обработки предмета
    [SerializeField] private float processingDuration = 1f;

    private int currentValue;
    private bool isProcessing;
    private HashSet<ItemCore> processedItems = new HashSet<ItemCore>();

    private void Awake()
    {
        if (processingCollider == null)
            processingCollider = GetComponent<Collider>();

        UpdateDisplay();
    }

    private void LateUpdate()
    {
        processedItems.Clear();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isProcessing) return;

        ItemCore item = other.GetComponentInParent<ItemCore>();
        if (item != null && !processedItems.Contains(item))
        {
            ProcessItem(item);
        }
    }

    private void ProcessItem(ItemCore item)
    {
        currentValue += item.price;
        UpdateDisplay();

        // Проигрываем звук удаления предмета
        if (itemProcessedSound != null)
            AudioSource.PlayClipAtPoint(itemProcessedSound, transform.position);

        Destroy(item.gameObject);

        if (currentValue >= targetPrice)
        {
            StartCoroutine(ProcessingRoutine());
        }
    }

    private IEnumerator ProcessingRoutine()
    {
        isProcessing = true;

        if (processingEffect != null) processingEffect.Play();
        if (processingSound != null)
            AudioSource.PlayClipAtPoint(processingSound, transform.position);

        yield return new WaitForSeconds(processingDuration);

        SpawnResultItem();
        ResetMachine();

        isProcessing = false;
    }

    private void SpawnResultItem()
    {
        Vector3 spawnPos = GetGroundAdjustedPosition(spawnPoint.position);
        Item resultItem = LootManager.GenerateBoxLoot(outputLootConfig, 1, 0, spawnPos, false);

        if (resultItem?.itemCore != null)
        {
            Rigidbody rb = resultItem.itemCore.GetComponent<Rigidbody>();
            if (rb != null)
            {
                float force = Random.Range(spawnForceRange.x, spawnForceRange.y);
                rb.AddForce(Random.insideUnitSphere * force, ForceMode.Impulse);
            }
        }
    }

    private void ResetMachine()
    {
        // Умножаем целевую цену на множитель
        targetPrice = Mathf.RoundToInt(targetPrice * priceMultiplier);
        currentValue = 0;
        UpdateDisplay();
    }

    private Vector3 GetGroundAdjustedPosition(Vector3 position)
    {
        RaycastHit hit;
        if (Physics.Raycast(position + Vector3.up * 0.5f, Vector3.down, out hit, 2f))
        {
            return hit.point + Vector3.up * 0.1f;
        }
        return position;
    }

    private void UpdateDisplay()
    {
        if (progressText != null)
            progressText.text = $"{currentValue}/{targetPrice}";

        if (progressIndicator != null)
            progressIndicator.UpdateProgress(currentValue, targetPrice);
    }
}