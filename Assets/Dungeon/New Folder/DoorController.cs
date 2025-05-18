using UnityEngine;
using TMPro;
public class DoorController : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private TextMeshProUGUI roomNumberText;
    [SerializeField] private GameObject hasSellerIcon;
    public DoorState currentState;
    public RoomConnectionPoint connectionPoint;
    public System.Action<DoorController, Door> OnDoorInteracted; //Actually on Door opened
    public System.Action OnDoorClosed; //Actually on Door opened
    public int price;
    [SerializeField] private int stableDoorPriceMultiplier = 5;
    [SerializeField] private int randomDoorPriceMinMultiplier = 1;
    [SerializeField] private int randomDoorPriceMaxMultiplier = 5;
        
    public bool startOpened = false;

    private Door _door;//I'll make it private for now. Mb I can refactor t later to not double connectionPoint
    private void Start()
    {
        if (startOpened) OpenDoor();
    }
    
    public void GeneratePrice(int currentRoomNumber)
    {
        int newPrice = currentRoomNumber * stableDoorPriceMultiplier;

        for (int i = 0; i < currentRoomNumber; i++)
        {
            newPrice += Random.Range(randomDoorPriceMinMultiplier, randomDoorPriceMaxMultiplier);
        }
        SetPrice(newPrice);
    }
    public void SetRoomNumber(int roomNumber)
    {

        roomNumberText.text = roomNumber.ToString();

        if (roomNumber < 1)
            roomNumberText.text = "";
    }

    public void SetRoomConfig(RoomConfiguration _roomConfig)
    {

        _door.roomConfiguration = _roomConfig;

        hasSellerIcon.SetActive(_roomConfig.hasSellMachine);
    }

    public void SetPrice(int newPrice)
    {
        price = newPrice;
        priceText.text = price.ToString();

        if (price < 1)
            priceText.text = "";
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
        _door.connectionPoint = connectionPoint;
  
        OnDoorInteracted?.Invoke(this, _door);

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