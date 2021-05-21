using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Custom.Audio;

public class SoundManagerFunctions : MonoBehaviour
{
    public void StopBackgroundMusic(bool instant)
    {
        if(SoundManager.instance != null)
        {
            SoundManager.instance.EndBackgroundMusic(instant);
        }
    }

    public void SpawnAudio(AudioPrefabSO audio)
    {
        if (SoundManager.instance != null)
        {
            Destroy(SoundManager.instance.SpawnAudio(audio.audio.clip, audio.audio.volume, false), audio.audio.clip.length);
        }
    }

    public void ChangeBackgroundMusic(AudioPrefabSO music)
    {
        if (SoundManager.instance != null)
        {
            SoundManager.instance.SetBackgroundMusic(music.audio);
        }
    }
}
