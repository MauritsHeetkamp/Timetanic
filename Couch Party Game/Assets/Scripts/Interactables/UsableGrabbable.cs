using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UsableGrabbable : Grabbable
{
    public bool canUse = true;
    [SerializeField] float useDelay;


    // Checks if this item can be used
    public virtual bool CheckUse()
    {
        if (canUse)
        {
            return true;
        }
        return false;
    }

    // Disattaches the item
    public override void Disattach()
    {
        StopUse(); // Makes sure the item is done being used
        base.Disattach();
    }

    // Breaks the item
    public override void Break()
    {
        StopUse(); // Makes sure the item is done being used
        base.Break();
    }

    // Uses the item
    public virtual void Use()
    {
        canUse = false;
    }

    // Stops using the item
    public virtual void StopUse()
    {
        StartUseDelay();
    }

    // Starts the delay between uses
    public void StartUseDelay()
    {
        StartCoroutine(UseDelay());
    }

    // Delay before item can be used again
    IEnumerator UseDelay()
    {
        yield return new WaitForSeconds(useDelay);
        canUse = true;
    }
}
