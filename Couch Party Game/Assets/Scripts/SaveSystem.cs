using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SaveSystem : MonoBehaviour
{
    public static bool resetPlayerprefs = false;

    public static SaveSystem instance;
    public int currentResWidth, currentResHeigth;
    [SerializeField] AudioMixer audio;
    [SerializeField] string sfxName, backgroundMusicName, masterAudioName;

    [SerializeField] bool defaultFullscreen = true;

    private void Awake()
    {
        if (resetPlayerprefs)
        {
            resetPlayerprefs = false;
            PlayerPrefs.DeleteAll();
        }
    }

    private void Start()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
        LoadSettings();
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    public void Save()
    {

    }

    public void LoadSettings()
    {
        if (PlayerPrefs.HasKey(masterAudioName))
        {
            float actualVolume = Mathf.Log10(PlayerPrefs.GetFloat(masterAudioName)) * 20;

            audio.SetFloat(masterAudioName, actualVolume);
        }
        if (PlayerPrefs.HasKey(backgroundMusicName))
        {
            float actualVolume = Mathf.Log10(PlayerPrefs.GetFloat(backgroundMusicName)) * 20;

            audio.SetFloat(backgroundMusicName, actualVolume);
        }
        if (PlayerPrefs.HasKey(sfxName))
        {
            float actualVolume = Mathf.Log10(PlayerPrefs.GetFloat(sfxName)) * 20;

            audio.SetFloat(sfxName, actualVolume);
        }

        if (PlayerPrefs.HasKey("QualityLevel"))
        {
            QualitySettings.SetQualityLevel(PlayerPrefs.GetInt("QualityLevel"));
        }

        if(PlayerPrefs.HasKey("ResolutionWidth") && PlayerPrefs.HasKey("ResolutionHeight"))
        {
            int width = PlayerPrefs.GetInt("ResolutionWidth");
            int heigth = PlayerPrefs.GetInt("ResolutionHeight");



            foreach(Resolution resolution in Screen.resolutions)
            {
                if(resolution.width == width && resolution.height == heigth)
                {
                    currentResWidth = width;
                    currentResHeigth = heigth;
                    Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
                    break;
                }
            }

            if(currentResWidth == 0 || currentResHeigth == 0)
            {
                currentResWidth = Screen.currentResolution.width;
                currentResHeigth = Screen.currentResolution.height;
            }
        }

        if (PlayerPrefs.HasKey("RefreshRate"))
        {
            int refreshRate = PlayerPrefs.GetInt("RefreshRate");
            for (int i = 0; i < Screen.resolutions.Length; i++)
            {
                Resolution resolution = Screen.resolutions[i];
                resolution.refreshRate = refreshRate;
            }

            Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, Screen.fullScreen, refreshRate);
        }

        if (PlayerPrefs.HasKey("Fullscreen"))
        {
            Screen.fullScreen = PlayerPrefs.GetInt("Fullscreen") == 0 ? false : true;
        }
        else
        {
            Screen.fullScreen = defaultFullscreen;
        }
    }

    void Initialize()
    {

    }
}
