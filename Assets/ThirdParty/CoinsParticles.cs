using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinsParticles : MonoBehaviour
{
    public ParticleSystem part;

    void Start()
    {
        part = GetComponent<ParticleSystem>();
    }

    void OnParticleCollision(GameObject other)
    {
        PlayerCore player = other.GetComponent<PlayerCore>();
        if (player == null) return;

        player.GiveCoins(1);

    }

    public void SpawnCoins(int ammount)
    {
        part.Emit(ammount);
    }

    public void ReparentHigher()
    {
        transform.SetParent(transform.parent.parent);
    }
}
