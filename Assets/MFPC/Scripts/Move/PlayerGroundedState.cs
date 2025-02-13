namespace MFPC
{
    public class PlayerGroundedState : PlayerState
    {
        public PlayerGroundedState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, MFPCPlayerRotation playerRotation) : base(
            player, stateMachine, playerData, playerRotation)
        {
        }
        
        public override void Update()
        {
            playerRotation.UpdatePlayerRotation();
            
            if (player.CharacterController.isGrounded)
            {
                if (player.CurrentMoveCondition == MoveConditions.Fall) player.ChangeMoveCondition(MoveConditions.Fell);
            }
            else player.ChangeMoveCondition(MoveConditions.Fall);
        }
        
        #region Callback

        protected void OnJumpEvent()
        {
            stateMachine.ChangeState(stateMachine.JumpState);
        }
        
        #endregion
    }
}