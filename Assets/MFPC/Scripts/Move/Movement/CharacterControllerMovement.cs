using UnityEngine;


namespace MFPC.Movement
{
    public class CharacterControllerMovement : IMovement
    {
        public bool IsLockGravity { get; set; } = false;

        private CharacterController _characterController;
        private Transform _player;
        private Vector3 _moveDirection;
        private PlayerData _playerData;
        private bool _isGrounded;
        private float _verticalDirection;

        private Vector3 _lastDirection;
        private float _lastSpeed;
        private float _lastPlayerPositionY;

        public CharacterControllerMovement(Transform player, CharacterController characterController,
            PlayerData playerData)
        {
            _player = player;
            _characterController = characterController;
            _playerData = playerData;
        }

        public void MoveHorizontal(Vector3 direction, float speed = 1f)
        {
            direction.Normalize();

            if (_characterController.isGrounded || IsLockGravity)
            {
                _lastDirection = direction;
                _lastSpeed = speed;

                //Direction of movement
                _moveDirection = _player.transform.TransformDirection(new Vector3(direction.x, 0.0f, direction.z)) *
                                 speed;
            }
        }

        public void MoveVertical(Vector3 direction, float speed = 1f)
        {
            direction.Normalize();

            _verticalDirection = direction.y * speed;
        }

        public void MoveUpdate()
        {
            
            Gravity();

            _characterController.Move(GetPlayerDirection());
        }

        private void Gravity()
        {
            if (IsLockGravity) return;

            if (_characterController.isGrounded)
            {
                if (!_isGrounded) _verticalDirection = -0.01f;

                _isGrounded = true;
            }
            else
            {
                _verticalDirection -= _playerData.Gravity * Time.deltaTime;

                _isGrounded = false;
            }
        }

        private Vector3 GetPlayerDirection()
        {
            Vector3 inAirDirection;

            if (!_characterController.isGrounded && _playerData.AirControl && !IsLockGravity)
            {
                inAirDirection = _player.transform.TransformDirection(_lastDirection) * _lastSpeed;
                return new Vector3(inAirDirection.x, _verticalDirection, inAirDirection.z) *
                       Time.deltaTime;
            }
            else
            {
                return new Vector3(_moveDirection.x, _verticalDirection, _moveDirection.z) *
                       Time.deltaTime;
            }
        }
    }
}