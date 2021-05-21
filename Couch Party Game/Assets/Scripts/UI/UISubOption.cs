using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UISubOption : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Animator animator;
    public bool canInteract = true;
    public UnityEvent onHover, onHoverController, onLeaveHover, onLeaveHoverController, reset;
    public AudioPrefabSO hoverAudio, selectAudio;

    //Do this when the cursor enters the rect area of this selectable UI object.
    public void OnPointerEnter(PointerEventData eventData)
    {
        if(canInteract)
        {
            OnHover(false);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (canInteract)
        {
            OnLeaveHover();
        }
    }

    public void OnHover(bool init)
    {
        if (onHover != null)
        {
            onHover.Invoke();
        }
        if (!init && SoundManager.instance != null && hoverAudio.audio.clip != null)
        {
            Destroy(SoundManager.instance.SpawnAudio(hoverAudio), hoverAudio.audio.clip.length);
        }
    }

    public void OnLeaveHover()
    {
        if (onLeaveHover != null)
        {
            onLeaveHover.Invoke();
        }
    }

    public virtual void SetInteract(bool _canInteract)
    {
        canInteract = _canInteract;
    }

    public virtual void Interact()
    {
        if (canInteract)
        {
            if (selectAudio != null && SoundManager.instance != null)
            {
                Destroy(SoundManager.instance.SpawnAudio(selectAudio), selectAudio.audio.clip.length);
            }
        }
    }
}
