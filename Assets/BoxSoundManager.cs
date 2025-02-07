using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxSoundManager : MonoBehaviour
{
    [SerializeField] BoxSoundPackSO soundPack;
    public AudioSource audioSource;
    public void PlayBreakSound()
    {
        audioSource.PlayOneShot(soundPack.PickBreakSound());
    }
    public void PlaySound(AudioClip audio)
    {if (audio == null) return;
        audioSource.PlayOneShot(audio);
    }
    public void PlayHitSound()
    {
        audioSource.PlayOneShot(soundPack.PickHitSound());
    }
}
