using UnityEngine;

public static class WorldItemFactory
{
    public static WorldItem CreateWorldItem(Item item, Vector3 position, Quaternion rotation, bool applyThrowForce = false)
    {
        if (item == null || item.worldItemPrefab == null)
        {
            Debug.LogWarning($"Can't create world item for {item?.itemName}");
            return null;
        }

        var worldItem = Object.Instantiate(item.worldItemPrefab, position, rotation);
        worldItem.Initialize(item, applyThrowForce);
        return worldItem;
    }
}