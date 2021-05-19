using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Custom.Audio;

public class Entity : Damagable
{
    [HideInInspector] public int disables;
    public Rigidbody thisRigid;
    bool canMove = true;

    public Animator animator;
    [SerializeField] ParticleSystem electrifiedParticles;
    [SerializeField] string shockAnimation;

    public Coroutine knockbackRoutine;
    [SerializeField] Vector3 maxStopKnockVelocity = new Vector3(0.01f, 0.01f, 0.01f);

    public ThreeDAudioPrefab gruntSFX;

    public GameObject deathParticle;
    public float deathParticleDuration = 1;

    [SerializeField] Transform center;
    public Transform leftHand, rightHand;

    public ThreeDAudioPrefab drownSFX;

    // Toggles movement
    public virtual void ToggleMovement()
    {
        canMove = !canMove;
    }

    public virtual void Seat(bool seat)
    {

    }

    public override void OnDeath()
    {
        base.OnDeath();

        GameObject newEntityPoof = Instantiate(deathParticle, center.position, Quaternion.identity);

        Destroy(newEntityPoof, deathParticleDuration);
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

    public virtual void Knockback(Vector3 globalKnockbackVelocity)
    {
        if(knockbackRoutine == null && thisRigid != null && globalKnockbackVelocity != Vector3.zero)
        {
            if(SoundManager.instance != null && gruntSFX.clip != null)
            {
                GameObject spawnedSFX = SoundManager.instance.Spawn3DAudio(gruntSFX, transform.position);
                Destroy(spawnedSFX, gruntSFX.clip.length);
            }
            Disable(true);
            thisRigid.AddForce(globalKnockbackVelocity);
            knockbackRoutine = StartCoroutine(CheckStopKnockback());
        }
    }

    public IEnumerator CheckStopKnockback()
    {
        while (true)
        {
            yield return null;
            if (thisRigid.velocity.x < maxStopKnockVelocity.x && thisRigid.velocity.y < maxStopKnockVelocity.y && thisRigid.velocity.z < maxStopKnockVelocity.z)
            {
                if(thisRigid.velocity.x > -maxStopKnockVelocity.x && thisRigid.velocity.y > -maxStopKnockVelocity.y && thisRigid.velocity.z > -maxStopKnockVelocity.z)
                {
                    OnStoppedKnockback();
                    break;
                }
                //Player is not moving anymore
            }
        }
        knockbackRoutine = null;
    }

    public virtual void OnStoppedKnockback()
    {
        Disable(false);
    }

    public enum BodyPart
    {
        player, leftHand, rightHand
    }
}
