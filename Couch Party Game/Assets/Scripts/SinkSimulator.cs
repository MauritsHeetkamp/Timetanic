using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinkSimulator : MonoBehaviour
{
    public bool isSimulating;
    [SerializeField] Transform water;
    [SerializeField] CameraHandler cameraHandler;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isSimulating)
        {
            foreach(Transform camera in cameraHandler.allPlayerCameras)
            {
                Transform cameraHolder = camera.parent;

                if(cameraHolder != null)
                {
                    cameraHolder.eulerAngles = new Vector3(cameraHolder.eulerAngles.x, cameraHolder.eulerAngles.y, water.eulerAngles.z);
                }
            }
            cameraHandler.globalCamera.eulerAngles = new Vector3(cameraHandler.globalCamera.eulerAngles.x, cameraHandler.globalCamera.eulerAngles.y, water.eulerAngles.z);
        }
    }
}
