using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Passenger : MovingEntity
{
    public NPCSpawner spawnHandler;
    public Player ownerPlayer; // Player the passenger is attached from

    public AIState currentState;

    public UnityAction onFollowPlayer;


    private void Start()
    {
        if(spawnHandler != null)
        {
            spawnHandler.availableNPCs.Add(gameObject);
        }
    }

    public override void Knockback(Vector3 globalKnockbackVelocity)
    {
        Debug.Log("KNOCKED BACK");
        if (knockbackRoutine == null && thisRigid != null && globalKnockbackVelocity != Vector3.zero)
        {
            thisRigid.constraints = RigidbodyConstraints.FreezeRotation;
            if (SoundManager.instance != null && gruntSFX.clip != null)
            {
                GameObject spawnedSFX = SoundManager.instance.Spawn3DAudio(gruntSFX, transform.position);
                Destroy(spawnedSFX, gruntSFX.clip.length);
            }
            Disable(true);
            thisRigid.AddForce(globalKnockbackVelocity);
            knockbackRoutine = StartCoroutine(CheckStopKnockback());
        }
    }

    public override void OnStoppedKnockback()
    {
        thisRigid.constraints = RigidbodyConstraints.FreezeAll;
        base.OnStoppedKnockback();
    }

    public void SetRandomIdleState()
    {
        switch (Random.Range(0, 2))
        {
            case 0:
                SetState(AIState.IdleScared);
                break;

            case 1:
                SetState(AIState.RunningAround);
                break;
        }
    }

    public virtual void SetState(AIState state)
    {
        if(state != currentState)
        {
            currentState = state;
        }
    }

    public override void OnDeath()
    {
        GameHandler gameHandler = GameObject.FindGameObjectWithTag("GameManager").GetComponentInChildren<GameHandler>();
        gameHandler.PassengerDied();
        if (spawnHandler != null)
        {
            spawnHandler.availableNPCs.Remove(gameObject);
            spawnHandler.CheckNPCCount();
        }
        base.OnDeath();
    }

    public enum AIState
    {
        IdleScared,
        Idle,
        RunningAround,
        Following,
        Rescued
    }
}
