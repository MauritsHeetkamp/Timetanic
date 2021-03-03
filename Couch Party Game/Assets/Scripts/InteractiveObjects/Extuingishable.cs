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


    // Extuingishes fire for a set amount
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

    // What happens when extuingished
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

    // Disables the extuingishable
    public virtual void Disable()
    {

    }

    // Resets the extuingishable
    public virtual void Reset()
    {
        lastPlayerToExtuingish = null;
        health = maxHealth;
    }
}
