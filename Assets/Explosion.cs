using UnityEngine;
using System.Collections.Generic;



public class Explosion : MonoBehaviour
{
    [SerializeField] private ExplosionData _explosionData;
    [SerializeField] private ParticleSystem _particles;

    private void Awake()
    {
        _particles = GetComponent<ParticleSystem>();
    }
    public void SetExplosionData(ExplosionData data)
    {
        _explosionData = data;
    }
    private void Start()
    {
        TriggerExplosion();
        Destroy(gameObject, _particles.main.duration);
    }

    private void TriggerExplosion()
    {
        // Find all colliders in explosion radius
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, _explosionData.radius, _explosionData.affectedLayers);

        HashSet<GameObject> processedObjects = new HashSet<GameObject>();

        foreach (Collider hitCollider in hitColliders)
        {
            GameObject target = hitCollider.attachedRigidbody != null ?
                hitCollider.attachedRigidbody.gameObject :
                hitCollider.gameObject;

            if (processedObjects.Contains(target)) continue;
            processedObjects.Add(target);

            // Calculate damage with falloff
            float distance = Vector3.Distance(transform.position, target.transform.position);
            float normalizedDistance = Mathf.Clamp01(distance / _explosionData.radius);
            float finalDamage = _explosionData.useDamageFalloff ?
                _explosionData.baseDamage * _explosionData.damageFalloff.Evaluate(normalizedDistance) :
                _explosionData.baseDamage;

            // Apply explosion effects
            if (target.TryGetComponent(out IDamageableByExplosion damageable))
            {
                damageable.TakeExplosionDamage(_explosionData, transform.position);
            }

            // Apply physics force
            if (_explosionData.applyForce && target.TryGetComponent(out Rigidbody rb))
            {
                Vector3 forceDirection = (target.transform.position - transform.position).normalized;
                rb.AddForce(forceDirection * _explosionData.force, ForceMode.Impulse);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (_explosionData.radius > 0)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _explosionData.radius);
        }
    }
}
