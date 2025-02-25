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
        Transform player = enemy.GetPlayerTransform();

        if (player == null || enemy.isPlayerHiding)
        {
            Debug.Log(enemy.name + " stopped chasing: Player is hiding or not found.");
            stateMachine.ChangeState(enemy.defaultState); // Return to patrol or idle
            return;
        }

        float distanceToPlayer = Vector3.Distance(enemy.transform.position, player.position);

        // If within attack range, stop moving and switch to AttackState
        if (distanceToPlayer <= 1.5f) // Attack range threshold
        {
            agent.isStopped = true; // Stop movement
            Debug.Log(enemy.name + " is in range. Switching to Attack State.");
            stateMachine.ChangeState(enemy.attackState);
            return;
        }
        else
        {
            agent.isStopped = false; // Ensure movement continues if not attacking
            agent.SetDestination(player.position);
        }

        // If player moves too far away, return to patrol or idle
        if (distanceToPlayer > 6f)
        {
            Debug.Log(enemy.name + " lost the player. Returning to patrol.");
            stateMachine.ChangeState(enemy.defaultState);
        }
    }

    public override void ExitState(EnemyBase enemy, EnemyStateMachine stateMachine)
    {
        Debug.Log(enemy.name + " stopped chasing.");
    }
}
