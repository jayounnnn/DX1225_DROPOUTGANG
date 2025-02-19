using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class QuestObjective : ScriptableObject, IResettable
{
    [TextArea]
    public string description;
    public bool isCompleted;

    public abstract bool CheckCompletion();

    public virtual void ResetData()
    {
        isCompleted = false;
    }
}
