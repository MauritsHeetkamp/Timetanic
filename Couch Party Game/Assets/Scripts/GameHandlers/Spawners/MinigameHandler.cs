using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MinigameHandler : MonoBehaviour
{
    [SerializeField] Minigame[] allMinigames;
    public TaskManager taskHandler;

    public UnityAction onCompleteInit;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void Initialize()
    {
        foreach(Minigame minigame in allMinigames)
        {
            minigame.Initialize(this);
        }

        if(onCompleteInit != null)
        {
            onCompleteInit.Invoke();
        }
    }
}
