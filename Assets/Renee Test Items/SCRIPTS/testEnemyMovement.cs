using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public class testEnemyMovement : EnemyBase
{
    [Header("States")]
    public IEnemyState patrolState;
    public IEnemyState chaseState;
    //public IEnemyState attackState;
    public IEnemyState idleState;
    public IEnemyState investigateState;

    [Header("Detection Settings")]
    private ConeCast3D coneCast;
    private Transform detectedPlayer;

    [Header("Idle Settings")]
    public float idleTimeAtWaypoint = 2f; 

    [Header("Roaming Settings")]
    private float roamRadius = 10f;
    private Vector3 roamTarget;

    private NavMeshAgent agent;
    private int playerLayer;
    private float detectionRange = 5f;
    private float raycastHeight = 1.5f;

    protected override void Start()
    {
        base.Start();
        agent = GetComponent<NavMeshAgent>();
        agent.speed = speed;

        playerLayer = LayerMask.GetMask("Player");
        playerTransform = GetPlayerTransform();

        if (waypoints.Count > 0)
        {
            SetNextWaypoint();
        }
        else
        {
            SetNewRoamTarget();
        }

        if (stateMachine == null)
        {
            Debug.LogError(name + " is missing stateMachine", this);
        }

        coneCast = GetComponent<ConeCast3D>();
        if (coneCast == null)
        {
            Debug.LogError(name + " is missing ConeCast3D component!", this);
        }

        if (idleState != null)
        {
            stateMachine.ChangeState(idleState);
        }
    }

    private void Update()
    {
        if (DetectPlayerWithRaycast() && chaseState != null)
        {
            stateMachine.ChangeState(chaseState);
            return;
        }

/*        // If enemy reaches destination and is not already waiting, idle before moving
        if (!isWaiting && !agent.pathPending && agent.remainingDistance < 0.5f)
        {
            if (waypoints.Count > 0)
            {
                StartCoroutine(IdleBeforeNextWaypoint());
            }
            else
            {
                SetNewRoamTarget();
            }
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            if (!(stateMachine.GetCurrentState() is StunState))
            {
                StunState.ForceStun(this);
                return;
            }
        }*/

        detectedPlayer = DetectPlayerWithConeCast();
        if (detectedPlayer != null && chaseState != null)
        {
            stateMachine.ChangeState(chaseState);
            return;
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Throwable"))
        {
            lastThrowablePosition = other.transform.position;
            Debug.Log(name + " detected a throwable at " + lastThrowablePosition);

            if (investigateState != null)
            {
                stateMachine.ChangeState(investigateState);
            }
        }
    }

    private Transform DetectPlayerWithConeCast()
    {
        return coneCast?.GetDetectedPlayer();
    }

    protected override void Attack()
    {
        Debug.Log("Zombie attacks the player!");
    }

    private bool DetectPlayerWithRaycast()
    {
        RaycastHit hit;
        Vector3 rayOrigin = transform.position + Vector3.up * raycastHeight;

        if (Physics.Raycast(rayOrigin, transform.forward, out hit, detectionRange, playerLayer))
        {
            if (hit.collider.CompareTag("Player")) // Validate player detection
            {
                playerTransform = hit.collider.transform;
                Debug.Log("Zombie detected the player with Raycast!");
                return true;
            }
        }
        return false;
    }

    /*private IEnumerator IdleBeforeNextWaypoint()
    {
        isWaiting = true;
        agent.isStopped = true; // Stop movement

        Debug.Log("Idling at waypoint: " + waypoints[currentWaypointIndex].name);
        yield return new WaitForSeconds(idleTimeAtWaypoint);

        agent.isStopped = false; // Resume movement
        isWaiting = false;
        SetNextWaypoint();
    }*/

    public void SetNextWaypoint()
    {
        if (waypoints.Count == 0) return;

        agent.SetDestination(waypoints[currentWaypointIndex].position);
        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Count;
    }

    private void SetNewRoamTarget()
    {
        Vector3 randomPosition = GetRandomNavMeshPosition();
        agent.SetDestination(randomPosition);
    }

    private Vector3 GetRandomNavMeshPosition()
    {
        Vector3 randomDirection = Random.insideUnitSphere * roamRadius;
        randomDirection += transform.position;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, roamRadius, NavMesh.AllAreas))
        {
            return hit.position; 
        }
        return transform.position; 
    }
}
