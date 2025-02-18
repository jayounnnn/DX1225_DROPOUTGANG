using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] private UIManager _uiManager;
    [SerializeField] private Animator _animator;
    [SerializeField] private List<string> _lightAttackNames;
    [SerializeField] private List<string> _heavyAttackNames;
    [SerializeField] private List<string> _swordAttackNames;

    [SerializeField] private float parryWindow = 0.3f;

    private List<IEnumerator> _attackQueue = new List<IEnumerator>();
    private int _attackStep = 0;
    private bool _isAttacking = false;
    private bool _isBlocking = false;
    private bool _isParrying = false;
    private int _attackType = 0; // 1 = Light, 2 = Heavy, 3 = Sword

    private bool canTriggerFinalAttack = false;

    public bool CanTriggerFinalAttack => canTriggerFinalAttack;

    private Coroutine _parryCoroutine;

    private void Awake()
    {
    
    }

    public void QueueLightAttack()
    {
        if (_attackType != 1)
        {
            ResetCombo();
        }

        if (_attackQueue.Count < 3)
        {
            _attackQueue.Add(PerformAttack(_lightAttackNames, 1)); 
        }

        if (_attackQueue.Count == 1)
        {
            StartCombo();
        }
    }

    public void QueueHeavyAttack()
    {
        if (_attackType != 2)
        {
            ResetCombo();
        }

        if (_attackQueue.Count < 3)
        {
            _attackQueue.Add(PerformAttack(_heavyAttackNames, 2)); 
        }

        if (_attackQueue.Count == 1)
        {
            StartCombo();
        }
    }

    public void QueueSwordAttack()
    {
        if (_attackType != 3)
        {
            ResetCombo();
        }

        if (_attackStep == 2)
        {
            canTriggerFinalAttack = true;
        }

        if (_attackQueue.Count < 2)
        {
            _attackQueue.Add(PerformAttack(_swordAttackNames, 3));
        }

        if (_attackQueue.Count == 1)
        {
            StartCombo();
        }
    }
    public void QueueFinalSwordAttack()
    {
        if (canTriggerFinalAttack)
        {
            canTriggerFinalAttack = false;
            _attackQueue.Add(PerformAttack(_swordAttackNames, 3));
            StartCombo();
        }
    }
    private void StartCombo()
    {
        _isAttacking = true;
        _animator.SetBool("IsAttack", _isAttacking);
        if (_attackQueue.Count > 0)
        {
            StartCoroutine(_attackQueue[0]);
        }
    }

    private IEnumerator PerformAttack(List<string> attackNames, int attackType)
    {
        _attackStep++;
        _attackType = attackType;

        _animator.SetInteger("AttackStep", _attackStep);
        _animator.SetInteger("AttackType", _attackType); 

        while (!IsCurrentAnimationReadyForNextStep(attackNames[_attackStep - 1]))
        {
            yield return null;
        }

        if (_attackStep >= _attackQueue.Count)
        {
            ResetCombo();
        }
        else
        {
            StartCoroutine(_attackQueue[_attackStep]);
        }
    }

    private bool IsCurrentAnimationReadyForNextStep(string name)
    {
        AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        return stateInfo.normalizedTime >= 0.7f && stateInfo.IsName(name);
    }

    public void ResetCombo()
    {
        _isAttacking = false;
        _attackStep = 0;
        _attackType = 0;

        canTriggerFinalAttack = false;

        _animator.SetInteger("AttackStep", _attackStep);
        _animator.SetInteger("AttackType", _attackType);
        _animator.SetBool("IsAttack", false);
        _attackQueue.Clear();
        StopAllCoroutines();
    }

    public void StartBlocking()
    {
        _isBlocking = true;
        _animator.SetBool("IsBlocking", true);
    }

    public void StopBlocking()
    {
        _isBlocking = false;
        _animator.SetBool("IsBlocking", false);
    }

    public void StartParry()
    {
        if (_parryCoroutine == null)
        {
            _parryCoroutine = StartCoroutine(ParryWindow());
        }
    }

    private IEnumerator ParryWindow()
    {
        _isParrying = true;
        _animator.SetTrigger("Parry");

        yield return new WaitForSeconds(parryWindow);

        _isParrying = false;
        _parryCoroutine = null;
    }

    public bool IsBlocking()
    {
        return _isBlocking;
    }

    public bool IsParrying()
    {
        return _isParrying;
    }
}
