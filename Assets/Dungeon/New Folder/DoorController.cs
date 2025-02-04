using UnityEngine;

public class DoorController : MonoBehaviour
{
    public DoorState currentState;
    public RoomConnectionPoint connectionPoint;
    public System.Action<DoorController, RoomConnectionPoint> OnDoorInteracted; //Actually on Door opened
    public System.Action OnDoorClosed; //Actually on Door opened
    public int price;
    public bool startOpened = false;

    private void Start()
    {
        if (startOpened) OpenDoor();
    }
    
    public void SetState(DoorState newState)
    {
        Debug.Log("Set state " + newState);
        currentState = newState;
        UpdateVisuals();


    }

    private void OnValidate()
    {
       
        if (currentState == DoorState.Opened)
            OpenDoor();

        if (currentState != DoorState.Opened)
            CloseDoor();


    }

    public void TryOpenningDoor()
    {
        if (false) return; //Put some logic later;
        if (currentState == DoorState.Unlocked)
        { OpenDoor(); }
       
    }
    private void OpenDoor()
    {
        Debug.Log("Door Opened");
        OnDoorInteracted?.Invoke(this, connectionPoint);

        if (currentState!=DoorState.Opened) SetState(DoorState.Opened);
     

        //Just temp
       // this.gameObject.SetActive(false);
    }


    public void CloseDoor()
    {
        Debug.Log("Door closed");


        OnDoorClosed?.Invoke();
        if (currentState == DoorState.Opened) SetState(DoorState.Unlocked);
        //Just temp
        // this.gameObject.SetActive(false);
    }

    void UpdateVisuals()
    {
        // Animation/visual state logic
    }

    
}