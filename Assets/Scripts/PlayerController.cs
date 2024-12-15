using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerController : MonoBehaviour
{
    [Header("Input Settings")]
    [SerializeField] private PlayerInput playerInput;

    [SerializeField] float moveSpeed;
    [SerializeField] float mouseSensitivity;
    [SerializeField] float gravity;
    [SerializeField] private float walkSpeedMultiplier;
    [SerializeField] private float sprintSpeedMultiplier;
    [SerializeField] private float moveSpeedTransition;

    [Header("Camera Settings")]
    [SerializeField] private float bobbingFrequency;
    [SerializeField] private float bobbingAmplitude;
    [SerializeField] private float shakeStrength;
    [SerializeField] private float shakeDuration;

    private CharacterController characterController;

    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction jumpAction;
    private InputAction runAction;
    private InputAction crouchAction;
    private InputAction shootAction;
    private InputAction slideAction;

    private Vector3 move;
    private Vector2 mouseDelta;

    private Vector3 jumpVelocity = Vector3.zero;
    private float jumpHeight = 3.0f;

    private float speed;
    private float currentSpeedMultiplier;

    private float normalHeight;
    private float crouchHeight = 0.0f;

    private float cameraPitch;

    private float timer;

    private float bobbingOffset;
    private float originalCameraY;

    private Vector3 originalPosition;
    private Vector3 shakeOffSet;
    private float shakeTimeRemaing;

    private bool isSprinting = false;
    [SerializeField] private float normalFov;
    [SerializeField] private float sprintFov;
    [SerializeField] private float currentFov;

    // Sliding variables
    [SerializeField] private float slideDuration = 0.5f;
    [SerializeField] private float slideSpeedMultiplier = 2.0f;
    [SerializeField] private int maxSlides = 2;

    private int remainingSlides;
    private float slideCooldown = 1.0f;

    private bool isSliding = false;





    private void Awake()
    {
        moveAction = playerInput.actions["Move"];
        lookAction = playerInput.actions["Look"];
        jumpAction = playerInput.actions["Jump"];
        runAction = playerInput.actions["Run"];
        crouchAction = playerInput.actions["Crouch"];
        shootAction = playerInput.actions["Shoot"];
        slideAction = playerInput.actions["Slide"];
    }

    void Start()
    {
        remainingSlides = maxSlides;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        characterController = GetComponent<CharacterController>();

        normalHeight = characterController.height;

        originalPosition = Camera.main.transform.localPosition;

        currentFov = normalFov = Camera.main.fieldOfView;
        sprintFov = Camera.main.fieldOfView * 1.25f;
    }

    void Update()
    {
        Vector2 input = moveAction.ReadValue<Vector2>();

        Look();
        Jump();
        Run();
        Crouch();
        Shoot();


        originalCameraY = transform.position.y;

        jumpVelocity.y += gravity * Time.deltaTime;

        move = transform.right * input.x + transform.forward * input.y;
        float speedMultiplier = isSliding ? slideSpeedMultiplier : currentSpeedMultiplier;
        speed = Mathf.Lerp(speed, moveSpeed * currentSpeedMultiplier,
                           moveSpeedTransition * Time.deltaTime * 0.5f);

        characterController.Move((jumpVelocity + move * speed) *
                                  Time.deltaTime);

        HandleSlide();

        Debug.Log("Current Speed: " + speed);
    }


    private void LateUpdate()
    {
        HandleCameraPitch();
        HandleCameraBob();
        HandleCameraShake();
        HandleCameraFov();

        Transform camTransform = Camera.main.transform;

        camTransform.localPosition = originalPosition + shakeOffSet + new Vector3(0, bobbingOffset, 0);
    }

    private void HandleCameraFov()
    {
        if (isSprinting)
        {
            currentFov = Mathf.Lerp(currentFov, sprintFov, Time.deltaTime);
        }
        else
        {
            currentFov = Mathf.Lerp(currentFov, normalFov, Time.deltaTime);
        }

        Camera.main.fieldOfView = currentFov;
    }

    private void HandleCameraShake()
    {
        if (shakeTimeRemaing > 0)
        {
            shakeOffSet = Random.insideUnitSphere * shakeStrength;
            shakeTimeRemaing -= Time.deltaTime;
        }
        else
        {
            shakeOffSet = Vector3.zero;
        }
    }
    private void Shoot()
    {
        if (shootAction.IsPressed())
        {
            shakeTimeRemaing = shakeDuration;
        }
    }

    private void HandleCameraBob()
    {
        if (!characterController.isGrounded) return;

        bool isMoving = move.magnitude > 0;

        if (isMoving)
        {
            timer += Time.deltaTime * bobbingFrequency;
            bobbingOffset = Mathf.Sin(timer) * bobbingAmplitude;
        }
        else
        {
            bobbingOffset = Mathf.Lerp(bobbingOffset, 0, Time.deltaTime);
        }

        Transform camTransform = Camera.main.transform;

        camTransform.localPosition = new Vector3(camTransform.localPosition.x,
                                                originalCameraY + bobbingOffset,
                                                 camTransform.localPosition.z);
    }

    private void HandleCameraPitch()
    {
        float mouseY = mouseDelta.y * mouseSensitivity * Time.deltaTime;

        cameraPitch -= mouseY;
        cameraPitch = Mathf.Clamp(cameraPitch, -90f, 90);
        Camera.main.transform.localRotation = Quaternion.Euler(cameraPitch, 0, 0);
    }

    private void Look()
    {
        mouseDelta = lookAction.ReadValue<Vector2>();
        float mouseX = mouseDelta.x * mouseSensitivity * Time.deltaTime;

        transform.Rotate(Vector3.up, mouseX, Space.World);
    }

    private void Crouch()
    {
        if (crouchAction.IsPressed())
        {
            characterController.height = crouchHeight;
            characterController.center = new Vector3(0, crouchHeight / 2, 0);
        }
        else
        {
            characterController.height = normalHeight;
            characterController.center = new Vector3(0, 0, 0);
        }
    }

    private void Run()
    {
        if (runAction.IsPressed())
        {
            currentSpeedMultiplier = sprintSpeedMultiplier;
            isSprinting = true;
        }
        else
        {
            currentSpeedMultiplier = walkSpeedMultiplier;
            isSprinting = false;
        }
    }

    private void Jump()
    {
        if (jumpAction.IsPressed() && characterController.isGrounded)
        {
            jumpVelocity.y = Mathf.Sqrt(-2 * gravity * jumpHeight);
        }

        if (characterController.isGrounded && jumpVelocity.y < 0)
        {
            jumpVelocity.y = -2;
        }
    }

    private void HandleSlide()
    {
        // Check if player can slide (must be sprinting and have slides remaining)
        if (slideAction.triggered && isSprinting && remainingSlides > 0 && !isSliding)
        {
            StartCoroutine(Slide());
        }
    }

    private IEnumerator Slide()
    {
        isSliding = true;
        remainingSlides--;

        characterController.height = crouchHeight;
        float slideTime = 0f;

        while (slideTime < slideDuration)
        {
            slideTime += Time.deltaTime;
            characterController.Move(move * (moveSpeed * slideSpeedMultiplier) * Time.deltaTime);
            yield return null;
        }

        // Slide is over; reset height back to normal
        characterController.height = normalHeight;
        isSliding = false;
    }
}