﻿using System.Collections;
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
    
    
    void Awake()
    {
        instance = this;
        DontDestroyOnLoad(this); // Prevents object from being destroyed when swapping scenes
    }

    // Gets called when a new player connected
    public void NewPlayerConnected(PlayerInput input)
    {
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
