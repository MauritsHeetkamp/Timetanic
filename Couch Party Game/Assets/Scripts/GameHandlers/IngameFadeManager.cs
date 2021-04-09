using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class IngameFadeManager : MonoBehaviour
{

    [SerializeField] FadeManager globalFader; // The object that holds the fade panels


    // Fade the screen in and out
    public FadePanel FadeInOut(float duration = 0, Player specificPlayer = null)
    {
        FadeManager targetFader = specificPlayer != null ? specificPlayer.attachedSplitscreen.fadeManager : globalFader; // Select which panel should be faded

        return targetFader.FadeInOut(duration, true, specificPlayer);
    }

    // Fade the screen in and out
    public FadePanel FadeIn(float duration = 0, Player specificPlayer = null)
    {
        FadeManager targetFader = specificPlayer != null ? specificPlayer.attachedSplitscreen.fadeManager : globalFader; // Select which panel should be faded

        return targetFader.FadeIn(duration, true, specificPlayer);
    }

    // Fade the screen out
    public FadePanel FadeOut(float duration = 0, Player specificPlayer = null)
    {
        FadeManager targetFader = specificPlayer != null ? specificPlayer.attachedSplitscreen.fadeManager : globalFader; // Select which panel should be faded

        return targetFader.FadeOut(duration, true, specificPlayer);
    }

}
