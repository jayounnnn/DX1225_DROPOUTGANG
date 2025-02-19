using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class HotbarSlot : MonoBehaviour, IDropHandler
{
    public Image hotbarItemUI; // UI panel that updates when an item is added/removed


    private void Start()
    {
        hotbarItemUI.enabled = false;
    }

    public void OnDrop(PointerEventData eventData)
    {
        // Prevent multiple items in one slot
        if (transform.childCount > 0)
        {
            Debug.Log("Hotbar slot is already occupied!");
            return;
        }

        GameObject dropped = eventData.pointerDrag;
        DraggableItem draggableItem = dropped.GetComponent<DraggableItem>();

        // Ensure the item is not a battery
        Item item = InventoryManager.instance.items.Find(i => i.itemName == dropped.name);
        if (item != null && item.itemType == ItemType.Battery)
        {
            Debug.Log("Batteries cannot be placed in the Hotbar!");
            return;
        }

        // Assign item to slot
        draggableItem.parentAfterDrag = transform;

        // Update the hotbar UI panel
        UpdateHotbarUI(item.icon);
    }

    private void Update()
    {
        if (transform.childCount == 0)
        {
            RemoveItem();
        }
        else
        {
            // Get the first child item (assuming only one item per slot)
            Transform itemInSlot = transform.GetChild(0);
            DraggableItem draggableItem = itemInSlot.GetComponent<DraggableItem>();

            if (draggableItem != null)
            {
                Item item = InventoryManager.instance.items.Find(i => i.itemName == itemInSlot.name);
                if (item != null)
                {
                    UpdateHotbarUI(item.icon);
                }
            }
        }
    }

    private void UpdateHotbarUI(Sprite itemIcon)
    {
        if (hotbarItemUI != null)
        {
            hotbarItemUI.sprite = itemIcon;
            hotbarItemUI.enabled = true; // Show the item
        }

    }

    public void RemoveItem()
    {
        if (hotbarItemUI != null)
        {
            Debug.Log("Removing Item From Hotbar");
            hotbarItemUI.sprite = null;
            hotbarItemUI.enabled = false; // Hide the item
        }
    }
}
