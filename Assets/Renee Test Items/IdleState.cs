using UnityEngine;
using UnityEngine.AI; // Import NavMesh

public class IdleState : IEnemyState
{
    private EnemyBase enemy;
    private EnemyStateMachine stateMachine;
    private float idleTime = 2f; // Time before switching states
    private float timer;
    private NavMeshAgent agent;

    public IdleState(EnemyBase enemy, EnemyStateMachine stateMachine)
    {
        this.enemy = enemy;
        this.stateMachine = stateMachine;
        agent = enemy.GetComponent<NavMeshAgent>();

        agent.ResetPath(); // Stop movement
    }

    public void EnterState()
    {
        Debug.Log(enemy.name + " is now Idle.");
        timer = 0f;
    }

    public void UpdateState()
    {
        timer += Time.deltaTime;
        if (timer >= idleTime)
        {
            stateMachine.ChangeState(new PatrolState(enemy, stateMachine));
        }
    }

    public void ExitState()
    {
        Debug.Log(enemy.name + " is leaving Idle state.");
    }
}
