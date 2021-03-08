using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuOptions : Options
{
    [SerializeField] Slider sfxSlider, backgroundMusicSlider, masterVolumeSlider;

    // Start is called before the first frame update
    void Start()
    {
        masterVolumeSlider.value = PlayerPrefs.GetFloat(masterAudioString);
        backgroundMusicSlider.value = PlayerPrefs.GetFloat(backgroundAudioString);
        sfxSlider.value = PlayerPrefs.GetFloat(sfxAudioString);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
