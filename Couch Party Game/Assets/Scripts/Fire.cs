using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : Extuingishable
{
    Vector3 defaultScale;
    // Start is called before the first frame update
    void Start()
    {
        defaultScale = transform.localScale;
    }

    public override void Extuingish(float amount)
    {
        base.Extuingish(amount);
        Vector3 newScale = defaultScale;
        if(health > 0 &&  maxHealth > 0)
        {
            newScale *= health / maxHealth;
        }
        else
        {
            newScale = Vector3.zero;
        }
        transform.localScale = newScale;
    }
}
