using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupItem : MonoBehaviour
{
    public string QuestID;
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
        CollectItem(QuestID);
        Destroy(gameObject);
    }

    private void CollectItem(string name)
    {
        QuestManager questManager = FindObjectOfType<QuestManager>();

        if (questManager != null)
        {
            foreach (Quest quest in questManager.activeQuests)
            {
                foreach (QuestObjective objective in quest.objectives)
                {
                    if (objective is CollectItemObjective collectObjective && collectObjective.itemName == name)
                    {
                        collectObjective.AddItem(name, 1);
                        Debug.Log("Collected " + 1 + " of " + name);
                        return;
                    }
                }
            }
        }
    }
}