using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : Extuingishable
{
    [SerializeField] GameObject particleHolder; // The fire particle object

    [SerializeField] Vector3 smallestScale; // The smallest visual scale it can be before being removed
    [SerializeField] float finalizerSpeed; // The speed at which the fire shrinks
    [SerializeField] float timeBeforeReset;  // The time before the fire regrows after not being extuingished

    Coroutine resetRoutine; // The reset routine for the fire regrowth
    Coroutine scaleRoutine; // The scaling routine for the fires size
    Vector3 targetScale;
    Vector3 defaultScale; // The default scale of the fire


    void Awake()
    {
        defaultScale = particleHolder.transform.localScale;
    }

    // Extuingishes for a set amount
    public override void Extuingish(float amount, Player owner)
    {
        if (health > 0)
        {
            health -= amount;

            lastPlayerToExtuingish = owner; // Sets last extuingisher

            if(resetRoutine != null)
            {
                StopCoroutine(resetRoutine); // Stops reset routine if it was active
            }
            resetRoutine = StartCoroutine(ResetTimer()); // Resets reset routine

            CalculateTargetScale();
        }
    }

    // Calculate the target scale of the fire based on health
    void CalculateTargetScale()
    {
        targetScale = defaultScale - smallestScale;

        if (health > 0 && maxHealth > 0)
        {
            targetScale = smallestScale + (targetScale * (health / maxHealth)); // Calculates scale based on health
        }
        else
        {
            targetScale = Vector3.zero;
        }

        if (scaleRoutine == null)
        {
            scaleRoutine = StartCoroutine(ScaleRoutine()); // Starts scale routine if it was not active yet
        }
    }

    // Timer before fire resets
    IEnumerator ResetTimer()
    {
        yield return new WaitForSeconds(timeBeforeReset);
        Reset();
        resetRoutine = null;
    }

    // Coroutine that handles the fires smooth scaling
    IEnumerator ScaleRoutine()
    {

        while (particleHolder.transform.localScale != targetScale)
        {
            yield return null;
            particleHolder.transform.localScale = Vector3.MoveTowards(particleHolder.transform.localScale, targetScale, finalizerSpeed * Time.deltaTime); // Scales fire towards target scale over time
        }

        if(health <= 0) // Is fire dead?
        {
            if (destroyOnExtuingished && resetRoutine != null)
            {
                StopCoroutine(resetRoutine); // Stops coroutine if object is getting destroyed
                resetRoutine = null;
            }
            OnExtuingished();
        }
        scaleRoutine = null;
    }

    // What happens when the fire is extuingished
    public override void OnExtuingished()
    {
        if (onExtuingished != null)
        {
            onExtuingished.Invoke(this);
        }
        if (destroyOnExtuingished)
        {
            Destroy(gameObject);
        }
        else
        {
            particleHolder.SetActive(false); // Disables the fire particle
            GetComponent<Collider>().enabled = false; // Disables the fires collision
        }
    }

    // Disables the fire entirely
    public override void Disable()
    {
        if(resetRoutine != null)
        {
            StopCoroutine(resetRoutine); // Makes sure the fire doesnt reset
            resetRoutine = null;
        }
        particleHolder.SetActive(false); // Disables the fire particle
        GetComponent<Collider>().enabled = false; // Disables the fires collision
    }

    // Resets the fire
    public override void Reset()
    {
        base.Reset();
        particleHolder.SetActive(true); // Enables the fire particle
        CalculateTargetScale(); // Lets the fire regrow
        GetComponent<Collider>().enabled = true; // Enables the fires collision
    }
}
