using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class ExplosiveBox : BreakableBox
{
    [SerializeField] private ExplosionData boxExplosion;
    protected override void Break(Vector3 hitPoint)
    {
       

        ExplosionFactory.Instance.CreateExplosion(transform.position, boxExplosion);

        base.Break(hitPoint);
    }
}
