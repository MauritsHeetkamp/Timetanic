using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtuingishMinigame : Minigame
{
    [SerializeField] List<Extuingishable> extuingishableFires;


    Player finisher; // Player that extuingished the last fire


    // Start is called before the first frame update
    void Start()
    {
        Reset();
        if (startOnStart)
        {
            StartMinigame();
        }
        foreach (Extuingishable extuingishable in extuingishableFires)
        {
            extuingishable.onExtuingished += CheckExtuingishables; // Makes sure the minigame checks the state of the minigame after a fire has been extuingished
        }
    }

    // Checks if there is at least one fire active still
    void CheckExtuingishables(Extuingishable lastExtuingished)
    {
        if (!finished)
        {
            foreach (Extuingishable extuingishable in extuingishableFires)
            {
                if (extuingishable != null && extuingishable.health > 0 || extuingishable == null) // Is this extuingishable still alive?
                {
                    return;
                }
            }

            finisher = lastExtuingished.lastPlayerToExtuingish; // Sets the last player that extuingished
            FinishMinigame();
        }
    }

    // Finishes the minigame
    public override void FinishMinigame()
    {
        foreach (Extuingishable extuingishable in extuingishableFires)
        {
            extuingishable.Disable(); // Disables the fire from regrowing
        }

        foreach (GameObject target in trappedNPCS)
        {
            if (finisher != null)
            {
                //target.GetComponent<MobilePassenger>().FollowTarget(finisher); // Let the NPC's follow the finisher
                target.GetComponent<SphereCollider>().enabled = true;
            }
        }

        base.FinishMinigame();
    }

    // Starts the minigame
    public override void StartMinigame()
    {
        base.StartMinigame();
        foreach (Extuingishable extuingishable in extuingishableFires)
        {
            extuingishable.Reset(); // Resets the fire to burn again
        }
    }

    // Resets the entire minigame
    public override void Reset()
    {
        base.Reset();
        finisher = null;
    }
}
