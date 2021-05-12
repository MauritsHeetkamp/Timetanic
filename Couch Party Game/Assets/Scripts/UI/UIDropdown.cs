using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Custom.Types;

public class UIDropdown : UIOption
{
    public DropdownData[] dropdownData;
    public int selected;

    [SerializeField] TextMeshProUGUI selectedText, debug;


    private void Update()
    {
        if(debug != null)
        {
            debug.text = Screen.currentResolution.width + "x" + Screen.currentResolution.height;
        }
    }

    public void Initialize(DropdownData[] data)
    {
        dropdownData = data;
        LoadData();
    }

    public void LoadData()
    {
        if(dropdownData.Length > 0)
        {
            if(selected < dropdownData.Length)
            {
                DropdownData selectedData = dropdownData[selected];
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
