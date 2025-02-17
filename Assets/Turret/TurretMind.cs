// TurretMind.cs
using UnityEngine;
using UnityEngine.Events;

public enum TurretState { Idle, Alert, Aiming, Shooting }

[RequireComponent(typeof(TurretGun))]
public class TurretMind : MonoBehaviour
{
    [SerializeField] private TurretMindSO _settings;
    [SerializeField] private Transform _head;
    [SerializeField] private Transform _gunPivot;

    public UnityEvent OnShoot;


    private float _currentRotationSpeed;
    private float _currentHorizontalSpeed;
    private float _currentVerticalSpeed;
    private Vector3 _lastTargetDirection;
    private float _rotationVelocity;

    private TurretState _currentState;
    [SerializeField] private Transform _player;
    private float _shootCooldown;
    private Quaternion _targetRotation;
    private Quaternion _idleStartRotation;
    private Quaternion _idleTargetA;
    private Quaternion _idleTargetB;
    private Quaternion _currentIdleTarget;
    private float _idleRotationProgress;

    [Header("Combat")]
    public float initialShotDelay = 1f; // Add this to TurretMindSO

    private float _initialShotTimer;
    private bool _hasFiredFirstShot;

    [SerializeField] private PlayerDetector _playerDetector;



    private void Awake()
    {
        InitializeIdleRotation();
        _playerDetector = GetComponent<PlayerDetector>();
        _playerDetector.OnPlayerDetected += HandlePlayerDetected;
        _playerDetector.OnPlayerLost += HandlePlayerLost;
    }

    private void HandlePlayerDetected(Transform player)
    {
        _player = player;
      
    }

    private void HandlePlayerLost()
    {
        _player = null;

    }

    private void InitializeIdleRotation()
    {
        _idleStartRotation = _head.rotation;

        // Clamp idle points to turret limits
        Vector2 clampedPointA = new Vector2(
            Mathf.Clamp(_settings.idlePointA.x, _settings.minHorizontalAngle, _settings.maxHorizontalAngle),
            Mathf.Clamp(_settings.idlePointA.y, _settings.minVerticalAngle, _settings.maxVerticalAngle)
        );

        Vector2 clampedPointB = new Vector2(
            Mathf.Clamp(_settings.idlePointB.x, _settings.minHorizontalAngle, _settings.maxHorizontalAngle),
            Mathf.Clamp(_settings.idlePointB.y, _settings.minVerticalAngle, _settings.maxVerticalAngle)
        );

        _idleTargetA = _idleStartRotation * Quaternion.Euler(clampedPointA.y, clampedPointA.x, 0);
        _idleTargetB = _idleStartRotation * Quaternion.Euler(clampedPointB.y, clampedPointB.x, 0);
        _currentIdleTarget = _idleTargetA;
    }


    private void Update()
    {
        switch (_currentState)
        {
            case TurretState.Idle:
                HandleIdleRotation();
                ScanForPlayer();
                break;

            case TurretState.Alert:
                TrackPlayer();
                break;

            case TurretState.Aiming:
                AimAtPlayer();
                if (!CanSeePlayer()) _currentState = TurretState.Idle;
                break;

            case TurretState.Shooting:
                HandleShooting();
                break;
        }
    }

    private void TrackPlayer()
    {
        Vector3 direction = _player.position - _head.position;
        _targetRotation = Quaternion.LookRotation(direction);

        if (Quaternion.Angle(_head.rotation, _targetRotation) < _settings.aimThreshold)
        {
            _currentState = TurretState.Aiming;
            _initialShotTimer = _settings.initialShotDelay; // Reset timer when entering aiming
            _hasFiredFirstShot = false;
        }
    }

    private void AimAtPlayer()
    {
        RotateToPlayer();

        if (Quaternion.Angle(_head.rotation, _targetRotation) < 1f)
        {
            if (!_hasFiredFirstShot)
            {
                // Count down initial delay
                _initialShotTimer -= Time.deltaTime;

                if (_initialShotTimer <= 0)
                {
                    _hasFiredFirstShot = true;
                    _currentState = TurretState.Shooting;
                    _shootCooldown = _settings.shootCooldown;
                }
            }
            else
            {
                _currentState = TurretState.Shooting;
                _shootCooldown = _settings.shootCooldown;
            }
        }
    }
    private void HandleIdleRotation()
    {
        // Smoothly rotate between idle points
        _head.rotation = Quaternion.RotateTowards(_head.rotation, _currentIdleTarget,
            _settings.idleRotationSpeed * Time.deltaTime);

        // Check if reached target
        if (Quaternion.Angle(_head.rotation, _currentIdleTarget) < 1f)
        {
            _currentIdleTarget = (_currentIdleTarget == _idleTargetA) ? _idleTargetB : _idleTargetA;
        }
    }

    private void ScanForPlayer()
    {
        if (CanSeePlayer())
        {
            _currentState = TurretState.Alert;
        }
    }

    private bool CanSeePlayer()
    {
        Vector3 directionToPlayer = _player.position - _head.position;
        float distance = directionToPlayer.magnitude;

        if (distance > _settings.detectionRange) return false;

        float angle = Vector3.Angle(_head.forward, directionToPlayer);
        if (angle > _settings.fovAngle / 2) return false;

        if (Physics.Raycast(_head.position, directionToPlayer, distance, _settings.obstructionLayers))
            return false;

        return true;
    }


    private void RotateToPlayer()
    {
        Vector3 direction = _player.position - _head.position;
        _targetRotation = Quaternion.LookRotation(direction);
        _head.rotation = Quaternion.RotateTowards(_head.rotation, _targetRotation,
          _settings.aimSpeed * Time.deltaTime);
    }



    private void HandleShooting()
    {

      
        _shootCooldown -= Time.deltaTime;

        if (_shootCooldown <= 0)
        {
            OnShoot?.Invoke();
            _shootCooldown = _settings.shootCooldown;
        }

        if (!CanSeePlayer())
        {
            _currentState = TurretState.Idle;
        }
        else 
        { RotateToPlayer(); }
    }
}