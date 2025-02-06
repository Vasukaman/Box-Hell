// BreakableBox.cs (Example implementation)
using UnityEngine;
using System.Collections;


public class BreakableBox : DamageableBase
{
    [SerializeField] private ParticleSystem breakEffect;
    [SerializeField] private Transform breakEffectPoint;
    [SerializeField] private float minExplosionSpeed;
    [SerializeField] private float maxExplosionSpeed;
    [SerializeField] private float breakFragmentsForce;

    [SerializeField] private Rigidbody rigidBody;
    [SerializeField] private LootConfiguration lootConfig;

    [SerializeField] Transform fragmentsParent;
    [SerializeField] Transform lootSpawnPoint;


    protected override void Break(Vector3 hitPoint)
    {
        // Play effects
     

        float luck = 0;
        Item droppedItem = LootManager.GenerateBoxLoot(lootConfig, 1, luck, lootSpawnPoint.position);


        if (droppedItem!=null)
        if (droppedItem.raritySO.dropEffect != null)
        {
            Instantiate(droppedItem.raritySO.dropEffect, breakEffectPoint.position, Quaternion.identity);
        }


        fragmentsParent.gameObject.SetActive(true);
        Vector3 globalPos =  fragmentsParent.position;
        fragmentsParent.SetParent(null);
        fragmentsParent.position = globalPos;

        foreach(Rigidbody fragment in fragmentsParent.GetComponentsInChildren<Rigidbody>())
        {
            fragment.AddExplosionForce(breakFragmentsForce, hitPoint, 10);
        }
        Destroy(this.gameObject);
      

       // base.Break();
    }

   

    public override void TakePush(Vector3 position, Vector3 force)
    {
        Debug.Log("Push");
        if (!rigidBody) return;
        Debug.Log("Pushed");
        rigidBody.AddForceAtPosition(force, position, ForceMode.Impulse);
    }

    
}