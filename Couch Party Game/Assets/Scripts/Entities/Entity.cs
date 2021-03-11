using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : Damagable
{
    [HideInInspector] public int disables;
    public Rigidbody thisRigid;
    bool canMove = true;

    // Toggles movement
    public virtual void ToggleMovement()
    {
        canMove = !canMove;
    }

    public virtual void Seat(bool seat)
    {

    }

    public virtual void Disable(bool disable)
    {
        if (disable)
        {
            disables++;
        }
        else
        {
            disables--;
        }
    }
}
