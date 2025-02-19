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
    private bool isInventoryOpen = false;

    [SerializeField] public GameObject[] itemPrefabs;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public void AddItem(Item newItem)
    {
        if (newItem.itemType == ItemType.Torch)
        {
            Debug.Log("Torch picked up! Enabling Flashlight Panel.");
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
        return null; // Return null if the prefab is not found
    }

    public void ToggleInventory()
    {
        isInventoryOpen = !isInventoryOpen;
        inventoryUI.SetActive(isInventoryOpen);

        if (isInventoryOpen)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Time.timeScale = 0f; // Pause game
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Time.timeScale = 1f; // Resume game
        }
    }
}
