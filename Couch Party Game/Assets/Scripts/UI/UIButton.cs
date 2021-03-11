using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIButton : UIOption
{
    [SerializeField] Button button;

    private void Start()
    {
        button.interactable = interactable;
    }

    // What happens when button is clicked
    public override void Interact()
    {
        if (interactable)
        {
            button.onClick.Invoke();
        }
    }

    public override void SetInteractable(bool _interactable)
    {
        base.SetInteractable(_interactable);
        button.interactable = _interactable;
    }
}
