using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UISubOptionButton : UISubOption
{
    public bool autoInteract = true;
    public Button thisButton;
    public TextMeshProUGUI buttonText;

    public override void Interact()
    {
        base.Interact();
        Debug.Log("ITTEERR");
        thisButton.onClick.Invoke();
    }

    public override void SetInteract(bool _canInteract)
    {
        base.SetInteract(_canInteract);
        thisButton.interactable = _canInteract;
    }
}
