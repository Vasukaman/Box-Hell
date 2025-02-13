using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolSoundManager : MonoBehaviour
{
    public ToolSoundPackSO soundPack;
    public AudioSource audioSource;
    public void PlayPrepareSound()
    {
        audioSource.PlayOneShot(soundPack.PickPrepareSound());
    }
    public void PlayHitSound()
    {
        audioSource.PlayOneShot(soundPack.PickHitSound());
    }

    public void PlaySwingSound()
    {
        audioSource.PlayOneShot(soundPack.PickSwingSound());
    }
    public void PlayReturnSound()
    {
        audioSource.PlayOneShot(soundPack.PickReturnSound());
    }
}


