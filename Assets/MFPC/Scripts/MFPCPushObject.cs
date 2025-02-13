using UnityEngine;

namespace MFPC
{
    /// <summary>
    /// Allows you to interact with objects with a Ridgetbody component
    /// </summary>
    [RequireComponent(typeof(Player))]
    public class MFPCPushObject : MonoBehaviour
    {
        /// <summary>
        /// The force with which the object will be pushed
        /// </summary>
        [Range(0.1f, 10.0f), SerializeField] private float forcePush;
        
        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            Rigidbody body = hit.collider.attachedRigidbody;

            if (body != null && !body.isKinematic)
            {
                Vector3 forceDirection = hit.gameObject.transform.position - transform.position;
                forceDirection.Normalize();

                body.AddForceAtPosition(
                    forceDirection * forcePush, transform.position, ForceMode.Impulse);
            }
        }
    }
}