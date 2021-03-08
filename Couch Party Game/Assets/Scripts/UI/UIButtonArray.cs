using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIButtonArray : UIOption
{
    int selectedButton;
    [SerializeField] UISubOptionButton[] buttons;


    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    void Initialize()
    {
        if(buttons.Length > selectedButton)
        {
            if(buttons[selectedButton].onHover != null)
            {
                buttons[selectedButton].onHover.Invoke();
            }
        }
    }

    public override void Interact()
    {
        buttons[selectedButton].Interact();
        base.Interact();
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

            selected.Interact();
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

                    selected.Interact();
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
