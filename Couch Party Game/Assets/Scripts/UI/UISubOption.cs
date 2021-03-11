using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UISubOption : MonoBehaviour, IPointerEnterHandler
{
    public bool canInteract = true;
    public UnityEvent onHover, onLeaveHover, reset;

    //Do this when the cursor enters the rect area of this selectable UI object.
    public void OnPointerEnter(PointerEventData eventData)
    {
        if(onHover != null && canInteract)
        {
            onHover.Invoke();
        }
    }

    public virtual void SetInteract(bool _canInteract)
    {
        canInteract = _canInteract;
    }

    public virtual void Interact()
    {

    }
}
