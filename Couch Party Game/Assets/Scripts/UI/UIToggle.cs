using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIToggle : UIOption
{
    public Toggle toggle;

    public void Initialize(bool value)
    {
        toggle.isOn = value;
        if (toggle.onValueChanged != null)
        {
            toggle.onValueChanged.Invoke(toggle.isOn);
        }
    }

    public override void Interact()
    {
        base.Interact();
        toggle.isOn = !toggle.isOn;
        if(toggle.onValueChanged != null)
        {
            toggle.onValueChanged.Invoke(toggle.isOn);
        }
    }
}
