using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BatterySlot : MonoBehaviour, IDropHandler
{
    public Slider batterySlider;
    private Item batteryItem;
    private GameObject currentBatteryObject;

    private void Start()
    {
        batterySlider.gameObject.SetActive(false);
    }

    public void OnDrop(PointerEventData eventData)
    {
        GameObject dropped = eventData.pointerDrag;
        DraggableItem draggableItem = dropped.GetComponent<DraggableItem>();

        Item item = InventoryManager.instance.items.Find(i => i.itemName == dropped.name);
        if (item != null && item.itemType == ItemType.Battery)
        {
            // Place in slot and set up UI
            draggableItem.parentAfterDrag = transform;
            batteryItem = item;
            currentBatteryObject = dropped;
            batterySlider.gameObject.SetActive(true);

            batteryItem.batteryLife = 100f;
            batterySlider.value = 0f;
        }
        else
        {
            Debug.Log("Only batteries can be placed in the Battery Slot!");
        }
    }

    public void DrainBattery(float drainAmount)
    {
        if (batteryItem != null)
        {
            batteryItem.batteryLife -= drainAmount;
            batterySlider.value = 1f - (batteryItem.batteryLife / 100f); // Fill up as battery drains

            if(batteryItem.batteryLife <= 0f)
            {
                RemoveBattery(this);
            }
        }
    }

    public bool HasBattery()
    {
        return batteryItem != null;
    }

    public void RemoveBattery(BatterySlot slot)
    {
        if (slot.batteryItem == null) return; // Prevent multiple removals

        Debug.Log("Battery depleted! Removing from slot: " + slot.name);
        //Reset battery life
        slot.batteryItem.batteryLife = 100f;
        slot.batterySlider.value = 0f;
        slot.batteryItem = null;
        slot.batterySlider.gameObject.SetActive(false);

        if (slot.currentBatteryObject != null)
        {
            Destroy(slot.currentBatteryObject); // Remove the battery from UI
            slot.currentBatteryObject = null;
        }

    }
}
