using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.Events;

public class Cutscene : MonoBehaviour
{
    [SerializeField] VideoPlayer videoPlayer;
    public UnityAction onCompletedCutscene;
    [SerializeField] float endVideoOffset = -1;

    public void PlayCutscene(VideoClip clip)
    {
        videoPlayer.clip = clip;
        videoPlayer.Play();
        StartCoroutine(WaitUntilVideoEnded());
    }

    IEnumerator WaitUntilVideoEnded()
    {
        yield return new WaitForSeconds((float)videoPlayer.clip.length + endVideoOffset);
        if(onCompletedCutscene != null)
        {
            onCompletedCutscene.Invoke();
        }
    }
}
