using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class FadeManager : MonoBehaviour
{
    [SerializeField] float defaultFadeDuration = 1;

    [SerializeField] bool unfadeOnStart;

    [SerializeField] RectTransform globalPanelHolder; // The object that holds the fade panels
    [SerializeField] GameObject fadePanel; // Fade panel prefab


    // Start is called before the first frame update
    void Start()
    {
        if (unfadeOnStart)
        {
            FadeOut();
        }
    }

    // Fade the screen in and out
    public FadePanel FadeInOut(float duration = 0, Player specificPlayer = null)
    {
        duration = duration > 0 ? duration : defaultFadeDuration; // Is the duration longer then 0?

        Transform targetLocation = specificPlayer != null ? specificPlayer.attachedSplitscreen.transform : globalPanelHolder; // Select which panel should be faded

        FadePanel newPanel = Instantiate(fadePanel, targetLocation).GetComponent<FadePanel>(); // Creates fade panel

        StartCoroutine(FadeInOutRoutine(newPanel, duration, true, specificPlayer));

        return newPanel;
    }

    // Fade the screen in and out
    public FadePanel FadeIn(float duration = 0, Player specificPlayer = null)
    {
        duration = duration > 0 ? duration : defaultFadeDuration; // Is the duration longer then 0?

        Transform targetLocation = specificPlayer != null ? specificPlayer.attachedSplitscreen.transform : globalPanelHolder; // Select which panel should be faded

        FadePanel newPanel = Instantiate(fadePanel, targetLocation).GetComponent<FadePanel>(); // Creates fade panel

        StartCoroutine(FadeInRoutine(newPanel, duration, true, specificPlayer));

        return newPanel;
    }

    // Fade the screen out
    public FadePanel FadeOut(float duration = 0, Player specificPlayer = null)
    {
        duration = duration > 0 ? duration : defaultFadeDuration; // Is the duration longer then 0?

        Transform targetLocation = specificPlayer != null ? specificPlayer.attachedSplitscreen.transform : globalPanelHolder; // Select which panel should be faded

        FadePanel newPanel = Instantiate(fadePanel, targetLocation).GetComponent<FadePanel>(); // Creates fade panel

        StartCoroutine(FadeOutRoutine(newPanel, duration, true, specificPlayer));

        return newPanel;
    }


    // Fade screen in and out coroutine
    IEnumerator FadeInOutRoutine(FadePanel target, float duration, bool destroyOnComplete = true, Player specificPlayer = null)
    {
        target.fadePanel.raycastTarget = true; // People çan't interact with UI anymore
        Color newColor = target.fadePanel.color; // Creates copy of current panel color
        newColor.a = 0;
        target.fadePanel.color = newColor;

        float modifyAmount = 1;
        modifyAmount /= duration; // Calculates fade amount per second
        modifyAmount *= 2; // Makes fade duration for both sides

        yield return null;
        while (target.fadePanel.color.a < 1)
        {
            newColor.a += modifyAmount * Time.deltaTime;
            target.fadePanel.color = newColor;
            yield return null;
        }

        if(target.onFadedIn != null)
        {
            target.onFadedIn.Invoke(); // Effect on fade in
        }
        if(target.onFadedInSpecificPlayer != null && specificPlayer != null)
        {
            target.onFadedInSpecificPlayer.Invoke(specificPlayer.gameObject); // Effect on fade in for specific player object
        }

        target.fadePanel.raycastTarget = false; // People can interact with ui again

        modifyAmount = -1;
        modifyAmount /= duration; // Calculates fade amount per second
        modifyAmount *= 2; // Makes fade duration for both sides

        while (target.fadePanel.color.a > 0)
        {
            newColor.a += modifyAmount * Time.deltaTime;
            target.fadePanel.color = newColor;
            yield return null;
        }

        if(target.onFadedOut != null)
        {
            target.onFadedOut.Invoke(); // Effect on fade in
        }
        if (target.onFadedOutSpecificPlayer != null && specificPlayer != null)
        {
            target.onFadedOutSpecificPlayer.Invoke(specificPlayer.gameObject); // Effect on fade in for specific player object
        }
        if (destroyOnComplete) // Should the object be destroyed once complete?
        {
            Destroy(target.gameObject);
        }
    }

    // Fade screen out coroutine
    IEnumerator FadeOutRoutine(FadePanel target, float duration, bool destroyOnComplete = true, Player specificPlayer = null)
    {
        target.fadePanel.raycastTarget = false; // People can interact with ui again
        Color newColor = target.fadePanel.color; // Creates copy of current panel color
        newColor.a = 1;
        target.fadePanel.color = newColor;

        float modifyAmount = -1;
        modifyAmount /= duration; // Calculates fade amount per second
        modifyAmount /= 2; // Makes fade duration for both sides

        yield return null;
        while (target.fadePanel.color.a > 0)
        {
            newColor.a += modifyAmount * Time.deltaTime;
            target.fadePanel.color = newColor;
            yield return null;
        }

        if (target.onFadedOut != null)
        {
            target.onFadedOut.Invoke(); // Effect on fade in
        }
        if (target.onFadedOutSpecificPlayer != null && specificPlayer != null)
        {
            target.onFadedOutSpecificPlayer.Invoke(specificPlayer.gameObject); // Effect on fade in for specific player object
        }


        if (destroyOnComplete) // Should the object be destroyed once complete?
        {
            Destroy(target.gameObject);
        }
    }

    // Fade screen in coroutine
    IEnumerator FadeInRoutine(FadePanel target, float duration, bool destroyOnComplete = true, Player specificPlayer = null)
    {
        target.fadePanel.raycastTarget = true; // People çan't interact with UI anymore
        Color newColor = target.fadePanel.color; // Creates copy of current panel color
        newColor.a = 0;
        target.fadePanel.color = newColor;

        float modifyAmount = 1;
        modifyAmount /= duration; // Calculates fade amount per second
        modifyAmount /= 2; // Makes fade duration for both sides

        yield return null;
        while (target.fadePanel.color.a < 1)
        {
            newColor.a += modifyAmount * Time.deltaTime;
            target.fadePanel.color = newColor;
            yield return null;
        }

        if (target.onFadedIn != null)
        {
            target.onFadedIn.Invoke(); // Effect on fade in
        }
        if (target.onFadedInSpecificPlayer != null && specificPlayer != null)
        {
            target.onFadedInSpecificPlayer.Invoke(specificPlayer.gameObject); // Effect on fade in for specific player object
        }

        if (destroyOnComplete) // Should the object be destroyed once complete?
        {
            Destroy(target.gameObject);
        }
    }

}
