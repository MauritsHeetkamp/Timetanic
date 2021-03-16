using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minigame : MonoBehaviour
{
    [SerializeField] PlayerCounter[] playerCounters;
    public GameObject[] triggerZones; // Zones that trigger the start of the minigame
    bool active;
    public bool finished;
    [SerializeField] TaskData task;
    [HideInInspector] MinigameHandler owner;

    List<Task> trackingTasks = new List<Task>();


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
        active = true;
        foreach (PlayerCounter counter in playerCounters)
        {
            counter.ToggleUI(false); // Disables the counters their UI
        }
        foreach (GameObject triggerZone in triggerZones)
        {
            triggerZone.SetActive(false); // Disables the trigger zones
        }
    }

    // Stops the minigame
    public virtual void StopMinigame()
    {
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
        foreach (PlayerCounter counter in playerCounters) // Resets the player counters
        {
            counter.Reset();
            counter.gameObject.SetActive(true);
        }
        foreach (GameObject triggerZone in triggerZones) // Resets the trigger zones
        {
            triggerZone.SetActive(true);
        }
    }

    // Finishes the minigame
    public virtual void FinishMinigame()
    {
        active = false;
        finished = true;
        foreach (PlayerCounter counter in playerCounters) // Disables player counter
        {
            counter.gameObject.SetActive(false);
        }
        foreach (Task task in trackingTasks)
        {
            task.Complete();
        }
        trackingTasks = new List<Task>();
    }
}
