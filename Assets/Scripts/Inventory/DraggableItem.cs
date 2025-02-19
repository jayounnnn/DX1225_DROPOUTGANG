using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] public Image image;
    [HideInInspector] public Transform parentAfterDrag;
    private Canvas canvas;
    private RectTransform inventoryRect;
    private Item itemData;

    void Start()
    {
        canvas = FindObjectOfType<Canvas>();
        inventoryRect = InventoryManager.instance.inventoryUI.GetComponent<RectTransform>();
        itemData = InventoryManager.instance.items.Find(i => i.itemName == gameObject.name);
    }

    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("Begin Dragging Item");
        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
        image.raycastTarget = false;
    }
    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("Dragging Item");
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Check if dropped outside inventory UI
        if (!IsInsideInventory(eventData.position)) 
        {
            DropItemInWorld();
        }
        else
        {
            transform.SetParent(parentAfterDrag);
        }

        image.raycastTarget = true;
    }

    private bool IsInsideInventory(Vector2 screenPosition)
    {
        return RectTransformUtility.RectangleContainsScreenPoint(inventoryRect, screenPosition);
    }

    private void DropItemInWorld()
    {
        Debug.Log("Dropping item: " + gameObject.name);
        Transform playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        if (playerTransform != null && itemData != null)
        {
            Vector3 dropPosition = playerTransform.position + playerTransform.forward * 1.5f + playerTransform.up; // Drop in front of player

            // Get prefab from InventoryManager's array
            GameObject prefab = InventoryManager.instance.GetItemPrefab(itemData.itemName);
            if (prefab != null)
            {
                Instantiate(prefab, dropPosition, Quaternion.identity);
                InventoryManager.instance.items.Remove(itemData); // Remove from inventory list
                Destroy(gameObject); // Remove from UI
            }
            else
            {
                Debug.LogError("Prefab not found for: " + itemData.itemName);
            }
        }
    }
}
