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
}
