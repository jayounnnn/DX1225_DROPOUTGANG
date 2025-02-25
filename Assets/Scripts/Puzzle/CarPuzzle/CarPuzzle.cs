using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CarPuzzle : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Slider fuelSlider;
    [SerializeField] private GameObject fuelSlot;
    [SerializeField] private GameObject wrenchSlot;
    [SerializeField] private GameObject[] boltImages;
    [SerializeField] private GameObject completionTextBox;
    [SerializeField] private TMP_Text fuelPercentageText;

    [Header("Puzzle Settings")]
    [SerializeField] private float fuelFillRate = 0.04f;
    [SerializeField] private float boltPenalty = 0.04f;
    [SerializeField] private float minBoltLooseTime = 3f;
    [SerializeField] private float maxBoltLooseTime = 5f;

    [Header("Draggable Items")]
    [SerializeField] private GameObject draggableGasolinePrefab;
    [SerializeField] private GameObject draggableWrenchPrefab;
    [SerializeField] private Transform draggableItemsContainer;

    private int looseBolts = 0;
    private bool isFueling = false;
    private bool isPuzzleComplete = false;

    private void Start()
    {
        fuelSlider.value = 0;
        if (completionTextBox != null)
        {
            completionTextBox.SetActive(false); // Hide textbox initially
        }
        UpdateFuelPercentage();
    }

    public void OpenCarPuzzle()
    {
        //if draggableItemsContainer has no Children, spawn DraggableItem
        if (draggableItemsContainer.childCount == 0)
        {
            SpawnDraggableItem(draggableGasolinePrefab);
            SpawnDraggableItem(draggableWrenchPrefab);
        }
        if(isFueling && !isPuzzleComplete)
        {
            StartCoroutine(LoosenBoltsRoutine());
        }
    }

    private void SpawnDraggableItem(GameObject itemPrefab)
    {
        if (itemPrefab != null && draggableItemsContainer != null)
        {
            Instantiate(itemPrefab, draggableItemsContainer);
        }
    }

    private void Update()
    {
        if (isFueling)
        {
            float currentFillRate = fuelFillRate - (looseBolts * boltPenalty);
            fuelSlider.value += currentFillRate * Time.deltaTime;
            UpdateFuelPercentage();

            if (fuelSlider.value >= 1)
            {
                CompletePuzzle();
            }
        }
    }

    private IEnumerator LoosenBoltsRoutine()
    {
        while (!isPuzzleComplete)
        {
            yield return new WaitForSeconds(Random.Range(minBoltLooseTime, maxBoltLooseTime));

            if (looseBolts < boltImages.Length)
            {
                boltImages[looseBolts].SetActive(true);
                looseBolts++;
                Debug.Log($"A bolt has loosened! Loose bolts: {looseBolts}");
            }
        }
    }

    private void UpdateFuelPercentage()
    {
        if (fuelPercentageText != null)
        {
            int fuelPercent = Mathf.RoundToInt(fuelSlider.value * 100);
            fuelPercentageText.text = $"Fuel: {fuelPercent}%";
        }
    }

    public void StartFueling()
    {
        if (!isPuzzleComplete)
        {
            isFueling = true;
            Debug.Log("Fueling started...");
            StartCoroutine(LoosenBoltsRoutine());
        }
    }

    public void FixBolts()
    {
        if (looseBolts > 0)
        {
            boltImages[looseBolts - 1].SetActive(false);
            looseBolts--;
            Debug.Log($"Fixed a bolt! Remaining loose bolts: {looseBolts}");
        }
    }

    private void CompletePuzzle()
    {
        isPuzzleComplete = true;
        isFueling = false;
        Debug.Log("Car refueled and repaired!");

        if (completionTextBox != null)
        {
            completionTextBox.SetActive(true);
        }
    }    
}
