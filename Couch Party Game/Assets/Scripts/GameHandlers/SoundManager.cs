using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Custom.Audio;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance; // Global soundmanager object instance
    [SerializeField] GameObject audioPrefab;
    [SerializeField] AudioSource backgroundMusic;
    [SerializeField] float backgroundMusicFadeSpeed = 1; // Speed at which the background music fades when changing clips

    [SerializeField] AudioPrefab[] defaultBackgroundMusic;
    int lastAudioUsed = -1; // Last audio index that was used
    public List<AudioPrefab> playlist = new List<AudioPrefab>(); // Audio playlist
    Coroutine swapBackgroundRoutine; // Coroutine that swaps background music
    Coroutine currentMusicRoutine; // Coroutine that tracks the current background music and its duration
    Coroutine endBackgroundRoutine;
    float currentDurationPassed;

    void Awake()
    {
        DontDestroyOnLoad(gameObject); // Makes sure this object doesn't get destroyed when swapping scenes
        if(instance != null)
        {
            Destroy(instance.gameObject); // Removes last instance if it excists
        }
        instance = this;
    }

    // Start is called before the first frame update
    private void Start()
    {
        ShuffleBackgroundAudio();
    }

    // Set new background audio list
    public void SetBackgroundAudioList(AudioPrefab[] newAudioList)
    {
        defaultBackgroundMusic = newAudioList;
        ShuffleBackgroundAudio(false); // Makes sure new audio gets played
    }

    // Shuffles the background audio playlist
    public void ShuffleBackgroundAudio(bool instant = true)
    {
        List<AudioPrefab> remainingClips = new List<AudioPrefab>(defaultBackgroundMusic);
        playlist = new List<AudioPrefab>();

        while (remainingClips.Count > 0)
        {
            int selectedIndex = Random.Range(0, remainingClips.Count); // Selects clip from remaining clips

            if(defaultBackgroundMusic.Length > 1 && playlist.Count < defaultBackgroundMusic.Length / 2 && lastAudioUsed != -1) // Checks if this clip was played as last clip
            {
                if(selectedIndex == lastAudioUsed)
                {
                    continue;
                }
            }

            AudioPrefab selectedPrefab = remainingClips[selectedIndex]; // Selected audio prefab
            playlist.Add(selectedPrefab); // Adds audio prefab to playlist
            remainingClips.RemoveAt(selectedIndex);
        }

        if(playlist.Count > 0)
        {
            SetBackgroundMusic(playlist[0], instant); // Sets background music
            playlist.RemoveAt(0);
        }
    }

    // Plays specific music clip
    public void SetBackgroundMusic(AudioPrefab audioPrefab, bool instant = false)
    {
        if (swapBackgroundRoutine != null)
        {
            StopCoroutine(swapBackgroundRoutine); // Stops current swap music coroutine
        }
        if(endBackgroundRoutine != null)
        {
            StopCoroutine(endBackgroundRoutine);
            endBackgroundRoutine = null;
        }

        if(backgroundMusic.clip != audioPrefab.clip)
        {
            swapBackgroundRoutine = StartCoroutine(SwapBackgroundMusic(audioPrefab.clip, audioPrefab.volume, audioPrefab.pitch, audioPrefab.loop, instant)); // Starts new swap music coroutine
        }
    }

    public void EndBackgroundMusic(bool instant = false)
    {
        if (backgroundMusic.isPlaying)
        {
            if (instant)
            {
                backgroundMusic.Stop();

                if(endBackgroundRoutine != null)
                {
                    StopCoroutine(endBackgroundRoutine);
                    endBackgroundRoutine = null;
                }
            }
            else
            {
                if (endBackgroundRoutine == null)
                {
                    endBackgroundRoutine = StartCoroutine(EndBackgroundMusicRoutine());
                }
            }
        }
    }

    IEnumerator EndBackgroundMusicRoutine()
    {
        while(backgroundMusic.volume > 0)
        {
            Debug.Log("ENDING");
            backgroundMusic.volume += -backgroundMusicFadeSpeed * Time.unscaledDeltaTime;
            yield return new WaitForSeconds(Time.unscaledDeltaTime);
        }
        backgroundMusic.Stop();
        endBackgroundRoutine = null;
    }

    // Swaps background music
    IEnumerator SwapBackgroundMusic(AudioClip newClip, float volume = 1, float pitch = 1, bool loop = false, bool instant = false)
    {
        if (!instant && backgroundMusicFadeSpeed > 0 && backgroundMusic.volume > 0) // Should the transition be smooth?
        {

            float changeAmount = -backgroundMusicFadeSpeed;

            while (backgroundMusic.volume > 0)
            {
                backgroundMusic.volume += changeAmount * Time.unscaledDeltaTime;
                yield return null;
            }
        }

        lastAudioUsed = -1; // Resets last audio used

        for(int i = 0; i < defaultBackgroundMusic.Length; i++)
        {
            if(defaultBackgroundMusic[i].clip == newClip)
            {
                lastAudioUsed = i; // Sets last audio used
            }
        }

        backgroundMusic.volume = 0;
        backgroundMusic.clip = newClip;
        backgroundMusic.pitch = pitch;
        backgroundMusic.loop = loop;

        if(currentMusicRoutine != null)
        {
            StopCoroutine(currentMusicRoutine); // Stops current music tracking coroutine
        }
        if (!backgroundMusic.loop)
        {
            currentMusicRoutine = StartCoroutine(MusicDurationCounter(false)); // Starts new music tracking coroutine
        }

        if (volume > 0) // Should the new clip be played at all?
        {
            backgroundMusic.Play();
            if (!instant && backgroundMusicFadeSpeed > 0) // Should the music fade in smoothly?
            {
                float changeAmount = backgroundMusicFadeSpeed;

                while (backgroundMusic.volume < volume)
                {
                    backgroundMusic.volume += changeAmount * Time.unscaledDeltaTime;
                    yield return null;
                }
            }

            backgroundMusic.volume = volume; // Sets volume to final target
        }
        else
        {
            backgroundMusic.Stop(); // Stops background music
        }
        swapBackgroundRoutine = null;
    }

    // Tracks music and its duration
    IEnumerator MusicDurationCounter(bool resume)
    {
        if (!resume) // Should it reset?
        {
            currentDurationPassed = 0;
        }

        while(currentDurationPassed < backgroundMusic.clip.length) // Is clip not completed yet?
        {
            yield return null;
            currentDurationPassed += Time.unscaledDeltaTime * backgroundMusic.pitch;
        }

        currentMusicRoutine = null;
        if(playlist.Count > 0) // Play next clip in playlist if possible
        {
            SetBackgroundMusic(playlist[0], true);
            playlist.RemoveAt(0);
        }
        else
        {
            ShuffleBackgroundAudio(); // Shuffle if playlist was finished
        }
    }

    public GameObject Spawn3DAudio(ThreeDAudioPrefab audioSettings, Vector3 target)
    {
        Debug.Log("CHECKING");
        if(Physics.OverlapSphere(target, audioSettings.range, audioSettings.targets, QueryTriggerInteraction.Ignore).Length > 0)
        {
            Debug.Log("TRUE");
            GameObject newAudio = Instantiate(audioPrefab);
            newAudio.transform.position = target;
            AudioSource audioSource = newAudio.GetComponent<AudioSource>();

            audioSource.clip = audioSettings.clip;
            audioSource.volume = audioSettings.volume;
            audioSource.pitch = audioSettings.pitch;
            audioSource.loop = audioSettings.loop;

            audioSource.Play();

            return newAudio;
        }
        return null;
    }


    // Spawns audio clip
    public GameObject SpawnAudio(AudioClip clip, bool loop)
    {
        GameObject newAudio = Instantiate(audioPrefab);
        AudioSource audioSource = newAudio.GetComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.loop = loop;
        audioSource.Play();

        return newAudio;
    }

    public GameObject SpawnAudio(AudioPrefabSO audio)
    {
        GameObject newAudio = Instantiate(audioPrefab);
        AudioSource audioSource = newAudio.GetComponent<AudioSource>();
        audioSource.clip = audio.audio.clip;
        audioSource.loop = audio.audio.loop;
        audioSource.volume = audio.audio.volume;
        audioSource.pitch = audio.audio.pitch;
        audioSource.Play();

        return newAudio;
    }

    // Spawns audio clip
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

    // Spawns audio clip
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

    // Spawns audio clip
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

    [System.Serializable]
    public struct ThreeDAudioPrefab
    {
        public float volume;
        public float pitch;
        public AudioClip clip;
        public bool loop;

        public float range;
        public LayerMask targets;

        public ThreeDAudioPrefab(AudioClip _clip, float _range , LayerMask _targets, float _volume = 1, float _pitch = 1, bool _loop = false)
        {
            volume = _volume;
            pitch = _pitch;
            clip = _clip;
            loop = _loop;

            targets = _targets;
            range = _range;
        }
    }
}
