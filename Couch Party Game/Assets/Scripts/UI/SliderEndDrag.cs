using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;


public class SliderEndDrag : MonoBehaviour, IPointerUpHandler
{
    public UnityAction<PointerEventData> onEndedDrag;
    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("ENDED");
        if(onEndedDrag != null)
        {
            onEndedDrag.Invoke(eventData);
        }
    }
}
