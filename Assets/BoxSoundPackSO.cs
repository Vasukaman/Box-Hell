using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tools/BoxSoundPackSO")]
public class BoxSoundPackSO : ScriptableObject
{


    public List<AudioClip> breakSounds;
    public List<AudioClip> hitSounds;


    public AudioClip PickBreakSound()
    {
        return breakSounds[Random.Range(0, breakSounds.Count)];
    } 
    
    public AudioClip PickHitSound()
    {
        return hitSounds[Random.Range(0, hitSounds.Count)];
    }


}

