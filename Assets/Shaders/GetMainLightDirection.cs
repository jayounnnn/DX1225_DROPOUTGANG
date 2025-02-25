using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class GetMainLightDirection : MonoBehaviour
{
    [SerializeField] private Material skyboxMaterial;
    [SerializeField] private float dayCycleSpeed = 10f; // Speed of day/night cycle

    private void Update()
    {
        //RotateLight();

        skyboxMaterial.SetVector("_MainLightDirection", transform.forward);
    }

    private void RotateLight()
    {
        float rotationAngle = dayCycleSpeed * Time.deltaTime;

        Quaternion incrementalRotation = Quaternion.Euler(rotationAngle, 0, 0);

        transform.rotation = transform.rotation * incrementalRotation;
    }
}
