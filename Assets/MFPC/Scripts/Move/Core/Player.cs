using System;
using UnityEngine;
using MFPC.Input.PlayerInput;
using MFPC.Movement;
using MFPC.Utils;

namespace MFPC
{
    [RequireComponent(typeof(CharacterController), typeof(AudioSource))]
    public class Player : MonoBehaviour
    {
        public event Action<MoveConditions> OnMoveCondition;

        [SerializeField] private PlayerData playerData;
        [SerializeField] private MFPCCameraRotation cameraRotation;

        // StateMachine - Designed to change the player's state
        public PlayerStateMachine StateMachine { get; private set; }
        public CharacterController CharacterController { get; private set; }
        public AudioSource AudioSource { get; private set; }
        public IPlayerInput Input { get => InputTuner.CurrentPlayerInputHandler; }
        public PlayerInputTuner InputTuner { get; private set; }
        public MoveConditions CurrentMoveCondition { get; private set; }
        public IMovement Movement { get; private set; }

        private MFPCPlayerRotation playerRotation;

        public void Initialize(PlayerInputTuner playerInputTuner)
        {
            InputTuner = playerInputTuner;
            CharacterController = this.GetComponent<CharacterController>();
            AudioSource = this.GetComponent<AudioSource>();

            playerRotation = new MFPCPlayerRotation(this.transform, Input, InputTuner);
            Movement = new CharacterControllerMovement(this.transform, CharacterController, playerData);
            StateMachine = new PlayerStateMachine(this, playerData, cameraRotation, playerRotation);
        }

        private void Update()
        {
            StateMachine.CurrentState.Update();
            Movement.MoveUpdate();
        }

        /// <summary>
        /// Changes the current state of movement if the old state is different
        /// </summary>
        /// <param name="newMoveCondition">Current state of movement</param>
        public void ChangeMoveCondition(MoveConditions newMoveCondition)
        {
            if (newMoveCondition == CurrentMoveCondition) return;

            CurrentMoveCondition = newMoveCondition;
            OnMoveCondition?.Invoke(CurrentMoveCondition);
        }

        public void SetRotation(float angle)
        {
            playerRotation.SetRotation = angle;
        }

        public void SetPosition(Vector3 position)
        {
            CharacterController.Transfer(position);
        }
    }
}