using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Custom.Types;

public class UIDropdownApex : UIOption
{
    public DropdownData[] dropdownData;
    public int selected;

    [SerializeField] TextMeshProUGUI selectedText;
    [SerializeField] RectTransform selectedIconHolder;
    [SerializeField] GameObject selectedIconPrefab;
    [SerializeField] HorizontalLayoutGroup iconHolderLayoutGroup;
    Image[] selectedIcons;
    [SerializeField] Color selectedColor, defaultColor;


    public void Initialize(DropdownData[] data)
    {
        dropdownData = data;
        LoadData();
    }

    public void LoadData()
    {
        if(dropdownData.Length > 0)
        {
            float widthToRemove = (iconHolderLayoutGroup.spacing * dropdownData.Length - 1) + iconHolderLayoutGroup.padding.left + iconHolderLayoutGroup.padding.right;
            widthToRemove /= dropdownData.Length;
            Vector2 newButtonSize = new Vector2((selectedIconHolder.rect.width / dropdownData.Length) - widthToRemove, selectedIconHolder.rect.height);
            List<Image> newIcons = new List<Image>();
            foreach (DropdownData data in dropdownData)
            {
                Image newIcon = Instantiate(selectedIconPrefab, selectedIconHolder).GetComponent<Image>();
                newIcon.GetComponent<RectTransform>().sizeDelta = newButtonSize;
                newIcon.color = defaultColor;
                newIcons.Add(newIcon);
            }

            selectedIcons = newIcons.ToArray();
            if(selected < dropdownData.Length)
            {
                DropdownData selectedData = dropdownData[selected];
                selectedIcons[selected].color = selectedColor;
                selectedText.text = selectedData.name;
                if(selectedData.onSelected != null)
                {
                    selectedData.onSelected.Invoke();
                }
            }
        }

    }

    public void ChangeSelected(int amount)
    {
        if (interactable)
        {
            selectedIcons[selected].color = defaultColor;
            selected += amount;

            if (selected >= dropdownData.Length)
            {
                selected = 0;
            }
            if (selected < 0)
            {
                selected = dropdownData.Length - 1;
            }

            DropdownData selectedData = dropdownData[selected];
            selectedIcons[selected].color = selectedColor;
            selectedText.text = selectedData.name;
            if (selectedData.onSelected != null)
            {
                selectedData.onSelected.Invoke();
            }
        }
    }

    public override void OnMovedHorizontal(int amount)
    {
        if(dropdownData.Length > 0)
        {
            base.OnMovedHorizontal(amount);

            ChangeSelected(amount);
        }
    }
}
