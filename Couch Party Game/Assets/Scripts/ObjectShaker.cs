using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Custom.Shake;

public class ObjectShaker : MonoBehaviour
{
    [SerializeField] float resetCameraSpeed;

    [SerializeField]ShakeData currentUsingShake;
    [SerializeField]List<ShakeData> currentShakes = new List<ShakeData>();

    Coroutine currentShakeRoutine;
    Coroutine finishShakeRoutine;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    public void Shake(ShakeData shakeData)
    {
        ShakeData actualShakeData = new ShakeData(shakeData);
        currentShakes.Add(actualShakeData);

        if(currentShakeRoutine == null)
        {
            if(finishShakeRoutine != null)
            {
                StopCoroutine(finishShakeRoutine);
                finishShakeRoutine = null;
            }
            currentUsingShake = actualShakeData;
            currentShakeRoutine = StartCoroutine(ShakeRoutine());
        }
        else
        {
            CheckShakeUpdate();
        }
    }

    void CheckShakeUpdate()
    {
        if(currentShakes.Count > 0)
        {
            ShakeData bestShake = currentShakes[0];
            foreach (ShakeData data in currentShakes)
            {
                if(data.intensity > bestShake.intensity)
                {
                    bestShake = data;
                }
                else
                {
                    if(data.intensity == bestShake.intensity && data.shakeInterval > bestShake.shakeInterval)
                    {
                        bestShake = data;
                    }
                }
            }

            currentUsingShake = bestShake;
        }
    }

    IEnumerator ShakeRoutine()
    {

        Vector3 defaultLocation = transform.localPosition;
        Vector3 defaultRotation = transform.localEulerAngles;
        int currentSwayIndex = 0;
        Vector3 currentSwayRotation = Vector3.zero;




        while (currentShakes.Count > 0)
        {
            if(currentUsingShake.remainingDuration > 0 || currentUsingShake.duration <= 0)
            {
                Vector3 targetLocation = defaultLocation + new Vector3(Random.Range(-currentUsingShake.intensity, currentUsingShake.intensity), Random.Range(-currentUsingShake.intensity, currentUsingShake.intensity), Random.Range(-currentUsingShake.intensity, currentUsingShake.intensity));
                if (currentUsingShake.smoothShake)
                {
                    float remainingInterval = currentUsingShake.shakeInterval;

                    while(remainingInterval > 0)
                    {
                        if(currentUsingShake.cameraSwayRotations.Length > 0)
                        {
                            currentSwayRotation = Vector3.MoveTowards(currentSwayRotation, currentUsingShake.cameraSwayRotations[currentSwayIndex], currentUsingShake.cameraSwaySpeed);

                            Vector3 targetEulers = transform.localEulerAngles;

                            if(currentSwayRotation.x > 0)
                            {
                                targetEulers.x = currentSwayRotation.x % 360;
                            }
                            else
                            {
                                if(currentSwayRotation.x < 0)
                                {
                                    targetEulers.x = 360 + (currentSwayRotation.x % -360);
                                }
                            }

                            if (currentSwayRotation.y > 0)
                            {
                                targetEulers.y = currentSwayRotation.y % 360;
                            }
                            else
                            {
                                if (currentSwayRotation.y < 0)
                                {
                                    targetEulers.y = 360 + (currentSwayRotation.y % -360);
                                }
                            }

                            if (currentSwayRotation.z > 0)
                            {
                                targetEulers.z = currentSwayRotation.z % 360;
                            }
                            else
                            {
                                if (currentSwayRotation.z < 0)
                                {
                                    targetEulers.z = 360 + (currentSwayRotation.z % -360);
                                }
                            }

                            Debug.Log(targetEulers + " IS TARGET EULERS");

                            transform.localEulerAngles = targetEulers;


                            if(currentSwayRotation == currentUsingShake.cameraSwayRotations[currentSwayIndex])
                            {
                                currentSwayIndex = Random.Range(0, currentUsingShake.cameraSwayRotations.Length);
                            }
                        }

                        transform.localPosition = Vector3.Lerp(transform.localPosition, targetLocation, currentUsingShake.smoothSpeed);
                        yield return null;

                        for(int i = currentShakes.Count - 1; i >= 0; i--)
                        {
                            ShakeData thisData = currentShakes[i];

                            if(thisData.duration > 0)
                            {
                                thisData.remainingDuration -= Time.deltaTime;

                                if (thisData.remainingDuration <= 0 && thisData != currentUsingShake)
                                {
                                    currentShakes.RemoveAt(i);
                                }
                            }
                        }

                        remainingInterval -= Time.deltaTime;
                    }
                }
                else
                {
                    transform.localPosition = targetLocation;

                    if(currentUsingShake.cameraSwayRotations.Length > 0)
                    {
                        currentSwayRotation = currentUsingShake.cameraSwayRotations[currentSwayIndex];

                        Vector3 targetEulers = transform.localEulerAngles;

                        if (currentSwayRotation.x > 0)
                        {
                            targetEulers.x = currentSwayRotation.x % 360;
                        }
                        else
                        {
                            if (currentSwayRotation.x < 0)
                            {
                                targetEulers.x = 360 + (currentSwayRotation.x % -360);
                            }
                        }

                        if (currentSwayRotation.y > 0)
                        {
                            targetEulers.y = currentSwayRotation.y % 360;
                        }
                        else
                        {
                            if (currentSwayRotation.y < 0)
                            {
                                targetEulers.y = 360 + (currentSwayRotation.y % -360);
                            }
                        }

                        if (currentSwayRotation.z > 0)
                        {
                            targetEulers.z = currentSwayRotation.z % 360;
                        }
                        else
                        {
                            if (currentSwayRotation.z < 0)
                            {
                                targetEulers.z = 360 + (currentSwayRotation.z % -360);
                            }
                        }

                        Debug.Log(targetEulers + " IS TARGET EULERS");

                        transform.localEulerAngles = targetEulers;


                        if (currentSwayRotation == currentUsingShake.cameraSwayRotations[currentSwayIndex])
                        {
                            currentSwayIndex = Random.Range(0, currentUsingShake.cameraSwayRotations.Length);
                        }
                    }
                    yield return new WaitForSeconds(currentUsingShake.shakeInterval);
                }


                for (int i = currentShakes.Count - 1; i >= 0; i--)
                {
                    ShakeData thisData = currentShakes[i];

                    if (thisData.duration > 0)
                    {
                        thisData.remainingDuration -= currentUsingShake.shakeInterval;

                        if (thisData.remainingDuration <= 0 && thisData != currentUsingShake)
                        {
                            currentShakes.RemoveAt(i);
                        }
                    }
                }
            }
            else
            {
                currentShakes.Remove(currentUsingShake);
                CheckShakeUpdate();
            }
        }

        Debug.Log("COMPLETE");
        transform.localPosition = defaultLocation;
        currentShakeRoutine = null;

        finishShakeRoutine = StartCoroutine(ResetCamera(defaultLocation, defaultRotation));
    }

    IEnumerator ResetCamera(Vector3 defaultLocation, Vector3 defaultRotation)
    {
        while(transform.position != defaultLocation || transform.localEulerAngles != defaultRotation)
        {
            transform.position = Vector3.MoveTowards(transform.position, defaultLocation, resetCameraSpeed);
            transform.localEulerAngles = Vector3.MoveTowards(transform.localEulerAngles, defaultLocation, resetCameraSpeed);
            yield return null;
        }

        finishShakeRoutine = null;
    }

}

namespace Custom.Shake
{
    [System.Serializable]
    public class ShakeData
    {
        public float remainingDuration;

        public float duration;
        public float intensity;
        public float shakeInterval;

        public bool smoothShake;
        public float smoothSpeed;

        public Vector3[] cameraSwayRotations;
        public float cameraSwaySpeed;


        public ShakeData(float _duration, float _intensity, float _speed, Vector3[] _cameraSwayRotations, float _cameraSwaySpeed, bool _smoothShake = false, float _smoothSpeed = 0)
        {
            duration = _duration;
            intensity = _intensity;
            shakeInterval = _speed;
            remainingDuration = duration;


            smoothShake = _smoothShake;
            smoothSpeed = _smoothSpeed;

            cameraSwayRotations = _cameraSwayRotations;
            cameraSwaySpeed = _cameraSwaySpeed;
        }

        public ShakeData(ShakeData _data)
        {
            duration = _data.duration;
            intensity = _data.intensity;
            shakeInterval = _data.shakeInterval;
            remainingDuration = _data.duration;


            smoothShake = _data.smoothShake;
            smoothSpeed = _data.smoothSpeed;

            cameraSwayRotations = _data.cameraSwayRotations;
            cameraSwaySpeed = _data.cameraSwaySpeed;
        }
    }
}
