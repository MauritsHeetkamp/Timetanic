using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Electrified : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Water" && other.GetComponent<Electrified>() == null)
        {
            Electrified newElectrifiedPart = other.gameObject.AddComponent<Electrified>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Water" && other.GetComponent<Electrified>() != null)
        {
            Destroy(other.gameObject.GetComponent<Electrified>());
        }
    }
}
