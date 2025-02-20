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
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float transitionSpeed = 10f;

    [Header("FOV & Head Bobbing")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float defaultFOV = 60f;
    [SerializeField] private float sprintFOV = 75f;
    [SerializeField] private float fovTransitionSpeed = 10f;
    [SerializeField] private float bobSpeedWalk = 14f;
    [SerializeField] private float bobAmountWalk = 0.05f;
    [SerializeField] private float bobSpeedRun = 18f;
    [SerializeField] private float bobAmountRun = 0.1f;
    [SerializeField] private float bobSmoothness = 10f;

    private CharacterController characterController;
    private Vector3 jumpVelocity = Vector3.zero;
    private Vector3 moveDirection = Vector3.zero;
    private float currentSpeedMultiplier;
    private float currentSpeed;
    private float normalHeight;
    private float crouchHeight;
    private float targetHeight;
    private float normalCameraHeight;
    private float crouchCameraHeight;
    private float bobTimer = 0f;
    private bool isJumping = false;

    void Awake()
    {
        characterController = GetComponent<CharacterController>();

        normalHeight = characterController.height;
        crouchHeight = normalHeight / 2f;
        targetHeight = normalHeight;

        if (cameraTransform != null)
        {
            normalCameraHeight = cameraTransform.localPosition.y;
            crouchCameraHeight = normalCameraHeight - 1.8f;
        }

        if (playerCamera != null)
        {
            playerCamera.fieldOfView = defaultFOV;
        }
    }

    public void Move(Vector2 input, bool isRunning, bool isCrouching)
    {
        float centerOffset = 0.1f;
        currentSpeedMultiplier = isRunning ? sprintSpeedMultiplier : walkSpeedMultiplier;

        if (isCrouching)
        {
            targetHeight = crouchHeight;
            characterController.center = new Vector3(0, targetHeight / 2f + centerOffset, 0);
        }
        else
        {
            targetHeight = normalHeight;
            characterController.center = new Vector3(0, targetHeight / 2f + centerOffset, 0);
        }

        characterController.height = Mathf.Lerp(characterController.height, targetHeight, transitionSpeed * Time.deltaTime);

        if (cameraTransform != null)
        {
            float targetCameraHeight = isCrouching ? crouchCameraHeight : normalCameraHeight;
            Vector3 cameraPosition = cameraTransform.localPosition;
            cameraPosition.y = Mathf.Lerp(cameraPosition.y, targetCameraHeight, transitionSpeed * Time.deltaTime);
            cameraTransform.localPosition = cameraPosition;
        }

        if (playerCamera != null)
        {
            float targetFOV = isRunning ? sprintFOV : defaultFOV;
            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, targetFOV, Time.deltaTime * fovTransitionSpeed);
        }

        moveDirection = transform.right * input.x + transform.forward * input.y;
        currentSpeed = Mathf.Lerp(currentSpeed, moveSpeed * currentSpeedMultiplier, moveSpeedTransition * Time.deltaTime);

        Vector3 velocity = moveDirection * currentSpeed + jumpVelocity;
        characterController.Move(velocity * Time.deltaTime);

        if (characterController.isGrounded && moveDirection.magnitude > 0.1f)
        {
            HandleHeadBobbing(isRunning);
        }
        else
        {
            ResetHeadBobbing();
        }
    }

    public void Jump(bool jumpPressed)
    {
        if (jumpPressed && characterController.isGrounded)
        {
            jumpVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            isJumping = true;
        }

        if (characterController.isGrounded && jumpVelocity.y < 0)
        {
            jumpVelocity.y = -2f;
            isJumping = false;
        }

        jumpVelocity.y += gravity * Time.deltaTime;
    }

    private void HandleHeadBobbing(bool isRunning)
    {
        if (isJumping) return; 

        float bobSpeed = isRunning ? bobSpeedRun : bobSpeedWalk;
        float bobAmount = isRunning ? bobAmountRun : bobAmountWalk;

        bobTimer += Time.deltaTime * bobSpeed;
        float bobOffset = Mathf.Sin(bobTimer) * bobAmount;

        Vector3 localPos = cameraTransform.localPosition;
        localPos.y = Mathf.Lerp(localPos.y, normalCameraHeight + bobOffset, Time.deltaTime * bobSmoothness);
        cameraTransform.localPosition = localPos;
    }

    private void ResetHeadBobbing()
    {
        Vector3 localPos = cameraTransform.localPosition;
        localPos.y = Mathf.Lerp(localPos.y, normalCameraHeight, Time.deltaTime * bobSmoothness);
        cameraTransform.localPosition = localPos;
    }
}
