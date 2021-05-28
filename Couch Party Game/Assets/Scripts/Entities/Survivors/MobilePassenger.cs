using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MobilePassenger : Passenger
{
    bool init = true;

    [SerializeField] bool followOnTrigger;
    [SerializeField] float followDistance; // How far away the npc should stay from the player
    public NavMeshAgent navmeshAgent;
    [SerializeField] float checkFollowDelay = 0.5f; // Delay between location checks
    Coroutine followRoutine; // The coroutine that updates the npc destination

    [SerializeField] float minimalDistanceFromLastPoint;
    [SerializeField] float delayBeforeNextCheck;
    [SerializeField] float distanceBeforeNewPoint;
    [SerializeField] float runAroundDistanceFromOrigin;
    [SerializeField] string[] scaredAnimations;
    [SerializeField] string lookLeftString, lookRightString;
    [SerializeField] float minLookDelay, maxLookDelay;
    [SerializeField] string runString;

    public bool debug;
    public Vector3 debucLoc;

    Coroutine currentBehaviourRoutine;

    // Start is called before the first frame update
    void Start()
    {

        navmeshAgent.speed = movementSpeed;

        if (init)
        {
            init = false;
            SetState(AIState.IdleScared);
        }
    }

    public override void Init()
    {
        base.Init();
    }

    public override void SetState(AIState state)
    {
        if (init)
        {
            init = false;
        }

        if (state != currentState)
        {
            currentState = state;

            if(currentBehaviourRoutine != null)
            {
                StopCoroutine(currentBehaviourRoutine);
            }

            switch (state)
            {
                case AIState.Idle:
                    try
                    {
                        navmeshAgent.isStopped = true;
                    }
                    catch
                    {

                    }
                    animator.SetBool(runString, false);
                    break;

                case AIState.IdleScared:
                    try
                    {
                        navmeshAgent.isStopped = true;
                    }
                    catch
                    {

                    }
                    animator.SetBool(runString, false);
                    currentBehaviourRoutine = StartCoroutine(ScaredBehaviourRoutine());
                    break;

                case AIState.RunningAround:

                    try
                    {
                        navmeshAgent.isStopped = false;
                    }
                    catch
                    {

                    }
                    animator.SetBool(runString, true);
                    currentBehaviourRoutine = StartCoroutine(RunAround());
                    break;

                case AIState.Following:
                    try
                    {
                        navmeshAgent.isStopped = false;
                    }
                    catch
                    {

                    }
                    animator.SetBool(runString, true);
                    break;
            }
        }
    }

    IEnumerator RunAround()
    {
        Vector3 origin = transform.position;

        Vector3 targetLocation = new Vector3(origin.x + Random.Range(-runAroundDistanceFromOrigin, runAroundDistanceFromOrigin), origin.y, origin.z + Random.Range(-runAroundDistanceFromOrigin, runAroundDistanceFromOrigin));
        Vector3 lastLocation = targetLocation;

        bool forceNewLocation = false;

        while (true)
        {
            debucLoc = targetLocation;
            if(Vector3.Distance(transform.position, targetLocation) <= distanceBeforeNewPoint || forceNewLocation)
            {
                lastLocation = targetLocation;
                targetLocation = new Vector3(origin.x + Random.Range(-runAroundDistanceFromOrigin, runAroundDistanceFromOrigin), origin.y, origin.z + Random.Range(-runAroundDistanceFromOrigin, runAroundDistanceFromOrigin));

                if (forceNewLocation)
                {
                    forceNewLocation = false;
                }

                while (minimalDistanceFromLastPoint != 0 && Vector3.Distance(lastLocation, targetLocation) < minimalDistanceFromLastPoint)
                {
                    targetLocation = new Vector3(origin.x + Random.Range(-runAroundDistanceFromOrigin, runAroundDistanceFromOrigin), origin.y, origin.z + Random.Range(-runAroundDistanceFromOrigin, runAroundDistanceFromOrigin));
                    yield return null;
                }
            }

            if (!navmeshAgent.SetDestination(targetLocation))
            {
                forceNewLocation = true;
            }

            yield return new WaitForSeconds(delayBeforeNextCheck);
        }
    }

    IEnumerator ScaredBehaviourRoutine()
    {
        animator.SetBool(scaredAnimations[Random.Range(0, scaredAnimations.Length)], true);

        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minLookDelay, maxLookDelay));

            switch(Random.Range(0, 4))
            {
                case 0:
                    animator.SetTrigger(lookLeftString);
                    break;

                case 1:
                    animator.SetTrigger(lookRightString);
                    break;

                case 2:
                    animator.SetTrigger(lookLeftString);
                    animator.SetTrigger(lookRightString);
                    break;

                case 3:
                    animator.SetTrigger(lookRightString);
                    animator.SetTrigger(lookLeftString);
                    break;
            }
        }
    }

    public override void Disable(bool disable)
    {
        base.Disable(disable);
        if (disables > 0)
        {
            if (followRoutine != null)
            {
                StopCoroutine(followRoutine);
                followRoutine = null;
            }

            if(ownerPlayer != null)
            {
                ownerPlayer.followingPassengers.Remove(this);
                ownerPlayer.attachedSplitscreen.UpdateFollowingPlayerAmount();
                ownerPlayer = null;
            }

            navmeshAgent.enabled = false;

            SetState(AIState.IdleScared);
        }
        else
        {
            navmeshAgent.enabled = true;
        }
    }

    public override void OnDeath()
    {
        base.OnDeath();

        StopFollowTarget(true);
        Destroy(gameObject);
    }

    private void OnTriggerStay(Collider other)
    {
        if(disables <= 0 && followOnTrigger && other.tag == "Player" && ownerPlayer == null) // Checks if it should follow the target
        {
            Player player = other.GetComponent<Player>();

            if(player != null && player.followingPassengers.Count < player.maxFollowingPassengers)
            {
                FollowTarget(other.GetComponent<Player>());
            }
        }
    }

    // Make the npc follow a player
    public void FollowTarget(Player target)
    {
        if(disables <= 0)
        {
            SetState(AIState.Following);
            ownerPlayer = target; // Sets its target
            ownerPlayer.followingPassengers.Add(this); // Lets the player know the npc is following him/her
            ownerPlayer.attachedSplitscreen.UpdateFollowingPlayerAmount();
            followRoutine = StartCoroutine(FollowTarget());

            if(onFollowPlayer != null)
            {
                onFollowPlayer.Invoke();
            }

        }
    }

    // Make the npc stop following a player
    public void StopFollowTarget(bool death = false)
    {
        if(ownerPlayer != null)
        {
            ownerPlayer.followingPassengers.Remove(this); // Lets the player know the npc stopped following him/her
            ownerPlayer.attachedSplitscreen.UpdateFollowingPlayerAmount();
            ownerPlayer = null; // Removes its owner

            if (!death)
            {
                navmeshAgent.SetDestination(transform.position);
                if (followRoutine != null) // Checks if the npc has an active follow routine
                {
                    StopCoroutine(followRoutine);
                    followRoutine = null;
                }

                SetRandomIdleState();
            }
        }
    }

    // Keeps the npc updated on where to go
    IEnumerator FollowTarget()
    {
        navmeshAgent.SetDestination(ownerPlayer.transform.position); // Set its initial location
        while (ownerPlayer != null)
        {
            yield return new WaitForSeconds(checkFollowDelay);
            navmeshAgent.SetDestination(ownerPlayer.transform.position); // Set location to target
            if(Vector3.Distance(transform.position, ownerPlayer.transform.position) < followDistance) // Check if the npc is close enough
            {
                SetState(AIState.Idle);
                navmeshAgent.isStopped = true;
            }
            else
            {
                SetState(AIState.Following);
                navmeshAgent.isStopped = false;
            }
        }
        followRoutine = null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (debug)
        {
            Gizmos.DrawCube(debucLoc, Vector3.one);
        }
    }
}
