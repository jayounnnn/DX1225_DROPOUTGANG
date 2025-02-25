using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Generator : MonoBehaviour
{
    public float repairTime = 10f;
    public float baseProgressRate = 1f;
    private float repairProgress = 0f;
    private bool isRepaired = false;
    public Slider progressBar;
    public GameObject quickTimeBox;
    public float quickTimeDuration = 2f;
    public float progressBoost = 2f;
    public float progressPenalty = 1f;
    public float spawnChancePerSecond = 0.1f;
    private bool playerInRange = false;
    private bool isInteracting = false;
    private GameObject interactingPlayer;
    private bool quickTimeActive = false;
    private float quickTimeTimer = 0f;
    public CameraSwitcher cameraSwitcher;

    void Start()
    {
        if (cameraSwitcher == null)
        {
            cameraSwitcher = FindAnyObjectByType<CameraSwitcher>();
        }

        Transform generatorUI = GameObject.Find("Generator UI").transform;
        progressBar = generatorUI.Find("Slider").GetComponent<Slider>();
        quickTimeBox = generatorUI.Find("Image").gameObject;

        if (progressBar != null)
            progressBar.gameObject.SetActive(false);
        if (quickTimeBox != null)
            quickTimeBox.SetActive(false);
    }

    void Update()
    {
        if (isRepaired)
            return;
        if (isInteracting && !Input.GetKey(KeyCode.E))
        {
            StopInteraction();
            return;
        }
        if (!isInteracting && playerInRange && Input.GetKeyDown(KeyCode.E))
            StartInteraction();
        if (isInteracting)
        {
            if (!quickTimeActive)
            {
                repairProgress += baseProgressRate * Time.deltaTime;
                UpdateProgressUI();
                if (repairProgress >= repairTime)
                {
                    CompleteRepair();
                    StopInteraction();
                    return;
                }
                float chanceThisFrame = spawnChancePerSecond * Time.deltaTime;
                if (Random.value < chanceThisFrame)
                    ActivateQuickTime();
            }
            if (quickTimeActive)
            {
                quickTimeTimer += Time.deltaTime;
                UpdateQuickTimeFade();
                if (Input.GetKeyDown(KeyCode.Space))
                    ApplyQuickTimeSuccess();
                else if (quickTimeTimer >= quickTimeDuration)
                    ApplyQuickTimeFailure();
            }
        }
    }

    void StartInteraction()
    {
        isInteracting = true;
        if (progressBar != null)
            progressBar.gameObject.SetActive(true);
        if (cameraSwitcher != null)
            cameraSwitcher.EnterTemporaryFirstPerson();
        if (playerInRange && interactingPlayer != null)
        {
            PlayerController pc = interactingPlayer.GetComponent<PlayerController>();
            if (pc != null)
                pc.enabled = false;
        }
    }

    void StopInteraction()
    {
        isInteracting = false;
        repairProgress = 0f;
        UpdateProgressUI();
        if (progressBar != null)
            progressBar.gameObject.SetActive(false);
        if (interactingPlayer != null)
        {
            PlayerController pc = interactingPlayer.GetComponent<PlayerController>();
            if (pc != null)
                pc.enabled = true;
        }
        if (quickTimeActive)
            DeactivateQuickTime();
        if (cameraSwitcher != null)
            cameraSwitcher.ExitTemporaryFirstPerson();
    }

    void ActivateQuickTime()
    {
        quickTimeActive = true;
        quickTimeTimer = 0f;
        if (quickTimeBox != null)
        {
            quickTimeBox.SetActive(true);
            Image img = quickTimeBox.GetComponent<Image>();
            if (img != null)
            {
                Color c = img.color;
                c.a = 1f;
                img.color = c;
            }
        }
    }

    void ApplyQuickTimeSuccess()
    {
        if (!quickTimeActive)
            return;
        repairProgress += progressBoost;
        UpdateProgressUI();
        DeactivateQuickTime();
        if (repairProgress >= repairTime)
        {
            CompleteRepair();
            StopInteraction();
        }
    }

    void ApplyQuickTimeFailure()
    {
        if (!quickTimeActive)
            return;
        repairProgress = Mathf.Max(0, repairProgress - progressPenalty);
        UpdateProgressUI();
        DeactivateQuickTime();
    }

    void DeactivateQuickTime()
    {
        quickTimeActive = false;
        quickTimeTimer = 0f;
        if (quickTimeBox != null)
            quickTimeBox.SetActive(false);
    }

    void UpdateProgressUI()
    {
        if (progressBar != null)
            progressBar.value = repairProgress / repairTime;
    }

    void CompleteRepair()
    {
        isRepaired = true;
        Debug.Log("Generator fully repaired!");
    }

    void UpdateQuickTimeFade()
    {
        if (quickTimeBox != null)
        {
            Image img = quickTimeBox.GetComponent<Image>();
            if (img != null)
            {
                float alpha = Mathf.Lerp(1f, 0f, quickTimeTimer / quickTimeDuration);
                Color c = img.color;
                c.a = alpha;
                img.color = c;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            interactingPlayer = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (isInteracting)
                StopInteraction();
        }
    }
}
