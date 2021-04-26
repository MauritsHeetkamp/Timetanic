using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Location : MonoBehaviour
{
    public AudioClip goToThisLocationAudio;

    public GameObject[] minigames;



    private void Awake()
    {
        foreach(GameObject minigame in minigames)
        {
            Minigame minigameComponent = minigame.GetComponent<Minigame>();

            if(minigameComponent != null)
            {
                minigameComponent.location = this;
            }
        }
    }
}
