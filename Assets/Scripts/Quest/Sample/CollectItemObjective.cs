using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CollectItemObjective", menuName = "Quests/Objectives/Collect Item")]
public class CollectItemObjective : QuestObjective
{
    public int requiredAmount = 5;
    public int currentAmount = 0;
    public void AddItem(int amount = 1)
    {
        currentAmount += amount;
        if (currentAmount >= requiredAmount)
        {
            isCompleted = true;
        }
    }

    public override bool CheckCompletion()
    {
        return isCompleted;
    }
}