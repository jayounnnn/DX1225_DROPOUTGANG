using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class ChaseState : IEnemyState
{
    private EnemyBase enemy;
    private EnemyStateMachine stateMachine;
    private Transform player;
    private NavMeshAgent agent;

    private float detectionRange = 5f;
    private float lostPlayerTime = 3f; // Time before returning to patrol
    private float timeSinceLastSeen = 0f;
    private bool isChasing = false;

    public ChaseState(EnemyBase enemy, EnemyStateMachine stateMachine, Transform player)
    {
        this.enemy = enemy;
        this.stateMachine = stateMachine;
        this.player = player;
        agent = enemy.GetComponent<NavMeshAgent>();
    }

    public void EnterState()
    {
        Debug.Log(enemy.name + " started chasing the player!");
        isChasing = true;
        agent.isStopped = false;
    }

    public void UpdateState()
    {
        if (player == null)
        {
            timeSinceLastSeen += Time.deltaTime;
        }
        else
        {
            agent.SetDestination(player.position);
            timeSinceLastSeen = 0f; // Reset timer if player is visible
        }

        // If the player has been out of range for more than 'lostPlayerTime', return to patrol
        if (timeSinceLastSeen >= lostPlayerTime)
        {
            Debug.Log(enemy.name + " lost the player. Returning to patrol.");
            stateMachine.ChangeState(new PatrolState(enemy, stateMachine));
        }
    }

    public void ExitState()
    {
        Debug.Log(enemy.name + " stopped chasing.");
        isChasing = false;
    }
}
