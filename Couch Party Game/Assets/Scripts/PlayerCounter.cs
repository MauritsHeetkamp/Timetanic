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

    private void Start()
    {
        Initialize();
    }

    void Initialize()
    {

    }


    public void ToggleUI(bool value)
    {

    }

    public void Reset()
    {
        playerCount = new List<GameObject>();
        complete = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            playerCount.Add(other.gameObject);
            if (!complete && HasRequirements())
            {
                RequirementsComplete();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            playerCount.Remove(other.gameObject);
            if(complete && !HasRequirements())
            {
                RequirementsNotComplete();
            }
        }
    }

    void RequirementsComplete()
    {
        complete = true;
    }

    void RequirementsNotComplete()
    {
        complete = false;
    }

    public bool HasRequirements()
    {
        return playerCount.Count == requiredPlayerCount;
    }
}
