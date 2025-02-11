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
    [SerializeField] private List<DecalTextureData> decals;
    [SerializeField] private bool spawnParticlesOnHitPos;
    [SerializeField] private Transform vfxParticlePosition;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private ToolSoundManager soundManager;
    [SerializeField] private List<ParticleSystem> hitVFX;



    protected  void Start()
    {
        mainCamera = Camera.main;
        itemCore.OnItemThrowed += ToolThrown;

    }

    DecalTextureData PickDecal()
    {
        return decals[Random.Range(0, decals.Count)];
    }
    protected void TryHit(RaycastHit hit)    
    {
        

        var damageable = hit.collider.GetComponentInParent<IHittable>();
        if (damageable != null)
        {
            Vector3 force = hit.point - mainCamera.transform.position;
            force = force.normalized * hitForce;
            DecalTextureData decal = new DecalTextureData();
            HitData hitData = new HitData(damage, itemCore, hit.point, force, decal);
            Vector3 pos = hit.point + hit.normal * 0.1f;
            DecalFactory.Instance.SpawnDecal(PickDecal(), pos, hit.normal,hit.collider.transform);
            damageable.TakeHit(hitData);
            Shaker.ShakeAll(hitShakePreset);


            if (spawnParticlesOnHitPos)
                SpawnHitVFX(pos, Quaternion.LookRotation(hit.normal));
            else
                SpawnHitVFX(vfxParticlePosition.position, vfxParticlePosition.rotation);


            ReduceDurabilityUse();

            PlayHitSound();
        }
    }   

    private void PlayHitSound()
    {
        soundManager.PlayHitSound();
    }  

    private ParticleSystem PickHitVFX()
    {
        return hitVFX[Random.Range(0, hitVFX.Count)];
    }

    private void SpawnHitVFX(Vector3 position, Quaternion rotation)
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



