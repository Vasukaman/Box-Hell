using UnityEngine;

public class TurretGun : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _muzzle;
    [SerializeField] private ParticleSystem _muzzleFlash;
    [SerializeField] private LineRenderer _laserPointer;
    [SerializeField] private GameObject _hitParticlePrefab;
    [SerializeField] private HitData hitData;

    [Header("Settings")]
    [SerializeField] private float _damage = 10f;
    [SerializeField] private float _laserMaxRange = 100f;
    [SerializeField] private LayerMask _shootLayers;

    private void Start()
    {
        InitializeLaser();
    }

    private void Update()
    {
        UpdateLaserPointer();
    }


    private void InitializeLaser()
    {
        if(_laserPointer != null)
        {
            _laserPointer.positionCount = 2;
            _laserPointer.SetPosition(0, _muzzle.position);
            _laserPointer.SetPosition(1, _muzzle.position + _muzzle.forward * _laserMaxRange);
        }
    }

    private void UpdateLaserPointer()
    {
        if(_laserPointer == null) return;

        Vector3 endPoint;
        if(Physics.Raycast(_muzzle.position, _muzzle.forward, out RaycastHit hit, 
            _laserMaxRange, _shootLayers))
        {
            endPoint = hit.point;
        }
        else
        {
            endPoint = _muzzle.position + _muzzle.forward * _laserMaxRange;
        }

        _laserPointer.SetPosition(0, _muzzle.position);
        _laserPointer.SetPosition(1, endPoint);
    }

    private void Shoot()
    {
        // Play muzzle flash
        _muzzleFlash.Play();
        
        // Perform raycast
        if(Physics.Raycast(_muzzle.position, _muzzle.forward, out RaycastHit hit, 
            _laserMaxRange, _shootLayers))
        {
            // Spawn hit particles
            if(_hitParticlePrefab != null)
            {
                Instantiate(_hitParticlePrefab, hit.point, 
                    Quaternion.LookRotation(hit.normal));
            }

            // Apply damage
            IHittable hittable = hit.collider.GetComponent<IHittable>();
            if (hittable==null) hittable = hit.collider.GetComponentInParent<IHittable>();
            hittable?.TakeHit(hitData);
        }
    }
}