using UnityEngine;

public class EnemyStateMachine : MonoBehaviour
{
    [Header("Assigned States")]
    public IEnemyState initialState;


    private IEnemyState currentState;
    private EnemyBase enemy;

    private void Awake()
    {
        enemy = GetComponent<EnemyBase>();
    }

    private void Start()
    {
        if (initialState != null)
        {
            ChangeState(initialState);
        }
        else
        {
            Debug.LogError(name + " has no initial state assigned!", this);
        }
    }

    private void Update()
    {
        currentState?.UpdateState(enemy, this);
    }

    public void ChangeState(IEnemyState newState)
    {
        if (newState == null) return;

        currentState?.ExitState(enemy, this);
        currentState = newState;
        currentState?.EnterState(enemy, this);
    }

    public IEnemyState GetCurrentState()
    {
        return currentState;
    }
}
