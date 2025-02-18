// BreakableBox.cs (Example implementation)
using UnityEngine;
using System.Collections;


public class BreakableBox : HittableBase
{
    [SerializeField] private ParticleSystem breakEffect;
    [SerializeField] private Transform breakEffectPoint;

    [SerializeField] private float breakFragmentsForce;
    [SerializeField] private float timeBeforeBreak=0.1f;
    

    [SerializeField] private LootConfiguration lootConfig;

    [SerializeField] Transform fragmentsParent;
    [SerializeField] Transform lootSpawnPoint;
    [SerializeField] BoxSoundManager soundManager;
    [SerializeField] CoinsParticles coinSpawner;
    protected override void Break(Vector3 hitPoint)
    {
        StartCoroutine(WaitAndBreak());
       // base.Break();
    }

    IEnumerator WaitAndBreak()
    {
        yield return new WaitForSeconds(timeBeforeBreak);
        OnBreak();
    }
    protected void  OnBreak()
    {

        // Play effects

        if (coinSpawner != null)
        {
            coinSpawner.SpawnCoins(Random.Range(lootConfig.minCoins, lootConfig.maxCoins));
            coinSpawner.ReparentHigher();
        }

        float luck = 0;
        Item droppedItem = LootManager.GenerateBoxLoot(lootConfig, 1, luck, lootSpawnPoint.position);


        if (droppedItem != null)

        {
            if (droppedItem.raritySO.dropEffect != null)
            {
                Instantiate(droppedItem.raritySO.dropEffect, breakEffectPoint.position, Quaternion.identity);

            }
            if (droppedItem.raritySO.spawnSound.sound != null)
            {

                soundManager.PlaySound(droppedItem.raritySO.spawnSound);
            }
        }



        fragmentsParent.gameObject.SetActive(true);
        Vector3 globalPos = fragmentsParent.position;
        fragmentsParent.SetParent(null);
        fragmentsParent.position = globalPos;

        foreach (Rigidbody fragment in fragmentsParent.GetComponentsInChildren<Rigidbody>())
        {
            fragment.AddExplosionForce(breakFragmentsForce, transform.position, 10);
        }
        soundManager.transform.SetParent(fragmentsParent);
        soundManager.PlayBreakSound();

        Destroy(this.gameObject);


    }

    public override void TakeHit(HitData hitData)
    {
        base.TakeHit(hitData);

        soundManager.PlayHitSound();
    }





}