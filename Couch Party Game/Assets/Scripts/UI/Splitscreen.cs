using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class Splitscreen : MonoBehaviour
{
    public Player owner;
    public RawImage splitscreenRenderImage;
    [SerializeField] GameObject taskUI;
    public Transform taskHolder;
    bool initialized;

    private void OnEnable()
    {
        Debug.Log(owner);
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
            initialized = true;
            owner.owner.onTaskMenu += ToggleTaskUI;
        }
    }

    private void OnDisable()
    {
        initialized = false;
        Debug.Log("DISABLED");
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
            Debug.Log("TOGGLED");
            taskUI.SetActive(!taskUI.activeSelf);
        }
    }
}
