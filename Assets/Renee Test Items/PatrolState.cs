using UnityEngine;
using UnityEngine.AI; // Import NavMesh

public class PatrolState : IEnemyState
{
    private EnemyBase enemy;
    private EnemyStateMachine stateMachine;
    private NavMeshAgent agent;
    private float patrolRange = 5f;

    public PatrolState(EnemyBase enemy, EnemyStateMachine stateMachine)
    {
        this.enemy = enemy;
        this.stateMachine = stateMachine;
        agent = enemy.GetComponent<NavMeshAgent>();

        SetNewPatrolDestination();
    }

    public void EnterState()
    {
        Debug.Log(enemy.name + " is now Patrolling.");
    }

    public void UpdateState()
    {
        // If reached patrol target, switch to idle
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            stateMachine.ChangeState(new IdleState(enemy, stateMachine));
        }
    }

    public void ExitState()
    {
        Debug.Log(enemy.name + " is stopping Patrol.");
    }

    private void SetNewPatrolDestination()
    {
        Vector3 newTarget = GetRandomNavMeshPosition();
        agent.SetDestination(newTarget);
    }

    private Vector3 GetRandomNavMeshPosition()
    {
        Vector3 randomDirection = Random.insideUnitSphere * patrolRange;
        randomDirection += enemy.transform.position;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, patrolRange, NavMesh.AllAreas))
        {
            return hit.position;
        }
        return enemy.transform.position; // Fallback to current position
    }
}
