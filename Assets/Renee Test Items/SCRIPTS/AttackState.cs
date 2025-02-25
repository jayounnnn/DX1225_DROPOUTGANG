using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "EnemyStates/AttackState")]
public class AttackState : IEnemyState
{
    public float attackRange = 2f;
    public float attackCooldown = 1.5f;
    private float lastAttackTime = 0f;

    public override void EnterState(EnemyBase enemy, EnemyStateMachine stateMachine)
    {
        Debug.Log(enemy.name + " has entered Attack State!");
    }

    public override void UpdateState(EnemyBase enemy, EnemyStateMachine stateMachine)
    {
        NavMeshAgent agent = enemy.GetComponent<NavMeshAgent>();
        Transform player = enemy.GetPlayerTransform();

        if (player == null)
        {
            stateMachine.ChangeState(enemy.defaultState);
            return;
        }

        float distanceToPlayer = Vector3.Distance(enemy.transform.position, player.position);

        // Ensure enemy stays in AttackState while the cooldown is active
        if (distanceToPlayer <= attackRange)
        {
            agent.isStopped = true; // Ensure enemy doesn't move while attacking

            if (Time.time >= lastAttackTime + attackCooldown)
            {
                PerformAttack(enemy);
                lastAttackTime = Time.time;
            }
        }
        else
        {
            Debug.Log(enemy.name + " lost attack range. Resuming chase.");
            agent.isStopped = false;
            stateMachine.ChangeState(enemy.defaultState); // Return to chasing
        }

        // If player moves too far away, return to patrol
        if (distanceToPlayer > 6f)
        {
            Debug.Log(enemy.name + " lost the player. Returning to patrol.");
            stateMachine.ChangeState(enemy.defaultState);
        }
    }

    // Perform attack with sphere detection
    private void PerformAttack(EnemyBase enemy)
    {
        SphereCollider attackCollider = enemy.GetComponent<SphereCollider>();

        if (attackCollider == null)
        {
            Debug.LogError(enemy.name + " has no attack SphereCollider assigned!");
            return;
        }

        Debug.Log(enemy.name + " is attacking!");

        attackCollider.enabled = true;

        Collider[] hitColliders = Physics.OverlapSphere(attackCollider.transform.position, attackCollider.radius);
        foreach (Collider hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Player"))
            {
                Damageable target = hitCollider.GetComponent<Damageable>();
                PlayerCombat targetParry = hitCollider.GetComponent<PlayerCombat>();

                if (targetParry != null && targetParry.IsParrying())
                {
                    Rigidbody enemyRb = enemy.GetComponent<Rigidbody>();
                    if (enemyRb != null)
                    {
                        Vector3 knockbackDirection = (enemyRb.transform.position - target.transform.position).normalized;
                        enemyRb.AddForce(knockbackDirection * 5f, ForceMode.Impulse);
                    }

                    Debug.Log("Player parried the attack! Enemy knocked back.");
                }

                if (target != null)
                {
                    target.TakeDamage(10);
                }

                Debug.Log("Enemy hit Player");
            }
        }

        attackCollider.enabled = false;
        Debug.Log(enemy.name + " attack finished.");
    }

    public override void ExitState(EnemyBase enemy, EnemyStateMachine stateMachine)
    {
        Debug.Log(enemy.name + " is leaving Attack State.");
    }
}
