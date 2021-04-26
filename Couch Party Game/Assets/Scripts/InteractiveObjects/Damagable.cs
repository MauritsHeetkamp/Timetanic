﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Damagable : MonoBehaviour
{
    public bool dead;

    [SerializeField] int health;
    public int invulnerable;
    [SerializeField] float defaultInvulnerabilityDuration;
    float remainingInvulnerability;
    Coroutine invulnerabilityRoutine;

    public UnityAction onDeath;

    public virtual void Init()
    {

    }

    // Taking damage
    public virtual void TakeDamage(int damage, string damagedBy)
    {

    }

    public void ConfirmDeath()
    {
        if (!dead)
        {
            dead = true;
            OnDeath();

            if(onDeath != null)
            {
                onDeath.Invoke();
            }
        }
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
