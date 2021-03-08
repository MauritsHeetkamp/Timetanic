using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class Options : MonoBehaviour
{
    [SerializeField] AudioMixer audioMixer; // Handles the audio globally
    public string masterAudioString = "Master";
    public string sfxAudioString = "SFX";
    public string backgroundAudioString = "Background";

    public GameObject qualitySettingButton;
    [SerializeField] UIButtonArray qualitySettings;

    [SerializeField] Slider sfxSlider, backgroundMusicSlider, masterVolumeSlider;


    private void Start()
    {
        Initialize();
        QualitySettings.pixelLightCount = 10;
        Debug.Log(QualitySettings.pixelLightCount);
    }

    void LoadAudio()
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

    public virtual void Initialize()
    {
        LoadAudio();

        if(qualitySettings != null)
        {
            List<UISubOptionButton> newButtons = new List<UISubOptionButton>();
            Debug.Log("Initialized");
            for(int i = 0; i < QualitySettings.names.Length; i++)
            {
                int index = i;
                UISubOptionButton newButton = Instantiate(qualitySettingButton, qualitySettings.buttonHolder).GetComponent<UISubOptionButton>();
                newButton.buttonText.text = QualitySettings.names[index];
                newButton.thisButton.onClick.AddListener(() => SetQualityLevel(index));
                newButtons.Add(newButton);
            }

            RectTransform buttonHolder = qualitySettings.buttonHolder.GetComponent<RectTransform>();
            Vector2 newButtonSize = new Vector2(buttonHolder.rect.width / newButtons.Count, buttonHolder.rect.height);

            Debug.Log(buttonHolder.sizeDelta);

            foreach(UISubOptionButton button in newButtons)
            {
                button.GetComponent<RectTransform>().sizeDelta = newButtonSize;

            }

            qualitySettings.selectedButton = QualitySettings.GetQualityLevel();
            qualitySettings.Initialize(newButtons.ToArray());
        }
    }

    // Sets the quality level
    public void SetQualityLevel(int qualityLevelIndex)
    {
        Debug.Log(" QUALITY LEVEL IS NOW " + qualityLevelIndex);
        QualitySettings.SetQualityLevel(qualityLevelIndex, true);
        Save();
    }

    // Sets the master volume
    public void SetMasterVolume(Slider volumeSlider)
    {
        audioMixer.SetFloat(masterAudioString, volumeSlider.value);
        float value;
        audioMixer.GetFloat(masterAudioString, out value);
        PlayerPrefs.SetFloat(masterAudioString, value);
    }

    // Sets the sfx volume
    public void SetSFXVolume(Slider volumeSlider)
    {
        audioMixer.SetFloat(sfxAudioString, volumeSlider.value);
        float value;
        audioMixer.GetFloat(sfxAudioString, out value);
        PlayerPrefs.SetFloat(sfxAudioString, value);
    }

    // Sets the background volume
    public void SetBackgroundVolume(Slider volumeSlider)
    {
        audioMixer.SetFloat(backgroundAudioString, volumeSlider.value);
        float value;
        audioMixer.GetFloat(backgroundAudioString, out value);
        PlayerPrefs.SetFloat(backgroundAudioString, value);
    }

    void Save()
    {
        SaveSystem.instance.Save();
    }
}
