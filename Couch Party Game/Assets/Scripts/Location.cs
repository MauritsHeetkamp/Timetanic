using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Location : MonoBehaviour
{
    [SerializeField] Animator floodAnimator;
    [SerializeField] string floodString = "Flood";

    public void Flood()
    {
        if(floodAnimator != null)
        {
            floodAnimator.SetBool(floodString, true);
        }
    }
}
