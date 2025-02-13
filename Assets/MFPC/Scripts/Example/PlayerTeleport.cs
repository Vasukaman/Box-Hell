using UnityEngine;

namespace MFPC.Example
{
    public class PlayerTeleport : InteractTrigger
    {
        [SerializeField] private Transform teleportPoint;
        [SerializeField] private float playerRotationAngle;

        protected override void TriggerAction(Collider other)
        {
            if (other.TryGetComponent(out Player player))
            {
                player.SetPosition(teleportPoint.position);
                player.SetRotation(playerRotationAngle);
            }
        }
    }
}