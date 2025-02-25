using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "EnemyStates/PatrolState")]
public class PatrolState : IEnemyState
{
    public override void EnterState(EnemyBase enemy, EnemyStateMachine stateMachine)
    {
        Debug.Log(enemy.name + " started patrolling.");
        SetNextPatrolPoint(enemy);
    }

    public override void UpdateState(EnemyBase enemy, EnemyStateMachine stateMachine)
    {
        NavMeshAgent agent = enemy.GetComponent<NavMeshAgent>();

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            SetNextPatrolPoint(enemy);
        }
    }

    public override void ExitState(EnemyBase enemy, EnemyStateMachine stateMachine)
    {
        Debug.Log(enemy.name + " stopped patrolling.");
    }

/*    private void SetNextPatrolPoint(EnemyBase enemy)
    {
        testEnemyMovement enemyMovement = enemy as testEnemyMovement;
        if (enemyMovement == null || enemyMovement.waypoints.Count == 0) return;

        NavMeshAgent agent = enemy.GetComponent<NavMeshAgent>();
        agent.SetDestination(enemyMovement.waypoints[enemyMovement.currentWaypointIndex].position);
        enemyMovement.currentWaypointIndex = (enemyMovement.currentWaypointIndex + 1) % enemyMovement.waypoints.Count;
    }*/

    private void SetNextPatrolPoint(EnemyBase enemy)
    {
        enemy.SetNextWaypoint();
    }
}
