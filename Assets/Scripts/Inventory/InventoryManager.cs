using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;

    public List<Item> items = new List<Item>();
    public InventorySlot[] inventorySlots;
    [SerializeField] public GameObject inventoryUI;
    [SerializeField] private GameObject flashlightPanel;
    [SerializeField] private GameObject hotbarPanel;
    [SerializeField] private Button flashlightBtn;
    [SerializeField] private Button hotbarBtn;

    private bool isInventoryOpen = false;
    private bool hasTorch = false;

    [SerializeField] public GameObject[] itemPrefabs;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    private void Start()
    {
        // Ensure HotbarPanel is the default panel
        flashlightPanel.SetActive(false);
        hotbarPanel.SetActive(true);

        // Disable FlashlightBtn until a torch is collected
        flashlightBtn.interactable = false;

        // Add button listeners
        flashlightBtn.onClick.AddListener(ShowFlashlightPanel);
        hotbarBtn.onClick.AddListener(ShowHotbarPanel);
    }

    public void AddItem(Item newItem)
    {
        if (newItem.itemType == ItemType.Torch)
        {
            Debug.Log("Torch picked up! Enabling Flashlight Panel.");
            flashlightBtn.interactable = true;
            hasTorch = true;
            return; // Do not add to inventory
        }

        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (inventorySlots[i].transform.childCount == 0)
            {
                GameObject itemObject = new GameObject(newItem.itemName);
                itemObject.transform.SetParent(inventorySlots[i].transform);
                itemObject.AddComponent<Image>().sprite = newItem.icon;
                DraggableItem draggable = itemObject.AddComponent<DraggableItem>();
                draggable.image = itemObject.GetComponent<Image>();

                if (newItem.itemType == ItemType.Consumable)
                {
                    Consumable consumable = itemObject.AddComponent<Consumable>();
                    consumable.item = newItem;
                }

                items.Add(newItem);
                return;
            }
        }
        Debug.Log("Inventory Full!");
    }

    public GameObject GetItemPrefab(string itemName)
    {
        foreach (GameObject prefab in itemPrefabs)
        {
            if (prefab.name == itemName)
                return prefab;
        }
        return null;
    }

    public void ShowFlashlightPanel()
    {
        // Prevent switching if the torch hasn't been collected
        if (!hasTorch) return;
        Debug.Log("Switching to FlashlightPanel");
        flashlightPanel.SetActive(true);
        hotbarPanel.SetActive(false);
    }

    public void ShowHotbarPanel()
    {
        Debug.Log("Switching to HotbarPanel");
        hotbarPanel.SetActive(true);
        flashlightPanel.SetActive(false);
    }

    public void ToggleInventory()
    {
        isInventoryOpen = !isInventoryOpen;
        inventoryUI.SetActive(isInventoryOpen);

        if (isInventoryOpen)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            //Time.timeScale = 0f; // Pause game
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            //Time.timeScale = 1f; // Resume game
        }
    }
}
