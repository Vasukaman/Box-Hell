using UnityEngine;
using System.Collections.Generic;

public class ScannerVisual : MonoBehaviour
{
    [SerializeField] private TurretMind _turretMind;
    [SerializeField] private Transform _rayOrigin;
    [SerializeField] private GameObject _lineRendererPrefab;
    [SerializeField] private float _minFOV = 5f;
    [SerializeField] private int _numRays = 10;
    [SerializeField] private float _updateInterval = 0.1f;
    [SerializeField] private float _rayDuration = 0.5f;
    [SerializeField] private float _rayMaxDistance = 50f;
    [SerializeField] private LayerMask _obstructionLayers;
    [SerializeField] private float _fovTransitionSpeed = 5f;

    [Header("Spotlight Settings")]
    [SerializeField] private Light _spotlight; // Reference to the spotlight
    [SerializeField] private float _maxSpotlightFOV = 60f; // Maximum FOV for the spotlight
    [SerializeField] private float _minSpotlightFOV = 10f; // Minimum FOV for the spotlight

    private float _currentFOV;
    private float _timer;
    private Queue<LineRenderer> _lineRendererPool = new Queue<LineRenderer>();
    private List<LineRenderer> _activeRays = new List<LineRenderer>();

    private void Start()
    {
        _currentFOV = _turretMind.Settings.fovAngle;
        _timer = _updateInterval;
        InitializePool();

        // Initialize spotlight FOV
        if (_spotlight != null)
        {
            _spotlight.spotAngle = _currentFOV;
        }
    }

    private void InitializePool()
    {
        for (int i = 0; i < 20; i++)
        {
            GameObject go = Instantiate(_lineRendererPrefab, _rayOrigin);
            LineRenderer lr = go.GetComponent<LineRenderer>();
            lr.enabled = false;
            _lineRendererPool.Enqueue(lr);
        }
    }

    private void Update()
    {
        UpdateCurrentFOV();
        UpdateRayTimer();
        UpdateActiveRays();
        UpdateSpotlightFOV(); // Update the spotlight's FOV
    }

    private void UpdateCurrentFOV()
    {
        float targetFOV = _turretMind.CurrentState == TurretState.Idle
            ? _turretMind.Settings.fovAngle
            : _minFOV;

        _currentFOV = Mathf.MoveTowards(_currentFOV, targetFOV, _fovTransitionSpeed * Time.deltaTime);
    }

    private void UpdateSpotlightFOV()
    {
        if (_spotlight != null)
        {
            // Map the scanner's FOV to the spotlight's FOV range
            float spotlightFOV = Mathf.Lerp(_minSpotlightFOV, _turretMind.Settings.fovAngle, _currentFOV / _turretMind.Settings.fovAngle);
            _spotlight.spotAngle = spotlightFOV;
            _spotlight.innerSpotAngle = spotlightFOV;
        }
    }

    private void UpdateRayTimer()
    {
        _timer -= Time.deltaTime;
        if (_timer <= 0)
        {
            GenerateRays();
            _timer = _updateInterval;
        }
    }

    private void GenerateRays()
    {
        for (int i = 0; i < _numRays; i++)
        {
            Vector3 direction = GetRandomDirectionWithinCone(_currentFOV);
            RaycastHit hit;
            Vector3 endPoint = _rayOrigin.position + direction * _rayMaxDistance;

            if (Physics.Raycast(_rayOrigin.position, direction, out hit, _rayMaxDistance, _obstructionLayers, QueryTriggerInteraction.Ignore))
            {
                endPoint = hit.point;
            }

            CreateRayLine(_rayOrigin.position, endPoint);
        }
    }

    private Vector3 GetRandomDirectionWithinCone(float coneAngle)
    {
        float halfAngle = coneAngle * 0.5f;
        Vector3 forward = _rayOrigin.forward;
        float cosHalfAngle = Mathf.Cos(halfAngle * Mathf.Deg2Rad);

        while (true)
        {
            Vector3 randomDir = Random.onUnitSphere;
            if (Vector3.Dot(randomDir, forward) >= cosHalfAngle)
            {
                return randomDir.normalized;
            }
        }
    }

    private void CreateRayLine(Vector3 start, Vector3 end)
    {
        if (_lineRendererPool.Count == 0) return;

        LineRenderer lr = _lineRendererPool.Dequeue();
        lr.enabled = true;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        _activeRays.Add(lr);

        StartCoroutine(ReturnToPoolAfterDelay(lr, _rayDuration));
    }

    private System.Collections.IEnumerator ReturnToPoolAfterDelay(LineRenderer lr, float delay)
    {
        yield return new WaitForSeconds(delay);
        lr.enabled = false;
        _activeRays.Remove(lr);
        _lineRendererPool.Enqueue(lr);
    }

    private void UpdateActiveRays()
    {
        // Optional: Additional updates to active rays (e.g., fading)
    }
}