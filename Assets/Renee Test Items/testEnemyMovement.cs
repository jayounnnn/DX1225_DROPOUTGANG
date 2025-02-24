using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public class testEnemyMovement : EnemyBase
{
    [Header("Detection Settings")]
    private ConeCast3D coneCast;
    private Transform detectedPlayer;

    [Header("Patrol Settings")]
    public List<Transform> waypoints = new List<Transform>(); // Waypoints list
    private int currentWaypointIndex = 0;
    private bool isWaiting = false;

    [Header("Idle Settings")]
    public float idleTimeAtWaypoint = 2f; // Time to idle at each waypoint

    [Header("Roaming Settings")]
    private float roamRadius = 10f;
    private Vector3 roamTarget;

    private Transform player;
    private int playerLayer;
    private float detectionRange = 5f;
    private float raycastHeight = 1.5f; // Height for raycasts

    private NavMeshAgent agent;

    protected override void Start()
    {
        base.Start();
        agent = GetComponent<NavMeshAgent>();
        agent.speed = speed;

        playerLayer = LayerMask.GetMask("Player");

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
    }

    private void Update()
    {
        // Check for player detection
        if (DetectPlayerWithRaycast())
        {
            stateMachine.ChangeState(new AttackState(this, stateMachine, player));
            return;
        }

        // If enemy reaches destination and is not already waiting, idle before moving
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
        }

        detectedPlayer = DetectPlayerWithConeCast();
        if (detectedPlayer != null)
        {
            stateMachine.ChangeState(new ChaseState(this, stateMachine, detectedPlayer));
            return;
        }

    }

    private Transform DetectPlayerWithConeCast()
    {
        if (coneCast == null) return null;
        return coneCast.GetDetectedPlayer(); // Call ConeCast3D to check for player
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
                player = hit.collider.transform;
                Debug.Log("Zombie detected the player with Raycast!");
                return true;
            }
        }
        return false;
    }

    private IEnumerator IdleBeforeNextWaypoint()
    {
        isWaiting = true;
        agent.isStopped = true; // Stop movement

        Debug.Log("Idling at waypoint: " + waypoints[currentWaypointIndex].name);
        yield return new WaitForSeconds(idleTimeAtWaypoint);

        agent.isStopped = false; // Resume movement
        isWaiting = false;
        SetNextWaypoint();
    }

    private void SetNextWaypoint()
    {
        if (waypoints.Count == 0) return;

        agent.SetDestination(waypoints[currentWaypointIndex].position);
        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Count; // Loop through waypoints
    }

    private void SetNewRoamTarget()
    {
        Vector3 randomPosition = GetRandomNavMeshPosition();
        agent.SetDestination(randomPosition);
    }

    private Vector3 GetRandomNavMeshPosition()
    {
        Vector3 randomDirection = Random.insideUnitSphere * roamRadius;
        randomDirection += transform.position; // Offset from current position

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, roamRadius, NavMesh.AllAreas))
        {
            return hit.position; // Return a valid point on the NavMesh
        }
        return transform.position; // Fallback to current position
    }
}
