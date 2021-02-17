using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UsableGrabbable : Grabbable
{
    public bool canUse = true;
    [SerializeField] float useDelay;


    public bool CheckUse()
    {
        if (canUse)
        {
            return true;
        }
        return false;
    }

    public virtual void Use()
    {
        canUse = false;
    }

    public virtual void StopUse()
    {
        StartUseDelay();
    }

    public void StartUseDelay()
    {
        StartCoroutine(UseDelay());
    }

    IEnumerator UseDelay()
    {
        yield return new WaitForSeconds(useDelay);
        canUse = true;
    }
}
