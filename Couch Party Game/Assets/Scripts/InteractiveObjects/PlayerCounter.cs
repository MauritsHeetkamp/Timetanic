using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerCounter : MonoBehaviour
{
    bool complete;
    [SerializeField] List<GameObject> playerCount;
    [SerializeField] int requiredPlayerCount;
    [SerializeField] UnityEvent onPlayerAmountChanged;


    // Start is called before the first frame update
    private void Start()
    {
        Initialize();
    }

    // Initializes the player counter
    void Initialize()
    {

    }

    // Toggles the player counter UI
    public void ToggleUI(bool value)
    {

    }

    // Resets the player counter
    public void Reset()
    {
        playerCount = new List<GameObject>();
        complete = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            playerCount.Add(other.gameObject); // Adds player to counter
            if (!complete && HasRequirements()) // Checks if requirements have been met
            {
                RequirementsComplete();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            playerCount.Remove(other.gameObject); // Removes player from counter
            if(complete && !HasRequirements()) // Checks if requirements aren't complete anymore
            {
                RequirementsNotComplete();
            }
        }
    }

    // What happens when requirements are completed
    void RequirementsComplete()
    {
        complete = true;
    }

    // What happens when requirements are not completed
    void RequirementsNotComplete()
    {
        complete = false;
    }

    // Checks if requirements have been met
    public bool HasRequirements()
    {
        return playerCount.Count == requiredPlayerCount;
    }
}
