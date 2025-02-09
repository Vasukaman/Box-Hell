using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxSoundManager : MonoBehaviour
{
    [SerializeField] BoxSoundPackSO soundPack;
    public AudioSource audioSource;
    public void PlayBreakSound()
    {
        PlaySound(soundPack.PickBreakSound());
    }
    public void PlaySound(SoundData audio)
    {if (audio.sound == null) return;
        audioSource.PlayOneShot(audio.sound, audio.volume);
    }
    public void PlayHitSound()
    {
        PlaySound(soundPack.PickHitSound());
    }
}
