using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeaWater : MonoBehaviour
{
    [SerializeField] LayerMask effectableLayers;


    // Start is called before the first frame update
    void Start()
    {
        
    }



    private void FixedUpdate()
    {
        Collider[] targets = Physics.OverlapBox(transform.position, GetComponent<Collider>().bounds.extents, transform.rotation, effectableLayers, QueryTriggerInteraction.Collide);

        if(targets.Length > 0)
        {
            foreach(Collider targetCollider in targets)
            {
                if(targetCollider.GetComponent<Teleporter>() != null && targetCollider.GetComponent<Teleporter>().canTeleport)
                {

                    Teleporter thisTeleporter = targetCollider.GetComponent<Teleporter>();
                    thisTeleporter.canTeleport = false;
                    if(thisTeleporter.target.connectedTeleporter != null)
                    {
                        thisTeleporter.target.connectedTeleporter.canTeleport = false;
                    }
                }


                if(targetCollider.GetComponent<Entity>() != null)
                {
                    Entity thisEntity = targetCollider.GetComponent<Entity>();
                    thisEntity.ConfirmDeath();
                }

                if(targetCollider.GetComponent<SpawnLocation>() != null)
                {
                    targetCollider.GetComponent<SpawnLocation>().safe = false;
                }
            }
        }
    }
}
