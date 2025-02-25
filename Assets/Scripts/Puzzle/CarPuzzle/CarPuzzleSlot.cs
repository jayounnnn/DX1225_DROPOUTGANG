using UnityEngine;
using UnityEngine.EventSystems;

public class CarPuzzleSlot : MonoBehaviour, IDropHandler
{
    public enum SlotType { Fuel, Wrench }
    public SlotType slotType;

    private CarPuzzle carPuzzle;

    private void Start()
    {
        carPuzzle = FindObjectOfType<CarPuzzle>();
    }

    public void OnDrop(PointerEventData eventData)
    {
        GameObject droppedItem = eventData.pointerDrag;
        if (droppedItem != null)
        {
            DraggableItem draggableItem = droppedItem.GetComponent<DraggableItem>();

            if (draggableItem != null)
            {
                if (slotType == SlotType.Fuel && droppedItem.name.Contains("Gasoline"))
                {
                    Debug.Log("Fuel inserted! Refueling started...");
                    carPuzzle.StartFueling();
                    Destroy(droppedItem); //    Remove the draggable fuel item after use
                }
                else if (slotType == SlotType.Wrench && droppedItem.name.Contains("Wrench"))
                {
                    Debug.Log("Wrench inserted! Bolts fixed.");
                    carPuzzle.FixBolts();
                }
                else
                {
                    Debug.Log("Wrong item for this slot!");
                }
            }
        }
    }

}