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

    public void SetCursorVisibility(bool visible)
    {
        if (visible)
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
