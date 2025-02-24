using UnityEngine;
using System.Collections;

public class StunState : IEnemyState
{
    private EnemyBase enemy;
    private EnemyStateMachine stateMachine;
    private float stunDuration = 3f;
    private float elapsedTime = 0f;
    private float rotationSpeed = 100f;
    private float rotationAmplitude = 30f; // Maximum rotation angle
    private Quaternion originalRotation;
    private Vector3 storedDestination; // Store the patrol destination
    private bool wasMoving; // Was the enemy moving before stun?
    private UnityEngine.AI.NavMeshAgent agent;
    private IEnemyState previousState; // Store previous state

    public StunState(EnemyBase enemy, EnemyStateMachine stateMachine, IEnemyState lastState)
    {
        this.enemy = enemy;
        this.stateMachine = stateMachine;
        this.agent = enemy.GetComponent<UnityEngine.AI.NavMeshAgent>();
        this.previousState = lastState; // Store the last known state before stun
    }

    public void EnterState()
    {
        Debug.Log(enemy.name + " is stunned!");
        originalRotation = enemy.transform.rotation;

        wasMoving = agent.hasPath;
        storedDestination = agent.destination;

        agent.isStopped = true;
        agent.velocity = Vector3.zero;

        enemy.StartCoroutine(StunTimer());
    }

    public void UpdateState()
    {
        elapsedTime += Time.deltaTime;

        float angle = Mathf.Sin(elapsedTime * rotationSpeed * Mathf.Deg2Rad) * rotationAmplitude;
        enemy.transform.rotation = originalRotation * Quaternion.Euler(0, angle, 0);
    }

    public void ExitState()
    {
        Debug.Log(enemy.name + " recovered from stun.");
        enemy.transform.rotation = originalRotation; // Reset rotation

        if (wasMoving)
        {
            agent.isStopped = false;
            agent.SetDestination(storedDestination);
        }
    }

    private IEnumerator StunTimer()
    {
        yield return new WaitForSeconds(stunDuration);

        if (previousState != null)
        {
            Debug.Log(enemy.name + " returning to previous state after stun.");
            stateMachine.ChangeState(previousState);
        }
        else
        {
            // Default fallback: Resume patrol
            stateMachine.ChangeState(new PatrolState(enemy, stateMachine));
        }
    }

    public static void ForceStun(EnemyBase enemy)
    {
        EnemyStateMachine stateMachine = enemy.GetComponent<EnemyStateMachine>();
        if (stateMachine != null)
        {
            Debug.Log(enemy.name + " is forcefully stunned!");

            IEnemyState lastState = stateMachine.GetCurrentState();

            stateMachine.ChangeState(new StunState(enemy, stateMachine, lastState));
        }
        else
        {
            Debug.LogError("Enemy does not have an EnemyStateMachine!");
        }
    }
}
