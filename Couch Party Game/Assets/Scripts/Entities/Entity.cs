using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : Damagable
{
    public Rigidbody thisRigid;
    bool canMove = true;

    public virtual void ToggleMovement()
    {
        canMove = !canMove;
    }
}
