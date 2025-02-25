using UnityEngine;

public abstract class IEnemyState : ScriptableObject
{
    public abstract void EnterState(EnemyBase enemy, EnemyStateMachine stateMachine);
    public abstract void UpdateState(EnemyBase enemy, EnemyStateMachine stateMachine);
    public abstract void ExitState(EnemyBase enemy, EnemyStateMachine stateMachine);
}
