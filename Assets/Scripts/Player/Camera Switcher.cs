using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    public CinemachineVirtualCamera firstPersonCamera;
    public CinemachineFreeLook thirdPersonCamera;

    public GameObject characterModel;

    public float hideDelay = 1.0f;

    private SkinnedMeshRenderer[] meshRenderers;
    private bool isFirstPerson = false;
    private Coroutine meshDisableCoroutine;

    public bool IsFirstPerson => isFirstPerson;

    void Start()
    {
        if (characterModel != null)
            meshRenderers = characterModel.GetComponentsInChildren<SkinnedMeshRenderer>();

        SetFirstPerson();
    }

    public void ToggleCamera()
    {
        if (isFirstPerson)
            SetThirdPerson();
        else
            SetFirstPerson();
    }

    void SetFirstPerson()
    {
        isFirstPerson = true;
        firstPersonCamera.Priority = 20;
        thirdPersonCamera.Priority = 10;

        if (meshDisableCoroutine != null)
            StopCoroutine(meshDisableCoroutine);
        meshDisableCoroutine = StartCoroutine(DelayedDisableMeshRenderers(hideDelay));
    }

    void SetThirdPerson()
    {
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
            }
        }
        meshDisableCoroutine = null;
    }

    void EnableMeshRenderers()
    {
        if (meshRenderers != null)
        {
            foreach (SkinnedMeshRenderer mr in meshRenderers)
            {
                mr.enabled = true;
            }
        }
    }
}
