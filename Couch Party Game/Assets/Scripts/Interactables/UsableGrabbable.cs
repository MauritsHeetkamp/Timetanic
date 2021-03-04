using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UsableGrabbable : Grabbable
{
    public bool canUse = true;
    [SerializeField] float useDelay;


    public virtual bool CheckUse()
    {
        if (canUse)
        {
            return true;
        }
        return false;
    }

    public override void Disattach()
    {
        StopUse();
        base.Disattach();
    }

    public override void Break()
    {
        StopUse();
        base.Break();
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
