using UnityEngine;

[CreateAssetMenu(menuName = "EnemyStates/IdleState")]
public class IdleState : IEnemyState
{
    public float idleTime = 5f;
    private float timer;

    public override void EnterState(EnemyBase enemy, EnemyStateMachine stateMachine)
    {
        Debug.Log(enemy.name + " is now Idle for " + idleTime + " seconds.");
        timer = 0f;
    }

    public override void UpdateState(EnemyBase enemy, EnemyStateMachine stateMachine)
    {
        timer += Time.deltaTime;
        if (timer >= idleTime)
        {
            testEnemyMovement enemyMovement = enemy as testEnemyMovement;
            stateMachine.ChangeState(enemyMovement.patrolState);
        }
    }

    public override void ExitState(EnemyBase enemy, EnemyStateMachine stateMachine)
    {
        Debug.Log(enemy.name + " is leaving Idle state.");
    }
}
