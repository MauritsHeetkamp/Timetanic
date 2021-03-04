using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerEffect : MonoBehaviour
{
    [SerializeField] bool onlyPlayers = true; // Should only players be able to trigger this effect
    [SerializeField] UnityEvent onTriggerEnter, onTriggerExit;


    private void OnTriggerEnter(Collider other)
    {
        if (onlyPlayers)
        {
            if(other.GetComponent<Player>() != null)
            {
                onTriggerEnter.Invoke();
            }
        }
        else
        {
            onTriggerEnter.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (onlyPlayers)
        {
            if (other.GetComponent<Player>() != null)
            {
                onTriggerExit.Invoke();
            }
        }
        else
        {
            onTriggerExit.Invoke();
        }
    }
}
