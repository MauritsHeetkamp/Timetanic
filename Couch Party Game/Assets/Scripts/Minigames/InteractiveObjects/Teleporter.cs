using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    public bool canTeleport = true;
    [SerializeField] bool resetCamera; // Reset the camera its location
    [SerializeField] bool teleportOnTrigger;


    public TeleportLocation target; // Target to teleport to

    [SerializeField] float fadeDuration = 0.5f; // Fade duration in teleports

    [HideInInspector] public List<GameObject> teleportingTargets; // Targets that should be ignored from teleportation

    [SerializeField] CameraRelocator cameraRelocator;

    [SerializeField] AudioClip teleportSFX;


    // Performs teleport
    void PerformTeleport(GameObject targetToTeleport)
    {
        Player player = targetToTeleport.GetComponent<Player>();

        targetToTeleport.transform.position = target.targetPosition.position; // Teleports target to target location

        if (player != null) // Is the target a player?
        {
            if(SoundManager.instance != null)
            {
                Destroy(SoundManager.instance.SpawnAudio(teleportSFX, false), teleportSFX.length);
            }

            for(int i = player.followingPassengers.Count - 1; i>= 0; i--)
            {
                MobilePassenger passenger = player.followingPassengers[i];
                if (passenger != null)
                {
                    passenger.navmeshAgent.Warp(target.targetPosition.position); // Teleports the npc to the target location
                    passenger.transform.Translate(Vector3.one); // Makes sure that the navmesh agents aren't stacked up in eachother
                }
                else
                {
                    player.followingPassengers.RemoveAt(i);
                }
            }

            /*foreach(MobilePassenger passenger in player.followingPassengers) // Goes through all the passengers that are following this player
            {
                if(passenger != null)
                {
                    passenger.navmeshAgent.Warp(target.targetPosition.position); // Teleports the npc to the target location
                    passenger.transform.Translate(Vector3.one); // Makes sure that the navmesh agents aren't stacked up in eachother
                }
            }*/

            if(cameraRelocator != null)
            {
                cameraRelocator.ChangeCameraInstant(targetToTeleport);
            }

            if (resetCamera) // Should the camera be reset
            {
                GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraHandler>().ResetCamera(); // Resets single screen camera
                player.ResetCameraLocation(); // Resets split screen camera
            }
        }

        teleportingTargets.Remove(targetToTeleport);
    }

    // Teleport target
    public void Teleport(GameObject targetToTeleport)
    {
        Player player = targetToTeleport.GetComponent<Player>();
        if (canTeleport)
        {
            teleportingTargets.Add(targetToTeleport);
            if (player == null) // Is the target not a player?
            {
                PerformTeleport(targetToTeleport);
            }
            else
            {
                if(fadeDuration > 0) // Is there a fade required?
                {
                    IngameFadeManager fadeManager = GameObject.FindGameObjectWithTag("GlobalFader").GetComponent<IngameFadeManager>(); // Finds fade handler

                    if (fadeManager != null)
                    {
                        FadePanel fader = fadeManager.FadeInOut(fadeDuration, player);
                        fader.onFadedInSpecificPlayer += PerformTeleport; // Makes sure the player gets teleported after the fade is complete
                    }
                    else
                    {
                        PerformTeleport(targetToTeleport);
                    }
                }
                else
                {
                    PerformTeleport(targetToTeleport);
                }
            }
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (teleportOnTrigger && !other.isTrigger) // Checks if target can be teleported
        {
            Teleport(other.gameObject);
        }
    }
}
