using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewQuest", menuName = "Quests/Quest")]
public class Quest : ScriptableObject
{
    public string questName;
    [TextArea]
    public string questDescription;
    public List<QuestObjective> objectives;

    public Quest nextQuest; 
    public bool IsQuestCompleted()
    {
        foreach (var objective in objectives)
        {
            if (!objective.CheckCompletion())
                return false;
        }
        return true;
    }
}
