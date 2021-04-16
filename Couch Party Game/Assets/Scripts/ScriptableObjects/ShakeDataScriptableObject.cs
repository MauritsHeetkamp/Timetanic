using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu( fileName = "New ShakeData", menuName = "New ShakeData")]
public class ShakeDataScriptableObject : ScriptableObject
{
    public float duration;
    public float intensity;
    public float shakeInterval;

    public bool smoothShake;
    public float smoothSpeed;

    public Vector3[] cameraSwayRotations;
    public float cameraSwaySpeed;
}
