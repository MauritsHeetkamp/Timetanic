using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UISlider : UIOption
{
    [HideInInspector] public SliderEndDrag endDragHandler;
    [SerializeField] float sensitivity = 0.01f;
    [SerializeField] Slider attachedSlider;

    private void Start()
    {
        endDragHandler = GetComponent<SliderEndDrag>();
        endDragHandler = gameObject.AddComponent<SliderEndDrag>();
    }

    public override void OnMovedHorizontalStay(float amount)
    {
        if (interactable)
        {
            attachedSlider.value += Mathf.Abs(attachedSlider.maxValue - attachedSlider.minValue) * (amount * Time.deltaTime * sensitivity);
            if (attachedSlider.onValueChanged != null)
            {
                attachedSlider.onValueChanged.Invoke(attachedSlider.value);
            }
            base.OnMovedHorizontalStay(amount);
        }
    }
}
