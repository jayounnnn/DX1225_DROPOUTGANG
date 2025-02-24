using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    public CinemachineVirtualCamera firstPersonCamera;
    public CinemachineFreeLook thirdPersonCamera;

    public PlayerMovement playermovement;

    public GameObject characterModel;
    public GameObject RightArm;

    public float hideDelay = 1.0f;

    private SkinnedMeshRenderer[] meshRenderers;
    private bool isFirstPerson = false;
    private Coroutine meshDisableCoroutine;
    private Animator animator;

    public bool IsFirstPerson => isFirstPerson;

    private bool temporarySwitchActive = false;
    private bool previousIsFirstPerson;

    void Start()
    {
        if (characterModel != null)
            meshRenderers = characterModel.GetComponentsInChildren<SkinnedMeshRenderer>();

        animator = characterModel.GetComponent<Animator>();

        playermovement = characterModel.GetComponent<PlayerMovement>();

        SetFirstPerson();
    }

    public void ToggleCamera()
    {
        if (isFirstPerson)
            SetThirdPerson();
        else
            SetFirstPerson();
    }

    public void SetFirstPerson()
    {
        isFirstPerson = true;
        firstPersonCamera.Priority = 20;
        thirdPersonCamera.Priority = 10;

        if (meshDisableCoroutine != null)
            StopCoroutine(meshDisableCoroutine);
        meshDisableCoroutine = StartCoroutine(DelayedDisableMeshRenderers(hideDelay));
    }

    public void SetThirdPerson()
    {
        animator.enabled = true;
        isFirstPerson = false;
        firstPersonCamera.Priority = 10;
        thirdPersonCamera.Priority = 20;

        if (meshDisableCoroutine != null)
        {
            StopCoroutine(meshDisableCoroutine);
            meshDisableCoroutine = null;
        }

        EnableMeshRenderers();
    }

    IEnumerator DelayedDisableMeshRenderers(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (isFirstPerson && meshRenderers != null)
        {
            foreach (SkinnedMeshRenderer mr in meshRenderers)
            {
                mr.enabled = false;
                RightArm.gameObject.SetActive(true);
                animator.enabled = false;
            }
        }
        meshDisableCoroutine = null;
    }

   private void EnableMeshRenderers()
    {
        if (meshRenderers != null)
        {
            foreach (SkinnedMeshRenderer mr in meshRenderers)
            {
                mr.enabled = true;
                RightArm.gameObject.SetActive(false);
            }
        }
    }

    public void SetFixedCameraRotation(Vector3 forwardDirection)
    {
        if (firstPersonCamera != null)
        {
            firstPersonCamera.transform.rotation = Quaternion.LookRotation(forwardDirection, Vector3.up);
        }
    }

    public void EnterTemporaryFirstPerson()
    {
        if (!isFirstPerson)
        {
            previousIsFirstPerson = isFirstPerson;
            temporarySwitchActive = true;
            playermovement.Reset();
            SetFirstPerson();
        }
    }

    public void ExitTemporaryFirstPerson()
    {
        if (temporarySwitchActive)
        {
            if (!previousIsFirstPerson)
                SetThirdPerson();
            else
                SetFirstPerson();
            temporarySwitchActive = false;
        }
    }
}
