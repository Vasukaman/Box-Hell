using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MilkShake;

public class AxeTool : Tool
{
    [SerializeField] private float range = 5f;


    [Header("Damage")]
    [SerializeField] protected float damage = 100f;
    [SerializeField] protected LayerMask damageableLayers;
    [SerializeField]  private Camera mainCamera;

    [SerializeField] private MilkShake.ShakePreset cameraShakePreset;


    private void Start()
    {
        mainCamera = Camera.main;

    }

    protected void TryDamage(RaycastHit hit)
    {
        var damageable = hit.collider.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(damage, item);
            Shaker.ShakeAll(cameraShakePreset);
        }
    }


    public override void Use()
    {
        base.Use();
        //PerformRaycast();
        Debug.Log($"Axe used");
        TryAttacking();
    }

    private void TryAttacking()
    {

        mainCamera = Camera.main;
        Ray ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, range, damageableLayers))
        {
            TryDamage(hit);
            Debug.Log($"Hit {hit.collider.name} with ");
            // Add your interaction logic here
        }
    }
}



