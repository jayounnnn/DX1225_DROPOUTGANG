using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ToggleFog : MonoBehaviour
{
    public ScriptableRendererFeature fullScreenPassFeature;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            if (fullScreenPassFeature != null)
            {
                fullScreenPassFeature.SetActive(!fullScreenPassFeature.isActive);
                Debug.Log("Full Screen Pass Renderer Feature: " + (fullScreenPassFeature.isActive ? "Enabled" : "Disabled"));
            }
            else
            {
                Debug.LogWarning("FullScreenPass Renderer Feature is not assigned.");
            }
        }
    }
}
