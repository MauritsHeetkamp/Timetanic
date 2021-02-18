using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireExtuingisher : Extuingisher
{
    [SerializeField] Transform extuingishZone;
    [SerializeField] ParticleSystem extuingishParticles;
    Coroutine extuingishRoutine;

    public override bool CheckUse()
    {
        if (canUse && extuingishZone != null)
        {
            return true;
        }
        return false;
    }
    public override void Use()
    {
        base.Use();

        if(extuingishRoutine != null)
        {
            StopCoroutine(extuingishRoutine);
        }
        extuingishRoutine = StartCoroutine(Extuinguish());
    }

    public override void StopUse()
    {
        extuingishParticles.Stop();
        if (extuingishRoutine != null)
        {
            StopCoroutine(extuingishRoutine);
            extuingishRoutine = null;
        }
        base.StopUse();
    }

    IEnumerator Extuinguish()
    {
        extuingishParticles.Play();
        while (true)
        {
            Collider[] targets = Physics.OverlapBox(extuingishZone.position, extuingishZone.lossyScale / 2, extuingishZone.rotation, extuingishableLayers);
            if (targets.Length > 0)
            {
                foreach(Collider target in targets)
                {
                    Extuingishable extuingishable = target.GetComponent<Extuingishable>();
                    if(extuingishable != null)
                    {
                        extuingishable.Extuingish(extuingishAmount * Time.deltaTime);
                    }
                }
            }
            yield return null;
        }
    }
}
