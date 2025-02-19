using UnityEngine;
using UnityEngine.AI;

public class testEnemyMovement : EnemyBase
{
    private Transform player;
    private int playerLayer;
    private float detectionRange = 5f;
    private float raycastHeight = 1.5f; // Height at which raycasts are fired
    private float roamRadius = 10f;
    private Vector3 roamTarget;

    private NavMeshAgent agent;

    protected override void Awake()
    {
        base.Awake();

        agent = GetComponent<NavMeshAgent>();
        agent.speed = speed; 

        playerLayer = LayerMask.GetMask("Player"); 
        SetNewRoamTarget();

        if (stateMachine == null)
        {
            Debug.LogError(name + " is missing statemachine", this);
        }
    }

    private void Update()
    {
        // Check for player using Raycasts
        if (DetectPlayerWithRaycast())
        {
            stateMachine.ChangeState(new AttackState(this, stateMachine, player));
            return;
        }

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            SetNewRoamTarget();
        }
    }

    protected override void Attack()
    {
        Debug.Log("Zombie attacks the player!");
    }

    private bool DetectPlayerWithRaycast()
    {
        RaycastHit hit;
        Vector3 rayOrigin = transform.position + Vector3.up * raycastHeight;

        // Cast a ray in front of the zombie
        if (Physics.Raycast(rayOrigin, transform.forward, out hit, detectionRange, playerLayer))
        {
            if (hit.collider.CompareTag("Player")) // Just in case we want extra validation
            {
                player = hit.collider.transform;
                Debug.Log("Zombie detected the player with Raycast!");
                return true;
            }
        }

        return false;
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
