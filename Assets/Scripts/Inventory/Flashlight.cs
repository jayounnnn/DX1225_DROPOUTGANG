using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashlight : MonoBehaviour
{
    [SerializeField] private BatterySlot[] batterySlots;
    public Light flashlight;
    public float batteryDrainRate = 2f;
    private bool isOn = false;
    private int currBatteryIndex = -1;

    private void Start()
    {
        flashlight.enabled = false;
    }

    public void ToggleFlashlight()
    {
        if (!HasBattery()) // Prevent activation if no battery is inserted
        {
            Debug.Log("No battery! Flashlight cannot be turned on.");
            return;
        }

        isOn = !isOn;
        flashlight.enabled = isOn;

        if (isOn)
        {
            StartCoroutine(DrainBattery());
        }
    }

    private IEnumerator DrainBattery()
    {
        while (isOn && HasBattery())
        {
            BatterySlot currentBattery = GetCurrentBatterySlot();
            if (currentBattery != null)
            {
                Debug.Log("draining from battery" + currentBattery);
                currentBattery.DrainBattery(batteryDrainRate * Time.deltaTime);

                // If battery is depleted, move to the next battery
                if (!currentBattery.HasBattery())
                {
                    RemoveDepletedBattery(currentBattery);
                    GetNextAvailableBatterySlot();
                }
            }
            yield return null;
        }

        // If no battery left, turn off flashlight
        flashlight.enabled = false;
        isOn = false;
    }

    private bool HasBattery()
    {
        foreach (var slot in batterySlots)
        {
            if (slot.HasBattery())
            {
                return true;
            }
        }
        return false;
    }

    private BatterySlot GetCurrentBatterySlot()
    {
        // If no battery is currently selected, find the first available one
        if (currBatteryIndex == -1 || !batterySlots[currBatteryIndex].HasBattery())
        {
            return GetNextAvailableBatterySlot();
        }

        return batterySlots[currBatteryIndex];
    }

    private BatterySlot GetNextAvailableBatterySlot()
    {
        for (int i = 0; i < batterySlots.Length; i++)
        {
            if (batterySlots[i].HasBattery())
            {
                currBatteryIndex = i;
                return batterySlots[i];
            }
        }
        // No battery available
        currBatteryIndex = -1; 
        return null;
    }

    private void RemoveDepletedBattery(BatterySlot slot)
    {
        slot.RemoveBattery(slot); // Pass the specific slot to remove its battery
    }
}
//Add to playerController
//if (_inputActions["ToggleFlashlight"].WasPressedThisFrame())
//{
//    if (flashlight != null)
//    {
//        flashlight.ToggleFlashlight();
//    }
//}
