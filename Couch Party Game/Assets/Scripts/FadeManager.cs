using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class FadeManager : MonoBehaviour
{
    [SerializeField] float defaultFadeDuration = 1;

    [SerializeField] bool unfadeOnStart;
    // Start is called before the first frame update
    void Start()
    {
        if (unfadeOnStart)
        {
            //FadeOut();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void FadeInOut()
    {
        //StartCoroutine(FadeInOutRoutine(defaultFadeDuration));
    }

    public void FadeInOut(float duration)
    {
        //StartCoroutine(FadeInOutRoutine(duration));
    }

    public void FadeIn()
    {
        //StartCoroutine(FadeInRoutine(defaultFadeDuration));
    }

    public void FadeIn(float duration)
    {
        //StartCoroutine(FadeInRoutine(duration));
    }

    public void FadeOut()
    {
        //StartCoroutine(FadeOutRoutine(defaultFadeDuration));
    }

    public void FadeOut(float duration)
    {
        //StartCoroutine(FadeOutRoutine(duration));
    }

    IEnumerator FadeInOutRoutine(FadePanel target, float duration, bool destroyOnComplete = true)
    {
        Debug.Log("ZZZ");
        target.fadePanel.raycastTarget = true;
        Color newColor = target.fadePanel.color;

        float modifyAmount = 1;
        modifyAmount /= duration;
        modifyAmount /= 2;

        while(target.fadePanel.color.a < 1)
        {
            newColor.a += modifyAmount * Time.deltaTime;
            target.fadePanel.color = newColor;
            yield return null;
        }

        if(target.onFadedIn != null)
        {
            target.onFadedIn.Invoke();
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
        if (destroyOnComplete)
        {
            Destroy(target.gameObject);
        }
    }

    IEnumerator FadeOutRoutine(FadePanel target, float duration, bool destroyOnComplete = true)
    {
        target.fadePanel.raycastTarget = false;
        Color newColor = target.fadePanel.color;
        float modifyAmount = -1;
        modifyAmount /= duration;
        modifyAmount /= 2;

        while (target.fadePanel.color.a > 0)
        {
            newColor.a += modifyAmount * Time.deltaTime;
            target.fadePanel.color = newColor;
            yield return null;
        }

        if (target.onFadedIn != null)
        {
            target.onFadedIn.Invoke();
        }

        if (destroyOnComplete)
        {
            Destroy(target.gameObject);
        }
    }

    IEnumerator FadeInRoutine(FadePanel target, float duration, bool destroyOnComplete = true)
    {
        target.fadePanel.raycastTarget = true;
        Color newColor = target.fadePanel.color;

        float modifyAmount = 1;
        modifyAmount /= duration;
        modifyAmount /= 2;

        while (target.fadePanel.color.a < 1)
        {
            newColor.a += modifyAmount * Time.deltaTime;
            target.fadePanel.color = newColor;
            yield return null;
        }

        if (target.onFadedOut != null)
        {
            target.onFadedOut.Invoke();
        }

        if (destroyOnComplete)
        {
            Destroy(target.gameObject);
        }
    }

}
