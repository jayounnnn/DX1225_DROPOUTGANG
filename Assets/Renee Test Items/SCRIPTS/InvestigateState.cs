using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class InvestigationState : IEnemyState
{
    private EnemyBase enemy;
    private EnemyStateMachine stateMachine;
    private NavMeshAgent agent;
    private Vector3 investigationTarget;
    private PatrolState patrolState;
    private float investigationTime = 3f; // Time spent investigating
    private float idleTime = 2f; // Time spent idling after investigating
    private float lookAroundSpeed = 10f; // Speed of rotation lerp
    private int lookAroundTurns = 3; // Number of times the enemy looks around

    public InvestigationState(EnemyBase enemy, EnemyStateMachine stateMachine, Vector3 itemPosition, PatrolState patrolState)
    {
        this.enemy = enemy;
        this.stateMachine = stateMachine;
        this.agent = enemy.GetComponent<NavMeshAgent>();
        this.investigationTarget = itemPosition;
        this.patrolState = patrolState;
    }

    public void EnterState()
    {
        Debug.Log(enemy.name + " is investigating an item at " + investigationTarget);
        agent.SetDestination(investigationTarget);
    }

    public void UpdateState()
    {
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            Debug.Log(enemy.name + " is inspecting the item.");
            enemy.StartCoroutine(IdleAndLookAround());
        }
    }

    public void ExitState()
    {
        Debug.Log(enemy.name + " finished investigating.");
    }

    private IEnumerator IdleAndLookAround()
    {
        agent.isStopped = true; // Stop moving
        yield return new WaitForSeconds(investigationTime); // Simulate inspecting the object

        Debug.Log(enemy.name + " is idling and looking around.");
        for (int i = 0; i < lookAroundTurns; i++)
        {
            float randomAngle = Random.Range(-90f, 90f); // Pick a random angle
            yield return RotateToAngle(randomAngle);
            yield return new WaitForSeconds(0.5f); // Pause before turning again
        }

        Debug.Log(enemy.name + " is resuming patrol.");
        agent.isStopped = false;
        stateMachine.ChangeState(patrolState);
    }

    private IEnumerator RotateToAngle(float angleOffset)
    {
        Quaternion startRotation = enemy.transform.rotation;
        Quaternion targetRotation = Quaternion.Euler(0, enemy.transform.eulerAngles.y + angleOffset, 0);
        float elapsedTime = 0f;

        while (elapsedTime < lookAroundSpeed)
        {
            enemy.transform.rotation = Quaternion.Slerp(startRotation, targetRotation, elapsedTime / lookAroundSpeed);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        enemy.transform.rotation = targetRotation; // Ensure final rotation is exact
    }
}
