using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Custom.Shake;

public class ScreenshakeManager : MonoBehaviour
{
    [SerializeField] CameraHandler cameraHandler;

    [SerializeField] bool useDefaultShake;
    [SerializeField] ShakeData defaultGlobalShake;
    // Start is called before the first frame update
    void Start()
    {
        if (useDefaultShake)
        {
            ShakeAllScreens(defaultGlobalShake);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShakeSpecificPlayersScreen(Player target, ShakeData shakeData)
    {
        if(target.screenShake != null)
        {
            target.screenShake.Shake(shakeData);
        }
    }

    public void ShakeAllScreens(ShakeDataScriptableObject shakeDataSO)
    {
        ShakeData newShakeData = new ShakeData(shakeDataSO.duration, shakeDataSO.intensity, shakeDataSO.shakeInterval, shakeDataSO.cameraSwayRotations, shakeDataSO.cameraSwaySpeed, shakeDataSO.smoothShake, shakeDataSO.smoothSpeed);
        ShakeAllScreens(newShakeData);
    }

    public void ShakeAllScreens(ShakeData shakeData)
    {
        if(cameraHandler != null)
        {
            Debug.Log("SHAKING");
            foreach (Transform camera in cameraHandler.allPlayerCameras)
            {
                ObjectShaker screenShake = camera.GetComponent<ObjectShaker>();
                if (screenShake != null)
                {
                    screenShake.Shake(shakeData);
                }
            }

            if(cameraHandler.globalScreenshake != null)
            {
                cameraHandler.globalScreenshake.Shake(shakeData);
            }
        }


    }
}
