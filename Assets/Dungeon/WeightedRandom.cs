using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class WeightedRandom
{
    public static T Select<T>(System.Collections.Generic.IEnumerable<T> items) where T : struct, IWeighted
    {
        int totalWeight = 0;
        foreach (var item in items) totalWeight += item.Weight;

        int randomValue = Random.Range(1, totalWeight + 1);
        int current = 0;

        foreach (var item in items)
        {
            current += item.Weight;
            if (randomValue <= current) return item;
        }

        return default;
    }

    public interface IWeighted
    {
        int Weight { get; }
    }
}
