using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
struct TurretSoundPack
{
    public SoundData idleSound;
    public SoundData moveSound;
    public SoundData foundPlayerSound;
    public SoundData lostPlayerSound;
    public SoundData aimAtPlayerSound;
    public SoundData prepareToShootSound;

}   

public class TurretSoundManager : MonoBehaviour
{
    [SerializeField] TurretSoundPack soundPack;
    public AudioSource audioSource;
    

    //THE LAYZIEST THING I COULD HAVE DONE
    public void PlayTurretSound(string name)
    {
        switch(name)
        {
           case "Idle": PlaySound(soundPack.idleSound); break;
           case "Move": PlaySound(soundPack.moveSound); break;
           case "FoundPlayer": PlaySound(soundPack.foundPlayerSound); break;
           case "LostPlayer": PlaySound(soundPack.lostPlayerSound); break;
           case "AimAtPlayer": PlaySound(soundPack.aimAtPlayerSound); break;
           case "PrepareToShoot": PlaySound(soundPack.prepareToShootSound); break;
        }

    }
    
    
    private void PlaySound(SoundData audio)
    {
        if (audio.sound == null) return;
        audioSource.PlayOneShot(audio.sound, audio.volume);
    }


}
