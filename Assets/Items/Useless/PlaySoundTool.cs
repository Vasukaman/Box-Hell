using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySoundTool : Tool
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private List<SoundData> sounds;
    public override void Use()
    {
        if (sounds.Count>0)
        audioSource.PlayOneShot(sounds[Random.Range(0, sounds.Count)].sound);//play random sound from list


        base.Use();
        

    }
}
