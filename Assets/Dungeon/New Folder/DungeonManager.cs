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

    void Start()
    {
        currentTier = 0;
        GenerateInitialRoom();
    }

    void GenerateInitialRoom()
    {
        if (initialRoom == null)
         initialRoom= GetRandomRoomConfig(currentTier);

        currentRoom = Instantiate(initialRoom.roomPrefab, worldAnchor).GetComponent<RoomManager>();
        currentRoom.tier = currentTier; // Set initial tier
        currentRoom.OnExitSelected += HandleExitSelected;
        playerCore.SetCurrentRoomManager(currentRoom);
        currentRoom.ActivateRoom();
    }

    void HandleExitSelected(RoomConnectionPoint exitPoint)
    {
        Debug.Log("Start spawn sequence");
        StartCoroutine(RoomTransitionSequence(exitPoint));

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

    System.Collections.IEnumerator RoomTransitionSequence(RoomConnectionPoint exitPoint)
    {
      

        // Determine next tier (example: increment by 1)
        int nextTier = currentTier + 1;
        if (nextTier >= dungeonTiers.Count) nextTier = dungeonTiers.Count - 1;

        var nextRoomConfig = GetRandomRoomConfig(nextTier);
        var corridorConfig = GetRandomCorridorConfig(currentTier);

        SpawnNewSection(exitPoint, nextRoomConfig, corridorConfig, nextTier);
        yield return new WaitForSeconds(0.1f);
        //Destroy(currentCorridor?.gameObject);
        // Destroy(currentRoom.gameObject);
    }
    void SpawnNewSection(RoomConnectionPoint exitPoint, RoomConfiguration nextRoomConfig, CorridorConfiguration corridorConfig, int newTier)
    {
        Debug.Log("SPAWNING");
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
        currentRoom.ActivateRoom();


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

    RoomConfiguration GetRandomRoomConfig(int targetTier)
    {
        foreach (var tierData in dungeonTiers)
        {
            if (tierData.tier == targetTier)
            {
                return GetWeightedRandom(tierData.roomConfigurations);
            }
        }
        return null;
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