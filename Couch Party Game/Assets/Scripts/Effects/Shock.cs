using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shock : EffectBase
{
    public float shockDuration;
    Entity owner;
    Coroutine shockRoutine;
    bool active;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Initialize(Shock data)
    {
        shockDuration = data.shockDuration;
        owner = GetComponent<Entity>();
    }

    public void Initialize(float duration)
    {
        shockDuration = duration;
        owner = GetComponent<Entity>();
    }

    public override void ApplyEffect()
    {
        if(owner.invulnerable <= 0)
        {
            active = true;
            owner.SetShock(true);
            shockRoutine = StartCoroutine(ShockTimer());
            base.ApplyEffect();
        }
        else
        {
            Destroy(this);
        }
    }

    public override void RemoveEffect()
    {
        if (active)
        {
            active = false;
            owner.SetShock(false);
            if (shockRoutine != null)
            {
                StopCoroutine(shockRoutine);
                shockRoutine = null;
            }

            base.RemoveEffect();

            Destroy(this);
        }
    }

    IEnumerator ShockTimer()
    {
        yield return new WaitForSeconds(shockDuration);
        RemoveEffect();
    }
}
