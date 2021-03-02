using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtuingishMinigame : Minigame
{
    [SerializeField] List<Extuingishable> extuingishableFires;
    [SerializeField] GameObject[] npcObjects; // NPC's that can spawn here
    [SerializeField] Transform[] spawnLocations; // Spawn locations for the NPC's
    [SerializeField] int minSpawns, maxSpawns;
    [SerializeField] List<GameObject> trappedNPCS = new List<GameObject>(); // List of all the NPC's that can be rescued


    Player finisher; // Player that extuingished the last fire


    // Start is called before the first frame update
    void Start()
    {
        Reset(); // Initializes the game
        foreach(Extuingishable extuingishable in extuingishableFires)
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
        base.FinishMinigame();

        foreach(Extuingishable extuingishable in extuingishableFires)
        {
            extuingishable.Disable(); // Disables the fire from regrowing
        }

        foreach(GameObject target in trappedNPCS)
        {
            if(finisher != null)
            {
                target.GetComponent<MobilePassenger>().FollowTarget(finisher); // Let the NPC's follow the finisher
                target.GetComponent<SphereCollider>().enabled = true;
            }
        }

        if(finisher != null)
        {
            trappedNPCS = new List<GameObject>();
        }
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
        finisher = null;
        base.Reset();
        if(trappedNPCS.Count == 0)
        {
            int spawnAmount = Random.Range(minSpawns, maxSpawns + 1); // How many npc's should be spawned
            if(spawnAmount > spawnLocations.Length)
            {
                spawnAmount = spawnLocations.Length;
            }

            List<Transform> availableLocations = new List<Transform>(spawnLocations);

            for(int i = 0; i < spawnAmount; i++)
            {
                int selectedSpawn = Random.Range(0, availableLocations.Count); // Selects spawn location

                GameObject npc = Instantiate(npcObjects[Random.Range(0, npcObjects.Length)], availableLocations[selectedSpawn].position, availableLocations[selectedSpawn].rotation); // Spawns npc
                npc.GetComponent<SphereCollider>().enabled = false; // Makes sure npc doesn't follow players

                trappedNPCS.Add(npc); // Adds npc to the trapped npc list

                availableLocations.RemoveAt(selectedSpawn);
            }
        }
    }
}
