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

        rockScript.Initialize(player.transform.position , player.transform.forward);

        Debug.Log("Threw a rock!");
    }
}