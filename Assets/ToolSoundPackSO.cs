using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tools/ToolSoundPackSO")]
public class ToolSoundPackSO : ScriptableObject
{


    public List<AudioClip> prepareSounds;
    public List<AudioClip> swingSounds;
    public List<AudioClip> hitSounds;
    public List<AudioClip> returnSounds;

    public AudioClip PickPrepareSound()
    {
        if (prepareSounds.Count == 0) return null;
        return prepareSounds[Random.Range(0, prepareSounds.Count)];
    } 
    
    public AudioClip PickSwingSound()
    {
        if (swingSounds.Count == 0) return null;    
        return swingSounds[Random.Range(0, swingSounds.Count)];
    }  
    public AudioClip PickHitSound()
    {
        if (hitSounds.Count == 0) return null;
        return hitSounds[Random.Range(0, hitSounds.Count)];
    }    
    public AudioClip PickReturnSound()
    {
        if (returnSounds.Count == 0) return null;
        return returnSounds[Random.Range(0, returnSounds.Count)];
    }

}

