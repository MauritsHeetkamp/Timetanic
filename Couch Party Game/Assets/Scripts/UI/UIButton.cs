using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIButton : UIOption
{
    [SerializeField] Button button;

    // What happens when button is clicked
    public override void Interact()
    {
        button.onClick.Invoke();
        base.Interact();
    }
}
