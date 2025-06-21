using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxTeleporter : MonoBehaviour
{

    [Header("Teleporter Settings")]
    [SerializeField] private Transform destination; // Where objects will be teleported
    [SerializeField] private LayerMask teleportableLayers = ~0; // All layers by default
    [SerializeField] private float cooldown = 0.5f; // Prevent immediate re-teleportation

    [Header("Orientation")]
    [SerializeField] private bool maintainRelativePosition = true;
    [SerializeField] private bool maintainRelativeRotation = true;
    [SerializeField] private bool maintainVelocity = true;

    private float lastTeleportTime;
    private Transform playerCamera;

    private void Start()
    {
        playerCamera = Camera.main.transform;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if cooldown is active
        if (Time.time < lastTeleportTime + cooldown) return;

        // Check if object is on teleportable layer
        int layerMask = 1 << other.gameObject.layer;
        if ((teleportableLayers.value & layerMask) == 0) return;

        Rigidbody box = other.attachedRigidbody;
       
        if (box.GetComponent<BreakableBox>())
        TeleportObject(box.transform);

    }

    public void TeleportObject(Transform objectToTeleport)
    {
        if (destination == null)
        {
            Debug.LogWarning("Teleporter destination not set!", this);
            return;
        }
       
        // Calculate relative position/rotation
        Vector3 relativePosition = maintainRelativePosition ?
            transform.InverseTransformPoint(objectToTeleport.position) : Vector3.zero;

        Quaternion relativeRotation = maintainRelativeRotation ?
            Quaternion.Inverse(transform.rotation) * objectToTeleport.rotation : Quaternion.identity;

        // Apply teleportation
        objectToTeleport.position = maintainRelativePosition ?
            destination.TransformPoint(relativePosition) : destination.position;

        objectToTeleport.rotation = maintainRelativeRotation ?
            destination.rotation * relativeRotation : destination.rotation;

        // Handle physics
        HandlePhysics(objectToTeleport);

        lastTeleportTime = Time.time;
    }

    private void HandlePhysics(Transform obj)
    {
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb && maintainVelocity)
        {
            // Transform velocity to new orientation
            Vector3 relativeVelocity = transform.InverseTransformDirection(rb.velocity);
            rb.velocity = destination.TransformDirection(relativeVelocity);

            // Transform angular velocity
            Vector3 relativeAngularVelocity = transform.InverseTransformDirection(rb.angularVelocity);
            rb.angularVelocity = destination.TransformDirection(relativeAngularVelocity);
        }
    }

    // Optional: Draw gizmos in editor
    private void OnDrawGizmosSelected()
    {
        if (destination != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, destination.position);
            Gizmos.DrawWireSphere(destination.position, 0.5f);
        }
    }
}
