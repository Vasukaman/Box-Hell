using UnityEngine;

public class DoorController : MonoBehaviour
{
    public DoorState currentState;
    public RoomConnectionPoint connectionPoint;
    public System.Action<DoorController, RoomConnectionPoint> OnDoorInteracted; //Actually on Door opened
    public int price;
   
    public void SetState(DoorState newState)
    {
        Debug.Log("Set state " + newState);
        currentState = newState;
        UpdateVisuals();
    }

    public void TryOpenningDoor()
    {
        if (false) return; //Put some logic later;
        if (currentState == DoorState.Unlocked)
        { OpenDoor(); }
       
    }
    private void OpenDoor()
    {
        Debug.Log("DoorOpened");

        SetState(DoorState.Opened);
        OnDoorInteracted?.Invoke(this, connectionPoint);

        //Just temp
        this.gameObject.SetActive(false);
    }

    void UpdateVisuals()
    {
        // Animation/visual state logic
    }

    
}