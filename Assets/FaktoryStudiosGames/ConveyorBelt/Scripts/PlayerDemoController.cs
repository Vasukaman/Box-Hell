using UnityEngine;

namespace FaktoryStudiosGames {
    /// <summary>
    /// Very basic Player Controller for demo purposes only. WASD controls with Space to jump
    /// NOT game tested. Feel free to build on this and use it or toss it.
    /// Does NOT use new input system.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class PlayerDemoController : MonoBehaviour
    {
        public float moveSpeed = 5f;
        public float jumpHeight = 2f;
        public float gravity = -9.81f;
        public float conveyorStickiness = 5f; // Higher = stick more aggressively

        private CharacterController controller;
        private Vector3 velocity;
        private Vector3 conveyorForce = Vector3.zero;
        private Vector3 desiredConveyorForce = Vector3.zero;
        private bool isGrounded;
        private bool touchingConveyor = false;

        void Start()
        {
            controller = GetComponent<CharacterController>();
        }

        void Update()
        {
            isGrounded = controller.isGrounded;
            if (isGrounded && velocity.y < 0)
                velocity.y = -2f;

            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");

            Vector3 move = transform.right * x + transform.forward * z;
            if (!Mathf.Approximately(move.magnitude, 0)) {
                controller.Move(((move * moveSpeed) + conveyorForce) * Time.deltaTime);
            }

            if (Input.GetButtonDown("Jump") && isGrounded)
                velocity.y = Mathf.Sqrt(Mathf.Max(jumpHeight * -2f * gravity, 0f));

            velocity.y += gravity * Time.deltaTime;
            velocity.y = Mathf.Max(velocity.y, -20f);
            controller.Move(velocity);

            // Reset for next frame
            desiredConveyorForce = Vector3.zero;
            touchingConveyor = false;
        }

        void OnTriggerStay(Collider other)
        {
            if (other.tag.Equals("ConveyorBelt")) {
                controller.Move(other.GetComponent<ConveyorBelt>().GetSpeed() * Time.deltaTime);
            }
        }
    }
}