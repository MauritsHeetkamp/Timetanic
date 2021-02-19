using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : Extuingishable
{
    Vector3 defaultScale;
    [SerializeField] Vector3 smallestScale;
    [SerializeField] float finalizerSpeed;
    // Start is called before the first frame update
    void Start()
    {
        defaultScale = transform.localScale;
    }

    public override void Extuingish(float amount)
    {
        if (health > 0)
        {
            health -= amount;
            Vector3 newScale = defaultScale - smallestScale;
            if (health > 0 && maxHealth > 0)
            {
                newScale = smallestScale + (newScale * (health / maxHealth));
            }
            else
            {
                newScale = smallestScale;
            }
            transform.localScale = newScale;

            if (smallestScale != Vector3.zero)
            {
                if (newScale == smallestScale)
                {
                    StartCoroutine(ScaleRoutine());
                }
            }
            else
            {
                if (newScale == Vector3.zero)
                {
                    OnExtuingished();
                }
            }
        }
    }

    IEnumerator ScaleRoutine()
    {
        while(transform.localScale != Vector3.zero)
        {
            yield return null;
            transform.localScale = Vector3.MoveTowards(transform.localScale, Vector3.zero, finalizerSpeed * Time.deltaTime);
        }

        OnExtuingished();
        Debug.Log("COMPLETE");
    }

}
