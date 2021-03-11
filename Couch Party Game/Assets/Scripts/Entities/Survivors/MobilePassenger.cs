using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MobilePassenger : Passenger
{
    [SerializeField] bool followOnTrigger;
    [SerializeField] float followDistance; // How far away the npc should stay from the player
    public NavMeshAgent navmeshAgent;
    [SerializeField] float checkFollowDelay = 0.5f; // Delay between location checks
    Coroutine followRoutine; // The coroutine that updates the npc destination

    // Start is called before the first frame update
    void Start()
    {
        navmeshAgent.speed = movementSpeed;
    }

    public override void Disable(bool disable)
    {
        if(disables > 0)
        {
            if (followRoutine != null)
            {
                StopCoroutine(followRoutine);
                followRoutine = null;
            }

            navmeshAgent.enabled = false;
        }
        else
        {
            navmeshAgent.enabled = true;
        }
        base.Disable(disable);
    }

    private void OnTriggerStay(Collider other)
    {
        if(disables <= 0 && followOnTrigger && other.tag == "Player" && ownerPlayer == null) // Checks if it should follow the target
        {
            FollowTarget(other.GetComponent<Player>());
        }
    }

    // Make the npc follow a player
    public void FollowTarget(Player target)
    {
        if(disables <= 0)
        {
            ownerPlayer = target; // Sets its target
            ownerPlayer.followingPassengers.Add(this); // Lets the player know the npc is following him/her
            StartCoroutine(FollowTarget());
        }
    }

    // Make the npc stop following a player
    public void StopFollowTarget()
    {
        ownerPlayer.followingPassengers.Remove(this); // Lets the player know the npc stopped following him/her
        ownerPlayer = null; // Removes its owner
        navmeshAgent.SetDestination(transform.position);
        if(followRoutine != null) // Checks if the npc has an active follow routine
        {
            StopCoroutine(followRoutine);
            followRoutine = null;
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
                navmeshAgent.isStopped = true;
            }
            else
            {
                navmeshAgent.isStopped = false;
            }
        }
        followRoutine = null;
    }
}
