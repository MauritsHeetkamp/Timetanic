using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Custom.Types.Events;

public class TriggerEffect : TriggerEffectBase
{
    public TimedColliderEventArray onTriggerEnter, onTriggerExit;


    private void OnTriggerEnter(Collider other)
    {
        if (ValidTarget(other))
        {
            StartCoroutine(onTriggerEnter.InvokeEvents(other));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (ValidTarget(other))
        {
            StartCoroutine(onTriggerExit.InvokeEvents(other));
        }
    }
}
