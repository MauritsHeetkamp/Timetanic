using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : Damagable
{
    [HideInInspector] public int disables;
    public Rigidbody thisRigid;
    bool canMove = true;

    public Animator animator;
    [SerializeField] ParticleSystem electrifiedParticles;
    [SerializeField] string shockAnimation;

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

    public virtual void SetShock(bool value)
    {
        Debug.Log(value);
        Disable(value);
        if(electrifiedParticles != null)
        {
            if (value)
            {
                electrifiedParticles.Play();
            }
            else
            {
                electrifiedParticles.Stop();
            }
        }

        if(animator != null)
        {
            animator.SetBool(shockAnimation, value);
        }

        if (value)
        {
            BecomePermanentInvulnerable();
        }
        else
        {
            RemoveInvulnerability();
            BecomeInvulnerable();
        }
    }
}
