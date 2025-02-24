using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ElectricalMinigame : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [System.Serializable]
    public class WireConnection
    {
        public WireColorType wireColorType;
        public Transform leftPoint;
        public Transform rightPoint;
        public bool isConnected = false;
        public bool isDragging = false;
    }

    public List<WireConnection> wireConnections;
    private WireConnection selectedWire;

    public GameObject minigamePanel;
    public ElectricalBox electricalBox;

    public GameObject wireDragUIPrefab; // UI panel prefab for dragging wire
    private GameObject activeWireDragUI; // Currently active UI panel

    void Start()
    {
        ResetMinigame();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        foreach (var wire in wireConnections)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(wire.leftPoint.GetComponent<RectTransform>(), eventData.position))
            {
                selectedWire = wire;
                wire.isDragging = true;
                Debug.Log("Dragging wire: " + wire.wireColorType);

                // Create and set up UI panel for wire dragging
                activeWireDragUI = Instantiate(wireDragUIPrefab, minigamePanel.transform);
                Image uiImage = activeWireDragUI.GetComponent<Image>();

                if (uiImage != null)
                {
                    // Assign color based on enum
                    uiImage.color = GetColorFromEnum(wire.wireColorType);
                }
                else
                {
                    Debug.LogError("WireDragUIPrefab does not have an Image component!");
                }

                // Ensure visibility settings
                RectTransform rectTransform = activeWireDragUI.GetComponent<RectTransform>();
                rectTransform.sizeDelta = new Vector2(80, 80); // Ensure it is visible

                // Convert the wire's start position to UI space
                Vector2 wireStartScreenPos = RectTransformUtility.WorldToScreenPoint(null, wire.leftPoint.position);
                RectTransformUtility.ScreenPointToLocalPointInRectangle(minigamePanel.GetComponent<RectTransform>(), wireStartScreenPos, null, out Vector2 localWireStartPos);
                rectTransform.anchoredPosition = localWireStartPos; // Start at the wire's initial position

                return;
            }
        }
    }



    public void OnDrag(PointerEventData eventData)
    {
        if (selectedWire != null && selectedWire.isDragging && activeWireDragUI != null)
        {
            RectTransform rectTransform = activeWireDragUI.GetComponent<RectTransform>();
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(minigamePanel.GetComponent<RectTransform>(), eventData.position, null, out localPoint);
            rectTransform.anchoredPosition = localPoint; // Move with the mouse
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (selectedWire == null) return;

        Transform closestRightPoint = GetClosestRightPoint(selectedWire, eventData.position);

        if (closestRightPoint != null && IsMatchingWire(selectedWire, closestRightPoint))
        {
            selectedWire.isConnected = true;
            selectedWire.rightPoint = closestRightPoint;

            // Snap UI position to the correct right point
            RectTransform wireRect = selectedWire.rightPoint.GetComponent<RectTransform>();
            if (wireRect != null)
            {
                wireRect.anchoredPosition = closestRightPoint.GetComponent<RectTransform>().anchoredPosition;
            }
        }

        selectedWire.isDragging = false;
        selectedWire = null;

        // Destroy UI dragging panel
        if (activeWireDragUI != null)
        {
            Destroy(activeWireDragUI);
            activeWireDragUI = null;
        }

        CheckIfAllWiresConnected();
    }

    private Transform GetClosestRightPoint(WireConnection wire, Vector2 screenPosition)
    {
        Transform closest = null;
        float minDistance = Mathf.Infinity;

        foreach (var connection in wireConnections)
        {
            //if (connection.leftPoint == wire.leftPoint) continue; // Skip same-side wires

            // Only consider rightPoints with the same wire color
            if (connection.wireColorType != wire.wireColorType) continue;

            // Convert world positions to UI screen positions
            Vector2 rightPointScreenPos = RectTransformUtility.WorldToScreenPoint(null, connection.rightPoint.position);
            float distance = Vector2.Distance(screenPosition, rightPointScreenPos);

            if (distance < minDistance)
            {
                minDistance = distance;
                closest = connection.rightPoint;
            }
        }

        if (closest != null)
        {
            Debug.Log($"Found closest right point for {wire.wireColorType}");
        }
        else
        {
            Debug.LogError($"No matching right point found for {wire.wireColorType}!");
        }

        return closest;
    }


    private bool IsMatchingWire(WireConnection wire, Transform rightPoint)
    {
        WireConnection targetWire = wireConnections.Find(w => w.rightPoint == rightPoint);

        if (targetWire != null)
        {
            Debug.Log($"Checking wire {wire.wireColorType} with {targetWire.wireColorType}");

            // Directly compare their color types
            if (wire.wireColorType == targetWire.wireColorType)
            {
                Debug.Log("Correct wire connected!");
                return true;
            }
        }

        Debug.Log("Incorrect wire connection!");
        return false;
    }

    private Color GetColorFromEnum(WireColorType wireColorType)
    {
        switch (wireColorType)
        {
            case WireColorType.Red: return Color.red;
            case WireColorType.Yellow: return Color.yellow;
            case WireColorType.Green: return Color.green;
            case WireColorType.Blue: return Color.blue;
            default: return Color.white; // Fallback color
        }
    }


    private void CheckIfAllWiresConnected()
    {
        foreach (var wire in wireConnections)
        {
            if (!wire.isConnected) return;
        }

        Debug.Log("All wires connected! Minigame complete.");
        CloseMinigame();
    }

    public void ResetMinigame()
    {
        foreach (var wire in wireConnections)
        {
            wire.isConnected = false;
        }
    }

    public void CloseMinigame()
    {
        if (electricalBox != null)
        {
            electricalBox.CloseMinigame();
        }
    }
}
public enum WireColorType
{
    Red,
    Yellow,
    Green,
    Blue
}
