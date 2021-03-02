using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Custom.Audio;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    [SerializeField] GameObject audioPrefab;
    [SerializeField] AudioSource backgroundMusic;
    [SerializeField] float backgroundMusicFadeSpeed = 1;

    [SerializeField] AudioPrefab[] defaultBackgroundMusic;
    int lastAudioUsed = -1;
    public List<AudioPrefab> playlist = new List<AudioPrefab>();
    Coroutine swapBackgroundRoutine;
    Coroutine currentMusicRoutine;
    float currentDurationPassed;
    // Start is called before the first frame update
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if(instance != null)
        {
            Destroy(instance.gameObject);
        }
        instance = this;
    }

    private void Start()
    {
        ShuffleBackgroundAudio();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetBackgroundAudioList(AudioPrefab[] newAudioList)
    {
        defaultBackgroundMusic = newAudioList;
        ShuffleBackgroundAudio(false);
    }

    public void ShuffleBackgroundAudio(bool instant = true)
    {
        List<AudioPrefab> remainingClips = new List<AudioPrefab>(defaultBackgroundMusic);
        playlist = new List<AudioPrefab>();

        while (remainingClips.Count > 0)
        {
            int selectedIndex = Random.Range(0, remainingClips.Count);

            if(defaultBackgroundMusic.Length > 1 && playlist.Count < defaultBackgroundMusic.Length / 2 && lastAudioUsed != -1)
            {
                if(selectedIndex == lastAudioUsed)
                {
                    continue;
                }
            }

            AudioPrefab selectedPrefab = remainingClips[selectedIndex];
            playlist.Add(selectedPrefab);
            remainingClips.RemoveAt(selectedIndex);
        }

        if(playlist.Count > 0)
        {
            SetBackgroundMusic(playlist[0], instant);
            playlist.RemoveAt(0);
        }
    }

    public void SetBackgroundMusic(AudioPrefab audioPrefab, bool instant = false)
    {
        if (swapBackgroundRoutine != null)
        {
            StopCoroutine(swapBackgroundRoutine);
        }
        swapBackgroundRoutine = StartCoroutine(SwapBackgroundMusic(audioPrefab.clip, audioPrefab.volume, audioPrefab.pitch, audioPrefab.loop, instant));
    }

    IEnumerator SwapBackgroundMusic(AudioClip newClip, float volume = 1, float pitch = 1, bool loop = false, bool instant = false)
    {
        if (!instant && backgroundMusicFadeSpeed > 0 && backgroundMusic.volume > 0)
        {
            float changeAmount = -backgroundMusicFadeSpeed;

            while (backgroundMusic.volume > 0)
            {
                backgroundMusic.volume += changeAmount * Time.deltaTime;
                yield return null;
            }
        }

        lastAudioUsed = -1;

        for(int i = 0; i < defaultBackgroundMusic.Length; i++)
        {
            if(defaultBackgroundMusic[i].clip == newClip)
            {
                lastAudioUsed = i;
            }
        }

        backgroundMusic.volume = 0;
        backgroundMusic.clip = newClip;
        backgroundMusic.pitch = pitch;
        backgroundMusic.loop = loop;

        if(currentMusicRoutine != null)
        {
            StopCoroutine(currentMusicRoutine);
        }
        if (!backgroundMusic.loop)
        {
            currentMusicRoutine = StartCoroutine(MusicDurationCounter(false));
        }

        if(volume > 0)
        {
            backgroundMusic.Play();
            if (!instant && backgroundMusicFadeSpeed > 0)
            {
                float changeAmount = backgroundMusicFadeSpeed;

                while (backgroundMusic.volume < volume)
                {
                    backgroundMusic.volume += changeAmount * Time.deltaTime;
                    yield return null;
                }
            }

            backgroundMusic.volume = volume;
        }
        else
        {
            backgroundMusic.Stop();
        }
        swapBackgroundRoutine = null;
    }

    IEnumerator MusicDurationCounter(bool resume)
    {
        if (!resume)
        {
            currentDurationPassed = 0;
        }

        while(currentDurationPassed < backgroundMusic.clip.length)
        {
            yield return null;
            currentDurationPassed += Time.deltaTime * backgroundMusic.pitch;
        }

        currentMusicRoutine = null;
        if(playlist.Count > 0)
        {
            SetBackgroundMusic(playlist[0], true);
            playlist.RemoveAt(0);
        }
        else
        {
            ShuffleBackgroundAudio();
        }
    }

    public GameObject SpawnAudio(AudioClip clip, bool loop)
    {
        GameObject newAudio = Instantiate(audioPrefab);
        AudioSource audioSource = newAudio.GetComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.loop = loop;
        audioSource.Play();

        return newAudio;
    }

    public GameObject SpawnAudio(AudioClip clip, bool loop, float pitch)
    {
        GameObject newAudio = Instantiate(audioPrefab);
        AudioSource audioSource = newAudio.GetComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.loop = loop;
        audioSource.pitch = pitch;
        audioSource.Play();

        return newAudio;
    }

    public GameObject SpawnAudio(AudioClip clip, float volume, bool loop)
    {
        GameObject newAudio = Instantiate(audioPrefab);
        AudioSource audioSource = newAudio.GetComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.loop = loop;
        audioSource.volume = volume;
        audioSource.Play();

        return newAudio;
    }

    public GameObject SpawnAudio(AudioClip clip, bool loop, float pitch, float volume)
    {
        GameObject newAudio = Instantiate(audioPrefab);
        AudioSource audioSource = newAudio.GetComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.loop = loop;
        audioSource.volume = volume;
        audioSource.pitch = pitch;
        audioSource.Play();

        return newAudio;
    }
}

namespace Custom.Audio
{
    [System.Serializable]
    public struct AudioPrefab
    {
        public float volume;
        public float pitch;
        public AudioClip clip;
        public bool loop;
    }
}
