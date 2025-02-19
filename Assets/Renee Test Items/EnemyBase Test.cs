using UnityEngine;

public abstract class EnemyBase : MonoBehaviour
{
    public float health = 100f;
    public float speed = 2f;
    public int damage = 10;
    public bool isInvincible = false;

    protected Animator animator;
    protected Rigidbody rb;
    protected EnemyStateMachine stateMachine;

    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        stateMachine = GetComponent<EnemyStateMachine>();

        if (stateMachine == null)
        {
            stateMachine = gameObject.AddComponent<EnemyStateMachine>(); // Auto-add if missing
        }
    }

    private void Start()
    {
        if (stateMachine == null)
        {
            Debug.LogError(name + " missing EnemyStateMachine", this);
            return; // Prevent further execution
        }

        stateMachine.ChangeState(new IdleState(this, stateMachine)); // Start in Idle
    }

    public virtual void TakeDamage(float amount)
    {
        health -= amount;
        if (health <= 0)
        {
            Die();
        }
    }

    protected abstract void Attack();

    public void PerformAttack()
    {
        Attack();
    }

    protected virtual void Die()
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
}



/*    public void PerformAttack()
    {
        Attack();
    }*/