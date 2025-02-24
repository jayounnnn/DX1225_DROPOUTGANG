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

    [SerializeField] private Door door;
    [SerializeField] private GameObject minigamePanel;
    [SerializeField] private ElectricalBox electricalBox;

    [SerializeField] private GameObject wireDragUIPrefab;
    [SerializeField] private GameObject completionMessageUI;
    [SerializeField] private GameObject mismatchMessageUI;
    [SerializeField] private float messageDuration = 2f;
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

            // Create a permanent wire UI connection
            CreateWireConnectionUI(selectedWire);
        }
        else
        {
            ShowMismatchMessage();
        }
        if (activeWireDragUI != null)
        {
            Destroy(activeWireDragUI);
            activeWireDragUI = null;
        }

        selectedWire.isDragging = false;
        selectedWire = null;

        CheckIfAllWiresConnected();
    }


    private Transform GetClosestRightPoint(WireConnection wire, Vector2 screenPosition)
    {
        Transform closest = null;
        float minDistance = Mathf.Infinity;

        foreach (var connection in wireConnections)
        {
            //Allow checking all right points (no longer restricting to same-pair)
            Vector2 rightPointScreenPos = RectTransformUtility.WorldToScreenPoint(null, connection.rightPoint.position);
            float distance = Vector2.Distance(screenPosition, rightPointScreenPos);

            if (distance < minDistance)
            {
                minDistance = distance;
                closest = connection.rightPoint;
            }
        }

        return closest;
    }


    private bool IsMatchingWire(WireConnection wire, Transform rightPoint)
    {
        WireConnection targetWire = wireConnections.Find(w => w.rightPoint == rightPoint);

        if (targetWire != null && wire.leftPoint == targetWire.leftPoint)
        {
            Debug.Log($"Checking wire {wire.wireColorType} with {targetWire.wireColorType}");

            // Directly compare their color types
            if (wire.wireColorType == targetWire.wireColorType)
            {
                Debug.Log("Correct wire connected!");
                if (mismatchMessageUI != null)
                {
                    HideMismatchMessage();
                }
                return true;
            }
        }

        ShowMismatchMessage();
        return false;
    }

    private void CreateWireConnectionUI(WireConnection wire)
    {
        if (wire.leftPoint != null && wire.rightPoint != null)
        {
            // Create a new UI wire connection
            GameObject wireConnectionUI = Instantiate(wireDragUIPrefab, minigamePanel.transform);
            Image uiImage = wireConnectionUI.GetComponent<Image>();

            if (uiImage != null)
            {
                uiImage.color = GetColorFromEnum(wire.wireColorType);
            }

            // Set the wire's position and size
            RectTransform wireRect = wireConnectionUI.GetComponent<RectTransform>();
            Vector2 leftScreenPos = RectTransformUtility.WorldToScreenPoint(null, wire.leftPoint.position);
            Vector2 rightScreenPos = RectTransformUtility.WorldToScreenPoint(null, wire.rightPoint.position);

            RectTransformUtility.ScreenPointToLocalPointInRectangle(minigamePanel.GetComponent<RectTransform>(), leftScreenPos, null, out Vector2 localLeftPos);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(minigamePanel.GetComponent<RectTransform>(), rightScreenPos, null, out Vector2 localRightPos);

            wireRect.anchoredPosition = (localLeftPos + localRightPos) / 2; // Position in between
            wireRect.sizeDelta = new Vector2(Vector2.Distance(localLeftPos, localRightPos), 10); // Set wire width
            wireRect.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(localRightPos.y - localLeftPos.y, localRightPos.x - localLeftPos.x) * Mathf.Rad2Deg);

            Debug.Log($"Created wire UI between {wire.leftPoint.name} and {wire.rightPoint.name}");
        }
    }

    private void ShowMismatchMessage()
    {
        if (mismatchMessageUI != null)
        {
            mismatchMessageUI.SetActive(true);
        }
    }

    private void HideMismatchMessage()
    {
        if (mismatchMessageUI != null)
        {
            mismatchMessageUI.SetActive(false);
        }
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
        //Update UI
        if (mismatchMessageUI != null)
        {
            mismatchMessageUI.SetActive(false);
        }
        if (completionMessageUI != null)
        {
            completionMessageUI.SetActive(true);
        }

        //Open door
        if (door != null)
        {
            door.OpenDoor();
        }
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
