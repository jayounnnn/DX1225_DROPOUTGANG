using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "EnemyStates/ChaseState")]
public class ChaseState : IEnemyState
{
    public float lostPlayerTime = 3f;
    private float timeSinceLastSeen = 0f;

    public override void EnterState(EnemyBase enemy, EnemyStateMachine stateMachine)
    {
        Debug.Log(enemy.name + " started chasing the player!");
        NavMeshAgent agent = enemy.GetComponent<NavMeshAgent>();
        agent.isStopped = false;
    }

    public override void UpdateState(EnemyBase enemy, EnemyStateMachine stateMachine)
    {
        NavMeshAgent agent = enemy.GetComponent<NavMeshAgent>();
        testEnemyMovement enemyMovement = enemy as testEnemyMovement;
        Transform player = enemyMovement?.GetPlayerTransform();

        if (player == null)
        {
            timeSinceLastSeen += Time.deltaTime;
        }
        else
        {
            agent.SetDestination(player.position);
            timeSinceLastSeen = 0f;
        }

        if (timeSinceLastSeen >= lostPlayerTime)
        {
            stateMachine.ChangeState(enemy.defaultState);
        }
    }

    public override void ExitState(EnemyBase enemy, EnemyStateMachine stateMachine)
    {
        Debug.Log(enemy.name + " stopped chasing.");
    }
}
