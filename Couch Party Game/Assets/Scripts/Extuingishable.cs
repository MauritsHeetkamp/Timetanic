using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Extuingishable : MonoBehaviour
{
    public float maxHealth;
    public float health = 1;
    public UnityAction<Extuingishable> onExtuingished;
    public bool destroyOnExtuingished;

    [HideInInspector] public Player lastPlayerToExtuingish;

    public virtual void Extuingish(float amount, Player owner = null)
    {
        if(health > 0)
        {
            lastPlayerToExtuingish = owner;
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
            onExtuingished.Invoke(this);
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
        lastPlayerToExtuingish = null;
        health = maxHealth;
    }
}
