using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : Damagable
{
    public Rigidbody thisRigid;
    bool canMove = true;

    // Toggles movement
    public virtual void ToggleMovement()
    {
        canMove = !canMove;
    }
}
