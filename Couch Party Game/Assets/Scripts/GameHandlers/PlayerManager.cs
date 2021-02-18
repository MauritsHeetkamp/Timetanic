using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;
    public List<PlayerData> connectedToPCPlayers = new List<PlayerData>();

    public List<PlayerData> connectedToLobbyPlayers = new List<PlayerData>();

    public UnityAction<PlayerData> onNewPlayerConnected;
    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        DontDestroyOnLoad(this);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void NewPlayerConnected(PlayerInput input)
    {
        connectedToPCPlayers.Add(input.GetComponent<PlayerData>());
        if(onNewPlayerConnected != null)
        {
            onNewPlayerConnected.Invoke(input.GetComponent<PlayerData>());
        }
    }

    public void PlayerDisconnected(PlayerInput input)
    {
        connectedToPCPlayers.Remove(input.GetComponent<PlayerData>());
    }
}
