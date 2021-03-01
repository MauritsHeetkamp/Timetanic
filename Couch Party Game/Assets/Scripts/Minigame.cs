﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minigame : MonoBehaviour
{
    [SerializeField] PlayerCounter[] playerCounters;
    public GameObject[] triggerZones;
    bool active;
    public bool finished;
    public void CheckStart()
    {
        foreach(PlayerCounter counter in playerCounters)
        {
            if (!counter.HasRequirements())
            {
                if (active)
                {
                    StopMinigame();
                }
                return; //Counter didn't meet requirements
            }
        }

        if (!active)
        {
            StartMinigame();
        }
    }

    public virtual void StartMinigame()
    {
        active = true;
        foreach (PlayerCounter counter in playerCounters)
        {
            counter.ToggleUI(false);
        }
        foreach (GameObject triggerZone in triggerZones)
        {
            triggerZone.SetActive(false);
        }
    }

    public virtual void StopMinigame()
    {
        active = false;
        foreach (PlayerCounter counter in playerCounters)
        {
            counter.ToggleUI(true);
        }
    }

    public virtual void Reset()
    {
        finished = false;
        active = false;
        foreach (PlayerCounter counter in playerCounters)
        {
            counter.Reset();
            counter.gameObject.SetActive(true);
        }
        foreach (GameObject triggerZone in triggerZones)
        {
            triggerZone.SetActive(true);
        }
    }

    public virtual void FinishMinigame()
    {
        Debug.Log("Finished");
        active = false;
        finished = true;
        foreach (PlayerCounter counter in playerCounters)
        {
            counter.gameObject.SetActive(false);
        }
    }
}
