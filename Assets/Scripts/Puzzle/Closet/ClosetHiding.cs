using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Closet : MonoBehaviour
{
    public GameObject player;
    public Transform hidingSpot;
    public Transform exitSpot;
    public KeyCode interactKey = KeyCode.E;
    public float interactionDistance = 2f;
    public float facingThreshold = 0.5f;
    public float exitDelay = 1.0f;
    public Vector3 hidingCameraRotation = new Vector3(0, 0, 0);

    private CameraSwitcher cameraSwitcher;
    private PlayerController playerController;
    private bool isHiding = false;
    private bool _wasInFirstPerson = false;
    private bool canExit = false;

    void Start()
    {
        if (player != null)
        {
            cameraSwitcher = player.GetComponentInChildren<CameraSwitcher>();
            playerController = player.GetComponent<PlayerController>();
        }
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.transform.position);
        if (distance <= interactionDistance)
        {
            Vector3 closetForward = transform.forward;
            Vector3 toPlayer = (player.transform.position - transform.position).normalized;
            float dot = Vector3.Dot(closetForward, toPlayer);

            if (dot >= facingThreshold && Input.GetKeyDown(interactKey))
            {
                if (!isHiding)
                    EnterCloset();
            }
        }

        if (isHiding && canExit && Input.GetKeyDown(interactKey))
        {
            ExitCloset();
        }
    }

    void EnterCloset()
    {
        DisablePlayerCollisions();
        StoreCameraState();

        if (playerController != null)
            playerController.enabled = false;

        if (hidingSpot != null)
        {
            player.transform.position = hidingSpot.position;
            player.transform.rotation = hidingSpot.rotation;
            cameraSwitcher.SetFixedCameraRotation(transform.forward);
        }
        else
        {
            Debug.LogWarning("Closet: Hiding spot not assigned.");
        }

        isHiding = true;
        canExit = false;

        StartCoroutine(EnableExitAfterDelay());
    }

    IEnumerator EnableExitAfterDelay()
    {
        yield return new WaitForSeconds(exitDelay);
        canExit = true;
    }

    void ExitCloset()
    {
        if (!isHiding) return;

        if (exitSpot != null)
        {
            player.transform.position = exitSpot.position;
            player.transform.rotation = exitSpot.rotation;
        }
        else
        {
            Debug.LogWarning("Closet: Exit spot not assigned.");
        }

        EnablePlayerCollisions();
        RestoreCameraState();

        if (playerController != null)
            playerController.enabled = true;

        isHiding = false;
    }

    void DisablePlayerCollisions()
    {
        if (player.TryGetComponent(out CapsuleCollider capCol))
            capCol.enabled = false;

        if (player.TryGetComponent(out CharacterController cc))
            cc.enabled = false;
    }

    void EnablePlayerCollisions()
    {
        if (player.TryGetComponent(out CapsuleCollider capCol))
            capCol.enabled = true;

        if (player.TryGetComponent(out CharacterController cc))
            cc.enabled = true;
    }

    void StoreCameraState()
    {
        if (cameraSwitcher != null)
        {
            _wasInFirstPerson = cameraSwitcher.IsFirstPerson;
            if (!_wasInFirstPerson)
                cameraSwitcher.SetFirstPerson();
        }
    }

    void RestoreCameraState()
    {
        if (cameraSwitcher != null)
        {
            if (_wasInFirstPerson)
                cameraSwitcher.SetFirstPerson();
            else
                cameraSwitcher.SetThirdPerson();
        }
    }
}
