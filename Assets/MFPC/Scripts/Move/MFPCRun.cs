using UnityEngine;

namespace MFPC
{
    /// <summary>
    /// Allows the player to move forward only
    /// </summary>
    public class MFPCRun : PlayerGroundedState
    {
        public MFPCRun(Player player, PlayerStateMachine stateMachine, PlayerData playerData, MFPCPlayerRotation playerRotation) : base(
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

            RunPlayer();
            
            if (!player.Input.IsSprint) stateMachine.ChangeState(stateMachine.MovementState);
            
            if (player.CharacterController.isGrounded)
            {
                player.ChangeMoveCondition(MoveConditions.Run);
            }
        }

        public override void Exit()
        {
            base.Exit();

            player.Input.OnJumpAction -= OnJumpEvent;
        }

        private void RunPlayer()
        {
            player.Movement.MoveHorizontal(Vector3.forward, playerData.RunSpeed);
        }
    }
}