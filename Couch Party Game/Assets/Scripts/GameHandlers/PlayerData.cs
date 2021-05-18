using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class PlayerData : MonoBehaviour
{
    public static bool disableFirstFrameInteract;
    bool canInteract = true;
    public PlayerCharacterData preferredPlayer;

    public InputType inputType;

    public bool isConnected = true;
    public PlayerInput playerInput; // Connected input scheme

    public UnityAction<PlayerData> onPlayerDisconnect;
    public UnityAction<PlayerData> onPlayerReconnect;

    // This allows for easy usability with swapping controls (PlayerControls)
    public UnityAction<InputAction.CallbackContext, PlayerData> onMove;
    public UnityAction<InputAction.CallbackContext, PlayerData> onDash;
    public UnityAction<InputAction.CallbackContext, PlayerData> onDrop;
    public UnityAction<InputAction.CallbackContext, PlayerData> onThrow;
    public UnityAction<InputAction.CallbackContext, PlayerData> onInteract;
    public UnityAction<InputAction.CallbackContext, PlayerData> onUse;
    public UnityAction<InputAction.CallbackContext, PlayerData> onJump;
    public UnityAction<InputAction.CallbackContext, PlayerData> onMenuStart;
    public UnityAction<InputAction.CallbackContext, PlayerData> onTaskMenu;

    // This allows for easy usability with swapping controls (UI Controls)
    public UnityAction<InputAction.CallbackContext, PlayerData> onHorizontalAxis;
    public UnityAction<InputAction.CallbackContext, PlayerData> onVerticalAxis;
    public UnityAction<InputAction.CallbackContext, PlayerData> onSelect;
    public UnityAction<InputAction.CallbackContext, PlayerData> onMenuEnd;
    public UnityAction<InputAction.CallbackContext, PlayerData> onScroll;
    public UnityAction<InputAction.CallbackContext, PlayerData> onAnyInputUI;

    private void Awake()
    {
        if(disableFirstFrameInteract)
        {
            canInteract = false;
        }

        DontDestroyOnLoad(this); // Prevents object from being destroyed when swapping scenes
        Debug.Log("Started");
        if (playerInput == null)
        {
            Debug.LogError("No PlayerInput found");
        }
    }

    private void Start()
    {
        canInteract = true;

        if (playerInput.devices[0].displayName.Contains("Keyboard") || playerInput.devices[0].displayName.Contains("keyboard"))
        {
            inputType = InputType.Keyboard;
        }
        else
        {
            inputType = InputType.Controller;
        }
    }

    public void AnyButtonPressed(InputAction.CallbackContext context)
    {
        if (onAnyInputUI != null)
        {
            onAnyInputUI.Invoke(context, this);
        }
    }

    // Sets the player connection state to true
    public void SetConnectionTrue(PlayerInput input)
    {
        Debug.Log("DEVICE FOUND");
        isConnected = true;
        if(onPlayerReconnect != null)
        {
            onPlayerReconnect.Invoke(this);
        }
    }

    // Sets the player connection state to false
    public void SetConnectionFalse(PlayerInput input)
    {
        Debug.Log("DEVICE REMOVED");
        isConnected = false;
        if(onPlayerDisconnect != null)
        {
            onPlayerDisconnect.Invoke(this);
        }
    }

    // Swaps the used input action map
    public void SwapInputScheme(string schemeName)
    {
        Debug.Log("SWAPPED");
        playerInput.SwitchCurrentActionMap(schemeName);
    }

    // Called when the scroll button is used
    public void OnScrolled(InputAction.CallbackContext context)
    {
        if (onScroll != null && canInteract)
        {
            onScroll.Invoke(context, this);
        }
    }

    // Called when the movement button is used
    public void OnMove(InputAction.CallbackContext context)
    {
        if(onMove != null && canInteract)
        {
            onMove.Invoke(context, this);
        }
    }

    // Called when the dash button is used
    public void OnDash(InputAction.CallbackContext context)
    {
        if (onDash != null && canInteract)
        {
            onDash.Invoke(context, this);
        }
    }

    // Called when the drop button is used
    public void OnDrop(InputAction.CallbackContext context)
    {
        if (onDrop != null && canInteract)
        {
            onDrop.Invoke(context, this);
        }
    }

    // Called when the throw button is used
    public void OnThrow(InputAction.CallbackContext context)
    {
        if (onThrow != null && canInteract)
        {
            onThrow.Invoke(context, this);
        }
    }

    // Called when the interact button is used
    public void OnInteract(InputAction.CallbackContext context)
    {
        if (onInteract != null && canInteract)
        {
            onInteract.Invoke(context, this);
        }
    }

    // Called when the use button is used
    public void OnUse(InputAction.CallbackContext context)
    {
        if (onUse != null && canInteract)
        {
            onUse.Invoke(context, this);
        }
    }

    // Called when the jump button is used
    public void OnJump(InputAction.CallbackContext context)
    {
        if (onJump != null && canInteract)
        {
            onJump.Invoke(context, this);
        }
    }

    public void OnTaskMenu(InputAction.CallbackContext context)
    {
        Debug.Log("TASK MENU");
        if(onTaskMenu != null && canInteract)
        {
            onTaskMenu.Invoke(context, this);
        }
    }

    // Called when the menu button is used
    public void OnMenuStart(InputAction.CallbackContext context)
    {
        if (onMenuStart != null && canInteract)
        {
            onMenuStart.Invoke(context, this);
        }
    }

    // Called when the horizontal button is used
    public void OnHorizontalAxis(InputAction.CallbackContext context)
    {
        if (onHorizontalAxis != null && canInteract)
        {
            onHorizontalAxis.Invoke(context, this);
        }
    }

    // Called when the vertical button is used
    public void OnVerticalAxis(InputAction.CallbackContext context)
    {
        if (onVerticalAxis != null && canInteract)
        {
            onVerticalAxis.Invoke(context, this);
        }
    }

    // Called when the select button is used
    public void OnSelect(InputAction.CallbackContext context)
    {
        if (onSelect != null && canInteract)
        {
            onSelect.Invoke(context, this);
        }
    }

    // Called when the menu button is used
    public void OnMenuEnd(InputAction.CallbackContext context)
    {
        if (onMenuEnd != null && canInteract)
        {
            onMenuEnd.Invoke(context, this);
        }
    }

    public enum InputType
    {
        Keyboard, Controller
    }
}
