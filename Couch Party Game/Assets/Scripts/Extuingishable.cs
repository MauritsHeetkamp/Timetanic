using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Extuingishable : MonoBehaviour
{
    public float maxHealth;
    public float health = 1;
    public UnityAction onExtuingished;
    public bool destroyOnExtuingished;
    public virtual void Extuingish(float amount)
    {
        if(health > 0)
        {
            health -= amount;
            if (health < 0)
            {
                OnExtuingished();
            }
        }
    }

    public virtual void OnExtuingished()
    {
        if(onExtuingished != null)
        {
            onExtuingished.Invoke();
        }
        if (destroyOnExtuingished)
        {
            Destroy(gameObject);
        }
    }

    public virtual void Disable()
    {

    }

    public virtual void Reset()
    {
        health = maxHealth;
    }
}
