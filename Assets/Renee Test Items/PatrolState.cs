using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class PatrolState : IEnemyState
{
    private EnemyBase enemy;
    private EnemyStateMachine stateMachine;
    private NavMeshAgent agent;
    private List<Transform> waypoints;
    private int currentWaypointIndex = 0;
    private Vector3 defaultPatrolTarget;

    public PatrolState(EnemyBase enemy, EnemyStateMachine stateMachine, List<Transform> waypoints = null)
    {
        this.enemy = enemy;
        this.stateMachine = stateMachine;
        this.agent = enemy.GetComponent<NavMeshAgent>();

        if (waypoints != null && waypoints.Count > 0)
        {
            this.waypoints = new List<Transform>(waypoints);
        }
        else
        {
            this.waypoints = new List<Transform>();
            defaultPatrolTarget = enemy.transform.position; // Default patrol location
        }
    }

    public void EnterState()
    {
        Debug.Log(enemy.name + " started patrolling.");
        SetNextPatrolPoint();
    }

    public void UpdateState()
    {
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            SetNextPatrolPoint();
        }
    }

    public void ExitState()
    {
        Debug.Log(enemy.name + " stopped patrolling.");
    }

    private void SetNextPatrolPoint()
    {
        if (waypoints.Count > 0)
        {
            agent.SetDestination(waypoints[currentWaypointIndex].position);
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Count;
        }
        else
        {
            // Default patrol logic
            Vector3 randomPosition = GetRandomNavMeshPosition();
            agent.SetDestination(randomPosition);
        }
    }

    private Vector3 GetRandomNavMeshPosition()
    {
        Vector3 randomDirection = Random.insideUnitSphere * 5f;
        randomDirection += enemy.transform.position; // Offset from current position

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, 5f, NavMesh.AllAreas))
        {
            return hit.position; // Return a valid point on the NavMesh
        }

        return enemy.transform.position; // Fallback to current position
    }
}
