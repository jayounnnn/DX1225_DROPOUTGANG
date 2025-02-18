using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class QuestObjective : ScriptableObject
{
    [TextArea]
    public string description;
    public bool isCompleted;

    public abstract bool CheckCompletion();
}
