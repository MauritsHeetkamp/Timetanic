using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Popup : MonoBehaviour
{
    [SerializeField] Animator popupAnimator;
    [SerializeField] string itemPopupString = "Popup";
    public WorldspaceFollow worldspaceFollow;
    [SerializeField] bool popupOpened;

    private void Start()
    {
        popupOpened = !popupOpened;
        SetPopup(!popupOpened);
    }

    public void SetPopup(bool popup)
    {
        if (popup)
        {
            if (!popupOpened)
            {
                popupOpened = true;
                if (worldspaceFollow != null)
                {
                    worldspaceFollow.enabled = true;
                }
                popupAnimator.SetBool(itemPopupString, true);
            }
        }
        else
        {
            if (popupOpened)
            {
                popupOpened = false;
                if (worldspaceFollow != null)
                {
                    worldspaceFollow.enabled = false;
                }
                popupAnimator.SetBool(itemPopupString, false);
            }
        }
    }
}
