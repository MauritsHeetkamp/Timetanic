using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomFunctions : MonoBehaviour
{
    public void SetTimescale(float timescale)
    {
        if(Time.timeScale != timescale)
        {
            Time.timeScale = timescale;
        }
    }
}
