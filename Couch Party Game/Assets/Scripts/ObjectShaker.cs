using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectShaker : MonoBehaviour
{
    ShakeData currentUsingShake;
    [SerializeField] List<ShakeData> currentShakes = new List<ShakeData>();

    Coroutine currentShakeRoutine;

    [SerializeField] ShakeData data;
    [SerializeField] bool smoothShake;
    [SerializeField] float smoothSpeed;

    // Start is called before the first frame update
    void Start()
    {
        Shake(data);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Shake(float duration, float intensity, float speed)
    {
        ShakeData newData = new ShakeData(duration, intensity, speed);
        currentShakes.Add(newData);

        if(currentShakeRoutine == null)
        {
            currentUsingShake = newData;
            currentShakeRoutine = StartCoroutine(ShakeRoutine());
        }
        else
        {
            CheckShakeUpdate();
        }
    }

    public void Shake(ShakeData shakeData)
    {
        currentShakes.Add(shakeData);

        if(currentShakeRoutine == null)
        {
            currentUsingShake = shakeData;
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

        while(currentShakes.Count > 0)
        {
            Debug.Log(currentUsingShake.remainingDuration);
            if(currentUsingShake.remainingDuration > 0 || currentUsingShake.duration <= 0)
            {
                Vector3 targetLocation = defaultLocation + new Vector3(Random.Range(-currentUsingShake.intensity, currentUsingShake.intensity), Random.Range(-currentUsingShake.intensity, currentUsingShake.intensity), Random.Range(-currentUsingShake.intensity, currentUsingShake.intensity));
                if (smoothShake)
                {
                    float remainingInterval = currentUsingShake.shakeInterval;

                    while(remainingInterval > 0)
                    {
                        transform.localPosition = Vector3.Lerp(transform.localPosition, targetLocation, smoothSpeed);
                        yield return null;
                        remainingInterval -= Time.deltaTime;
                    }
                }
                else
                {
                    transform.localPosition = targetLocation;
                    yield return new WaitForSeconds(currentUsingShake.shakeInterval);
                }


                if(currentUsingShake.duration > 0)
                {
                    currentUsingShake.duration -= currentUsingShake.shakeInterval;
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
    }

    [System.Serializable]
    public class ShakeData
    {
        public float remainingDuration;

        public float duration;
        public float intensity;
        public float shakeInterval;


        public ShakeData(float _duration, float _intensity, float _speed)
        {
            duration = _duration;
            intensity = _intensity;
            shakeInterval = _speed;
            remainingDuration = duration;
        }
    }
}
