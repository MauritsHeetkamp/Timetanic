using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UIOption : MonoBehaviour, IPointerEnterHandler
{
    public UIController ownerController;
    public UnityEvent onHover, onLeaveHover;
    public bool interactable = true;
    public AudioPrefabSO hoverAudio, selectAudio;

    public virtual void SetInteractable(bool _interactable)
    {
        interactable = _interactable;
    }

    // What should happen when the horizontal move is pressed
    public virtual void OnMovedHorizontal(int amount)
    {

    }
     // What should happen when the move button is held
    public virtual void OnMovedHorizontalStay(float amount)
    {

    }
     
    // What happens when button is hovered over
    public virtual void OnHover(bool wasInit)
    {
        if (!wasInit)
        {
            if(SoundManager.instance != null && hoverAudio != null)
            {
                SoundManager.instance.SpawnAudio(hoverAudio);
            }
        }

        if(onHover != null)
        {
            onHover.Invoke();
        }
    }

    // What happens when button is no longer being hovered over
    public virtual void OnLeaveHover()
    {
        if(onLeaveHover != null)
        {
            onLeaveHover.Invoke();
        }
    }

    //Do this when the cursor enters the rect area of this selectable UI object.
    public void OnPointerEnter(PointerEventData eventData)
    {
        ownerController.SetSelected(this, false);
    }

    public virtual void Interact()
    {

    }
}
