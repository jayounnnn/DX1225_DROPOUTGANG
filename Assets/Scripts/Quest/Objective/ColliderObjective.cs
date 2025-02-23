using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ColliderObjective", menuName = "Quests/Objectives/Collider Objective")]
public class ColliderObjective : QuestObjective
{
    public string colliderName;

    public void CompleteObjective()
    {
        isCompleted = true;
    }

    public override bool CheckCompletion()
    {
        return isCompleted;
    }

    public override void ResetData()
    {
        base.ResetData();
    }
}
