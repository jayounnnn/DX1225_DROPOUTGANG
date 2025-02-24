using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallToggle : MonoBehaviour
{
    public CameraSwitcher cameraSwitcher;
    public Material firstPersonMaterial;
    public Material thirdPersonMaterial;

    public enum ColliderMode { FirstPerson, ThirdPerson }
    public ColliderMode NocolliderMode = ColliderMode.FirstPerson;

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

        if (objectCollider != null)
        {
            objectCollider.enabled = (NocolliderMode == ColliderMode.FirstPerson) ? !isFirstPerson : isFirstPerson;
        }

        if (objectRenderer != null)
        {
            objectRenderer.material = isFirstPerson ? firstPersonMaterial : thirdPersonMaterial;
        }
    }
}
