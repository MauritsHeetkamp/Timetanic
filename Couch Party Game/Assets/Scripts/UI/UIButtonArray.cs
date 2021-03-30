using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Custom.Types;

public class UIButtonArray : UIOption
{
    public int selectedButton;
    [SerializeField] UISubOptionButton[] buttons;

    [SerializeField] bool initializeOnStart;

    // Start is called before the first frame update
    void Start()
    {
        if (initializeOnStart)
        {
            Initialize();
        }
    }

    public override void SetInteractable(bool _interactable)
    {
        base.SetInteractable(_interactable);
        foreach(UISubOptionButton button in buttons)
        {
            button.SetInteract(_interactable);
        }
    }

    void Initialize()
    {
        SetInteractable(interactable);
        SetSelected(selectedButton);
    }

    public override void Interact()
    {
        if (interactable)
        {
            buttons[selectedButton].Interact();
        }
    }

    public void SetSelected(int index)
    {
        if(index >= 0 && index < buttons.Length && buttons.Length > 0)
        {
            UISubOptionButton selected = buttons[selectedButton];
            if (selected.onLeaveHover != null)
            {
                selected.onLeaveHover.Invoke();
            }

            if(selected.onLeaveHoverController != null)
            {
                selected.onLeaveHoverController.Invoke();
            }

            if(selected.reset != null)
            {
                selected.reset.Invoke();
            }

            selectedButton = index;
            selected = buttons[selectedButton];

            if (selected.onHover != null)
            {
                selected.onHover.Invoke();
            }

            if(selected.onHoverController != null)
            {
                selected.onHoverController.Invoke();
            }

            if (selected.autoInteract)
            {
                selected.Interact();
            }
        }
    }

    public void SetSelected(UISubOption target)
    {
        if(target != buttons[selectedButton])
        {
            for (int i = 0; i < buttons.Length; i++)
            {
                UISubOptionButton selectedTarget = buttons[i];

                if (selectedTarget == target)
                {
                    UISubOptionButton selected = buttons[selectedButton];
                    if (selected.onLeaveHover != null)
                    {
                        selected.onLeaveHover.Invoke();
                    }

                    if (selected.reset != null)
                    {
                        selected.reset.Invoke();
                    }

                    selectedButton = i;
                    selected = buttons[selectedButton];

                    if (selected.onHover != null)
                    {
                        selected.onHover.Invoke();
                    }

                    if (selected.autoInteract)
                    {
                        selected.Interact();
                    }
                    break;
                }
            }
        }
    }

    public override void OnMovedHorizontal(int amount)
    {
        if(buttons.Length > 1)
        {
            int newSelectedIndex = selectedButton + amount;

            if (newSelectedIndex >= buttons.Length)
            {
                newSelectedIndex = 0;
            }
            else
            {
                if (newSelectedIndex < 0)
                {
                    newSelectedIndex = buttons.Length - 1;
                }
            }

            SetSelected(newSelectedIndex);
            base.OnMovedHorizontal(amount);
        }
    }
}
