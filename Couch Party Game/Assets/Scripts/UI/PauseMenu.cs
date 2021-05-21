using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] MenuMasterObject masterUI;
    public UnityAction onPaused, onUnpaused;
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
                    data.onMenuStart += Pause;
                    data.onMenuEnd += Resume;
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
                    data.onMenuStart -= Pause;
                    data.onMenuEnd -= Resume;
                }
            }
        }
    }

    public void Pause(InputAction.CallbackContext context, PlayerData owner)
    {
        Time.timeScale = 0;
        if(onPaused != null)
        {
            onPaused.Invoke();
        }
    }

    public void Resume(InputAction.CallbackContext context, PlayerData owner)
    {
        Time.timeScale = 1;
        if(onUnpaused != null)
        {
            onUnpaused.Invoke();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
