using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorAnimatorController : MonoBehaviour
{
    [SerializeField] private Animation anim;
    [SerializeField] private AnimationClip openAnimClip;
    [SerializeField] private AnimationClip closeAnimClip;
    [SerializeField] DoorController door;

    private void Awake()
    {
        door.OnDoorInteracted += HandleDoorOpen;
        door.OnDoorClosed += CloseDoor;

        anim.AddClip(openAnimClip, "OpenDoor");
        anim.AddClip(closeAnimClip, "CloseDoor");
    }
    public void HandleDoorOpen(DoorController door, Door doorData)
    {
        OpenDoor();
    }
    public void OpenDoor()
    {
        Debug.Log("OPENING DOOR");
        anim.Play("OpenDoor");
    }

    public void CloseDoor()
    {
        
        anim.Play("CloseDoor"); 
    }

    private void OnDestroy()
    {
        door.OnDoorInteracted -= HandleDoorOpen;
        door.OnDoorClosed -= CloseDoor;
    }

}
