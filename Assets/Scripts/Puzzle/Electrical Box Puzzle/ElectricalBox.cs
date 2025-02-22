using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElectricalBox : MonoBehaviour
{
    [SerializeField] private GameObject minigameUIPanel;
    [SerializeField] private GameObject hotbarPanel;
    private bool isPlayerNearby = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            Debug.Log("Player is near the Electrical Box. Press E to interact.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            Debug.Log("Player left the Electrical Box.");
        }
    }

    private void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
        {
            OpenMinigame();
        }
    }

    public void OpenMinigame()
    {
        if (minigameUIPanel != null)
        {
            minigameUIPanel.SetActive(true);
            hotbarPanel.SetActive(false);
            Debug.Log("Minigame UI Opened.");
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Debug.LogWarning("Minigame UI Panel is not assigned in ElectricalBox!");
        }
    }

    public void CloseMinigame()
    {
        if (minigameUIPanel != null)
        {
            minigameUIPanel.SetActive(false);
            hotbarPanel.SetActive(true);
            Debug.Log("Minigame UI Closed.");
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
