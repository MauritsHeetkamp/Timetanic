using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SaveSystem : MonoBehaviour
{
    public static SaveSystem instance;
    [SerializeField] AudioMixer audio;
    [SerializeField] string sfxName, backgroundMusicName, masterAudioName;


    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
        LoadSettings();
    }

    // Start is called before the first frame update
    void Start()
    {
        
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
            audio.SetFloat(masterAudioName, PlayerPrefs.GetFloat(masterAudioName));
        }
        if (PlayerPrefs.HasKey(backgroundMusicName))
        {
            Debug.Log(PlayerPrefs.GetFloat(backgroundMusicName));
            audio.SetFloat(backgroundMusicName, PlayerPrefs.GetFloat(backgroundMusicName));
        }
        if (PlayerPrefs.HasKey(sfxName))
        {
            audio.SetFloat(sfxName, PlayerPrefs.GetFloat(sfxName));
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
                    Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
                    break;
                }
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
    }

    void Initialize()
    {

    }
}
