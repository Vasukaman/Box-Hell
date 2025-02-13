using MFPC.Movement;
using UnityEngine;

namespace MFPC
{
    public abstract class PlayerState
    {
        protected Player player;
        protected PlayerStateMachine stateMachine;
        protected PlayerData playerData;
        protected MFPCPlayerRotation playerRotation;

        public PlayerState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, MFPCPlayerRotation playerRotation)
        {
            this.player = player;
            this.stateMachine = stateMachine;
            this.playerData = playerData;
            this.playerRotation = playerRotation;
        }

        // True if it is possible to go to this state
        public virtual bool IsChanged() => true;

        public virtual void Enter()
        {
        }

        public virtual void Exit()
        {
        }

        public virtual void Update()
        {
        }

        protected void PlaySound(AudioClip audioClip)
        {
            if(audioClip != null) player.AudioSource.PlayOneShot(audioClip);
        }
    }
}