using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class EnemyBase : Damageable
{
    public float speed = 2f;
    public int damage = 10;
    public bool isInvincible = false;

    [Header("Investigation Settings")]
    public float investigationRadius = 5f; // Radius to detect thrown objects

    [Header("State Machine")]
    public EnemyStateMachine stateMachine;
    public IEnemyState defaultState;

    [Header("Patrol Settings")]
    public List<Transform> waypoints = new List<Transform>();
    public int currentWaypointIndex = 0;

    protected Animator animator;
    protected Rigidbody rb;
    protected Transform playerTransform;

    protected void Awake()
    {
        base.Start();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        if (stateMachine == null)
        {
            Debug.LogError(name + " is missing EnemyStateMachine!", this);
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Draw the investigation radius when the enemy is selected
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, investigationRadius);
    }

    public override void TakeDamage(float amount)
    {
        base.TakeDamage(amount);

        if (!isAlive)
        {
            OnDestroyed();
        }
    }

    protected abstract void Attack();

    public void PerformAttack()
    {
        Attack();
    }


    protected override void OnDestroyed()
    {
        if (!isInvincible)
        {
            Debug.Log(name + " has died.");
            Destroy(gameObject);
        }
        else
        {
            Debug.Log(name + " doesn't die.");
        }
    }

    public virtual Transform GetPlayerTransform()
    {
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;
            }
            else
            {
                Debug.LogError("Player object not found! Ensure it has the 'Player' tag.");
            }
        }
        return playerTransform;
    }

    public virtual void SetNextWaypoint()
    {
        if (waypoints.Count == 0) return;

        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        agent.SetDestination(waypoints[currentWaypointIndex].position);
        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Count;
    }

    public Vector3 lastThrowablePosition { get; set; }

}
