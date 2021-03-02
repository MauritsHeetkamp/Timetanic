using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class PlayerData : MonoBehaviour
{
    public bool isConnected = true;
    public PlayerInput playerInput;

    public UnityAction<PlayerData> onPlayerDisconnect;
    public UnityAction<PlayerData> onPlayerReconnect;
    // Start is called before the first frame update
    void Awake()
    {
        DontDestroyOnLoad(this);
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

    // Update is called once per frame
    void Update()
    {
        
    }

    void SetConnectionTrue(PlayerInput input)
    {
        isConnected = true;
        if(onPlayerReconnect != null)
        {
            onPlayerReconnect.Invoke(this);
        }
    }

    void SetConnectionFalse(PlayerInput input)
    {
        isConnected = false;
        if(onPlayerDisconnect != null)
        {
            onPlayerDisconnect.Invoke(this);
        }
    }
}
