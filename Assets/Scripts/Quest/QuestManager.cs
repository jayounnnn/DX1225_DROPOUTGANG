using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public List<Quest> activeQuests;

    public float questCompletionDelay = 1.0f;

    private HashSet<Quest> questsScheduledForRemoval = new HashSet<Quest>();

    void Update()
    {
        List<Quest> questsToCheck = new List<Quest>(activeQuests);

        foreach (var quest in questsToCheck)
        {
            if (quest.IsQuestCompleted() && !questsScheduledForRemoval.Contains(quest))
            {
                StartCoroutine(DelayedQuestCompletion(quest));
                questsScheduledForRemoval.Add(quest);
            }
        }
    }

    IEnumerator DelayedQuestCompletion(Quest quest)
    {
        yield return new WaitForSeconds(questCompletionDelay);

        //Debug.Log("Quest Completed: " + quest.questName);

        ResetQuest(quest);

        if (quest.nextQuest != null && !activeQuests.Contains(quest.nextQuest))
        {
            //Debug.Log("New Quest Unlocked: " + quest.nextQuest.questName);
            activeQuests.Add(quest.nextQuest);
        }

        activeQuests.Remove(quest);
        questsScheduledForRemoval.Remove(quest);
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

    public void ResetAllScriptableObjects()
    {
        foreach (Quest quest in activeQuests)
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

        activeQuests.Clear();
    }

    public void AddQuest(Quest newQuest)
    {
        if (newQuest != null && !activeQuests.Contains(newQuest))
        {
            activeQuests.Add(newQuest);
        }
    }

    private void OnApplicationQuit()
    {
        ResetAllScriptableObjects();
    }
}
