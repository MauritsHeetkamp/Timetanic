using System.Collections;
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
        Debug.Log("INIT");
        Initialize();
    }

    void AssignCallbacks()
    {
        if(masterVolumeSlider != null)
        {
            masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
        }

        if(backgroundMusicSlider != null)
        {
            backgroundMusicSlider.onValueChanged.AddListener(SetBackgroundVolume);
        }

        if(sfxSlider != null)
        {
            sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        }
    }

    void LoadSliders()
    {
        float val = 0;

        if(masterVolumeSlider != null)
        {
            audioMixer.GetFloat(masterAudioString, out val);
            val = Mathf.Pow(10.0f, val / 20.0f);
            masterVolumeSlider.value = val;
        }

        if(backgroundMusicSlider != null)
        {
            audioMixer.GetFloat(backgroundAudioString, out val);
            val = Mathf.Pow(10.0f, val / 20.0f);
            backgroundMusicSlider.value = val;
        }

        if(sfxSlider != null)
        {
            audioMixer.GetFloat(sfxAudioString, out val);
            val = Mathf.Pow(10.0f, val / 20.0f);
            sfxSlider.value = val;
        }
    }
    
    void LoadDropdowns()
    {
        Debug.Log("LOADING DROPDOWNS");
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
        float actualVolume = Mathf.Log10(volume) * 20;

        audioMixer.SetFloat(masterAudioString, actualVolume);
        PlayerPrefs.SetFloat(masterAudioString, volume);
    }

    // Sets the sfx volume
    public void SetSFXVolume(float volume)
    {
        float actualVolume = Mathf.Log10(volume) * 20;

        audioMixer.SetFloat(sfxAudioString, actualVolume);
        PlayerPrefs.SetFloat(sfxAudioString, volume);
    }

    // Sets the background volume
    public void SetBackgroundVolume(float volume)
    {
        float actualVolume = Mathf.Log10(volume) * 20;

        audioMixer.SetFloat(backgroundAudioString, actualVolume);
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
