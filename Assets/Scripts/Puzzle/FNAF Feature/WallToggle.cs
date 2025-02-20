using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallToggle : MonoBehaviour
{

    public CameraSwitcher cameraSwitcher;

    public Material firstPersonMaterial;

    public Material thirdPersonMaterial;


    public bool toggleCollider = true;

    private Collider objectCollider;
    private Renderer objectRenderer;

    void Awake()
    {
        objectCollider = GetComponent<Collider>();
        objectRenderer = GetComponent<Renderer>();

        if (cameraSwitcher == null)
        {
            cameraSwitcher = FindObjectOfType<CameraSwitcher>();
        }
    }

    void Update()
    {
        if (cameraSwitcher == null)
            return;

        bool isFirstPerson = cameraSwitcher.IsFirstPerson;

        if (toggleCollider && objectCollider != null)
        {
            objectCollider.enabled = isFirstPerson;
        }

        if (objectRenderer != null)
        {
            if (isFirstPerson)
            {
                if (firstPersonMaterial != null)
                    objectRenderer.material = firstPersonMaterial;
            }
            else
            {
                if (thirdPersonMaterial != null)
                    objectRenderer.material = thirdPersonMaterial;
            }
        }
    }
}
