using UnityEngine;

public static class ItemFactory
{
    public static ItemCore CreateWorldItem(ItemCore item, Vector3 position, Quaternion rotation, bool applyThrowForce = false)
    {
        if (item == null)
        {
            Debug.LogWarning($"Can't create world item for {item?.item.name}");
            return null;
        }

        var worldItem = Object.Instantiate(item, position, rotation);
        //worldItem.Initialize(item, applyThrowForce);
        return worldItem;
    }
}