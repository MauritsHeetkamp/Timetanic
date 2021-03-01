using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class FadeManager : MonoBehaviour
{
    [SerializeField] float defaultFadeDuration = 1;

    [SerializeField] bool unfadeOnStart;

    [SerializeField] RectTransform globalPanelHolder;
    [SerializeField] GameObject fadePanel;
    // Start is called before the first frame update
    void Start()
    {
        if (unfadeOnStart)
        {
            FadeOut();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public FadePanel FadeInOut(float duration = 0, Player specificPlayer = null)
    {
        duration = duration > 0 ? duration : defaultFadeDuration;

        Transform targetLocation = specificPlayer != null ? specificPlayer.attachedSplitscreen.transform : globalPanelHolder;

        FadePanel newPanel = Instantiate(fadePanel, targetLocation).GetComponent<FadePanel>();

        StartCoroutine(FadeInOutRoutine(newPanel, duration, true, specificPlayer));

        return newPanel;
    }

    public FadePanel FadeIn(float duration = 0, Player specificPlayer = null)
    {
        duration = duration > 0 ? duration : defaultFadeDuration;

        Transform targetLocation = specificPlayer != null ? specificPlayer.attachedSplitscreen.transform : globalPanelHolder;

        FadePanel newPanel = Instantiate(fadePanel, targetLocation).GetComponent<FadePanel>();

        StartCoroutine(FadeInRoutine(newPanel, duration, true, specificPlayer));

        return newPanel;
    }

    public FadePanel FadeOut(float duration = 0, Player specificPlayer = null)
    {
        duration = duration > 0 ? duration : defaultFadeDuration;

        Transform targetLocation = specificPlayer != null ? specificPlayer.attachedSplitscreen.transform : globalPanelHolder;

        FadePanel newPanel = Instantiate(fadePanel, targetLocation).GetComponent<FadePanel>();

        StartCoroutine(FadeOutRoutine(newPanel, duration, true, specificPlayer));

        return newPanel;
    }

    IEnumerator FadeInOutRoutine(FadePanel target, float duration, bool destroyOnComplete = true, Player specificPlayer = null)
    {
        target.fadePanel.raycastTarget = true;
        Color newColor = target.fadePanel.color;
        newColor.a = 0;
        target.fadePanel.color = newColor;

        float modifyAmount = 1;
        modifyAmount /= duration;
        modifyAmount /= 2;

        yield return null;
        while (target.fadePanel.color.a < 1)
        {
            newColor.a += modifyAmount * Time.deltaTime;
            target.fadePanel.color = newColor;
            yield return null;
        }

        if(target.onFadedIn != null)
        {
            target.onFadedIn.Invoke();
        }
        if(target.onFadedInSpecificPlayer != null && specificPlayer != null)
        {
            target.onFadedInSpecificPlayer.Invoke(specificPlayer.gameObject);
        }

        target.fadePanel.raycastTarget = false;

        modifyAmount = -1;
        modifyAmount /= duration;
        modifyAmount /= 2;

        while (target.fadePanel.color.a > 0)
        {
            newColor.a += modifyAmount * Time.deltaTime;
            target.fadePanel.color = newColor;
            yield return null;
        }

        if(target.onFadedOut != null)
        {
            target.onFadedOut.Invoke();
        }
        if (target.onFadedOutSpecificPlayer != null && specificPlayer != null)
        {
            target.onFadedOutSpecificPlayer.Invoke(specificPlayer.gameObject);
        }
        if (destroyOnComplete)
        {
            Destroy(target.gameObject);
        }
    }

    IEnumerator FadeOutRoutine(FadePanel target, float duration, bool destroyOnComplete = true, Player specificPlayer = null)
    {
        target.fadePanel.raycastTarget = false;
        Color newColor = target.fadePanel.color;
        newColor.a = 1;
        target.fadePanel.color = newColor;
        float modifyAmount = -1;
        modifyAmount /= duration;
        modifyAmount /= 2;

        yield return null;
        while (target.fadePanel.color.a > 0)
        {
            newColor.a += modifyAmount * Time.deltaTime;
            target.fadePanel.color = newColor;
            yield return null;
        }

        if (target.onFadedOut != null)
        {
            target.onFadedOut.Invoke();
        }
        if (target.onFadedOutSpecificPlayer != null && specificPlayer != null)
        {
            target.onFadedOutSpecificPlayer.Invoke(specificPlayer.gameObject);
        }


        if (destroyOnComplete)
        {
            Destroy(target.gameObject);
        }
    }

    IEnumerator FadeInRoutine(FadePanel target, float duration, bool destroyOnComplete = true, Player specificPlayer = null)
    {
        target.fadePanel.raycastTarget = true;
        Color newColor = target.fadePanel.color;
        newColor.a = 0;
        target.fadePanel.color = newColor;

        float modifyAmount = 1;
        modifyAmount /= duration;
        modifyAmount /= 2;

        yield return null;
        while (target.fadePanel.color.a < 1)
        {
            newColor.a += modifyAmount * Time.deltaTime;
            target.fadePanel.color = newColor;
            yield return null;
        }

        if (target.onFadedIn != null)
        {
            target.onFadedIn.Invoke();
        }
        if (target.onFadedInSpecificPlayer != null && specificPlayer != null)
        {
            target.onFadedInSpecificPlayer.Invoke(specificPlayer.gameObject);
        }

        if (destroyOnComplete)
        {
            Destroy(target.gameObject);
        }
    }

}
