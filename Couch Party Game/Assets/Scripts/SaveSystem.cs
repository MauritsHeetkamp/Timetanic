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
        float value;
        audio.GetFloat(masterAudioName, out value);
        PlayerPrefs.SetFloat(masterAudioName, value);

        audio.GetFloat(backgroundMusicName, out value);
        PlayerPrefs.SetFloat(backgroundMusicName, value);

        audio.GetFloat(sfxName, out value);
        PlayerPrefs.SetFloat(sfxName, value);
    }

    public void Load()
    {
        if (PlayerPrefs.HasKey(masterAudioName))
        {
            audio.SetFloat(masterAudioName, PlayerPrefs.GetFloat(masterAudioName));
        }
        if (PlayerPrefs.HasKey(backgroundMusicName))
        {
            audio.SetFloat(backgroundMusicName, PlayerPrefs.GetFloat(backgroundMusicName));
        }
        if (PlayerPrefs.HasKey(sfxName))
        {
            audio.SetFloat(sfxName, PlayerPrefs.GetFloat(sfxName));
        }
    }

    void Initialize()
    {

    }
}
