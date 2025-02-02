using UnityEngine;

public enum BoxType { Regular, Explosive }
public enum ItemType { Points, Useless, Consumable, Tool }

[System.Serializable]
public struct DropScoreItem
{
    public Item item;
    public int dropScore;
}

[System.Serializable]
public struct DropScoreEntry<T> where T : UnityEngine.Object
{
    public T value;
    public int dropScore;
}