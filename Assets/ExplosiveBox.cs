using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class ExplosiveBox : BreakableBox
{
    [SerializeField] private ExplosionData boxExplosion;
    [SerializeField] private Transform explosionPoint;
    [SerializeField] private float timeBeforeExplosion = 0.1f;
    protected override void Break(Vector3 hitPoint)
    {

        StartCoroutine(PauseAndExplode(hitPoint));
       

     
    }

    IEnumerator PauseAndExplode(Vector3 hitPoint)
    {
        yield return new WaitForSeconds(timeBeforeExplosion);
        ExplosionFactory.Instance.CreateExplosion(explosionPoint.position, boxExplosion);
        base.Break(hitPoint);
    }
}
