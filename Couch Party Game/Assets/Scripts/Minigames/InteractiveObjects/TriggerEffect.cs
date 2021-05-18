using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Custom.Types.Events;

public class TriggerEffect : TriggerEffectBase
{
    public TimedColliderEventArray onTriggerEnter, onTriggerExit;
    public TimedColliderEventArray onTriggerStay;

    private void OnTriggerEnter(Collider other)
    {
        if (ValidTarget(other))
        {
            StartCoroutine(onTriggerEnter.InvokeEvents(other));
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (ValidTarget(other))
        {
            StartCoroutine(onTriggerStay.InvokeEvents(other));
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
