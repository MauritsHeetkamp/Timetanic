using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Lobby : Menu
{
    [SerializeField] PlayerCharacterData[] allCharacters;
    List<PlayerCharacterData> availableCharacters;
    [SerializeField] bool onlyUniqueCharacters = true;
    [SerializeField] PlayerIcon[] playerIcons; // Icons for when players are connected
    [SerializeField] int maxPlayers = 2;
    [SerializeField] int minPlayers = 1;

    [SerializeField] UIButton startButton;

    [SerializeField] UIController controller; // Controls the ui buttons on screen

    private void Awake()
    {
        if(PlayerManager.instance != null)
        {
            foreach(PlayerData data in PlayerManager.instance.connectedToPCPlayers)
            {
                data.preferredPlayer = null;
            }
        }
    }

    private void OnEnable()
    {
        PlayerManager.instance.onNewPlayerConnected += PlayerConnectionAdded;
        foreach (PlayerData data in PlayerManager.instance.connectedToPCPlayers)
        {
            data.onPlayerReconnect += PlayerConnectionReconnected;
            data.onPlayerDisconnect += PlayerConnectionRemoved;
        }
        LoadConnectedPlayers(); // Reloads connection data
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

    // What happens when the menu is opened
    public override void OnOpenMenu()
    {
        base.OnOpenMenu();

        LoadConnectedPlayers(); // Reloads connection data
    }

    // Sets the maximum allowed players
    public void SetMaxPlayerAmount(int amount)
    {
        maxPlayers = amount;
        if(PlayerManager.instance.connectedToLobbyPlayers.Count > maxPlayers) // Are there more then allowed players?
        {
            PlayerManager.instance.connectedToLobbyPlayers.RemoveRange(maxPlayers, PlayerManager.instance.connectedToLobbyPlayers.Count - maxPlayers); // Remove players until it reaches max players again
        }
        if(PlayerManager.instance.connectedToLobbyPlayers.Count < maxPlayers) // Aren't all player slots filled?
        {
            while(PlayerManager.instance.connectedToLobbyPlayers.Count < maxPlayers)
            {
                PlayerManager.instance.connectedToLobbyPlayers.Add(null); // Create an open player slot
            }
        }
    }

    // Sets minimal player amount
    public void SetMinPlayerAmount(int amount)
    {
        minPlayers = amount;
        UpdateUI();
    }


    void SetPlayerCharacters()
    {
        availableCharacters = new List<PlayerCharacterData>(allCharacters);

        foreach (PlayerData player in PlayerManager.instance.connectedToLobbyPlayers)
        {
            if(player != null && player.preferredPlayer != null)
            {
                availableCharacters.Remove(player.preferredPlayer);
            }
        }

        foreach (PlayerData player in PlayerManager.instance.connectedToLobbyPlayers)
        {
            SetPlayerCharacter(player);
        }
    }

    void SetPlayerCharacter(PlayerData player)
    {
        if(player != null)
        {
            if (player.preferredPlayer == null || !availableCharacters.Contains(player.preferredPlayer))
            {
                if (availableCharacters.Count > 0 && onlyUniqueCharacters)
                {
                    int selectedCharacter = Random.Range(0, availableCharacters.Count);

                    player.preferredPlayer = availableCharacters[selectedCharacter];
                    availableCharacters.RemoveAt(selectedCharacter);
                }
                else
                {
                    int selectedCharacter = Random.Range(0, allCharacters.Length);

                    player.preferredPlayer = allCharacters[selectedCharacter];
                }


            }
            else
            {
                availableCharacters.Remove(player.preferredPlayer);
            }

            int playerIndex = PlayerManager.instance.connectedToLobbyPlayers.IndexOf(player);

            PlayerIcon icon = playerIcons[playerIndex];

            icon.previewingObject = Instantiate(player.preferredPlayer.lobbyInstance, icon.previewSpawnLocation.position, icon.previewSpawnLocation.rotation, icon.previewSpawnLocation);
            LobbyPlayer lobbyPlayer = icon.previewingObject.GetComponent<LobbyPlayer>();

            lobbyPlayer.previewCamera.targetTexture = icon.assignedRendertexture;
        }
    }

    // Reloads connection data
    void LoadConnectedPlayers()
    {
        List<PlayerData> availablePlayers = new List<PlayerData>();

        List<PlayerData> connectedToPcPlayers = PlayerManager.instance.connectedToPCPlayers; // Gets all players connected to the device
        for (int i = 0; i < connectedToPcPlayers.Count; i++)
        {
            if (connectedToPcPlayers[i].isConnected && !PlayerManager.instance.connectedToLobbyPlayers.Contains(connectedToPcPlayers[i])) // Checks if they can join and aren't already in the lobby
            {
                availablePlayers.Add(connectedToPcPlayers[i]); // Adds player to possible candidates
            }
        }

        for(int i = 0; i < PlayerManager.instance.connectedToLobbyPlayers.Count; i++) // Goes through all players in lobby
        {
            if (PlayerManager.instance.connectedToLobbyPlayers[i] == null || !PlayerManager.instance.connectedToLobbyPlayers[i].isConnected) // Checks if slot is free or player has disconnected
            {
                if(availablePlayers.Count > 0)
                {
                    PlayerManager.instance.connectedToLobbyPlayers[i] = availablePlayers[0]; // Adds player to lobby
                    availablePlayers.RemoveAt(0);
                }
                else
                {
                    PlayerManager.instance.connectedToLobbyPlayers[i] = null; // Frees up player slot
                }
            }
        }

        for (int i = maxPlayers - 1; i >= 0; i--)
        {
            if (PlayerManager.instance.connectedToLobbyPlayers[i] == null)
            {
                PlayerManager.instance.connectedToLobbyPlayers.RemoveAt(i); // Removes all empty spots
            }
        }

        // Everyone is now lined up again

        while (PlayerManager.instance.connectedToLobbyPlayers.Count < maxPlayers)
        {
            PlayerManager.instance.connectedToLobbyPlayers.Add(null); // Fill with empty spots
        }

        SetPlayerCharacters();
        UpdateUI();
    }

    // Updates the ui with the provided data
    void UpdateUI()
    {
        int connectedAmount = 0;
        for (int i = 0; i < PlayerManager.instance.connectedToLobbyPlayers.Count; i++)
        {
            if (PlayerManager.instance.connectedToLobbyPlayers[i] != null && PlayerManager.instance.connectedToLobbyPlayers[i].isConnected) // Checks if this belongs to a player and is connected
            {
                connectedAmount++;
                playerIcons[i].playerUIObject.SetActive(true); // enables that specific player icon
            }
            else
            {
                playerIcons[i].playerUIObject.SetActive(false); // disables that specific player icon
            }
        }

        startButton.SetInteractable(connectedAmount >= minPlayers ? true : false); // can the game start? enables button on result
    }

    // Try to add player to lobby
    void TryAddPlayer(PlayerData player)
    {
        for (int i = 0; i < PlayerManager.instance.connectedToLobbyPlayers.Count; i++)
        {
            if (PlayerManager.instance.connectedToLobbyPlayers[i] == null || !PlayerManager.instance.connectedToLobbyPlayers[i].isConnected) // Tries to find a spot that is free
            {
                PlayerManager.instance.connectedToLobbyPlayers[i] = player; // Connects player to the free spot
                SetPlayerCharacter(player);
                UpdateUI();
                break;
            }
        }
    }

    // New player connected
    void PlayerConnectionAdded(PlayerData data)
    {
        data.onPlayerReconnect += PlayerConnectionReconnected;
        data.onPlayerDisconnect += PlayerConnectionRemoved;
        TryAddPlayer(data);
    }

    // Old player reconnected
    void PlayerConnectionReconnected(PlayerData data)
    {
        TryAddPlayer(data);
    }

    // Old player disconnected
    void PlayerConnectionRemoved(PlayerData data)
    {
        Debug.Log(data.isConnected);

        if(data.preferredPlayer != null)
        {
            availableCharacters.Add(data.preferredPlayer);
        }

        int playerIndex = PlayerManager.instance.connectedToLobbyPlayers.IndexOf(data);

        PlayerIcon playerIcon = playerIcons[playerIndex];
        Destroy(playerIcon.previewingObject);

        LoadConnectedPlayers();
    }

    [System.Serializable]
    public class PlayerIcon
    {
        public RenderTexture assignedRendertexture;
        public GameObject playerUIObject;

        public Transform previewSpawnLocation;
        public GameObject previewingObject;
    }
}
