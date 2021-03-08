using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISlider : UIOption
{
    [SerializeField] float sensitivity = 0.01f;
    [SerializeField] Slider attachedSlider;

    public override void OnMovedHorizontalStay(float amount)
    {
        attachedSlider.value += Mathf.Abs(attachedSlider.maxValue - attachedSlider.minValue) * (amount * Time.deltaTime * sensitivity);
        if (attachedSlider.onValueChanged != null)
        {
            attachedSlider.onValueChanged.Invoke(attachedSlider.value);
        }
        base.OnMovedHorizontalStay(amount);
    }
}
