using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MobilePassenger : Passenger
{
    [SerializeField] bool followOnTrigger;
    [SerializeField] float followDistance;
    public NavMeshAgent navmeshAgent;
    [SerializeField] float checkFollowDelay = 0.5f;
    Coroutine followRoutine;
    // Start is called before the first frame update
    void Start()
    {
        navmeshAgent.speed = movementSpeed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(followOnTrigger && other.tag == "Player" && ownerPlayer == null)
        {
            FollowTarget(other.GetComponent<Player>());
        }
    }

    public void FollowTarget(Player target)
    {
        ownerPlayer = target;
        ownerPlayer.followingPassengers.Add(this);
        StartCoroutine(FollowTarget());
    }

    public void StopFollowTarget()
    {
        ownerPlayer.followingPassengers.Remove(this);
        ownerPlayer = null;
        navmeshAgent.SetDestination(transform.position);
        if(followRoutine != null)
        {
            StopCoroutine(followRoutine);
            followRoutine = null;
        }
    }

    IEnumerator FollowTarget()
    {
        navmeshAgent.SetDestination(ownerPlayer.transform.position);
        while (ownerPlayer != null)
        {
            yield return new WaitForSeconds(checkFollowDelay);
            navmeshAgent.SetDestination(ownerPlayer.transform.position);
            if(Vector3.Distance(transform.position, ownerPlayer.transform.position) < followDistance)
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
