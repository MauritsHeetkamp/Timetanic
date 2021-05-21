using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.Events;

public class Cutscene : MonoBehaviour
{
    PauseMenu pauseMenu;
    [SerializeField] VideoPlayer videoPlayer;
    public UnityAction onCompletedCutscene;
    [SerializeField] float endVideoOffset = -1;
    [SerializeField] string pauseMenuTag = "PauseMenu";

    public void PlayCutscene(VideoClip clip)
    {
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

        videoPlayer.clip = clip;
        videoPlayer.Play();
        StartCoroutine(WaitUntilVideoEnded());
    }

    public void Pause()
    {
        videoPlayer.Pause();
    }

    public void Unpause()
    {
        videoPlayer.Play();
    }

    IEnumerator WaitUntilVideoEnded()
    {
        yield return new WaitForSeconds((float)videoPlayer.clip.length + endVideoOffset);

        if (onCompletedCutscene != null)
        {
            onCompletedCutscene.Invoke();
        }

        yield return new WaitForSeconds(endVideoOffset);
        
        if (pauseMenu != null)
        {
            pauseMenu.onPaused -= Pause;
            pauseMenu.onUnpaused -= Unpause;
        }
    }
}
