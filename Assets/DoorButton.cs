using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorButton : MonoBehaviour, IHittable
{
    [SerializeField] private DoorController door;
    public void TakeHit(HitData hitData)
    {
        PlayerCore player = hitData.sourseItem.owner;
     //   if (player == null) return;

        if (door.currentState == DoorState.Unlocked)
        if (player.TryBuying(door.price))
        {
            door.TryOpenningDoor();
        }
    }
}
