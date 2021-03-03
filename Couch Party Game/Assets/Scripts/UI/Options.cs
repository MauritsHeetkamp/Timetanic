using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class Options : MonoBehaviour
{
    [SerializeField] AudioMixer audioMixer; // Handles the audio globally
    [SerializeField] string masterAudioString = "Master";
    [SerializeField] string sfxAudioString = "SFX";
    [SerializeField] string backgroundAudioString = "Background";

    // Sets the quality level
    public void SetQualityLevel(int qualityLevelIndex)
    {
        QualitySettings.SetQualityLevel(qualityLevelIndex, true);
    }

    // Sets the master volume
    public void SetMasterVolume(Slider volumeSlider)
    {
        audioMixer.SetFloat(masterAudioString, volumeSlider.value);
    }

    // Sets the sfx volume
    public void SetSFXVolume(Slider volumeSlider)
    {
        audioMixer.SetFloat(sfxAudioString, volumeSlider.value);
    }

    // Sets the background volume
    public void SetBackgroundVolume(Slider volumeSlider)
    {
        audioMixer.SetFloat(backgroundAudioString, volumeSlider.value);
    }
}
