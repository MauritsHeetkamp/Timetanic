using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Custom.Types.Events
{
    [Serializable]
    public class ColliderEvent : UnityEvent<Collider>
    {

    }

    [Serializable]
    public class TimedColliderEvent
    {
        public float delayBeforeActivation;
        public ColliderEvent colliderEvent;

        public IEnumerator StartDelay(Collider target)
        {
            if (delayBeforeActivation > 0)
            {
                yield return new WaitForSeconds(delayBeforeActivation);
            }
            colliderEvent.Invoke(target);
        }
    }

    [Serializable]
    public class TimedColliderEventArray
    {
        public TimedColliderEvent[] colliderEvents;

        public IEnumerator InvokeEvents(Collider target)
        {
            foreach(TimedColliderEvent colliderEvent in colliderEvents)
            {
                if(colliderEvent.delayBeforeActivation > 0)
                {
                    yield return new WaitForSeconds(colliderEvent.delayBeforeActivation);
                }
                colliderEvent.colliderEvent.Invoke(target);
            }
        }
    }
}
