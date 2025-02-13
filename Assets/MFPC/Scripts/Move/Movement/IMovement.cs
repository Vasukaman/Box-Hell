using UnityEngine;

namespace MFPC.Movement
{
    public interface IMovement
    {
        bool IsLockGravity { get; set; }

        /// <summary>
        /// Direction of movement: Horizontal
        /// </summary>
        /// <param name="direction">Pass values only in x and z</param>
        /// <param name="speed">Direction speed</param>
        void MoveHorizontal(Vector3 direction, float speed = 1f);

        /// <summary>
        /// Sets the character's direction either up or down
        /// </summary>
        /// <param name="direction">Pass values only to Vector3.up or Vector3.down or Vector3.zero</param>
        /// <param name="speed">Direction speed</param>
        void MoveVertical(Vector3 direction, float speed = 1f);

        /// <summary>
        /// Sets the character to move every frame
        /// </summary>
        void MoveUpdate();
    }
}