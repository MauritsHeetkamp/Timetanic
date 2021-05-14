using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Minigame : MonoBehaviour
{
    [HideInInspector] public Location location;

    public bool startOnStart;
    [SerializeField] PlayerCounter[] playerCounters; // Counters that check if enough players are in the room
    public GameObject[] triggerZones; // Zones that trigger the start of the minigame
    bool active;
    public bool finished;

    [Header("Event Callbacks")]
    [SerializeField] UnityEvent onStarted, onFinished, onReset, onStopped; 

    [Header("Tasks")]
    [SerializeField] TaskData task; // Task data to show up on the task list
    [HideInInspector] MinigameHandler owner; // Manager that handles the minigames their data

    List<Task> trackingTasks = new List<Task>(); // Tasks that this minigame is currently attached to

    [Header("NPC Spawn")]
    public GameObject[] npcObjects; // NPC's that can spawn here
    public Transform[] spawnLocations; // Spawn locations for the NPC's
    public int minSpawns, maxSpawns; // Minimum and maximum NPC spawn amount
    public List<GameObject> trappedNPCS = new List<GameObject>(); // List of all the NPC's that can be rescued


    private void Start()
    {
        Reset();
        if (startOnStart)
        {
            StartMinigame();
        }
    }

    public void Initialize(MinigameHandler handler)
    {
        owner = handler;

        Task[] tasks = owner.taskHandler.AddTask(task);
        foreach(Task thisTask in tasks)
        {
            trackingTasks.Add(thisTask);
        }
    }

    // Checks if the minigame can be started
    public void CheckStart()
    {
        foreach(PlayerCounter counter in playerCounters)
        {
            if (!counter.HasRequirements()) // Does the counter not have the requirements complete?
            {
                if (active)
                {
                    StopMinigame(); // Stops the minigame
                }
                return; //Counter didn't meet requirements
            }
        }

        if (!active) // Isnt the game already active?
        {
            StartMinigame(); // Starts the minigame
        }
    }

    // Starts the minigame
    public virtual void StartMinigame()
    {
        Debug.Log("STARTED");

        if(onStarted != null)
        {
            onStarted.Invoke();
        }
        active = true;
        foreach (PlayerCounter counter in playerCounters)
        {
            counter.ToggleUI(false); // Disables the counters their UI
        }
        foreach (GameObject triggerZone in triggerZones)
        {
            triggerZone.SetActive(false); // Disables the trigger zones
        }
        if (owner != null)
        {
            owner.activeMinigames.Add(this);
        }
    }

    // Stops the minigame
    public virtual void StopMinigame()
    {
        if(onStopped != null)
        {
            onStopped.Invoke();
        }
        active = false;
        foreach (PlayerCounter counter in playerCounters)
        {
            counter.ToggleUI(true); // Enables the counters
        }
    }

    // Resets the minigame
    public virtual void Reset()
    {
        finished = false;
        active = false;

        if(onReset != null)
        {
            onReset.Invoke();
        }

        foreach (PlayerCounter counter in playerCounters) // Resets the player counters
        {
            counter.Reset();
            counter.gameObject.SetActive(true);
        }
        foreach (GameObject triggerZone in triggerZones) // Resets the trigger zones
        {
            triggerZone.SetActive(true);
        }

        if (trappedNPCS.Count == 0)
        {
            NPCSpawner npcSpawner = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameHandler>().npcSpawner;

            int spawnAmount = Random.Range(minSpawns, maxSpawns); // How many npc's should be spawned

            if (npcSpawner.spawnBasedOnPlayerAmount)
            {
                spawnAmount *= PlayerManager.instance.connectedToLobbyPlayers.Count;
            }
            if (spawnAmount > spawnLocations.Length)
            {
                spawnAmount = spawnLocations.Length;
            }

            List<Transform> availableLocations = new List<Transform>(spawnLocations);

            for (int i = 0; i < spawnAmount; i++)
            {
                int selectedSpawn = Random.Range(0, availableLocations.Count); // Selects spawn location

                GameObject npc = npcSpawner.SpawnNPC(availableLocations[selectedSpawn].position);
                npc.GetComponent<SphereCollider>().enabled = false; // Makes sure npc doesn't follow players

                trappedNPCS.Add(npc); // Adds npc to the trapped npc list
                availableLocations.RemoveAt(selectedSpawn);
            }
        }
    }

    // Finishes the minigame
    public virtual void FinishMinigame()
    {
        Debug.Log("FINISHED MINIGAME");
        active = false;
        finished = true;

        if(onFinished != null)
        {
            onFinished.Invoke();
        }

        foreach (PlayerCounter counter in playerCounters) // Disables player counter
        {
            counter.gameObject.SetActive(false);
        }
        foreach (Task task in trackingTasks)
        {
            task.Complete();
        }
        trackingTasks = new List<Task>();

        if(owner != null)
        {
            owner.activeMinigames.Remove(this);
        }

        trappedNPCS = new List<GameObject>();
    }
}
