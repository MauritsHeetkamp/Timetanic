using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : Extuingishable
{
    [SerializeField] GameObject particleHolder;

    [SerializeField] Vector3 smallestScale;
    [SerializeField] float finalizerSpeed;
    [SerializeField] float timeBeforeReset;

    Coroutine resetRoutine;
    Coroutine scaleRoutine;
    Vector3 targetScale;
    Vector3 defaultScale;
    // Start is called before the first frame update
    void Awake()
    {
        defaultScale = particleHolder.transform.localScale;
    }

    public override void Extuingish(float amount, Player owner)
    {
        if (health > 0)
        {
            health -= amount;

            lastPlayerToExtuingish = owner;

            if(resetRoutine != null)
            {
                StopCoroutine(resetRoutine);
            }
            resetRoutine = StartCoroutine(ResetTimer());

            CalculateTargetScale();
        }
    }

    void CalculateTargetScale()
    {
        targetScale = defaultScale - smallestScale;

        if (health > 0 && maxHealth > 0)
        {
            targetScale = smallestScale + (targetScale * (health / maxHealth));
        }
        else
        {
            targetScale = Vector3.zero;
        }

        if (scaleRoutine == null)
        {
            scaleRoutine = StartCoroutine(ScaleRoutine());
        }
    }

    IEnumerator ResetTimer()
    {
        yield return new WaitForSeconds(timeBeforeReset);
        Reset();
        resetRoutine = null;
    }

    IEnumerator ScaleRoutine()
    {

        while (particleHolder.transform.localScale != targetScale)
        {
            yield return null;
            particleHolder.transform.localScale = Vector3.MoveTowards(particleHolder.transform.localScale, targetScale, finalizerSpeed * Time.deltaTime);
        }

        if(health <= 0)
        {
            particleHolder.SetActive(false);
            if (destroyOnExtuingished && resetRoutine != null)
            {
                StopCoroutine(resetRoutine);
                resetRoutine = null;
            }
            OnExtuingished();
        }
        scaleRoutine = null;
    }

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
            particleHolder.SetActive(false);
            GetComponent<Collider>().enabled = false;
        }
    }

    public override void Disable()
    {
        if(resetRoutine != null)
        {
            StopCoroutine(resetRoutine);
            resetRoutine = null;
        }
        particleHolder.SetActive(false);
        GetComponent<Collider>().enabled = false;
        Debug.Log("DISABLED");
    }

    public override void Reset()
    {
        base.Reset();
        particleHolder.SetActive(true);
        CalculateTargetScale();
        GetComponent<Collider>().enabled = true;
    }
}
