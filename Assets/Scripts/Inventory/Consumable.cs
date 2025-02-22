using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Consumable : MonoBehaviour
{
    public Item item;
    public bool isInHotbar = false;
    private GameObject rockPrefab;

    private void Start()
    {
        HotbarSlot[] hotbarSlots = FindObjectsOfType<HotbarSlot>();
        foreach (HotbarSlot slot in hotbarSlots)
        {
            if (slot.transform == transform.parent)
            {
                isInHotbar = true;
                break;
            }
        }
        // Load the rock prefab from assets if the item is a Rock
        if (item.itemName == "RockItem")
        {
            rockPrefab = InventoryManager.instance.GetItemPrefab("Rock");
            if (rockPrefab == null)
            {
                Debug.LogError("Rock prefab not found in InventoryManager!");
            }
        }
    }

    public void UseConsumable()
    {
        if (!isInHotbar)
        {
            Debug.Log("Consumable items can only be used from the hotbar!");
            return;
        }

        if (item.itemName == "Medkit")
        {
            PlayerController player = FindObjectOfType<PlayerController>();
            if (player != null)
            {
                Damageable damageable = player.GetComponent<Damageable>();
                if (damageable != null)
                {
                    damageable.Heal(20f); // Heal 20 HP
                    Debug.Log("Used Medkit! Player healed by 20.");
                }
            }
        }
        else if (item.itemName == "RockItem")
        {
            ThrowRock();
        }

        Destroy(gameObject);
    }

    private void ThrowRock()
    {
        if (rockPrefab == null)
        {
            Debug.LogError("Rock prefab is not assigned in InventoryManager!");
            return;
        }

        PlayerController player = FindObjectOfType<PlayerController>();
        if (player == null) return;

        Vector3 spawnPosition = player.transform.position + player.transform.forward + Vector3.up * 1.5f;
        GameObject rock = Instantiate(rockPrefab, spawnPosition, Quaternion.identity);

        Rock rockScript = rock.GetComponent<Rock>();
        if (rockScript == null)
        {
            Debug.LogError("Rock script is missing on Rock prefab!");
            return;
        }
        rockScript.Initialize(player.transform.position, player.transform.forward);

        Debug.Log("Threw a rock!");
    }
}
//Add this to Player controller
//public HotbarSlot[] hotbarSlots;
//if (_inputActions["UseItem1"].WasPressedThisFrame())
//{
//    UseHotbarItem(0);
//}
//if (_inputActions["UseItem2"].WasPressedThisFrame())
//{
//    UseHotbarItem(1);
//}
//
//private void UseHotbarItem(int slotIndex)
//{
//    // Prevent OutOfBounds exception
//    if (hotbarSlots == null || slotIndex >= hotbarSlots.Length)
//    {
//        Debug.LogError("Hotbar slot " + slotIndex + " is out of range! Make sure all hotbar slots are assigned.");
//        return;
//    }

//    if (hotbarSlots[slotIndex].transform.childCount > 0) // Check if slot has an item
//    {
//        Transform itemInSlot = hotbarSlots[slotIndex].transform.GetChild(0);
//        Consumable consumable = itemInSlot.GetComponent<Consumable>();

//        if (consumable != null)
//        {
//            Debug.Log("Using consumable from Hotbar Slot " + (slotIndex + 1));
//            consumable.UseConsumable();
//            Update UI after consuming the item
//            hotbarSlots[slotIndex].RemoveItem();
//        }
//        else
//        {
//            Debug.Log("Item in Hotbar Slot " + (slotIndex + 1) + " is not a consumable.");
//        }
//    }
//    else
//    {
//        Debug.Log("Hotbar Slot " + (slotIndex + 1) + " is empty.");
//    }
//}