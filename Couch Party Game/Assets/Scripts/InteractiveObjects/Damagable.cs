using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damagable : MonoBehaviour
{
    [SerializeField] int health;

    public virtual void TakeDamage(int damage, string damagedBy)
    {

    }

    public virtual void OnDeath()
    {

    }
}
