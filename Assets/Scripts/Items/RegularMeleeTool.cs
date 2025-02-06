using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MilkShake;

public class RegularMeleeTool : Tool
{
    [SerializeField] private float range = 5f;


    [Header("Damage")]
    [SerializeField] protected float damage = 40f;
    [SerializeField] protected LayerMask damageableLayers;
    [SerializeField] private Camera mainCamera; //Probshould change how hit works

    [SerializeField] private MilkShake.ShakePreset hitShakePreset;

    [SerializeField] private float hitForce;


    private void Start()
    {
        mainCamera = Camera.main;

    }

    protected void TryDamage(RaycastHit hit)    
    {

        var damageable = hit.collider.GetComponentInParent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(damage, item, hit.transform.position);
            Shaker.ShakeAll(hitShakePreset);
        }
    }

    protected void TryImpulse(RaycastHit hit)
    {
        Debug.Log("Try Pushing");
        var damageable = hit.collider.GetComponentInParent<IDamageable>();
        Debug.Log(damageable);
        if (damageable != null)
        {
            Debug.Log("Pushing");
            Vector3 force = hit.point - mainCamera.transform.position;
            force = force.normalized * hitForce;
            damageable.TakePush(hit.point, force);
            Shaker.ShakeAll(hitShakePreset);
        }
    }

    public override void Use()
    {
        base.Use();
        //PerformRaycast();

        //  TryAttacking();

    }

    public void TryAttacking()
    {

        mainCamera = Camera.main;
        Ray ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, range, damageableLayers))
        {
            TryDamage(hit);
            TryImpulse(hit);
            Debug.Log($"Hit {hit.collider.name} with ");
            // Add your interaction logic here
        }
    }
}



