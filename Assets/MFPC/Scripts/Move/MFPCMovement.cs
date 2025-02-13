using MFPC.Input;
using UnityEngine;

namespace MFPC
{
    /// <summary>
    /// The basic movement script allows you to move, turn the camera horizontally to direct the position
    /// </summary>
    public class MFPCMovement : PlayerGroundedState
    {
        public MFPCMovement(Player player, PlayerStateMachine stateMachine, PlayerData playerData, MFPCPlayerRotation playerRotation) : base(
            player, stateMachine, playerData, playerRotation)
        {
        }

        public override void Enter()
        {
            base.Enter();

            player.Input.OnJumpAction += OnJumpEvent;
        }

        public override void Update()
        {
            base.Update();

            MovePlayer();

            if (player.CharacterController.isGrounded)
            {
                //Set the current movement state
                player.ChangeMoveCondition(player.Input.MoveDirection != Vector2.zero ? MoveConditions.Walk : MoveConditions.Idle);

                if (player.Input.IsSprint) stateMachine.ChangeState(stateMachine.RunState);
            }
        }

        public override void Exit()
        {
            base.Exit();

            player.Input.OnJumpAction -= OnJumpEvent;
        }

        /// <summary>
        /// Responsible for moving the character using the "Character Controller" component
        /// </summary>
        private void MovePlayer()
        {
            player.Movement.MoveHorizontal(new Vector3(player.Input.MoveDirection.x, 0.0f, player.Input.MoveDirection.y),
                    playerData.WalkSpeed);
        }
    }
}