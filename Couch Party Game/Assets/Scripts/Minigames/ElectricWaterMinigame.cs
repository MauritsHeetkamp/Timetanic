using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricWaterMinigame : Minigame
{
    [SerializeField] List<RemoveableWater> waterToRemove; // All the removable water to finish the minigame


    private void Start()
    {
        Reset(); // Initialises the minigame

        if (startOnStart)
        {
            StartMinigame();
        }
    }

    // Checks if all the water has been removed
    public void CheckWater()
    {
        foreach(RemoveableWater water in waterToRemove)
        {
            if (!water.removed)
            {
                return; // There is still water that needs to be removed
            }
        }

        // All the water has been removed

        FinishMinigame(); // Ends the minigame
    }

    // Resets the minigame
    public override void Reset()
    {
        base.Reset();
        foreach (RemoveableWater water in waterToRemove) // Goes through all the water
        {
            water.Reset(); // Resets the water level
        }
        
    }

    // Starts the minigame
    public override void StartMinigame()
    {
        base.StartMinigame();

        foreach(RemoveableWater water in waterToRemove)
        {
            water.onRemovedWater += CheckWater; // Assigns check function when water has been removed
        }
    }

    // Finishes the minigame
    public override void FinishMinigame()
    {
        foreach (GameObject target in trappedNPCS)
        {
            target.GetComponent<SphereCollider>().enabled = true;
        }

        base.FinishMinigame();
        foreach (RemoveableWater water in waterToRemove)
        {
            water.onRemovedWater -= CheckWater; // Removes check function when water has been removed
        }
    }
}
