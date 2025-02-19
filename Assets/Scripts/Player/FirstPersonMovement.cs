using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float moveSpeedTransition = 10f;
    [SerializeField] private float walkSpeedMultiplier = 1f;
    [SerializeField] private float sprintSpeedMultiplier = 1.5f;

    [Header("Jump & Gravity Settings")]
    [SerializeField] private float jumpHeight = 1.5f;
    [SerializeField] private float gravity = -9.81f;

    [Header("Crouching Settings")]
    [SerializeField] private float transitionSpeed = 10f;

    private CharacterController characterController;
    private Vector3 jumpVelocity = Vector3.zero;
    private Vector3 moveDirection = Vector3.zero;
    private float currentSpeedMultiplier;
    private float currentSpeed;

    private float normalHeight;
    private float crouchHeight;
    private float targetHeight;

    void Awake()
    {
        characterController = GetComponent<CharacterController>();

        normalHeight = characterController.height;
        crouchHeight = normalHeight / 2f;
        targetHeight = normalHeight;
    }

    public void Move(Vector2 input, bool isRunning, bool isCrouching)
    {

        currentSpeedMultiplier = isRunning ? sprintSpeedMultiplier : walkSpeedMultiplier;

        if (isCrouching)
        {
            targetHeight = crouchHeight;
            characterController.center = new Vector3(0, 0.5f, 0);
        }
        else
        {
            targetHeight = normalHeight;
            characterController.center = new Vector3(0, 1.1f, 0);
        }

        characterController.height = Mathf.Lerp(characterController.height, targetHeight, transitionSpeed * Time.deltaTime);

        moveDirection = transform.right * input.x + transform.forward * input.y;

        currentSpeed = Mathf.Lerp(currentSpeed, moveSpeed * currentSpeedMultiplier, moveSpeedTransition * Time.deltaTime);

        Vector3 velocity = moveDirection * currentSpeed + jumpVelocity;
        characterController.Move(velocity * Time.deltaTime);
    }

    public void Jump(bool jumpPressed)
    {

        if (jumpPressed && characterController.isGrounded)
        {

            jumpVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        if (characterController.isGrounded && jumpVelocity.y < 0)
        {
            jumpVelocity.y = -2f;
        }

        jumpVelocity.y += gravity * Time.deltaTime;
    }
}
