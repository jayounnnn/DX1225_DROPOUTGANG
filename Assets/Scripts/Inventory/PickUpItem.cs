using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupItem : MonoBehaviour
{
    public Item item;
    public bool isPlayerNearby = false;
    private BillBoard billBoard;

    private void Start()
    {
        billBoard = GetComponentInChildren<BillBoard>();
        if (billBoard != null)
        {
            billBoard.SetCanvasActive(false); 
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            if (billBoard != null)
            {
                // Show world canvas
                billBoard.SetCanvasActive(true); 
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            if (billBoard != null)
            {
                // Hide world canvas
                billBoard.SetCanvasActive(false); 
            }
        }
    }

    public void PickUp()
    {
        Debug.Log("Picking up item: " + item.itemName);

        //Add Item to Inventory
        InventoryManager.instance.AddItem(item);
        Destroy(gameObject);
    }
}