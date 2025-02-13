using UnityEngine;

namespace MFPC
{
    /// <summary>
    /// Animates the camera based on the MoveParameterNamement state
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class MFPCCameraAnimation : MonoBehaviour
    {
        #region Const

        /// <summary>
        /// Animation parameter
        /// </summary>
        private const string MoveParameterName = "Move";

        /// <summary>
        /// Names animations
        /// </summary>
        private const string JumpNameAnimation = "Jump";
        private const string FellNameAnimation = "Fell";

        #endregion
        
        [SerializeField] private Player player;
        
        /// <summary>
        /// How smoothly the MoveParameterNamement animations will change
        /// </summary>
        [Range(1f, 5.0f), SerializeField] private float changeAnimationSpeed = 3;

        /// <summary>
        /// The time after which the fall animation will play. This is necessary so that it is not performed from small falls
        /// </summary>
        [Range(0f, 10.0f), SerializeField] private float timeToFallAnimation = 0.6f;

        private float blendAnimation;
        private float currentBlendAnimation;
        private float fallTime;
        private Animator cameraAnimator;

        #region MONO

        private void Awake()
        {
            cameraAnimator = this.GetComponent<Animator>();
            fallTime = 0.0f;
        }

        private void OnEnable() => player.OnMoveCondition += OnMoveAnimation;
        private void OnDisable() => player.OnMoveCondition -= OnMoveAnimation;

        #endregion

        private void Update()
        {
            //Smooth move animation
            currentBlendAnimation = Mathf.Lerp(currentBlendAnimation, blendAnimation, Time.deltaTime * changeAnimationSpeed);
            cameraAnimator.SetFloat(MoveParameterName, currentBlendAnimation);

            if (fallTime > 0.0f) fallTime += Time.deltaTime;
        }

        #region CALLBACK

        /// <summary>
        /// Changes the animation based on the current Player state
        /// </summary>
        /// <param name="MoveParameterNameCondition">Current player state</param>
        private void OnMoveAnimation(MoveConditions MoveParameterNameCondition)
        {
            switch (MoveParameterNameCondition)
            {
                case MoveConditions.Idle:
                case MoveConditions.Sit:
                case MoveConditions.Climb:
                    blendAnimation = 0.0f;
                    break;
                case MoveConditions.Walk:
                    blendAnimation = 0.5f;
                    break;
                case MoveConditions.Run:
                    blendAnimation = 1.0f;
                    break;
                case MoveConditions.Jump:
                    cameraAnimator.SetTrigger(JumpNameAnimation);
                    break;
                case MoveConditions.Fell:
                    if (fallTime >= timeToFallAnimation) cameraAnimator.Play(FellNameAnimation);
                    fallTime = 0.0f;
                    break;
                case MoveConditions.Fall:
                    fallTime += Time.deltaTime;
                    break;
            }
        }

        #endregion
    }
}