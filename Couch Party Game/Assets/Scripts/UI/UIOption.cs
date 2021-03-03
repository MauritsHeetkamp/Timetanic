using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UIOption : MonoBehaviour, IPointerEnterHandler
{
    [HideInInspector] public UIController ownerController;
    public UnityEvent onHover, onLeaveHover;


    // What should happen when the horizontal move is pressed
    public virtual void OnMovedHorizontal(int amount)
    {

    }
     // What should happen when the move button is held
    public virtual void OnMovedHorizontalStay(float amount)
    {

    }

    //Do this when the cursor enters the rect area of this selectable UI object.
    public void OnPointerEnter(PointerEventData eventData)
    {
        ownerController.SetSelected(this);
    }
}
