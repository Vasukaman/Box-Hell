using UnityEngine;
using System.Collections.Generic;

public class TimedRoomManager : RoomManager
{
    [SerializeField] float timeLimit;
    [SerializeField] int requiredBoxes;
    float currentTime;
    int boxesBroken;

    void Update()
    {
        if (!isActive) return;

        currentTime += Time.deltaTime;
        if (currentTime >= timeLimit)
        {
            HandleTimeOut();
        }
    }

    public void RegisterBoxBreak()
    {
        boxesBroken++;
        if (boxesBroken >= requiredBoxes)
        {
            CompleteChallenge();
        }
    }

    void CompleteChallenge()
    {
        DeactivateRoom();
        SetDoorStates(DoorState.Unlocked);
    }

    void HandleTimeOut()
    {
        // Penalty logic
        DeactivateRoom();
    }
}