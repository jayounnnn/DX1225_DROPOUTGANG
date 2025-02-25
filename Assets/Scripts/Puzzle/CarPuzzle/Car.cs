using UnityEngine;

public class Car : MonoBehaviour
{
    [SerializeField] private GameObject carPuzzleUI;
    private bool playerInRange = false;

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (PlayerHasRequiredItems())
            {
                ActivateCarPuzzle();
            }
            else
            {
                Debug.Log("You need both a Wrench and Gasoline in your hotbar to start the puzzle!");
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            Debug.Log("Press 'E' to start Car Puzzle!");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    private bool PlayerHasRequiredItems()
    {
        HotbarSlot[] hotbarSlots = FindObjectsOfType<HotbarSlot>(true);
        bool hasWrench = false;
        bool hasGasoline = false;

        foreach (HotbarSlot slot in hotbarSlots)
        {
            if (slot.transform.childCount > 0)
            {
                Transform itemInSlot = slot.transform.GetChild(0);
                Item item = InventoryManager.instance.items.Find(i => i.itemName == itemInSlot.name);

                if (item != null)
                {
                    if (item.itemName == "Wrench") {
                        Debug.Log("Has Wrench");
                        hasWrench = true;
                    }
                    if (item.itemName == "Gasoline") {
                        Debug.Log("Has Gasoline");
                        hasGasoline = true;
                    }
                    
                }
            }
        }

        return hasWrench && hasGasoline;
    }

    private void ActivateCarPuzzle()
    {
        if (carPuzzleUI != null)
        {
            carPuzzleUI.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            //Time.timeScale = 0f;
            CarPuzzle carPuzzle = carPuzzleUI.GetComponent<CarPuzzle>();
            if (carPuzzle != null)
            {
                carPuzzle.OpenCarPuzzle();
            }
            Debug.Log("Car Puzzle Activated!");
        }
    }

    public void CloseMinigame()
    {
        if (carPuzzleUI != null)
        {
            carPuzzleUI.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            //Time.timeScale = 1f;
            Debug.Log("Car Puzzle Closed!");
        }
    }
}
