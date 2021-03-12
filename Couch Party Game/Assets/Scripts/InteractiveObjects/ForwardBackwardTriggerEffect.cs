using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Custom.Types.Events;
using System;

public class ForwardBackwardTriggerEffect : TriggerEffectBase
{
    public TimedColliderEventArray onTriggerForward, onTriggerBackward;

    List<ColliderLocation> colliderTriggerEnterLocation = new List<ColliderLocation>();

    Vector3 forwardPosition;

    Vector3 backwardPosition;

    private void Awake()
    {
        Vector3 offset = transform.forward;
        forwardPosition = transform.position + offset;
        backwardPosition = transform.position + -offset;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (ValidTarget(other))
        {
            bool forward = Vector3.Distance(other.transform.position, forwardPosition) < Vector3.Distance(other.transform.position, backwardPosition);
            colliderTriggerEnterLocation.Add(new ColliderLocation(other, forward));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (ValidTarget(other))
        {
            ColliderLocation selectedLocation = null;
            for(int i = 0; i < colliderTriggerEnterLocation.Count; i++)
            {
                if(colliderTriggerEnterLocation[i].target == other)
                {
                    selectedLocation = colliderTriggerEnterLocation[i];
                    colliderTriggerEnterLocation.RemoveAt(i);
                    break;
                }
            }

            if(selectedLocation != null)
            {
                if (Vector3.Distance(other.transform.position, forwardPosition) < Vector3.Distance(other.transform.position, backwardPosition))
                {
                    if (!selectedLocation.forward)
                    {
                        StartCoroutine(onTriggerForward.InvokeEvents(other));
                    }
                }
                else
                {
                    if (selectedLocation.forward)
                    {
                        StartCoroutine(onTriggerBackward.InvokeEvents(other));
                    }
                }
            }
        }
    }

    [System.Serializable]
    public class ColliderLocation
    {
        public Collider target;
        public bool forward;

        public ColliderLocation(Collider _target, bool _forward)
        {
            target = _target;
            forward = _forward;
        }
    }
}
