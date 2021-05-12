using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class Splitscreen : MonoBehaviour
{
    public Player owner;
    public RawImage splitscreenRenderImage;
    [SerializeField] GameObject taskUI;
    public FadeManager fadeManager;
    public Transform taskHolder;
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

    private void Start()
    {
        if (owner != null && !initialized)
        {
            UpdateFollowingPlayerAmount();
            initialized = true;
            owner.owner.onTaskMenu += ToggleTaskUI;
        }
    }

    private void OnDisable()
    {
        initialized = false;
        if (taskUI.activeSelf)
        {
            taskUI.SetActive(false);
        }
        owner.owner.onTaskMenu -= ToggleTaskUI;
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

    public void SetItem(Sprite newImage, string itemName)
    {
        if(itemImage.sprite != newImage)
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
