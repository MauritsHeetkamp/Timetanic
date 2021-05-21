using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeaWater : MonoBehaviour
{
    [SerializeField] LayerMask effectableLayers;
    [SerializeField] Vector3 offset;

    [SerializeField] float sfxDelay = 0.2f;
    [SerializeField] bool canPlaySFX = true;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    IEnumerator SFXCooldown()
    {
        yield return new WaitForSecondsRealtime(sfxDelay);
        canPlaySFX = true;
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
                Player player = other.GetComponent<Player>();

                if (SoundManager.instance != null && thisEntity.drownSFX.clip != null)
                {
                    if (canPlaySFX)
                    {
                        canPlaySFX = false;
                        StartCoroutine(SFXCooldown());

                        if(player != null)
                        {
                            Destroy(SoundManager.instance.SpawnAudio(thisEntity.drownSFX.clip, thisEntity.drownSFX.loop, thisEntity.drownSFX.pitch, thisEntity.drownSFX.volume), thisEntity.drownSFX.clip.length);
                        }
                        else
                        {
                            Debug.Log("SPAWNED 3D AUDIO");
                            Destroy(SoundManager.instance.Spawn3DAudio(thisEntity.drownSFX, thisEntity.transform.position), thisEntity.drownSFX.clip.length);
                        }
                    }
                }

                if (passenger != null)
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
