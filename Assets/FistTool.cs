using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FistTool : RegularMeleeTool
{
    // Start is called before the first frame update
    private bool canUse = true;
    [SerializeField] private float timeToReload;
    [SerializeField] private PlayerCore playerCore;

    private void Awake()
    {
        itemCore.owner = playerCore;
    }
    public override void Use()
    {
        if (!canUse) return;

        currentDurability = 100;

        canUse = false;

        StartCoroutine(PauseAndAllowHit());

        TryAttacking();
        base.Use();

     
    }

    IEnumerator PauseAndAllowHit()
    {
        yield return new WaitForSeconds(timeToReload);
        canUse = true;
    }

}
