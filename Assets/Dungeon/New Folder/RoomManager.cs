using UnityEngine;
using System.Collections.Generic;
using System.Collections;
public class RoomManager : MonoBehaviour
{
      public int tier;
    
    [Header("Connections")]
    public List<RoomConnectionPoint> entryPoints;
    public List<RoomConnectionPoint> exitPoints;


    
    [Header("State")]
    public RoomType roomType;
    public bool isActive;
    
    [Header("References")]
    public List<DoorController> doors;
    public DoorController enterDoor;

    [Header("Boxes")]
    [SerializeField] private List<BoxSpawner> boxSpawners;

    public event System.Action<RoomConnectionPoint> OnExitSelected;
    public event System.Action OnEnterTrigger;
    private bool entered;

    void Start()
    {
        InitializeDoors();
   
        PrepareRoom();

        
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
        }
       enterDoor.TryOpenningDoor();
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
     //   SetDoorStates(DoorState.Locked);
        // Room-specific activation logic
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
        foreach (var entry in entryPoints)
        {
            if (entry.direction == direction)
                return entry;
        }
        return default;
    }

    public RoomConnectionPoint GetCompatibleExit(ConnectionDirection incomingDirection)
    {
        // Get opposite direction for exit selection
        ConnectionDirection targetDirection = incomingDirection switch
        {
            ConnectionDirection.North => ConnectionDirection.South,
            ConnectionDirection.East => ConnectionDirection.West,
            ConnectionDirection.South => ConnectionDirection.North,
            ConnectionDirection.West => ConnectionDirection.East,
            _ => ConnectionDirection.North
        };

        foreach (var exit in exitPoints)
        {
            if (exit.direction == targetDirection)
                return exit;
        }
        return exitPoints[0]; // Fallback
    }

    public void FinishRoom()
    {
        CloseAllDoors();
    }
}