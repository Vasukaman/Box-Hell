// BreakableBox.cs (Example implementation)
using UnityEngine;

public class BreakableBox : DamageableBase
{
    [SerializeField] private ParticleSystem breakEffect;
    [SerializeField] private SBS.ME.MeshExploder meshExploder;
    [SerializeField] private float minExplosionSpeed;
    [SerializeField] private float maxExplosionSpeed;

    protected override void Break()
    {
        // Play effects
        if (breakEffect != null)
        {
            Instantiate(breakEffect, transform.position, Quaternion.identity);
        }

        meshExploder.explosionInitSpeed = Random.Range(minExplosionSpeed, maxExplosionSpeed);
        meshExploder.EXPLODE();


       // base.Break();
    }
}