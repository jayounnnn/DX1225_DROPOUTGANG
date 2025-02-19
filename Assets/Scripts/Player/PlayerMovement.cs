using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private Animator _animator;
    private CharacterController _characterController;

    private float velocityZ = 0.0f;
    private float velocityX = 0.0f;
    public float Acceleration = 5.0f;
    public float Deceleration = 5.0f;
    public float MaxWalkSpeed = 0.5f;
    public float MaxRunSpeed = 2.0f;
    private bool wasRunning = false;
    private bool isCrouching = false;

    public void Reset()
    {
        velocityX = 0.0f;
        velocityZ = 0.0f;
    }
    void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
    }

    public void ProcessMovement(Vector2 input, bool isRunning, bool crouch, bool disableRotation = false)
    {

        isCrouching = crouch;

        if (crouch)
        {
            isRunning = false;
        }


        float targetMaxSpeed = isRunning ? MaxRunSpeed : MaxWalkSpeed;
        float speedChangeRate = (isRunning == wasRunning) ? Acceleration : Deceleration;

        if (isRunning && !wasRunning)
        {
            velocityZ = Mathf.Lerp(velocityZ, MaxRunSpeed, Time.deltaTime * Deceleration);
        }
        else if (!isRunning && wasRunning)
        {
            velocityZ = Mathf.Lerp(velocityZ, MaxWalkSpeed, Time.deltaTime * Deceleration);
        }


        if (input.y > 0)
        {
            velocityZ = Mathf.MoveTowards(velocityZ, targetMaxSpeed, Time.deltaTime * speedChangeRate);
        }
        else if (input.y < 0)
        {
            velocityZ = Mathf.MoveTowards(velocityZ, -targetMaxSpeed, Time.deltaTime * speedChangeRate);
        }
        else
        {
            velocityZ = Mathf.MoveTowards(velocityZ, 0, Time.deltaTime * Deceleration);
        }

    
        if (input.x > 0)
        {
            velocityX = Mathf.MoveTowards(velocityX, targetMaxSpeed, Time.deltaTime * speedChangeRate);
        }
        else if (input.x < 0)
        {
            velocityX = Mathf.MoveTowards(velocityX, -targetMaxSpeed, Time.deltaTime * speedChangeRate);
        }
        else
        {
            velocityX = Mathf.MoveTowards(velocityX, 0, Time.deltaTime * Deceleration);
        }

        _animator.SetFloat("XInput", velocityX);
        _animator.SetFloat("ZInput", velocityZ);
        _animator.SetBool("IsCrouching", isCrouching);

        Vector3 movement = new Vector3(velocityX, 0, velocityZ);
        movement = Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0) * movement;
        _characterController.Move(movement * Time.deltaTime);

        if (!disableRotation)
        {

            if (input.y >= 0)
            {
                if (movement.magnitude > 0)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(movement);
                    transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
                }
            }
        }

        wasRunning = isRunning;
    }

    private void OnAnimatorMove()
    {
        Vector3 velocity = _animator.deltaPosition;
        _characterController.Move(velocity);
    }
}
