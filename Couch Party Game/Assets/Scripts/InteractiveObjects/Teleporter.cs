using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    [SerializeField] bool canTeleport = true;
    [SerializeField] bool resetCamera; // Reset the camera its location
    [SerializeField] bool teleportOnTrigger;


    [SerializeField] Transform target; // Target to teleport to
    [SerializeField] Teleporter connectedTeleporter; // Teleporter that this is possibly teleporting to

    [SerializeField] float fadeDuration = 0.5f; // Fade duration in teleports

    [HideInInspector] public List<GameObject> attachedTargets; // Targets that should be ignored from teleportation

    [SerializeField] CameraRelocator cameraRelocator;

    // Performs teleport
    void PerformTeleport(GameObject targetToTeleport)
    {
        if (connectedTeleporter != null) // Does this lead to another teleporter?
        {
            connectedTeleporter.attachedTargets.Add(targetToTeleport); // Make sure the connected teleporter doesn't teleport us back
        }

        Player player = targetToTeleport.GetComponent<Player>();

        targetToTeleport.transform.position = target.position; // Teleports target to target location

        if (player != null) // Is the target a player?
        {
            foreach(MobilePassenger passenger in player.followingPassengers) // Goes through all the passengers that are following this player
            {
                if (connectedTeleporter != null) // Does this lead to another teleporter?
                {
                    connectedTeleporter.attachedTargets.Add(passenger.gameObject); // Make sure the connected teleporter doesn't teleport the npc back
                }
                passenger.navmeshAgent.Warp(target.position); // Teleports the npc to the target location
                passenger.transform.Translate(Vector3.one); // Makes sure that the navmesh agents aren't stacked up in eachother
            }

            if(cameraRelocator != null)
            {
                cameraRelocator.ChangeCamera(player);
            }

            if (resetCamera) // Should the camera be reset
            {
                GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraHandler>().ResetCamera(); // Resets single screen camera
                player.ResetCameraLocation(); // Resets split screen camera
            }
        }


    }

    // Teleport target
    public void Teleport(GameObject targetToTeleport)
    {
        Player player = targetToTeleport.GetComponent<Player>();
        if (canTeleport)
        {
            if(player == null) // Is the target not a player?
            {
                PerformTeleport(targetToTeleport);
            }
            else
            {
                if(fadeDuration > 0) // Is there a fade required?
                {
                    FadeManager fadeManager = GameObject.FindGameObjectWithTag("GlobalFader").GetComponent<FadeManager>(); // Finds fade handler

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

    private void OnTriggerEnter(Collider other)
    {
        if (teleportOnTrigger && !attachedTargets.Contains(other.gameObject)) // Checks if target can be teleported
        {
            Teleport(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (attachedTargets.Contains(other.gameObject)) // Checks if target was attached
        {
            attachedTargets.Remove(other.gameObject);
        }
    }
}
