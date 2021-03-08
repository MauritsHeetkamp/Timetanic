using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class Options : MonoBehaviour
{
    SaveSystem saveSystem;
    [SerializeField] AudioMixer audioMixer; // Handles the audio globally
    public string masterAudioString = "Master";
    public string sfxAudioString = "SFX";
    public string backgroundAudioString = "Background";

    // Sets the quality level
    public void SetQualityLevel(int qualityLevelIndex)
    {
        QualitySettings.SetQualityLevel(qualityLevelIndex, true);
        Save();
    }

    // Sets the master volume
    public void SetMasterVolume(Slider volumeSlider)
    {
        audioMixer.SetFloat(masterAudioString, volumeSlider.value);
        Save();
    }

    // Sets the sfx volume
    public void SetSFXVolume(Slider volumeSlider)
    {
        audioMixer.SetFloat(sfxAudioString, volumeSlider.value);
        Save();
    }

    // Sets the background volume
    public void SetBackgroundVolume(Slider volumeSlider)
    {
        audioMixer.SetFloat(backgroundAudioString, volumeSlider.value);
        Save();
    }

    void Save()
    {
        SaveSystem.instance.Save();
    }
}
