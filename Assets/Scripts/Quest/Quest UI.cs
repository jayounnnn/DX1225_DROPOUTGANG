using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestUI : MonoBehaviour
{
    public QuestManager questManager;

    public GameObject objectiveUIPrefab;

    public GameObject objectivesContainer;

    private Dictionary<QuestObjective, GameObject> objectiveUIInstances = new Dictionary<QuestObjective, GameObject>();

    void Update()
    {
        if (objectivesContainer == null)
        {
            Debug.LogError("Objectives container is not assigned in QuestUI!");
            return;
        }

        foreach (Quest quest in questManager.activeQuests)
        {
            foreach (QuestObjective objective in quest.objectives)
            {
                if (!objectiveUIInstances.ContainsKey(objective))
                {
                    GameObject newUI = Instantiate(objectiveUIPrefab, objectivesContainer.transform); 
                    objectiveUIInstances.Add(objective, newUI);

                    TMP_Text label = newUI.GetComponentInChildren<TMP_Text>(true);
                    if (label != null)
                    {
                        label.text = objective.description;
                    }
                    else
                    {
                        Debug.LogError("TMP_Text component not found in the prefab!");
                    }
                }
            }
        }

        List<QuestObjective> objectivesToRemove = new List<QuestObjective>();

        foreach (KeyValuePair<QuestObjective, GameObject> pair in objectiveUIInstances)
        {
            QuestObjective objective = pair.Key;
            GameObject uiObj = pair.Value;

            Toggle toggle = uiObj.GetComponentInChildren<Toggle>(true);
            if (toggle != null)
            {
                toggle.isOn = objective.isCompleted;
            }
            else
            {
                Debug.LogError("Toggle component not found in the prefab!");
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
