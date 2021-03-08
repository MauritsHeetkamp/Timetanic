using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UISubOptionButton : UISubOption
{
    public Button thisButton;
    public TextMeshProUGUI buttonText;

    public override void Interact()
    {
        base.Interact();
        thisButton.onClick.Invoke();
    }
}
