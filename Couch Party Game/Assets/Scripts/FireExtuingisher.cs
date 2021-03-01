using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireExtuingisher : Extuingisher
{
    [SerializeField] Transform extuingishZone;
    [SerializeField] ParticleSystem extuingishParticles;
    Coroutine extuingishRoutine;

    [SerializeField] AudioClip startSpray, staySpray, endSpray;
    GameObject sprayAudio;
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
        GameObject startSprayObject = SoundManager.instance.SpawnAudio(startSpray, false);
        Destroy(startSprayObject, startSprayObject.GetComponent<AudioSource>().clip.length);
        sprayAudio = SoundManager.instance.SpawnAudio(staySpray, true);
    }

    public override void StopUse()
    {
        if(sprayAudio != null)
        {
            Destroy(sprayAudio);
        }
        GameObject stopSprayObject = SoundManager.instance.SpawnAudio(endSpray, false);
        Destroy(stopSprayObject, stopSprayObject.GetComponent<AudioSource>().clip.length);
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
