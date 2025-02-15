using UnityEngine;
using System;

public class HittableDetector : MonoBehaviour
{
    [SerializeField] private LayerMask _detectionMask;

    private InventorySystem _inventory;
    private Camera _mainCamera;
    private IHittable _currentHittable;
    private float _lastDetectionDistance;

    // Events
    public event Action<IHittable, float> OnHittableDetected;
    public event Action OnHittableLost;
    public float currentRange;
    private void Awake()
    {
        _mainCamera = Camera.main;
        _inventory = FindObjectOfType<InventorySystem>();
    }

    private void Update()
    {
        //So, you I hope you refactored and rewrote this shit. You. Future me. Please!
        if (_inventory == null || _inventory.GetCurrentItem() == null)
        {
            HandleNoHittable();
            return;
        }

        currentRange = _inventory.GetCurrentItem().tool.range;
        _detectionMask = _inventory.GetCurrentItem().tool.damageableLayers;
        PerformLineDetection(currentRange);
    }
    private void PerformLineDetection(float maxRange)
    {
        Ray ray = new Ray(_mainCamera.transform.position, _mainCamera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, maxRange, _detectionMask))
        {
            IHittable hittable = hit.collider.GetComponent<IHittable>();
            if (hittable == null) hittable = hit.collider.GetComponentInParent<IHittable>(); //Some obj have collider and IHittable script ondifferent points in hierarchy. Hopefully this will be enough to fix the fackup of the architecture I creatted.

            if (hittable != null)
            {
                float distance = hit.distance;
                HandleHittableDetected(hittable, distance, maxRange);
                return;
            }
        }

        HandleNoHittable();
    }

    private void HandleHittableDetected(IHittable hittable, float distance, float maxRange)
    {
        if (_currentHittable == null || !ReferenceEquals(_currentHittable, hittable))
        {
            _currentHittable = hittable;
            OnHittableDetected?.Invoke(_currentHittable, distance);
        }
        else// if (Mathf.Abs(_lastDetectionDistance - distance) > 0.01f)
        {
            OnHittableDetected?.Invoke(_currentHittable, distance);
        }

        _lastDetectionDistance = distance;
    }

    private void HandleNoHittable()
    {
        if (_currentHittable != null)
        {
            _currentHittable = null;
            OnHittableLost?.Invoke();
        }
    }

    // Visualization for editor
    private void OnDrawGizmos()
    {
        if (_inventory != null && _inventory.GetCurrentItem() != null && _mainCamera != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(_mainCamera.transform.position, _mainCamera.transform.forward * _inventory.GetCurrentItem().tool.range);
        }
    }
}
