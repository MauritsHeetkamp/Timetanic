using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class PlayerData : MonoBehaviour
{
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

    // This allows for easy usability with swapping controls (UI Controls)
    public UnityAction<InputAction.CallbackContext, PlayerData> onHorizontalAxis;
    public UnityAction<InputAction.CallbackContext, PlayerData> onVerticalAxis;
    public UnityAction<InputAction.CallbackContext, PlayerData> onSelect;
    public UnityAction<InputAction.CallbackContext, PlayerData> onMenuEnd;

    void Awake()
    {
        DontDestroyOnLoad(this); // Prevents object from being destroyed when swapping scenes
        if(playerInput == null)
        {
            Debug.LogError("No PlayerInput found");
        }
        else
        {
            playerInput.onDeviceLost += SetConnectionFalse;
            playerInput.onDeviceRegained += SetConnectionTrue;
        }
    }

    // Sets the player connection state to true
    void SetConnectionTrue(PlayerInput input)
    {
        isConnected = true;
        if(onPlayerReconnect != null)
        {
            onPlayerReconnect.Invoke(this);
        }
    }

    // Sets the player connection state to false
    void SetConnectionFalse(PlayerInput input)
    {
        isConnected = false;
        if(onPlayerDisconnect != null)
        {
            onPlayerDisconnect.Invoke(this);
        }
    }

    // Swaps the used input action map
    public void SwapInputScheme(string schemeName)
    {
        playerInput.SwitchCurrentActionMap(schemeName);
    }

    // Called when the movement button is used
    public void OnMove(InputAction.CallbackContext context)
    {
        if(onMove != null)
        {
            onMove.Invoke(context, this);
        }
    }

    // Called when the dash button is used
    public void OnDash(InputAction.CallbackContext context)
    {
        if (onDash != null)
        {
            onDash.Invoke(context, this);
        }
    }

    // Called when the drop button is used
    public void OnDrop(InputAction.CallbackContext context)
    {
        if (onDrop != null)
        {
            onDrop.Invoke(context, this);
        }
    }

    // Called when the throw button is used
    public void OnThrow(InputAction.CallbackContext context)
    {
        if (onThrow != null)
        {
            onThrow.Invoke(context, this);
        }
    }

    // Called when the interact button is used
    public void OnInteract(InputAction.CallbackContext context)
    {
        if (onInteract != null)
        {
            onInteract.Invoke(context, this);
        }
    }

    // Called when the use button is used
    public void OnUse(InputAction.CallbackContext context)
    {
        if (onUse != null)
        {
            onUse.Invoke(context, this);
        }
    }

    // Called when the jump button is used
    public void OnJump(InputAction.CallbackContext context)
    {
        if (onJump != null)
        {
            onJump.Invoke(context, this);
        }
    }

    // Called when the menu button is used
    public void OnMenuStart(InputAction.CallbackContext context)
    {
        if (onMenuStart != null)
        {
            onMenuStart.Invoke(context, this);
        }
    }

    // Called when the horizontal button is used
    public void OnHorizontalAxis(InputAction.CallbackContext context)
    {
        if (onHorizontalAxis != null)
        {
            onHorizontalAxis.Invoke(context, this);
        }
    }

    // Called when the vertical button is used
    public void OnVerticalAxis(InputAction.CallbackContext context)
    {
        if (onVerticalAxis != null)
        {
            onVerticalAxis.Invoke(context, this);
        }
    }

    // Called when the select button is used
    public void OnSelect(InputAction.CallbackContext context)
    {
        if (onSelect != null)
        {
            onSelect.Invoke(context, this);
        }
    }

    // Called when the menu button is used
    public void OnMenuEnd(InputAction.CallbackContext context)
    {
        if (onMenuEnd != null)
        {
            onMenuEnd.Invoke(context, this);
        }
    }
}
