using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Torch : MonoBehaviour
{
    public GameObject flashlightPanel;

    public void ActivateFlashlightPanel()
    {
        if (flashlightPanel != null)
        {
            //Activate flashlight UI in inventory
            flashlightPanel.SetActive(true); 
            Debug.Log("Flashlight Panel Activated!");
        }
        else
        {
            Debug.LogWarning("Flashlight Panel is not assigned in Torch script!");
        }
    }
}
