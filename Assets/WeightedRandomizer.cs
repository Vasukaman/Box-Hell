using UnityEngine;
using System.Collections.Generic;

public class WeightedRandomizer : MonoBehaviour
{
    [System.Serializable]
    public struct WeightedObject
    {
        public GameObject gameObject;
        [Min(0)] public float weight;
    }

    [SerializeField] private List<WeightedObject> objects = new List<WeightedObject>();

    void Start()
    {
        if (objects.Count == 0)
        {
            Debug.LogError("No objects in the weighted objects list!");
            return;
        }

        GameObject selectedObject = GetRandomWeightedObject();

        if (selectedObject != null)
        {
            foreach (WeightedObject obj in objects)
            {
                if (obj.gameObject == selectedObject)
                {
                    obj.gameObject.SetActive(true);
                }
                else
                {
                    Destroy(obj.gameObject);
                }
            }
        }

        // Clear the list after processing
        objects.Clear();
    }

    private GameObject GetRandomWeightedObject()
    {
        float totalWeight = CalculateTotalWeight();
        float randomValue = Random.Range(0, totalWeight);
        float currentWeight = 0;

        foreach (WeightedObject obj in objects)
        {
            currentWeight += obj.weight;
            if (randomValue <= currentWeight)
            {
                return obj.gameObject;
            }
        }

        return null;
    }

    private float CalculateTotalWeight()
    {
        float total = 0;
        foreach (WeightedObject obj in objects)
        {
            total += obj.weight;
        }
        return total;
    }
}