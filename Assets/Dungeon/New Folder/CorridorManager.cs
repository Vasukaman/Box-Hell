using UnityEngine;

public class CorridorManager : MonoBehaviour
{
    public CorridorConnection connection;

    public event System.Action OnExitTrigger;
    public event System.Action OnEnterTrigger;


    public void HandleExitTrigger()
    {
        OnExitTrigger.Invoke();
    }
    public void HandleEnterTrigger()
    {
        OnExitTrigger.Invoke();
    }

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