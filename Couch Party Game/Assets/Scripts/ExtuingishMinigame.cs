using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtuingishMinigame : Minigame
{
    [SerializeField] List<Extuingishable> extuingishableFires;
    [SerializeField] GameObject[] npcObjects;
    [SerializeField] Transform[] spawnLocations;
    [SerializeField] int minSpawns, maxSpawns;
    [SerializeField] List<GameObject> trappedNPCS = new List<GameObject>();


    Player finisher;
    // Start is called before the first frame update
    void Start()
    {
        Reset();
        foreach(Extuingishable extuingishable in extuingishableFires)
        {
            extuingishable.onExtuingished += CheckExtuingishables;
        }
    }

    void CheckExtuingishables(Extuingishable lastExtuingished)
    {
        if (!finished)
        {
            Debug.Log("CHECKING");
            foreach (Extuingishable extuingishable in extuingishableFires)
            {
                if (extuingishable != null && extuingishable.health > 0 || extuingishable == null)
                {
                    return;
                }
            }

            Debug.Log("PASSED");
            finisher = lastExtuingished.lastPlayerToExtuingish;
            FinishMinigame();
        }
    }

    public override void FinishMinigame()
    {
        base.FinishMinigame();

        foreach(Extuingishable extuingishable in extuingishableFires)
        {
            extuingishable.Disable();
        }

        foreach(GameObject target in trappedNPCS)
        {
            if(finisher != null)
            {
                target.GetComponent<MobilePassenger>().FollowTarget(finisher);
                target.GetComponent<SphereCollider>().enabled = true;
                trappedNPCS = new List<GameObject>();
            }
        }

        Debug.Log("COMPLETE");
    }
    public override void StartMinigame()
    {
        base.StartMinigame();
        foreach (Extuingishable extuingishable in extuingishableFires)
        {
            extuingishable.Reset();
        }
    }

    public override void Reset()
    {
        finisher = null;
        base.Reset();
        if(trappedNPCS.Count == 0)
        {
            int spawnAmount = Random.Range(minSpawns, maxSpawns + 1);
            if(spawnAmount > spawnLocations.Length)
            {
                spawnAmount = spawnLocations.Length;
            }

            List<Transform> availableLocations = new List<Transform>(spawnLocations);

            for(int i = 0; i < spawnAmount; i++)
            {
                int selectedSpawn = Random.Range(0, availableLocations.Count);

                GameObject npc = Instantiate(npcObjects[Random.Range(0, npcObjects.Length)], availableLocations[selectedSpawn].position, availableLocations[selectedSpawn].rotation);
                npc.GetComponent<SphereCollider>().enabled = false;

                trappedNPCS.Add(npc);

                availableLocations.RemoveAt(selectedSpawn);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
