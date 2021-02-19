using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    [SerializeField] bool canTeleport = true;
    [SerializeField] bool resetCamera;
    [SerializeField] bool teleportOnTrigger;


    [SerializeField] Transform target;

    [Header("Local Camera Properties")]
    [SerializeField] bool newEulers; // If the camera should be updating its eulers
    [SerializeField] Vector3 newEulerAngles;
    [SerializeField] float newYDistance, newZDistance;

    [SerializeField] float fadeDuration = 0.5f;

    void PerformTeleport(GameObject targetToTeleport)
    {
        Player player = targetToTeleport.GetComponent<Player>();

        targetToTeleport.transform.position = target.position;

        if (player != null)
        {
            if (resetCamera)
            {
                GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraHandler>().ResetCamera();
                player.ResetCameraLocation();
            }

            if (newEulers)
            {
                player.playerCamera.eulerAngles = newEulerAngles;
            }

            if (newYDistance != 0)
            {
                player.yDistance = newYDistance;
            }

            if (newZDistance != 0)
            {
                player.zDistance = newZDistance;
            }
        }
    }

    public void Teleport(GameObject targetToTeleport)
    {
        Player player = targetToTeleport.GetComponent<Player>();
        if (canTeleport)
        {
            if(player == null)
            {
                PerformTeleport(targetToTeleport);
            }
            else
            {
                if(fadeDuration > 0)
                {
                    FadeManager fadeManager = GameObject.FindGameObjectWithTag("GlobalFader").GetComponent<FadeManager>();

                    if (fadeManager != null)
                    {
                        FadePanel fader = fadeManager.FadeInOut(fadeDuration, player);
                        fader.onFadedInSpecificPlayer += PerformTeleport;
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
        if (teleportOnTrigger)
        {
            Teleport(other.gameObject);
        }
    }
}
