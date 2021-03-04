using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIOptionButton : UIOption
{
    [SerializeField] Button button;
    public override void Interact()
    {
        button.onClick.Invoke();
        base.Interact();
    }
}
