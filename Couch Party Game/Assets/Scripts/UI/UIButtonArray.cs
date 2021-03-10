using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Custom.Types;

public class UIButtonArray : UIOption
{
    public int selectedButton;
    [SerializeField] DropdownData[] arrayData;
    [SerializeField] UISubOptionButton[] buttons;
    public Transform buttonHolderTransform;
    [SerializeField] HorizontalLayoutGroup holderLayoutGroup;
    public GameObject optionButton;

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

    public void Initialize(DropdownData[] _data)
    {
        arrayData = _data;
        LoadData();
    }

    void Initialize()
    {
        LoadData();
        SetInteractable(interactable);
    }

    void LoadData()
    {
        for (int i = buttons.Length - 1; i >= 0; i--)
        {
            Destroy(buttons[i].gameObject);
        }

        if(arrayData.Length > 0)
        {
            List<UISubOptionButton> newButtons = new List<UISubOptionButton>();

            foreach (DropdownData data in arrayData)
            {
                UISubOptionButton newButton = Instantiate(optionButton, buttonHolderTransform).GetComponent<UISubOptionButton>();
                newButton.buttonText.text = data.name;
                newButton.thisButton.onClick.AddListener(data.onSelected);
                newButtons.Add(newButton);
            }

            buttons = newButtons.ToArray();
            RectTransform buttonHolder = buttonHolderTransform.GetComponent<RectTransform>();
            float widthToRemove = (holderLayoutGroup.spacing * buttons.Length - 1) + holderLayoutGroup.padding.left + holderLayoutGroup.padding.right;
            widthToRemove /= buttons.Length;
            Vector2 newButtonSize = new Vector2((buttonHolder.rect.width / buttons.Length) - widthToRemove, buttonHolder.rect.height);

            foreach (UISubOptionButton button in buttons)
            {
                button.GetComponent<RectTransform>().sizeDelta = newButtonSize;
            }

            if (buttons.Length > selectedButton)
            {
                if (buttons[selectedButton].onHover != null)
                {
                    buttons[selectedButton].onHover.Invoke();
                }
            }
        }
        else
        {
            buttons = new UISubOptionButton[0];
        }
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
