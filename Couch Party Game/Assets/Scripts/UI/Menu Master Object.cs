using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MenuMasterObject : Menu
{
    [SerializeField] bool closable = true;
    [SerializeField] bool specificPlayerOnly;
    [SerializeField] bool lobbyPlayersOnly;
    [SerializeField] bool resetOnOpen;
    string previousControlScheme;
    [SerializeField] string newControlScheme = "UI";

    [SerializeField] GameObject[] allHolders;
    [SerializeField] GameObject targetMenu;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDisable()
    {
        try
        {
            PlayerManager.instance.onNewPlayerConnected -= OnNewPlayerJoined;
        }
        catch
        {
            Debug.Log("Event was already removed");
        }
    }

    private void OnEnable()
    {
        if (targetMenu.activeSelf)
        {
            foreach (PlayerData data in PlayerManager.instance.connectedToPCPlayers)
            {
                data.SwapInputScheme(newControlScheme);
            }
        }
    }

    public void OnNewPlayerJoined(PlayerData owner)
    {
        if(!specificPlayerOnly && targetMenu.activeSelf)
        {
            if (lobbyPlayersOnly)
            {
                if (PlayerManager.instance.connectedToLobbyPlayers.Contains(owner))
                {
                    owner.SwapInputScheme(newControlScheme);
                }
            }
            else
            {
                owner.SwapInputScheme(newControlScheme);
            }
        }
    }

    public void OpenMenu(InputAction.CallbackContext context, PlayerData owner)
    {
        if (context.started && !targetMenu.activeSelf)
        {
            PlayerManager.instance.onNewPlayerConnected += OnNewPlayerJoined;
            if (resetOnOpen)
            {
                foreach(GameObject target in allHolders)
                {
                    target.SetActive(false);
                }
            }
            previousControlScheme = owner.playerInput.currentControlScheme;
            if (lobbyPlayersOnly && PlayerManager.instance.connectedToLobbyPlayers.Contains(owner))
            {
                if (specificPlayerOnly)
                {
                    owner.SwapInputScheme(newControlScheme);
                }
                else
                {
                    foreach (PlayerData data in PlayerManager.instance.connectedToLobbyPlayers)
                    {
                        if(data != null)
                        {
                            data.SwapInputScheme(newControlScheme);
                        }
                    }
                }
            }
            else
            {
                if (specificPlayerOnly)
                {
                    owner.SwapInputScheme(newControlScheme);
                }
                else
                {
                    foreach (PlayerData data in PlayerManager.instance.connectedToPCPlayers)
                    {
                        data.SwapInputScheme(newControlScheme);
                    }
                }
            }

            targetMenu.SetActive(true);
        }
    }

    public void CloseMenu(InputAction.CallbackContext context, PlayerData owner)
    {
        if (closable && context.started && targetMenu.activeSelf)
        {
            PlayerManager.instance.onNewPlayerConnected -= OnNewPlayerJoined;

            previousControlScheme = owner.playerInput.currentControlScheme;
            if (lobbyPlayersOnly && PlayerManager.instance.connectedToLobbyPlayers.Contains(owner))
            {
                if (specificPlayerOnly)
                {
                    owner.SwapInputScheme(previousControlScheme);
                }
                else
                {
                    foreach (PlayerData data in PlayerManager.instance.connectedToLobbyPlayers)
                    {
                        if(data != null)
                        {
                            data.SwapInputScheme(previousControlScheme);
                        }
                    }
                }
            }
            else
            {
                if (specificPlayerOnly)
                {
                    owner.SwapInputScheme(previousControlScheme);
                }
                else
                {
                    foreach (PlayerData data in PlayerManager.instance.connectedToPCPlayers)
                    {
                        data.SwapInputScheme(previousControlScheme);
                    }
                }
            }

            targetMenu.SetActive(false);
        }
    }
}
