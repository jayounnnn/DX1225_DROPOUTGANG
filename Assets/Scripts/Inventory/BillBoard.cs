using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillBoard : MonoBehaviour
{
    public GameObject worldCanvas;

    void Start()
    {
        if (worldCanvas != null)
        {
            worldCanvas.SetActive(false);
        }
    }

    void Update()
    {
        // Maintain billboard effect
        Quaternion rotation = Camera.main.transform.rotation;
        transform.LookAt(transform.position + rotation * Vector3.forward, rotation * Vector3.up);
    }

    public void SetCanvasActive(bool isActive)
    {
        if (worldCanvas != null)
        {
            worldCanvas.SetActive(isActive);
        }
    }
}
