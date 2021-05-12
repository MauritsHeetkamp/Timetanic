using System.Collections.Generic;
using UnityEngine;

public class Lobby : Menu
{
    [SerializeField] PlayerCharacterData[] allCharacters;
    List<PlayerCharacterData> availableCharacters;
    [SerializeField] bool onlyUniqueCharacters = true;
    [SerializeField] LobbyPlayerData[] playerIcons; // Icons for when players are connected
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
        PlayerData.disableFirstFrameInteract = true;
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
        PlayerData.disableFirstFrameInteract = false;
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

        for(int i = 0; i < PlayerManager.instance.connectedToLobbyPlayers.Count; i++)
        {
            SetPlayerCharacter(i);
        }
    }

    void SetPlayerCharacter(int playerIndex)
    {
        LobbyPlayerData icon = playerIcons[playerIndex];

        PlayerData player = PlayerManager.instance.connectedToLobbyPlayers[playerIndex];
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

            if (icon.previewingObject != null && !icon.previewingObject.name.Contains(player.preferredPlayer.lobbyInstance.name))
            {
                DestroyImmediate(icon.previewingObject);
            }

            if(icon.previewingObject == null)
            {
                icon.previewingObject = Instantiate(player.preferredPlayer.lobbyInstance, icon.previewSpawnLocation.position, icon.previewSpawnLocation.rotation, icon.previewSpawnLocation);
                LobbyPlayer lobbyPlayer = icon.previewingObject.GetComponent<LobbyPlayer>();

                lobbyPlayer.previewCamera.targetTexture = icon.assignedRendertexture;
            }
        }
        else
        {
            if (icon.previewingObject != null)
            {
                DestroyImmediate(icon.previewingObject);
            }
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

        for(int i = PlayerManager.instance.connectedToLobbyPlayers.Count - 1; i >= 0; i--) // Goes through all players in lobby
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
                    PlayerManager.instance.connectedToPCPlayers.RemoveAt(i);
                }
            }
        }

        for(int i = 0; i < maxPlayers - PlayerManager.instance.connectedToLobbyPlayers.Count; i++)
        {
            if (availablePlayers.Count > 0)
            {
                PlayerManager.instance.connectedToLobbyPlayers.Add(availablePlayers[0]); // Adds player to lobby
                availablePlayers.RemoveAt(0);
            }
            else
            {
                break;
            }
        }

        /*for (int i = maxPlayers - 1; i >= 0; i--)
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
        }*/

        SetPlayerCharacters();
        UpdateUI();
    }

    // Updates the ui with the provided data
    void UpdateUI()
    {
        for (int i = 0; i < playerIcons.Length; i++)
        {
            UpdateSpecificUI(i);
        }

        //startButton.SetInteractable(connectedAmount >= minPlayers ? true : false); // can the game start? enables button on result
    }

    void UpdateSpecificUI(int targetIndex)
    {
        PlayerData targetPlayer = PlayerManager.instance.connectedToLobbyPlayers.Count > targetIndex ? PlayerManager.instance.connectedToLobbyPlayers[targetIndex] : null;

        if(targetPlayer != null && PlayerManager.instance.connectedToLobbyPlayers[targetIndex].isConnected)
        {
            playerIcons[targetIndex].playerUIObject.SetActive(true); // enables that specific player icon
        }
        else
        {
            playerIcons[targetIndex].playerUIObject.SetActive(false); // disables that specific player icon
        }
    }

    // Try to add player to lobby
    void TryAddPlayer(PlayerData player)
    {
        for (int i = 0; i < PlayerManager.instance.connectedToLobbyPlayers.Count; i++)
        {
            if (PlayerManager.instance.connectedToLobbyPlayers[i] == null || !PlayerManager.instance.connectedToLobbyPlayers[i].isConnected) // Tries to find a spot that is free
            {
                PlayerManager.instance.connectedToLobbyPlayers[i] = player; // Connects player to the free spot
                SetPlayerCharacter(i);
                UpdateSpecificUI(i);
                return;
            }
        }

        if(PlayerManager.instance.connectedToLobbyPlayers.Count < maxPlayers)
        {
            PlayerManager.instance.connectedToLobbyPlayers.Add(player);
            int playerIndex = PlayerManager.instance.connectedToLobbyPlayers.Count;
            SetPlayerCharacter(playerIndex);
            UpdateSpecificUI(playerIndex);
        }
    }

    public void RemovePlayer(PlayerData data)
    {
        if (data.preferredPlayer != null)
        {
            availableCharacters.Add(data.preferredPlayer);
        }

        int playerIndex = PlayerManager.instance.connectedToLobbyPlayers.IndexOf(data);

        LobbyPlayerData playerIcon = playerIcons[playerIndex];
        if(playerIcon.previewingObject != null)
        {
            Destroy(playerIcon.previewingObject);
        }

        PlayerManager.instance.connectedToLobbyPlayers.RemoveAt(playerIndex);

        UpdateSpecificUI(playerIndex);
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

        if (PlayerManager.instance.connectedToLobbyPlayers.Contains(data))
        {
            RemovePlayer(data);
        }

        //LoadConnectedPlayers();
    }

    [System.Serializable]
    public class LobbyPlayerData
    {
        public RenderTexture assignedRendertexture;
        public GameObject playerUIObject;

        public bool ready;
        public GameObject readyUI;

        public Transform previewSpawnLocation;
        public GameObject previewingObject;

        public void ToggleReadyOn()
        {
            if (!ready)
            {
                ready = true;
                readyUI.SetActive(true);
            }
        }

        public void ToggleReadyOff()
        {
            if (ready)
            {
                ready = false;
                readyUI.SetActive(false);
            }
        }
    }
}
