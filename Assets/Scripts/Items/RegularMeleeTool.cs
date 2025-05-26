using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MilkShake;

public class RegularMeleeTool : Tool
{


    [Header("Damage")]
   // [SerializeField] protected float damage = 40f;

    [SerializeField] protected Camera mainCamera; //Probshould change how hit works

    [SerializeField] protected MilkShake.ShakePreset hitShakePreset;

    [SerializeField] protected float hitForce;
    [SerializeField] protected List<DecalTextureData> decals;
    [SerializeField] protected bool spawnParticlesOnHitPos;
    [SerializeField] protected Transform vfxParticlePosition;
    [SerializeField] protected AudioSource audioSource;
    [SerializeField] protected ToolSoundManager soundManager;
    [SerializeField] protected List<ParticleSystem> hitVFX;


    public override void OnEquip()
    {
        base.OnEquip();
    }
    protected  void Start()
    {
        mainCamera = Camera.main;
        itemCore.OnItemThrowed += ToolThrown;
        itemCore.OnItemEquipped+=OnEquip;

    }

    protected DecalTextureData PickDecal()
    {
        return decals[Random.Range(0, decals.Count)];
    }
    protected virtual void TryHit(RaycastHit hit)    
    {
        

        var damageable = hit.collider.GetComponentInParent<IHittable>();
        if (damageable != null)
        {
            Vector3 force = hit.point - mainCamera.transform.position;
            force = force.normalized * hitForce;
            DecalTextureData decal = new DecalTextureData();
            HitData hitData = new HitData((int)damage, itemCore, hit.point, force, decal); //I don't want to fuckup weapon settings, so I will just make damage int here.
            Vector3 pos = hit.point + hit.normal * 0.1f;
            DecalFactory.Instance.SpawnDecal(PickDecal(), pos, hit.normal,hit.collider.transform);
            damageable.TakeHit(hitData);
            Shaker.ShakeAll(hitShakePreset);

            ReduceDurabilityUse();

            if (spawnParticlesOnHitPos)
                SpawnHitVFX(pos, Quaternion.LookRotation(hit.normal));
            else
                SpawnHitVFX(vfxParticlePosition.position, vfxParticlePosition.rotation);


 

            PlayHitSound();
        }
    }

    protected private void PlayHitSound()
    {
        soundManager.PlayHitSound();
    }

    protected ParticleSystem PickHitVFX()
    {
        return hitVFX[Random.Range(0, hitVFX.Count)];
    }

    protected void SpawnHitVFX(Vector3 position, Quaternion rotation)
    {
        ParticleSystem partSys = PickHitVFX();

       ParticleSystem particles =  Instantiate(partSys, position, rotation);
        particles.transform.localScale = partSys.transform.localScale;
    }

    protected void TryImpulse(RaycastHit hit)
    {
        //Debug.Log("Try Pushing");
        //var damageable = hit.collider.GetComponentInParent<IHittable>();
        //Debug.Log(damageable);
        //if (damageable != null)
        //{
        //    Debug.Log("Pushing");
        //    Vector3 force = hit.point - mainCamera.transform.position;
        //    force = force.normalized * hitForce;
        //    damageable.TakePush(hit.point, force);
        //    Shaker.ShakeAll(hitShakePreset);
        //}
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
            TryHit(hit);
            TryImpulse(hit);
            Debug.Log($"Hit {hit.collider.name} with ");
            // Add your interaction logic here
        }
    }
}



