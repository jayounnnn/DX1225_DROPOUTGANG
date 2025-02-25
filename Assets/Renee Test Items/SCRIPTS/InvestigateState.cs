using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[CreateAssetMenu(menuName = "EnemyStates/InvestigationState")]
public class InvestigationState : IEnemyState
{
    public float investigationTime = 3f;
    public float idleTime = 2f;
    public float lookAroundSpeed = 10f;
    public int lookAroundTurns = 3;

    private Vector3 investigationTarget;

    public override void EnterState(EnemyBase enemy, EnemyStateMachine stateMachine)
    {
        testEnemyMovement enemyMovement = enemy as testEnemyMovement;
        if (enemyMovement == null) return;

        investigationTarget = enemy.lastThrowablePosition;
        Debug.Log(enemy.name + " is investigating an item at " + investigationTarget);

        NavMeshAgent agent = enemy.GetComponent<NavMeshAgent>();
        agent.SetDestination(investigationTarget);
    }

    public override void UpdateState(EnemyBase enemy, EnemyStateMachine stateMachine)
    {
        NavMeshAgent agent = enemy.GetComponent<NavMeshAgent>();

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            Debug.Log(enemy.name + " is inspecting the item.");
            enemy.StartCoroutine(IdleAndLookAround(enemy, stateMachine));
        }
    }

    public override void ExitState(EnemyBase enemy, EnemyStateMachine stateMachine)
    {
        Debug.Log(enemy.name + " finished investigating.");
    }

    private IEnumerator IdleAndLookAround(EnemyBase enemy, EnemyStateMachine stateMachine)
    {
        NavMeshAgent agent = enemy.GetComponent<NavMeshAgent>();
        agent.isStopped = true;
        yield return new WaitForSeconds(investigationTime);

        Debug.Log(enemy.name + " is idling and looking around.");
        for (int i = 0; i < lookAroundTurns; i++)
        {
            float randomAngle = Random.Range(-90f, 90f);
            yield return RotateToAngle(enemy, randomAngle);
            yield return new WaitForSeconds(0.5f);
        }

        agent.isStopped = false;
        testEnemyMovement enemyMovement = enemy as testEnemyMovement;
        stateMachine.ChangeState(enemyMovement.patrolState);
    }

    private IEnumerator RotateToAngle(EnemyBase enemy, float angleOffset)
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

        enemy.transform.rotation = targetRotation;
    }
}
