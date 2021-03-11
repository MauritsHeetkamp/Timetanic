using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRelocator : MonoBehaviour
{
    CameraHandler cameraHandler;
    [Header("Local Camera Properties")]
    [SerializeField] Transform newCameraDirection; // New camera direction
    [SerializeField] float newYDistance, newZDistance; // New camera location

    [SerializeField] bool resetCamera = true;

    // Start is called before the first frame update
    void Awake()
    {
        GameObject targetObject = GameObject.FindGameObjectWithTag("MainCamera");
        if(targetObject != null)
        {
            cameraHandler = targetObject.GetComponent<CameraHandler>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeCamera(Collider target)
    {
        Player player = target.GetComponent<Player>();

        if(player != null)
        {
            if (newCameraDirection != null)
            {
                player.SetCameraRotationXZ(newCameraDirection.eulerAngles, false);
                player.SetCameraRotationY(newCameraDirection.eulerAngles, false);
            }

            if (newYDistance != 0)
            {
                player.yDistance = newYDistance;
            }

            if (newZDistance != 0)
            {
                player.zDistance = newZDistance;
            }

            if (resetCamera) // Should the camera be reset
            {
                Debug.Log("RESET");
                GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraHandler>().ResetCamera(); // Resets single screen camera
                player.ResetCameraLocation(); // Resets split screen camera
            }
        }
    }

    public void ChangeCamera(Player player)
    {
        if (player != null)
        {
            if (newCameraDirection != null)
            {
                player.SetCameraRotationXZ(newCameraDirection.eulerAngles);
                player.SetCameraRotationY(newCameraDirection.eulerAngles);
            }

            if (newYDistance != 0)
            {
                player.yDistance = newYDistance;
            }

            if (newZDistance != 0)
            {
                player.zDistance = newZDistance;
            }

            if (resetCamera) // Should the camera be reset
            {
                GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraHandler>().ResetCamera(); // Resets single screen camera
                player.ResetCameraLocation(); // Resets split screen camera
            }
        }
    }
}
