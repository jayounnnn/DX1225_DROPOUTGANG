using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class InvestigateState : IEnemyState
{
    private EnemyBase enemy;
    private EnemyStateMachine stateMachine;
    private NavMeshAgent agent;
    private Vector3 investigatePosition;
    private float investigateDuration = 3f; // Time to stand at investigation spot
    private bool hasArrived = false;

    public InvestigateState(EnemyBase enemy, EnemyStateMachine stateMachine, Vector3 position)
    {
        this.enemy = enemy;
        this.stateMachine = stateMachine;
        this.agent = enemy.GetComponent<NavMeshAgent>();
        this.investigatePosition = position;
    }

    public void EnterState()
    {
        Debug.Log(enemy.name + " is investigating at " + investigatePosition);
        agent.SetDestination(investigatePosition);
    }

    public void UpdateState()
    {
        // Check if enemy reached the investigation point
        if (!hasArrived && !agent.pathPending && agent.remainingDistance < 0.5f)
        {
            hasArrived = true;
            enemy.StartCoroutine(InvestigateTimer());
        }
    }

    public void ExitState()
    {
        Debug.Log(enemy.name + " finished investigating.");
    }

    private IEnumerator InvestigateTimer()
    {
        yield return new WaitForSeconds(investigateDuration);
        stateMachine.ChangeState(new PatrolState(enemy, stateMachine)); // Resume patrol after investigation
    }
}
