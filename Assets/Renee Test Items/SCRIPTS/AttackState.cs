using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "EnemyStates/AttackState")]
public class AttackState : IEnemyState
{
    public float attackRange = 2f;
    public float attackCooldown = 1.5f;
    private float lastAttackTime = 0f;

    public override void EnterState(EnemyBase enemy, EnemyStateMachine stateMachine)
    {
        Debug.Log(enemy.name + " has entered Attack State!");
    }

    public override void UpdateState(EnemyBase enemy, EnemyStateMachine stateMachine)
    {
        NavMeshAgent agent = enemy.GetComponent<NavMeshAgent>();
        testEnemyMovement enemyMovement = enemy as testEnemyMovement;
        Transform player = enemy.GetPlayerTransform();


        if (player == null)
        {
            stateMachine.ChangeState(enemyMovement.patrolState);
            return;
        }

        float distanceToPlayer = Vector3.Distance(enemy.transform.position, player.position);

        if (distanceToPlayer <= attackRange)
        {
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                enemy.PerformAttack();
                lastAttackTime = Time.time;
            }
        }
        else
        {
            agent.SetDestination(player.position);
        }

        // If player moves too far away, return to chasing
        if (distanceToPlayer > 6f)
        {
            stateMachine.ChangeState(enemyMovement.chaseState);
        }
    }

    public override void ExitState(EnemyBase enemy, EnemyStateMachine stateMachine)
    {
        Debug.Log(enemy.name + " is leaving Attack State.");
    }
}
