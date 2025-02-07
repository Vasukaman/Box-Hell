using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnOnTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        PlayerCore player = other.GetComponent<PlayerCore>();
        if (player == null) return;

        player.Respawn();
    }
}
