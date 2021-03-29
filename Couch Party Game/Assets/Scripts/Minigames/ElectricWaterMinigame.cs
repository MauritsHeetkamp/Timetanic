using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricWaterMinigame : Minigame
{
    [SerializeField] List<RemoveableWater> waterToRemove;

    public void CheckWater()
    {
        foreach(RemoveableWater water in waterToRemove)
        {
            if (!water.removed)
            {
                return;
            }
        }

        FinishMinigame();
    }

    public override void Reset()
    {
        /*
        base.Reset();
        foreach (RemoveableWater water in waterToRemove)
        {
            water.Reset();
        }
        */
    }

    public override void StartMinigame()
    {
        base.StartMinigame();
        Reset();

        foreach(RemoveableWater water in waterToRemove)
        {
            water.onRemovedWater += CheckWater;
        }
    }

    public override void FinishMinigame()
    {
        base.FinishMinigame();
        foreach (RemoveableWater water in waterToRemove)
        {
            water.onRemovedWater -= CheckWater;
        }
    }
}
