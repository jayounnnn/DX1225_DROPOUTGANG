using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestUI : MonoBehaviour
{
    public QuestManager questManager;

    public GameObject objectiveUIPrefab;

    public Transform objectivesContainer;

    private Dictionary<QuestObjective, GameObject> objectiveUIInstances = new Dictionary<QuestObjective, GameObject>();

    void Update()
    {

        foreach (Quest quest in questManager.activeQuests)
        {
            foreach (QuestObjective objective in quest.objectives)
            {
                if (!objectiveUIInstances.ContainsKey(objective))
                {
                    GameObject newUI = Instantiate(objectiveUIPrefab, objectivesContainer);
                    objectiveUIInstances.Add(objective, newUI);

                    ObjectiveUIElement uiElement = newUI.GetComponent<ObjectiveUIElement>();
                    if (uiElement != null)
                    {

                        uiElement.descriptionText.text = objective.description;

                        if (objective is CollectItemObjective collectObjective)
                        {
                            uiElement.progressText.text = $"{collectObjective.currentAmount}/{collectObjective.requiredAmount}";
                        }
                        else
                        {
                            uiElement.progressText.text = "";
                        }
                    }
                }
            }
        }

        List<QuestObjective> objectivesToRemove = new List<QuestObjective>();

        foreach (KeyValuePair<QuestObjective, GameObject> pair in objectiveUIInstances)
        {
            QuestObjective objective = pair.Key;
            GameObject uiObj = pair.Value;
            ObjectiveUIElement uiElement = uiObj.GetComponent<ObjectiveUIElement>();

            if (uiElement != null)
            {
                uiElement.objectiveToggle.isOn = objective.isCompleted;

                if (objective is CollectItemObjective collectObjective)
                {
                    uiElement.progressText.text = $"{collectObjective.currentAmount}/{collectObjective.requiredAmount}";
                }
            }

            bool stillActive = false;
            foreach (Quest activeQuest in questManager.activeQuests)
            {
                if (activeQuest.objectives.Contains(objective))
                {
                    stillActive = true;
                    break;
                }
            }
            if (!stillActive)
            {
                objectivesToRemove.Add(objective);
            }
        }

        foreach (QuestObjective obj in objectivesToRemove)
        {
            Destroy(objectiveUIInstances[obj]);
            objectiveUIInstances.Remove(obj);
        }
    }
}
