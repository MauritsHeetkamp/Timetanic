using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Popup : MonoBehaviour
{
    [SerializeField] Animator popupAnimator;
    [SerializeField] string itemPopupString = "Popup";
    public WorldspaceFollow worldspaceFollow;
    [SerializeField] int popupOpened;
    [SerializeField] bool forceOpen;

    private void Start()
    {
        if (forceOpen)
        {
            SetPopup(true);
        }
    }

    public void SetPopup(bool popup)
    {
        if (popup)
        {
            popupOpened++;
            if (popupOpened == 1)
            {
                if (worldspaceFollow != null)
                {
                    worldspaceFollow.enabled = true;
                }
                popupAnimator.SetBool(itemPopupString, true);
            }
        }
        else
        {
            popupOpened--;
            if (popupOpened <= 0)
            {
                if (worldspaceFollow != null)
                {
                    worldspaceFollow.enabled = false;
                }
                popupAnimator.SetBool(itemPopupString, false);
            }
        }
    }
}
