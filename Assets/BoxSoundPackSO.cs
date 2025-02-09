using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tools/BoxSoundPackSO")]
public class BoxSoundPackSO : ScriptableObject
{


    public List<SoundData> breakSounds;
    public List<SoundData> hitSounds;


    public SoundData PickBreakSound()
    {
        return breakSounds[Random.Range(0, breakSounds.Count)];
    } 
    
    public SoundData PickHitSound()
    {
        return hitSounds[Random.Range(0, hitSounds.Count)];
    }


}

