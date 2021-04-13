using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damagable : MonoBehaviour
{
    [SerializeField] int health;
    public int invulnerable;
    [SerializeField] float defaultInvulnerabilityDuration;
    float remainingInvulnerability;
    Coroutine invulnerabilityRoutine;


    public virtual void Init()
    {

    }

    // Taking damage
    public virtual void TakeDamage(int damage, string damagedBy)
    {

    }

    // What happens on death
    public virtual void OnDeath()
    {

    }

    public void BecomeInvulnerable()
    {
        if(invulnerabilityRoutine == null)
        {
            invulnerabilityRoutine = StartCoroutine(Invulnerability(defaultInvulnerabilityDuration));
        }
        else
        {
            if(remainingInvulnerability < defaultInvulnerabilityDuration)
            {
                remainingInvulnerability = defaultInvulnerabilityDuration;
            }
        }
    }

    public void BecomeInvulnerable(float duration)
    {
        if (invulnerabilityRoutine == null)
        {
            invulnerabilityRoutine = StartCoroutine(Invulnerability(duration));
        }
        else
        {
            if (remainingInvulnerability < duration)
            {
                remainingInvulnerability = duration;
            }
        }
    }

    public void BecomePermanentInvulnerable()
    {
        invulnerable++;
    }

    public void RemoveInvulnerability()
    {
        invulnerable--;
    }

    IEnumerator Invulnerability(float duration)
    {
        invulnerable++;
        remainingInvulnerability = duration;
        while(remainingInvulnerability > 0)
        {
            yield return null;
            remainingInvulnerability -= Time.deltaTime;
        }
        invulnerable--;
        invulnerabilityRoutine = null;
    }
}
