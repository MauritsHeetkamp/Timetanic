using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Passenger : MovingEntity
{
    public Player ownerPlayer; // Player the passenger is attached from

    public AIState currentState;


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
        base.OnDeath();
    }

    public enum AIState
    {
        IdleScared,
        Idle,
        RunningAround,
        Following
    }
}
