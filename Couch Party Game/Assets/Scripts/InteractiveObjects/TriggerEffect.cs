using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerEffect : MonoBehaviour
{
    [SerializeField] bool allowPlayers = true; // Should only players be able to trigger this effect
    [SerializeField] string[] allowedTags;
    public UnityEvent onTriggerEnter, onTriggerExit;
    public UnityAction<Collider> onTriggerEnterCollider, onTriggerExitCollider;


    private void OnTriggerEnter(Collider other)
    {
        if (!other.isTrigger)
        {
            if (allowPlayers)
            {
                if (other.GetComponent<Player>() != null)
                {
                    if (onTriggerEnter != null)
                    {
                        onTriggerEnter.Invoke();
                    }
                    if (onTriggerEnterCollider != null)
                    {
                        onTriggerEnterCollider.Invoke(other);
                    }
                }
                else
                {
                    foreach (string tag in allowedTags)
                    {
                        if (tag == other.tag)
                        {
                            if (onTriggerEnter != null)
                            {
                                onTriggerEnter.Invoke();
                            }
                            if (onTriggerEnterCollider != null)
                            {
                                onTriggerEnterCollider.Invoke(other);
                            }
                            break;
                        }
                    }
                }
            }
            else
            {
                foreach (string tag in allowedTags)
                {
                    if (tag == other.tag)
                    {
                        if (onTriggerEnter != null)
                        {
                            onTriggerEnter.Invoke();
                        }
                        if (onTriggerEnterCollider != null)
                        {
                            onTriggerEnterCollider.Invoke(other);
                        }
                        break;
                    }
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.isTrigger)
        {
            if (allowPlayers)
            {
                if (other.GetComponent<Player>() != null)
                {
                    if (onTriggerExit != null)
                    {
                        onTriggerExit.Invoke();
                    }
                    if (onTriggerExitCollider != null)
                    {
                        onTriggerExitCollider.Invoke(other);
                    }
                }
                else
                {
                    foreach (string tag in allowedTags)
                    {
                        if (tag == other.tag)
                        {
                            if (onTriggerExit != null)
                            {
                                onTriggerExit.Invoke();
                            }
                            if (onTriggerExitCollider != null)
                            {
                                onTriggerExitCollider.Invoke(other);
                            }
                            break;
                        }
                    }
                }
            }
            else
            {
                foreach (string tag in allowedTags)
                {
                    if (tag == other.tag)
                    {
                        if (onTriggerExit != null)
                        {
                            onTriggerExit.Invoke();
                        }
                        if (onTriggerExitCollider != null)
                        {
                            onTriggerExitCollider.Invoke(other);
                        }
                        break;
                    }
                }
            }
        }
    }
}
