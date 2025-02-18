using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Damageable
{
    private Animator animator;
    private Transform player;

    [SerializeField] private float walkingSpeed = 2f;
    //[SerializeField] private float runningSpeed = 5f;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackCooldown = 1.5f;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private float chaseRange = 10f;

    [SerializeField] private float deathAnimationDuration = 3f;

    private Vector3 lastPosition;
    private bool isAttacking;

    [SerializeField] private ParticleSystem bloodEffect;
    [SerializeField] private float hurtAnimationDuration = 0.5f;

    protected override void Start()
    {
        base.Start();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        lastPosition = transform.position;

        if (animator != null)
        {
            animator.applyRootMotion = false;
        }

        if (bloodEffect != null)
        {
            bloodEffect.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (!IsAlive || player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= chaseRange) 
        {
            if (!isAttacking)
            {
                MoveTowardsPlayer();
                UpdateAnimatorParameters();
            }

            TryAttack();
        }
        else
        {
            if (animator != null)
            {
                animator.SetFloat("Input", 0f); 
            }
        }
    }

    private void MoveTowardsPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;

        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            lookRotation.x = 0; 
            lookRotation.z = 0; 
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
        }

        transform.position += direction * moveSpeed * Time.deltaTime;
    }

    private void UpdateAnimatorParameters()
    {
        if (animator != null)
        {
            float speed = ((transform.position - lastPosition).magnitude) / Time.deltaTime;

            lastPosition = transform.position;

            if (speed == 0)
            {
                animator.SetFloat("Input", 0f);
            }
            else if (speed > 0 && speed <= walkingSpeed)
            {
                animator.SetFloat("Input", 0.5f); 
            }
            else if (speed > walkingSpeed)
            {
                animator.SetFloat("Input", 1f); 
            }
        }
    }

    private void TryAttack()
    {
        if (Vector3.Distance(transform.position, player.position) <= attackRange && !isAttacking)
        {
            StartCoroutine(Attack());
        }
    }

    private IEnumerator Attack()
    {
        isAttacking = true;

        yield return StartCoroutine(RotateTowardsPlayer());

        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }

        yield return new WaitForSeconds(attackCooldown);

        isAttacking = false;
    }

    private IEnumerator RotateTowardsPlayer()
    {
        if (player == null) yield break;

        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        targetRotation.x = 0;
        targetRotation.z = 0;

        while (Quaternion.Angle(transform.rotation, targetRotation) > 2f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            yield return null;
        }
    }

    public override void TakeDamage(float damage)
    {
        if (!IsAlive) return;
        base.TakeDamage(damage);
        if (animator != null)
        {
            animator.SetTrigger("Hurt");
        }

        if (bloodEffect != null)
        {
            StartCoroutine(ShowBloodEffect());
        }
    }

    private IEnumerator ShowBloodEffect()
    {
        bloodEffect.gameObject.SetActive(true);
        bloodEffect.Play();
        yield return new WaitForSeconds(hurtAnimationDuration);
        bloodEffect.Stop();
        bloodEffect.gameObject.SetActive(false);
    }


    protected override void OnDestroyed()
    {
        //Debug.Log("Dea");
        animator.SetBool("IsAlive", false);
        //animator.applyRootMotion = true;
        StartCoroutine(PlayDeathAnimation()); 
 
    }

    private IEnumerator PlayDeathAnimation()
    {
        yield return new WaitForSeconds(deathAnimationDuration);
        Destroy(gameObject);
    }
}
