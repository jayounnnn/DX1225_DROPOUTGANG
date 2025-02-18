using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;

    public List<Item> items = new List<Item>();
    public InventorySlot[] inventorySlots;
    [SerializeField] private GameObject inventoryUI;
    private bool isInventoryOpen = false;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public void AddItem(Item newItem)
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (inventorySlots[i].transform.childCount == 0)
            {
                GameObject itemObject = new GameObject(newItem.itemName);
                itemObject.transform.SetParent(inventorySlots[i].transform);
                if (newItem.icon != null)
                    itemObject.AddComponent<Image>().sprite = newItem.icon;
                else
                    Debug.Log("Sprite has no icon");
                Debug.Log("Updated Inv UI with " + newItem.itemName + " At slot " + i);
                DraggableItem draggable = itemObject.AddComponent<DraggableItem>();
                draggable.image = itemObject.GetComponent<Image>();

                items.Add(newItem);
                return;
            }
        }
        Debug.Log("Inventory Full!");
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
