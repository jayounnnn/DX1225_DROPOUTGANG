using UnityEngine;

public abstract class EnemyBase : Damageable // Now inherits from Damageable
{
    public float speed = 2f;
    public int damage = 10;
    public bool isInvincible = false;

    [Header("Investigation Settings")]
    public float investigationRadius = 5f; // Radius to detect thrown objects

    protected Animator animator;
    protected Rigidbody rb;
    protected EnemyStateMachine stateMachine;

    protected void Awake()
    {
        base.Start();

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
            return; 
        }

        stateMachine.ChangeState(new IdleState(this, stateMachine)); 
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

    protected override void OnDestroyed() // Overrides Damageable's OnDestroyed()
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
