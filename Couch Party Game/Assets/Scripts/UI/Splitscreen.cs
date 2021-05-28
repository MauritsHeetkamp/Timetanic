using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class Splitscreen : MonoBehaviour
{
    public Transform uiHolder;

    public Player owner;
    public RawImage splitscreenRenderImage;
    [SerializeField] GameObject taskUI;
    public FadeManager fadeManager;
    public Transform taskHolder;
    [SerializeField] RectTransform itemHolderTransform;
    bool initialized;

    [SerializeField] string noItemText = "None";
    [SerializeField] Image itemImage;
    [SerializeField] TextMeshProUGUI itemText;
    [SerializeField] float closeItemAnimLength, openItemAnimLength;
    [SerializeField] Animator itemAnimator;
    Coroutine swapItemRoutine;


    [SerializeField] TextMeshProUGUI passengerCountText;
    [SerializeField] Animator passengerCountState;
    [SerializeField] string maxPassengersBool = "Maxed";

    private void OnEnable()
    {
        if(owner != null && !initialized)
        {
            initialized = true;
            owner.owner.onTaskMenu += ToggleTaskUI;
        }
    }

    public void SetReferenceScale(int maxScreenOnSide)
    {
        if(maxScreenOnSide > 1)
        {
            taskUI.transform.localScale = Vector3.one / maxScreenOnSide;
            itemHolderTransform.localPosition += new Vector3(itemHolderTransform.sizeDelta.x / 2 / maxScreenOnSide, -itemHolderTransform.sizeDelta.y / 2 / maxScreenOnSide, 0);
            itemHolderTransform.localScale /= maxScreenOnSide;
        }
    }

    private void Start()
    {
        if (owner != null)
        {
            if (!initialized)
            {
                UpdateFollowingPlayerAmount();
                initialized = true;
                owner.owner.onTaskMenu += ToggleTaskUI;
            }
        }
        else
        {
            taskUI.SetActive(false);
        }
    }

    private void OnDisable()
    {
        initialized = false;
        if (taskUI.activeSelf)
        {
            taskUI.SetActive(false);
        }
        if (owner != null && owner.owner != null)
        {
            owner.owner.onTaskMenu -= ToggleTaskUI;
        }
    }


    public void ToggleTaskUI(InputAction.CallbackContext context, PlayerData owner)
    {
        if (context.started)
        {
            taskUI.SetActive(!taskUI.activeSelf);
        }
    }

    public void UpdateFollowingPlayerAmount()
    {
        passengerCountState.SetTrigger("ChangedAmount");
        passengerCountState.SetBool(maxPassengersBool, owner.followingPassengers.Count >= owner.maxFollowingPassengers);
        passengerCountText.text = owner.followingPassengers.Count.ToString() + "/" + owner.maxFollowingPassengers.ToString();
    }

    public void SetItemInstant(Sprite newImage, string itemName)
    {
        if (itemImage.sprite != newImage || itemName != itemText.text)
        {
            string selectedItemName = newImage != null ? itemName : noItemText;
            itemText.text = selectedItemName;
            itemImage.sprite = newImage;
        }
    }

    public void SetItem(Sprite newImage, string itemName)
    {
        if(itemImage.sprite != newImage || itemName != itemText.text)
        {
            string selectedItemName = newImage != null ? itemName : noItemText;

            if(swapItemRoutine != null)
            {
                StopCoroutine(swapItemRoutine);
            }
            swapItemRoutine = StartCoroutine(SetItemRoutine(newImage, selectedItemName));
        }
    }

    IEnumerator SetItemRoutine(Sprite _newImage, string itemName)
    {
        itemText.text = itemName;

        if (itemAnimator.GetBool("HasItem"))
        {
            itemAnimator.SetBool("HasItem", false);


            yield return new WaitForSeconds(closeItemAnimLength);
        }

        itemImage.sprite = _newImage;

        if (_newImage != null)
        {
            itemAnimator.SetBool("HasItem", true);
        }
    }
}
