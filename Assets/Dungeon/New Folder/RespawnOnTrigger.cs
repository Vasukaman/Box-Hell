using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnOnTrigger : MonoBehaviour
{
    [SerializeField] private bool damage = true;
    private void OnTriggerEnter(Collider other)
    {
        PlayerCore player = other.GetComponent<PlayerCore>();
        if (player == null) return;

        if (damage)
            player.TakeDamage();
        
        player.Respawn();
    }
}
