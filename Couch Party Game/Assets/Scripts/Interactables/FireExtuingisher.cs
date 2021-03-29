using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireExtuingisher : Extuingisher
{
    [SerializeField] Transform extuingishZone; // The zone the extuingisher extuingishes
    [SerializeField] ParticleSystem extuingishParticles;
    Coroutine extuingishRoutine; // The extuingish coroutine

    [SerializeField] AudioClip startSpray, staySpray, endSpray; // Spray audio clips
    GameObject sprayAudio; // Current audio object


    // Checks if item can be used
    public override bool CheckUse()
    {
        if (canUse && extuingishZone != null)
        {
            return true;
        }
        return false;
    }

    // Uses the item
    public override void Use()
    {
        base.Use();

        if(extuingishRoutine != null)
        {
            StopCoroutine(extuingishRoutine); // Stops the extuingish coroutine if active
        }
        extuingishRoutine = StartCoroutine(Extuinguish()); // Starts new extuingish coroutine

        if(SoundManager.instance != null)
        {
            GameObject startSprayObject = SoundManager.instance.SpawnAudio(startSpray, false); // Spawns start spray audio
            Destroy(startSprayObject, startSprayObject.GetComponent<AudioSource>().clip.length); // Removes start spray audio when complete
            sprayAudio = SoundManager.instance.SpawnAudio(staySpray, true); // Creates spray audio object
        }
    }

    // Stops using item
    public override void StopUse()
    {
        if (SoundManager.instance != null)
        {
            if (sprayAudio != null)
            {
                Destroy(sprayAudio); // Removes spray audio object
            }
            GameObject stopSprayObject = SoundManager.instance.SpawnAudio(endSpray, false); // Spawns end spray audio
            Destroy(stopSprayObject, stopSprayObject.GetComponent<AudioSource>().clip.length); // Removes end spray audio when complete
        }
        extuingishParticles.Stop(); 
        if (extuingishRoutine != null)
        {
            StopCoroutine(extuingishRoutine); // Stops extuingish coroutine
            extuingishRoutine = null;
        }
        base.StopUse();
    }

    // Extuingish coroutine
    IEnumerator Extuinguish()
    {
        extuingishParticles.Play();
        while (true)
        {
            Collider[] targets = Physics.OverlapBox(extuingishZone.position, extuingishZone.lossyScale / 2, extuingishZone.rotation, extuingishableLayers);
            if (targets.Length > 0) // Checks if there were any targets found
            {
                foreach(Collider target in targets)
                {
                    Extuingishable extuingishable = target.GetComponent<Extuingishable>();
                    if(extuingishable != null) // Is target extuingishable?
                    {
                        extuingishable.Extuingish(extuingishAmount * Time.deltaTime, currentInteractingPlayer); // Extuingishes fire by amount
                    }
                }
            }
            yield return null;
        }
    }
}
