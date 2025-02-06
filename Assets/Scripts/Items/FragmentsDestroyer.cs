using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FragmentsDestroyer : MonoBehaviour
{
    private float secondsToDestroy = 2f;

    // Start is called before the first frame update
    Collider[] fragments_col;
    private void OnEnable()

    {
        fragments_col = GetComponentsInChildren<Collider>();
        StartCoroutine(WaitAndDestroy());
      
    }
    IEnumerator WaitAndDestroy()
    {
        yield return new WaitForSeconds(secondsToDestroy);
        foreach (Collider fragment in fragments_col)
        {
            fragment.isTrigger = true;
        }
        yield return new WaitForSeconds(1);
        Destroy(this.gameObject);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
