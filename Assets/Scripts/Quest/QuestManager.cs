using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public List<Quest> activeQuests;

    void Update()
    {
        List<Quest> questsToCheck = new List<Quest>(activeQuests);

        foreach (var quest in questsToCheck)
        {
            if (quest.IsQuestCompleted())
            {
                Debug.Log("Quest Completed: " + quest.questName);

                ResetQuest(quest);

                if (quest.nextQuest != null)
                {
                    if (!activeQuests.Contains(quest.nextQuest))
                    {
                        Debug.Log("New Quest Unlocked: " + quest.nextQuest.questName);
                        activeQuests.Add(quest.nextQuest);
                    }
                }

                activeQuests.Remove(quest);
            }
        }
    }

    void ResetQuest(Quest quest)
    {
        foreach (QuestObjective objective in quest.objectives)
        {
            if (objective is IResettable resettableObjective)
            {
                resettableObjective.ResetData();
            }
        }

        if (quest is IResettable resettableQuest)
        {
            resettableQuest.ResetData();
        }
    }

}
