using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance; // Global playermanager instance
    public List<PlayerData> connectedToPCPlayers = new List<PlayerData>();

    public List<PlayerData> connectedToLobbyPlayers = new List<PlayerData>();

    public UnityAction<PlayerData> onNewPlayerConnected;


    [SerializeField] string[] bannedDevices;

    void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject); // Prevents object from being destroyed when swapping scenes
    }

    // Gets called when a new player connected
    public void NewPlayerConnected(PlayerInput input)
    {
        foreach(InputDevice device in input.devices)
        {
            foreach(string bannedDevice in bannedDevices)
            {
                if(device.name.Contains(bannedDevice))
                {
                    Destroy(input.gameObject);
                    return;
                }
            }
        }

        connectedToPCPlayers.Add(input.GetComponent<PlayerData>());
        if(onNewPlayerConnected != null)
        {
            onNewPlayerConnected.Invoke(input.GetComponent<PlayerData>());
        }
    }

    // Gets called when a player disconnects
    public void PlayerDisconnected(PlayerInput input)
    {
        connectedToPCPlayers.Remove(input.GetComponent<PlayerData>());
    }
}
