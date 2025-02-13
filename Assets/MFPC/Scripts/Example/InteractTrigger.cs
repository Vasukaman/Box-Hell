using UnityEngine;

namespace MFPC.Example
{
    public abstract class InteractTrigger : MonoBehaviour
    {
        private const string PlayerTag = "Player";
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(PlayerTag)) TriggerAction(other);
        }

        protected abstract void TriggerAction(Collider other);
    }
}
