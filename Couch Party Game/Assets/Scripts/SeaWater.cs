using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeaWater : MonoBehaviour
{
    [SerializeField] LayerMask effectableLayers;
    [SerializeField] Vector3 offset;

    // Start is called before the first frame update
    void Start()
    {
        
    }


    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject);
        if (!other.isTrigger)
        {
            if (other.GetComponent<Teleporter>() != null && other.GetComponent<Teleporter>().canTeleport)
            {
                Teleporter thisTeleporter = other.GetComponent<Teleporter>();
                thisTeleporter.canTeleport = false;
                if (thisTeleporter.target.connectedTeleporter != null)
                {
                    thisTeleporter.target.connectedTeleporter.canTeleport = false;
                }
            }


            if (other.GetComponent<Entity>() != null)
            {
                Entity thisEntity = other.GetComponent<Entity>();

                Passenger passenger = other.GetComponent<Passenger>();

                if(passenger != null)
                {
                    if(passenger.currentState != Passenger.AIState.Rescued)
                    {
                        thisEntity.ConfirmDeath();
                    }
                }
                else
                {
                    thisEntity.ConfirmDeath();
                }
            }

            if (other.GetComponent<SpawnLocation>() != null)
            {
                other.GetComponent<SpawnLocation>().safe = false;
            }
        }
    }
}
