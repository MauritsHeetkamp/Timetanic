using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class Lobby : Menu
{
    [SerializeField] Image[] playerIcons;
    [SerializeField] int maxPlayers = 2;
    [SerializeField] int minPlayers = 1;

    [SerializeField] Button startButton;

    [SerializeField] bool keepSpecificSpot = true;
    // Start is called before the first frame update

    private void OnEnable()
    {
        PlayerManager.instance.onNewPlayerConnected += PlayerConnectionAdded;
        foreach (PlayerData data in PlayerManager.instance.connectedToPCPlayers)
        {
            data.onPlayerReconnect += PlayerConnectionReconnected;
            data.onPlayerDisconnect += PlayerConnectionRemoved;
        }
        LoadConnectedPlayers();
    }

    private void OnDisable()
    {
        PlayerManager.instance.onNewPlayerConnected -= PlayerConnectionAdded;
        foreach (PlayerData data in PlayerManager.instance.connectedToPCPlayers)
        {
            data.onPlayerReconnect -= PlayerConnectionReconnected;
            data.onPlayerDisconnect -= PlayerConnectionRemoved;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnOpenMenu()
    {
        base.OnOpenMenu();

        LoadConnectedPlayers();
    }


    public void SetMaxPlayerAmount(int amount)
    {
        maxPlayers = amount;
        if(PlayerManager.instance.connectedToLobbyPlayers.Count > maxPlayers)
        {
            PlayerManager.instance.connectedToLobbyPlayers.RemoveRange(maxPlayers, PlayerManager.instance.connectedToLobbyPlayers.Count - maxPlayers);
        }
        if(PlayerManager.instance.connectedToLobbyPlayers.Count < maxPlayers)
        {
            while(PlayerManager.instance.connectedToLobbyPlayers.Count < maxPlayers)
            {
                PlayerManager.instance.connectedToLobbyPlayers.Add(null);
            }
        }
    }

    public void SetMinPlayerAmount(int amount)
    {
        minPlayers = amount;
        UpdateUI();
    }

    void LoadConnectedPlayers()
    {
        List<PlayerData> availablePlayers = new List<PlayerData>();

        List<PlayerData> connectedToPcPlayers = PlayerManager.instance.connectedToPCPlayers;
        for (int i = 0; i < connectedToPcPlayers.Count; i++)
        {
            if (connectedToPcPlayers[i].isConnected && !PlayerManager.instance.connectedToLobbyPlayers.Contains(connectedToPcPlayers[i]))
            {
                availablePlayers.Add(connectedToPcPlayers[i]);
            }
        }

        for(int i = 0; i < PlayerManager.instance.connectedToLobbyPlayers.Count; i++)
        {
            if (PlayerManager.instance.connectedToLobbyPlayers[i] == null || !PlayerManager.instance.connectedToLobbyPlayers[i].isConnected)
            {
                if(availablePlayers.Count > 0)
                {
                    PlayerManager.instance.connectedToLobbyPlayers[i] = availablePlayers[0];
                    availablePlayers.RemoveAt(0);
                }
                else
                {
                    PlayerManager.instance.connectedToLobbyPlayers[i] = null;
                }
            }
        }

        if (!keepSpecificSpot)
        {
            for (int i = maxPlayers - 1; i >= 0; i--)
            {
                if (PlayerManager.instance.connectedToLobbyPlayers[i] == null)
                {
                    PlayerManager.instance.connectedToLobbyPlayers.RemoveAt(i);
                }
            }
            while (PlayerManager.instance.connectedToLobbyPlayers.Count < maxPlayers)
            {
                PlayerManager.instance.connectedToLobbyPlayers.Add(null);
            }
        }
        UpdateUI();
    }

    void UpdateUI()
    {
        int connectedAmount = 0;
        for (int i = 0; i < PlayerManager.instance.connectedToLobbyPlayers.Count; i++)
        {
            if (PlayerManager.instance.connectedToLobbyPlayers[i] != null && PlayerManager.instance.connectedToLobbyPlayers[i].isConnected)
            {
                connectedAmount++;
                playerIcons[i].gameObject.SetActive(true);
            }
            else
            {
                playerIcons[i].gameObject.SetActive(false);
            }
        }

        startButton.interactable = connectedAmount >= minPlayers ? true : false;
    }

    void TryAddPlayer(PlayerData player)
    {
        for (int i = 0; i < PlayerManager.instance.connectedToLobbyPlayers.Count; i++)
        {
            if (PlayerManager.instance.connectedToLobbyPlayers[i] == null || !PlayerManager.instance.connectedToLobbyPlayers[i].isConnected)
            {
                PlayerManager.instance.connectedToLobbyPlayers[i] = player;
                UpdateUI();
                break;
            }
        }
    }

    void PlayerConnectionAdded(PlayerData data)
    {
        data.onPlayerReconnect += PlayerConnectionReconnected;
        data.onPlayerDisconnect += PlayerConnectionRemoved;
        TryAddPlayer(data);
    }

    void PlayerConnectionReconnected(PlayerData data)
    {
        TryAddPlayer(data);
    }

    void PlayerConnectionRemoved(PlayerData data)
    {
        Debug.Log(data.isConnected);
        LoadConnectedPlayers();
    }
}
