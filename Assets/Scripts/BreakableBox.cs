// BreakableBox.cs (Example implementation)
using UnityEngine;
using System.Collections;


public class BreakableBox : DamageableBase
{
    [SerializeField] private ParticleSystem breakEffect;
    [SerializeField] private SBS.ME.MeshExploder meshExploder;
    [SerializeField] private float minExplosionSpeed;
    [SerializeField] private float maxExplosionSpeed;


    [SerializeField] private Rigidbody rigidBody;
    [SerializeField] private LootConfiguration lootConfig;



    protected override void Break()
    {
        // Play effects
        if (breakEffect != null)
        {
            Instantiate(breakEffect, transform.position, Quaternion.identity);
        }

        float luck = 0;
        LootManager.GenerateBoxLoot(lootConfig, 1, luck, transform.position);

        meshExploder.explosionInitSpeed = Random.Range(minExplosionSpeed, maxExplosionSpeed);
        meshExploder.EXPLODE();
        StartCoroutine(WaitAndDestroy());

       // base.Break();
    }

    IEnumerator WaitAndDestroy()
    {
        yield return new WaitForSeconds(5);

        Destroy(this.gameObject);
    }

    public override void TakePush(Vector3 position, Vector3 force)
    {
        if (!rigidBody) return;

        rigidBody.AddForceAtPosition(force, position, ForceMode.Impulse);
    }

    
}