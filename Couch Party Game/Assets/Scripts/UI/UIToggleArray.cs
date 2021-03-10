using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIToggleArray : UIOption
{
    public int selected;
    [SerializeField] UIToggle[] toggles;

    public void Initialize()
    {
        foreach(UIToggle toggle in toggles)
        {
            toggle.onHover.AddListener(() => SetSelected(toggle));
        }

        if(selected < toggles.Length && selected >= 0)
        {
            toggles[selected].OnHover();
        }
        SetInteractable(interactable);
    }

    public override void SetInteractable(bool _interactable)
    {
        base.SetInteractable(_interactable);
        foreach(UIToggle toggle in toggles)
        {
            toggle.SetInteractable(_interactable);
        }
    }

    public void SetSelected(UIToggle selectedToggle)
    {
        if (interactable)
        {
            if (selectedToggle != toggles[selected])
            {
                for (int i = 0; i < toggles.Length; i++)
                {
                    if (toggles[i] == selectedToggle)
                    {
                        if (toggles[selected].onLeaveHover != null)
                        {
                            toggles[selected].onLeaveHover.Invoke();
                        }
                        selected = i;
                        break;
                    }
                }
            }
        }
    }

    void MoveSelected(int amount)
    {
        if (interactable)
        {
            if (toggles.Length > 1)
            {
                if (toggles[selected].onLeaveHover != null)
                {
                    toggles[selected].onLeaveHover.Invoke();
                }
                selected += amount;

                if (selected >= toggles.Length)
                {
                    selected = 0;
                }
                else
                {
                    if (selected < 0)
                    {
                        selected = toggles.Length - 1;
                    }
                }

                if (toggles[selected].onHover != null)
                {
                    toggles[selected].onHover.Invoke();
                }
            }
        }
    }

    public override void OnMovedHorizontal(int amount)
    {
        base.OnMovedHorizontal(amount);
        MoveSelected(amount);
    }

    public override void OnHover()
    {
    }


    public override void Interact()
    {
        base.Interact();
        toggles[selected].Interact();
    }
}
