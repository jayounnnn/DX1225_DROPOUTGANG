using UnityEngine;

public class EnemyStateMachine : MonoBehaviour
{
    private IEnemyState currentState;
    private EnemyBase enemy;

    private void Awake()
    {
        enemy = GetComponent<EnemyBase>();
    }

    private void Start()
    {
        ChangeState(new IdleState(enemy, this)); // Initial state
    }

    private void Update()
    {
        currentState?.UpdateState();
    }

    public void ChangeState(IEnemyState newState)
    {
        currentState?.ExitState();
        currentState = newState;
        currentState?.EnterState();
    }
}
