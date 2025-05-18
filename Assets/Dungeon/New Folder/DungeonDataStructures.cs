using UnityEngine;

[System.Serializable]
public struct RoomConnectionPoint
{
    public ConnectionDirection direction;
    public Transform doorTransform; // Visual element
    public Transform spawnAnchor;   // Position/orientation for connection
}

[System.Serializable]
public struct CorridorConnection
{
    public Transform startAnchor;
    public Transform endAnchor;
}


[System.Serializable]
public struct Door
{
    public RoomConnectionPoint connectionPoint;
    public RoomConfiguration roomConfiguration;
}