using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] MenuMasterObject masterUI;
    // Start is called before the first frame update
    void Start()
    {

    }

    private void OnEnable()
    {
        if (PlayerManager.instance != null && masterUI != null)
        {
            foreach (PlayerData data in PlayerManager.instance.connectedToLobbyPlayers)
            {
                if(data != null)
                {
                    data.onMenuStart += masterUI.OpenMenu;
                    data.onMenuEnd += masterUI.CloseMenu;
                }
            }
        }
    }

    private void OnDisable()
    {
        if (PlayerManager.instance != null && masterUI != null)
        {
            foreach (PlayerData data in PlayerManager.instance.connectedToLobbyPlayers)
            {
                if(data != null)
                {
                    data.onMenuStart -= masterUI.OpenMenu;
                    data.onMenuEnd -= masterUI.CloseMenu;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
