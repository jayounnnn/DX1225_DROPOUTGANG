using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CollectItemObjective", menuName = "Quests/Objectives/Collect Item")]
public class CollectItemObjective : QuestObjective
{
    public string itemName;
    public int requiredAmount = 5;
    public int currentAmount = 0;
    public void AddItem(string item, int amount = 1)
    {
        if (item == itemName)
        {
            currentAmount += amount;
            if (currentAmount >= requiredAmount)
            {
                isCompleted = true;
            }
        }  
    }

    public override bool CheckCompletion()
    {
        return isCompleted;
    }

    public override void ResetData()
    {
        base.ResetData();
        currentAmount = 0;
    }
}