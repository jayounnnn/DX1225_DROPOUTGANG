using UnityEngine;
using UnityEngine.AI; // Import NavMesh

public class IdleState : IEnemyState
{
    private EnemyBase enemy;
    private EnemyStateMachine stateMachine;
    private float idleTime = 5f; // 5-second delay before patrolling
    private float timer;

    public IdleState(EnemyBase enemy, EnemyStateMachine stateMachine)
    {
        this.enemy = enemy;
        this.stateMachine = stateMachine;
    }

    public void EnterState()
    {
        Debug.Log(enemy.name + " is now Idle. Waiting for " + idleTime + " seconds.");
        timer = 0f;
    }

    public void UpdateState()
    {
        timer += Time.deltaTime;
        if (timer >= idleTime)
        {
            stateMachine.ChangeState(new PatrolState(enemy, stateMachine)); // Start patrolling after 5 seconds
        }
    }

    public void ExitState()
    {
        Debug.Log(enemy.name + " is leaving Idle state.");
    }
}
