using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damagable : MonoBehaviour
{
    [SerializeField] int health;

    // Taking damage
    public virtual void TakeDamage(int damage, string damagedBy)
    {

    }

    // What happens on death
    public virtual void OnDeath()
    {

    }
}
