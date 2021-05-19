using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManagerFunctions : MonoBehaviour
{
    public void StopBackgroundMusic(bool instant)
    {
        if(SoundManager.instance != null)
        {
            SoundManager.instance.EndBackgroundMusic(instant);
        }
    }
}
