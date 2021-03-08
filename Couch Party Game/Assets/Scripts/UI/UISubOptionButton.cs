using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISubOptionButton : UISubOption
{
    [SerializeField] Button thisButton;

    public override void Interact()
    {
        base.Interact();
        thisButton.onClick.Invoke();
    }
}
