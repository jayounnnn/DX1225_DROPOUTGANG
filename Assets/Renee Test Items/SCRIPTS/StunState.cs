using UnityEngine;
using System.Collections;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "EnemyStates/StunState")]
public class StunState : IEnemyState
{
    public float stunDuration = 3f;
    public float rotationSpeed = 100f;
    public float rotationAmplitude = 30f;

    private Quaternion originalRotation;
    private Vector3 storedDestination;
    private bool wasMoving;

    public override void EnterState(EnemyBase enemy, EnemyStateMachine stateMachine)
    {
        Debug.Log(enemy.name + " is stunned!");
        NavMeshAgent agent = enemy.GetComponent<NavMeshAgent>();

        originalRotation = enemy.transform.rotation;
        wasMoving = agent.hasPath;
        storedDestination = agent.destination;

        agent.isStopped = true;
        agent.velocity = Vector3.zero;

        enemy.StartCoroutine(StunTimer(enemy, stateMachine));
    }

    public override void UpdateState(EnemyBase enemy, EnemyStateMachine stateMachine)
    {
        float angle = Mathf.Sin(Time.time * rotationSpeed * Mathf.Deg2Rad) * rotationAmplitude;
        enemy.transform.rotation = originalRotation * Quaternion.Euler(0, angle, 0);
    }

    public override void ExitState(EnemyBase enemy, EnemyStateMachine stateMachine)
    {
        Debug.Log(enemy.name + " recovered from stun.");
        NavMeshAgent agent = enemy.GetComponent<NavMeshAgent>();
        enemy.transform.rotation = originalRotation;

        if (wasMoving)
        {
            agent.isStopped = false;
            agent.SetDestination(storedDestination);
        }
    }

    private IEnumerator StunTimer(EnemyBase enemy, EnemyStateMachine stateMachine)
    {
        yield return new WaitForSeconds(stunDuration);
        testEnemyMovement enemyMovement = enemy as testEnemyMovement;

        if (enemyMovement != null)
        {
            Debug.Log(enemy.name + " returning to previous state after stun.");
            stateMachine.ChangeState(enemy.defaultState);
        }
    }

    public static void ForceStun(EnemyBase enemy)
    {
        EnemyStateMachine stateMachine = enemy.GetComponent<EnemyStateMachine>();
        if (stateMachine != null)
        {
            Debug.Log(enemy.name + " is forcefully stunned!");
            stateMachine.ChangeState(Resources.Load<StunState>("StunState"));
        }
        else
        {
            Debug.LogError("Enemy does not have an EnemyStateMachine!");
        }
    }
}
