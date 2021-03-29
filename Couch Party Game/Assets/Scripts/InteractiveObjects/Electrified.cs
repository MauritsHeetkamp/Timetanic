using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Electrified : MonoBehaviour
{
    public float shockDuration;
    Conductor thisConductor;


    private void OnTriggerStay(Collider other)
    {
        if(other.GetComponent<Conductor>() != null && other.GetComponent<Electrified>() == null)
        {
            Electrified newElectrifiedPart = other.gameObject.AddComponent<Electrified>();
            newElectrifiedPart.thisConductor = other.GetComponent<Conductor>();
            newElectrifiedPart.shockDuration = shockDuration;
            if(newElectrifiedPart.thisConductor != null)
            {
                foreach(ParticleSystem particle in newElectrifiedPart.thisConductor.electricParticles)
                {
                    particle.Play();
                }
            }

        }
        if(other.tag == "Player")
        {
            Shock shock = other.gameObject.AddComponent<Shock>();
            shock.Initialize(shockDuration);
            shock.ApplyEffect();
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        Debug.Log(collision.gameObject);
        if (collision.gameObject.GetComponent<Conductor>() != null && collision.gameObject.GetComponent<Electrified>() == null)
        {
            Electrified newElectrifiedPart = collision.gameObject.AddComponent<Electrified>();
            newElectrifiedPart.thisConductor = collision.gameObject.GetComponent<Conductor>();
            newElectrifiedPart.shockDuration = shockDuration;
            if (newElectrifiedPart.thisConductor != null)
            {
                foreach (ParticleSystem particle in newElectrifiedPart.thisConductor.electricParticles)
                {
                    particle.Play();
                }
            }

        }
        if (collision.transform.tag == "Player")
        {
            Shock shock = collision.gameObject.AddComponent<Shock>();
            shock.Initialize(shockDuration);
            shock.ApplyEffect();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Electrified exitingElectrifiedObject = null;
        exitingElectrifiedObject = other.GetComponent<Electrified>();
        if (other.GetComponent<Conductor>() != null && exitingElectrifiedObject != null)
        {
            if (exitingElectrifiedObject.thisConductor != null)
            {
                foreach (ParticleSystem particle in exitingElectrifiedObject.thisConductor.electricParticles)
                {
                    particle.Stop();
                }
            }
            Destroy(other.gameObject.GetComponent<Electrified>());
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        Electrified exitingElectrifiedObject = null;
        exitingElectrifiedObject = collision.gameObject.GetComponent<Electrified>();
        if (collision.gameObject.GetComponent<Conductor>() != null && exitingElectrifiedObject != null)
        {
            if (exitingElectrifiedObject.thisConductor != null)
            {
                foreach (ParticleSystem particle in exitingElectrifiedObject.thisConductor.electricParticles)
                {
                    particle.Stop();
                }
            }
            Destroy(exitingElectrifiedObject);
        }
    }
}
