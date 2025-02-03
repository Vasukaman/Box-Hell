using UnityEngine;

public class CorridorManager : MonoBehaviour
{
    public CorridorConnection connection;

    public void AlignToExit(RoomConnectionPoint exitPoint)
    {
        // Reset corridor rotation first
        transform.rotation = Quaternion.identity;

        // Match exit point rotation
        transform.rotation = exitPoint.spawnAnchor.rotation;

        // Position corridor start at exit point
        transform.position = exitPoint.spawnAnchor.position - connection.startAnchor.localPosition;
    }
}