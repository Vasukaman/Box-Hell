using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MilkShake;

public class ExplosionMeleeTool : RegularMeleeTool
{
    [SerializeField] protected ExplosionData explosionData;
    [SerializeField] protected float timeBeforeExplosion=0.1f;

    protected override void TryHit(RaycastHit hit)
    {

        var damageable = hit.collider.GetComponentInParent<IHittable>();
        if (damageable != null)
        {



            StartCoroutine(WaitAndExplode(hit.point));


          
        }
 
    }

    IEnumerator WaitAndExplode(Vector3 explosionPoint)
    {
        PlayHitSound();
        yield return new WaitForSeconds(timeBeforeExplosion);

        ExplosionFactory.Instance.CreateExplosion(explosionPoint, explosionData);
        BreakTool();
    }
}
