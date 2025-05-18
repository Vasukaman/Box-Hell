using UnityEngine;
using System.Collections.Generic;

public class DungeonManager : MonoBehaviour
{
    [SerializeField] List<RoomTier> dungeonTiers;
    [SerializeField] Transform worldAnchor;

    RoomManager currentRoom;
    private RoomManager previousRoom;
    CorridorManager currentCorridor;
    CorridorManager previousCorridor;
    int currentTier; // Track current progression tier
    [SerializeField] PlayerCore playerCore;
    [SerializeField] RoomConfiguration initialRoom;
    // Add these new variables
    private HashSet<RoomConfiguration> spawnedRooms = new HashSet<RoomConfiguration>();
    private List<RoomConfiguration> allPossibleRooms = new List<RoomConfiguration>();
    private int roomNumber=0;
    void Start()
    {
        currentTier = 0;
        GenerateInitialRoom();      
    }

    public void RestartScene()
    {
        Application.LoadLevel(Application.loadedLevel);
    }

    void GenerateInitialRoom()
    {
        if (initialRoom == null)
         initialRoom= GetRandomRoomConfig(currentTier);

 

        currentRoom = Instantiate(initialRoom.roomPrefab, worldAnchor).GetComponent<RoomManager>();
      
        currentRoom.tier = currentTier; // Set initial tier
        currentRoom.OnExitSelected += HandleExitSelected;
        playerCore.SetCurrentRoomManager(currentRoom);
        currentRoom.playerCore = playerCore;
        currentRoom.ActivateRoom();
 
        //roomNumber++;
        currentRoom.roomNumber = roomNumber;
        currentRoom.InitializeDoors(this);
    }

    void HandleExitSelected(Door door)
    {
        Debug.Log("Start spawn sequence");
        StartCoroutine(RoomTransitionSequence(door));

    }

    void HandleExitTrigger()
    {
        previousRoom.FinishRoom();
      
    }  
    void HandleEnterRoomTrigger()
    {

        StartCoroutine(BeginRoomWaitAndCleanUp());
    }

    System.Collections.IEnumerator BeginRoomWaitAndCleanUp()
    {
        currentRoom.BeginRoom();
        yield return new WaitForSeconds(0.5f);

        CleanUpPreviousRoom();
        CleanUpPreviousCorridor();
    }



        private void CleanUpPreviousRoom()
    {
        Destroy(previousRoom.gameObject);
    }

    private void CleanUpPreviousCorridor()
    {
        Destroy(previousCorridor.gameObject);
    }

    System.Collections.IEnumerator RoomTransitionSequence(Door _door)
    {
      

        // Determine next tier (example: increment by 1)
        int nextTier = currentTier + 1;
        if (nextTier >= dungeonTiers.Count) nextTier = dungeonTiers.Count - 1;


        RoomConfiguration nextRoomConfig;

        //Pick room config
        if (_door.roomConfiguration != null)
            nextRoomConfig = _door.roomConfiguration;
        else
            nextRoomConfig = GetRandomRoomConfig(nextTier);


        var corridorConfig = GetRandomCorridorConfig(currentTier);

        SpawnNewSection(_door.connectionPoint, nextRoomConfig, corridorConfig, nextTier);
        yield return new WaitForSeconds(0.1f);
        //Destroy(currentCorridor?.gameObject);
        // Destroy(currentRoom.gameObject);
    }
    void SpawnNewSection(RoomConnectionPoint exitPoint, RoomConfiguration nextRoomConfig, CorridorConfiguration corridorConfig, int newTier)
    {
        
        // Instantiate and align corridor first
        var newCorridor = Instantiate(corridorConfig.corridorPrefab);
        var corridorManager = newCorridor.GetComponent<CorridorManager>();
        corridorManager.AlignToExit(exitPoint);

        // Get corridor end position
        Vector3 corridorEndPosition = corridorManager.connection.endAnchor.position;
        Quaternion corridorEndRotation = corridorManager.connection.endAnchor.rotation;

        // Instantiate new room at corridor end
        var newRoom = Instantiate(nextRoomConfig.roomPrefab, corridorEndPosition, corridorEndRotation);
        var roomManager = newRoom.GetComponent<RoomManager>();

        playerCore.SetCurrentRoomManager(roomManager);
        roomManager.tier = newTier;

        spawnedRooms.Add(nextRoomConfig);


        // Fine-tune room position using entry point
        RoomConnectionPoint roomEntry = roomManager.GetEntryPoint(exitPoint.direction);
        Vector3 positionOffset = corridorEndPosition - roomEntry.spawnAnchor.position;
        newRoom.transform.position += positionOffset;


        // Get the current local rotation of the newRoom
        Quaternion currentRotation = newRoom.transform.localRotation;

        // Get the rotation of the spawn anchor
        Quaternion additionalRotation = roomEntry.spawnAnchor.localRotation;

        // Combine the rotations by multiplying the Quaternions
        newRoom.transform.localRotation = currentRotation * additionalRotation;



        //Disconecct old room. TODO MAKE A FUNCTION
        if (currentRoom)
        { 
            currentRoom.OnExitSelected -= HandleExitSelected;
            currentRoom.OnEnterTrigger -= HandleEnterRoomTrigger;
        }

        if (currentCorridor)
        currentCorridor.OnExitTrigger -= HandleExitTrigger;

 
        // Finalize connections
        previousCorridor = currentCorridor;
        currentCorridor = corridorManager;
        previousRoom = currentRoom;
        currentRoom = roomManager;
        currentRoom.OnExitSelected += HandleExitSelected;
        currentRoom.OnEnterTrigger += HandleEnterRoomTrigger;
        currentCorridor.OnExitTrigger += HandleExitTrigger;
        currentRoom.playerCore = playerCore;
        currentRoom.ActivateRoom();

        roomNumber++;
        currentRoom.roomNumber = roomNumber;
        roomManager.InitializeDoors(this);
        //// Cleanup old room after delay
        //StartCoroutine(CleanupOldRoom());
    }

    //IEnumerator CleanupOldRoom()
    //{
    //    RoomManager oldRoom = currentRoom;
    //    yield return new WaitForSeconds(1f);
    //    if (oldRoom != null) Destroy(oldRoom.gameObject);
    //}
    void AlignRoomToCorridor(RoomManager room, CorridorManager corridor, RoomConnectionPoint roomExit)
    {
        // Get world space positions
        Vector3 corridorEndPos = corridor.connection.endAnchor.position;
        Quaternion corridorEndRot = corridor.connection.endAnchor.rotation;

        // Calculate offset from room's exit to corridor end
        Vector3 positionOffset = corridorEndPos - roomExit.spawnAnchor.position;
        room.transform.position += positionOffset;

        // Align rotation
        Quaternion rotationOffset = Quaternion.FromToRotation(
            roomExit.spawnAnchor.forward,
            corridorEndRot * Vector3.forward
        );
        room.transform.rotation = rotationOffset * room.transform.rotation;
    }

   public RoomConfiguration GetRandomRoomConfig(int targetTier)
    {
        foreach (var tierData in dungeonTiers)
        {
            if (tierData.tier == targetTier)
            {
                // Build eligible rooms list
                var eligibleRooms = GetEligibleRooms(tierData);
                if (eligibleRooms.Count == 0)
                {
                    // Reset tracking if no available rooms
                    spawnedRooms.Clear();
                    eligibleRooms = GetEligibleRooms(tierData);
                }

                var selectedRoom = GetWeightedRandom(eligibleRooms);
          
                return selectedRoom;
            }
        }
        return null;
    }

    List<RoomConfiguration> GetEligibleRooms(RoomTier tierData)
    {
        List<RoomConfiguration> eligibleRooms = new List<RoomConfiguration>();
        allPossibleRooms.Clear();

        // Collect rooms from all eligible packs
        foreach (var packConfig in tierData.roomConfigurations)
        {
            if (roomNumber >= packConfig.minNumberToSPawn &&
                roomNumber <= packConfig.maxNumberToSPawn)
            {
                allPossibleRooms.AddRange(packConfig.roomPack.roomConfigurations);
            }
        }

        // Filter out already spawned rooms
        foreach (var config in allPossibleRooms)
        {
            if (!spawnedRooms.Contains(config))
            {
                eligibleRooms.Add(config);
            }
        }

        return eligibleRooms;
    }

    CorridorConfiguration GetRandomCorridorConfig(int targetTier)
    {
        foreach (var tierData in dungeonTiers)
        {
            if (tierData.tier == targetTier)
            {
                return GetWeightedRandom(tierData.corridorConfigurations);
            }
        }
        return null;
    }

    T GetWeightedRandom<T>(List<T> items) where T : WeightedConfig
    {
        int totalWeight = 0;
        foreach (var item in items) totalWeight += item.weight;

        int randomPoint = Random.Range(0, totalWeight);
        foreach (var item in items)
        {
            if (randomPoint < item.weight) return item;
            randomPoint -= item.weight;
        }
        return items[0];
    }
}