using UnityEngine;
using System.Collections.Generic;
using MilkShake;


public class Explosion : MonoBehaviour
{
    [SerializeField] private ExplosionData _explosionData;
    [SerializeField] private ParticleSystem _particles;
    [SerializeField] private MilkShake.ShakePreset shakePreset;

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
            if (target.TryGetComponent(out IDamageableByExplosion damageasble))
            {
                damageasble.TakeExplosionDamage(_explosionData, transform.position);
            }

            // Apply physics force
            if (_explosionData.applyForce && target.TryGetComponent(out Rigidbody rb))
            {
                Debug.Log("Tried pushing " + rb);
                rb.AddExplosionForce(_explosionData.force, transform.position, _explosionData.radius*4, _explosionData.forceUpwardsModifier, ForceMode.Impulse);
            }
        }

        if (shakePreset!=null)
        Shaker.ShakeAll(shakePreset);
        _particles?.Play();
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
