using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillBoard : MonoBehaviour
{
    public GameObject worldCanvas;
    private float rotationSpeed = 5f;

    void Start()
    {
        if (worldCanvas != null)
        {
            worldCanvas.SetActive(false);
        }
    }

    void Update()
    {
        if (Camera.main != null)
        {
            // Get the direction to the camera
            Vector3 directionToCamera = Camera.main.transform.position - transform.position;
            directionToCamera.y = 0; // Keep the canvas upright

            // Smoothly rotate towards the camera
            Quaternion targetRotation = Quaternion.LookRotation(-directionToCamera);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    public void SetCanvasActive(bool isActive)
    {
        if (worldCanvas != null)
        {
            worldCanvas.SetActive(isActive);
        }
    }
}
