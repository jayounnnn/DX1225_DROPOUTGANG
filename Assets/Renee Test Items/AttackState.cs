using UnityEngine;
using UnityEngine.AI; // Import NavMesh

public class AttackState : IEnemyState
{
    private EnemyBase enemy;
    private EnemyStateMachine stateMachine;
    private Transform player;
    private NavMeshAgent agent;
    private float attackRange = 2f;

    public AttackState(EnemyBase enemy, EnemyStateMachine stateMachine, Transform player)
    {
        this.enemy = enemy;
        this.stateMachine = stateMachine;
        this.player = player;
        agent = enemy.GetComponent<NavMeshAgent>();

        agent.SetDestination(player.position); // Move towards player
    }

    public void EnterState()
    {
        Debug.Log(enemy.name + " is Attacking!");
    }

    public void UpdateState()
    {
        if (Vector3.Distance(enemy.transform.position, player.position) <= attackRange)
        {
            enemy.PerformAttack(); // Attack when in range
        }
        else
        {
            agent.SetDestination(player.position); // Keep moving toward player
        }

        // If player is too far, return to patrol
        if (Vector3.Distance(enemy.transform.position, player.position) > 6f)
        {
            stateMachine.ChangeState(new PatrolState(enemy, stateMachine));
        }
    }

    public void ExitState()
    {
        Debug.Log(enemy.name + " is stopping Attack.");
    }
}
