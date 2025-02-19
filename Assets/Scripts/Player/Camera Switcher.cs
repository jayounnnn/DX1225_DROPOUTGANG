using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{

    public CinemachineVirtualCamera firstPersonCamera;

    public CinemachineVirtualCamera thirdPersonCamera;

    public GameObject characterModel;

    private MeshRenderer[] meshRenderers;

    private bool isFirstPerson = false;

    void Start()
    {
        if (characterModel != null)
            meshRenderers = characterModel.GetComponentsInChildren<MeshRenderer>();

        SetThirdPerson();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            ToggleCamera();
        }
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

        if (meshRenderers != null)
        {
            foreach (MeshRenderer mr in meshRenderers)
            {
                mr.enabled = false;
            }
        }
    }

    void SetThirdPerson()
    {
        isFirstPerson = false;
        firstPersonCamera.Priority = 10;
        thirdPersonCamera.Priority = 20;

        if (meshRenderers != null)
        {
            foreach (MeshRenderer mr in meshRenderers)
            {
                mr.enabled = true;
            }
        }
    }
}
