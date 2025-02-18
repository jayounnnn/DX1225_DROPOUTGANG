using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControllerOLD: MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private PlayerInput _playerInput;
    private InputActionAsset _inputActions;
    private List<IEnumerator> _attackQueue = new List<IEnumerator>();
    [SerializeField] private List<string> _attackNames;
    private int _attackStep = 0;
    private bool _isAttack = false;

    void Start()
    {
        _inputActions = _playerInput.actions;
    }

    void Update()
    {
        Vector2 input = _inputActions["Move"].ReadValue<Vector2>();
        Vector3 moveDirection = new Vector3(input.x, 0, Mathf.Clamp01(input.y));
        if (moveDirection.magnitude > 0)
        {
            _animator.SetBool("IsWalking", true);
            moveDirection = Quaternion.AngleAxis(Camera.main.transform.eulerAngles.y, Vector3.up) * moveDirection;
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * 100f);
        }
        else if (_inputActions["Attack"].IsPressed())
        {
            _animator.SetBool("IsAttack", true);
        }
        else
        {
            _animator.SetBool("IsWalking", false);
            //_animator.SetBool("IsAttack", false);
        }

        if (_inputActions["Attack"].WasPressedThisFrame())
        {

            if (_attackQueue.Count < 3)
            {
                _attackQueue.Add(PerformAttack());
            }

            if (_attackQueue.Count == 1)
            {
                StartCombo();
            }
        }
    }
    private void StartCombo()
    {
        _isAttack = true;
        _animator.SetBool("IsAttack", _isAttack);
        StartCoroutine(_attackQueue[0]);
    }

    private IEnumerator PerformAttack()
    {
        _attackStep++;
        _animator.SetInteger("AttackStep", _attackStep);

        while (!IsCurrentAnimationReadyForNextStep(_attackNames[_attackStep - 1]))
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

    private void ResetCombo()
    {
        _isAttack = false;
        _attackStep = 0;
        _animator.SetInteger("AttackStep", _attackStep);
        _animator.SetBool("IsAttack", false);
        _attackQueue.Clear();
    }



    private void OnAnimatorMove()
    {
        Vector3 velocity = _animator.deltaPosition;
        _characterController.Move(velocity);
    }

}