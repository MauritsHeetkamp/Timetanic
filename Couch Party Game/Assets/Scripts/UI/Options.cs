﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.Events;
using Custom.Types;

public class Options : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] AudioMixer audioMixer; // Handles the audio globally
    public string masterAudioString = "Master";
    public string sfxAudioString = "SFX";
    public string backgroundAudioString = "Background";
    [SerializeField] Slider sfxSlider, backgroundMusicSlider, masterVolumeSlider;

    [Header("Quality")]
    [SerializeField] UIDropdown qualitySettings;

    [Header("Resolution")]
    [SerializeField] UIDropdown resolutionSettings;

    [Header("Refresh Rate")]
    [SerializeField] UIDropdown refreshRateSettings;
    [SerializeField] int[] refreshRates;
    [SerializeField] int defaultRefreshRate = 60;

    [Header("Fullscreen")]
    [SerializeField] UIDropdown fullscreenSettings;
    private void Start()
    {
        Initialize();
    }

    void AssignCallbacks()
    {
        masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
        backgroundMusicSlider.onValueChanged.AddListener(SetBackgroundVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    void LoadSliders()
    {
        if (PlayerPrefs.HasKey(masterAudioString))
        {
            masterVolumeSlider.value = PlayerPrefs.GetFloat(masterAudioString);
        }
        else
        {
            if (masterVolumeSlider.minValue != 0 && masterVolumeSlider.maxValue != 0)
            {
                masterVolumeSlider.value = masterVolumeSlider.minValue + (masterVolumeSlider.maxValue - masterVolumeSlider.minValue) / 2;
            }
        }

        if (PlayerPrefs.HasKey(backgroundAudioString))
        {
            Debug.Log(PlayerPrefs.GetFloat(backgroundAudioString));
            backgroundMusicSlider.value = PlayerPrefs.GetFloat(backgroundAudioString);
        }
        else
        {
            if (backgroundMusicSlider.minValue != 0 && backgroundMusicSlider.maxValue != 0)
            {
                backgroundMusicSlider.value = backgroundMusicSlider.minValue + (backgroundMusicSlider.maxValue - backgroundMusicSlider.minValue) / 2;
            }
        }

        if (PlayerPrefs.HasKey(sfxAudioString))
        {
            sfxSlider.value = PlayerPrefs.GetFloat(sfxAudioString);
        }
        else
        {
            if (sfxSlider.minValue != 0 && sfxSlider.maxValue != 0)
            {
                sfxSlider.value = sfxSlider.minValue + (sfxSlider.maxValue - sfxSlider.minValue) / 2;
            }
        }
    }
    
    void LoadDropdowns()
    {
        if (qualitySettings != null)
        {
            List<DropdownData> data = new List<DropdownData>();
            for (int i = 0; i < QualitySettings.names.Length; i++)
            {
                int index = i;
                data.Add(new DropdownData(QualitySettings.names[i], () => SetQualityLevel(index)));
            }

            qualitySettings.selected = QualitySettings.GetQualityLevel();
            qualitySettings.Initialize(data.ToArray());
        }

        if (resolutionSettings != null)
        {
            List<DropdownData> data = new List<DropdownData>();

            for (int i = 0; i < Screen.resolutions.Length; i++)
            {
                Resolution resolution = Screen.resolutions[i];
                data.Add(new DropdownData(resolution.width + " x " + resolution.height, () => SetResolution(resolution)));

                if (resolution.width == Screen.currentResolution.width && resolution.height == Screen.currentResolution.height)
                {
                    resolutionSettings.selected = i;
                }
            }

            resolutionSettings.Initialize(data.ToArray());
        }

        if (refreshRateSettings != null)
        {
            List<DropdownData> data = new List<DropdownData>();

            foreach (int refreshRate in refreshRates)
            {
                data.Add(new DropdownData(refreshRate + "Hz", () => SetRefreshRate(refreshRate)));
            }

            if (PlayerPrefs.HasKey("RefreshRate"))
            {
                int refreshRate = PlayerPrefs.GetInt("RefreshRate");

                for (int i = 0; i < refreshRates.Length; i++)
                {
                    if (refreshRates[i] == refreshRate)
                    {
                        refreshRateSettings.selected = i;
                        break;
                    }
                }
            }
            else
            {
                for (int i = 0; i < refreshRates.Length; i++)
                {
                    if (refreshRates[i] == defaultRefreshRate)
                    {
                        refreshRateSettings.selected = i;
                        break;
                    }
                }
            }

            refreshRateSettings.Initialize(data.ToArray());
        }


        if (fullscreenSettings != null)
        {
            List<DropdownData> data = new List<DropdownData>();

            data.Add(new DropdownData("Off", () => SetFullscreen(false)));
            data.Add(new DropdownData("On", () => SetFullscreen(true)));

            fullscreenSettings.Initialize(data.ToArray());
        }
    }

    public virtual void Initialize()
    {
        LoadSliders();
        LoadDropdowns();
        AssignCallbacks();
    }

    public void SetFullscreen(bool value)
    {
        PlayerPrefs.SetInt("Fullscreen", value ? 1 : 0);
        Screen.fullScreen = value;
    }

    public void SetRefreshRate(int refreshRate)
    {
        PlayerPrefs.SetInt("RefreshRate", refreshRate);
        for (int i = 0; i < Screen.resolutions.Length; i++)
        {
            Resolution resolution = Screen.resolutions[i];
            resolution.refreshRate = refreshRate;
        }

        Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, Screen.fullScreen, refreshRate);
    }

    public void SetResolution(Resolution resolution)
    {
        PlayerPrefs.SetInt("ResolutionWidth", resolution.width);
        PlayerPrefs.SetInt("ResolutionHeight", resolution.height);
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    // Sets the quality level
    public void SetQualityLevel(int qualityLevelIndex)
    {
        QualitySettings.SetQualityLevel(qualityLevelIndex, true);
        PlayerPrefs.SetInt("QualityLevel", qualityLevelIndex);
    }

    // Sets the master volume
    public void SetMasterVolume(float volume)
    {
        audioMixer.SetFloat(masterAudioString, volume);
        PlayerPrefs.SetFloat(masterAudioString, volume);
    }

    // Sets the sfx volume
    public void SetSFXVolume(float volume)
    {
        audioMixer.SetFloat(sfxAudioString, volume);
        PlayerPrefs.SetFloat(sfxAudioString, volume);
    }

    // Sets the background volume
    public void SetBackgroundVolume(float volume)
    {
        audioMixer.SetFloat(backgroundAudioString, volume);
        PlayerPrefs.SetFloat(backgroundAudioString, volume);
    }

    void Save()
    {
        SaveSystem.instance.Save();
    }
}

namespace Custom.Types
{
    [System.Serializable]
    public struct DropdownData
    {
        public string name;
        public UnityAction onSelected;

        public DropdownData(string _name, UnityAction _onSelected)
        {
            name = _name;
            onSelected = _onSelected;
        }
    }
}
