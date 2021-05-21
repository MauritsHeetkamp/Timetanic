using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.Events;
using Custom.Audio;

public class Cutscene : MonoBehaviour
{
    PauseMenu pauseMenu;
    [SerializeField] VideoPlayer videoPlayer;
    [SerializeField] AudioSource audioSource;
    public UnityAction onCompletedCutscene;
    [SerializeField] float endVideoOffset = -1;
    [SerializeField] string pauseMenuTag = "PauseMenu";

    CutsceneData backupData;

    Coroutine fadeAudioRoutine;


    public void PlayCutscene(CutsceneData data)
    {
        backupData = data;

        if (pauseMenu == null)
        {
            GameObject pauseMenuObject = GameObject.FindGameObjectWithTag(pauseMenuTag);
            if (pauseMenuObject != null)
            {
                pauseMenu = pauseMenuObject.GetComponent<PauseMenu>();
            }
        }

        if(pauseMenu != null)
        {
            pauseMenu.onPaused += Pause;
            pauseMenu.onUnpaused += Unpause;
        }

        videoPlayer.clip = data.video;
        videoPlayer.Play();
        audioSource.clip = data.audio.clip;
        audioSource.volume = data.audio.volume;
        audioSource.pitch = data.audio.pitch;
        audioSource.loop = data.audio.loop;
        audioSource.Play();
        StartCoroutine(WaitUntilVideoEnded());
    }

    public void Pause()
    {
        videoPlayer.Pause();
        audioSource.Pause();
        /*if(SoundManager.instance != null && audioSource.clip != null)
        {
            if(fadeAudioRoutine != null)
            {
                StopCoroutine(fadeAudioRoutine);
            }
            fadeAudioRoutine = StartCoroutine(SoundManager.instance.FadeAudioSource(audioSource, 0));
        }*/
    }

    public void Unpause()
    {
        videoPlayer.Play();
        audioSource.UnPause();


        /*if (SoundManager.instance != null && audioSource.clip != null)
        {
            if (fadeAudioRoutine != null)
            {
                StopCoroutine(fadeAudioRoutine);
            }
            fadeAudioRoutine = StartCoroutine(SoundManager.instance.FadeAudioSource(audioSource, backupData.audio.volume));
        }*/
    }

    IEnumerator WaitUntilVideoEnded()
    {
        yield return new WaitForSeconds((float)videoPlayer.clip.length + endVideoOffset);

        if (onCompletedCutscene != null)
        {
            onCompletedCutscene.Invoke();
        }
    }

    private void OnDestroy()
    {
        try
        {
            if (pauseMenu != null)
            {
                pauseMenu.onPaused -= Pause;
                pauseMenu.onUnpaused -= Unpause;
            }
        }
        catch
        {

        }
    }

    [System.Serializable]
    public struct CutsceneData
    {
        public VideoClip video;
        public AudioPrefab audio;

        public CutsceneData(VideoClip _video, AudioPrefab _audio)
        {
            video = _video;
            audio = _audio;
        }

        public CutsceneData(VideoClip _video, AudioPrefabSO _audio)
        {
            video = _video;
            audio = _audio.audio;
        }
    }
}
