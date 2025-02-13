using UnityEngine;

namespace MFPC
{
    public class PlayerStateMachine
    {
        /// <summary>
        /// Current executable state
        /// </summary>
        public PlayerState CurrentState { get; private set; }
        
        public MFPCMovement MovementState { get; private set; }
        public MFPCRun RunState { get; private set; }
        public MFPCJump JumpState { get; private set; }

        public PlayerStateMachine(Player player, PlayerData playerData, MFPCCameraRotation cameraRotation, MFPCPlayerRotation playerRotation)
        {
            MovementState = new MFPCMovement(player, this, playerData, playerRotation);
            if (playerData.IsAvailableState(PlayerStates.Run)) RunState = new MFPCRun(player, this, playerData, playerRotation);
            if (playerData.IsAvailableState(PlayerStates.Jump)) JumpState = new MFPCJump(player, this, playerData, playerRotation);
            
            Start(MovementState);
        }
        
        private void Start(PlayerState startingState)
        {
            CurrentState = startingState;
            CurrentState.Enter();
        }

        public bool ChangeState(PlayerState newState)
        {
            if(newState == null || !newState.IsChanged()) return false;
            
            CurrentState.Exit();
            CurrentState = newState;
            CurrentState.Enter();
            
            return true;
        }
    }
}