using UnityEngine;
using System.Collections.Generic;
using System.Collections;
public class RoomManager : MonoBehaviour
{
      public int tier;
    


    
    [Header("State")]
    public RoomType roomType;
    public bool isActive;
    
    [Header("References")]
    public List<DoorController> doors;
    public DoorController enterDoor;

    [Header("Boxes")]
    [SerializeField] private List<BoxSpawner> boxSpawners;

    [SerializeField] private Transform respawnTransform;
    [SerializeField] private PlayerCore playerCore;
    public event System.Action<RoomConnectionPoint> OnExitSelected;
    public event System.Action OnEnterTrigger;
    private bool entered;
    public int roomNumber;

    void Start()
    {
        InitializeDoors();
   
        PrepareRoom();

        
    }

    public Transform GetRespawnTransform()
    {
        if (respawnTransform==null)
        {
            Debug.LogWarning("No respawn transform. Will give room transform. Pray this works.");
            return transform;
        }

        return respawnTransform;
    }


    void PrepareRoom()
    {
        Debug.Log("Preparing Room!");
        SpawnBoxes();
    }

    void SpawnBoxes()
    {
        StartCoroutine(SpawnBoxesSpread());
    }


    IEnumerator SpawnBoxesSpread()
    {
        foreach (BoxSpawner boxSpawner in boxSpawners)
        {
            Debug.Log("Try To Spawn Box");
            boxSpawner.SpawnBox();
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
        }
     
    }


    void InitializeDoors()
    {
        foreach (var door in doors)
        {
            door.OnDoorInteracted += HandleDoorInteraction;
            door.GeneratePrice(roomNumber);
            door.SetRoomNumber(roomNumber+1);
        }
       
       enterDoor.TryOpenningDoor();
        enterDoor.SetPrice(0);
        enterDoor.SetRoomNumber(0);
    }

    void HandleDoorInteraction(DoorController door, RoomConnectionPoint connection)
    {
        if (isActive)
        {
            Debug.Log("Exit seleted");
            OnExitSelected?.Invoke(connection);
        }
    }
        
    public void HandleEnterTrigger()
    {
        if (entered) return;
        
        OnEnterTrigger?.Invoke();
        entered = true;
        
    }

    public void ActivateRoom()
    {
        isActive = true;

    }

    public void DeactivateRoom()
    {
        isActive = false;
        SetDoorStates(DoorState.Unlocked);
    }

    public void CloseAllDoors()
    {
        foreach (var door in doors)
        {

            door.CloseDoor();
        }
    }

   protected  void SetDoorStates(DoorState state)
    {
        foreach (var door in doors)
        {

            door.SetState(state);
        }
    }   
    
    protected  void CloseDoors(DoorState state)
    {
        foreach (var door in doors)
        {
            door.CloseDoor();
        }
    }


    public void BeginRoom()
    {
        CloseEnterDoor();
    }
    public void CloseEnterDoor()
    {
        enterDoor.CloseDoor();
    }




    public RoomConnectionPoint GetEntryPoint(ConnectionDirection direction)
    {
        return enterDoor.connectionPoint;
    }


    public void FinishRoom()
    {
        CloseAllDoors();
    }
}