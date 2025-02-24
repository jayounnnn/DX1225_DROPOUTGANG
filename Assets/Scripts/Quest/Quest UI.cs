using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestUI : MonoBehaviour
{
    public QuestManager questManager;
    public GameObject objectiveUIPrefab;
    public Transform objectivesContainer;

    public float fadeInAnimationDuration = 0.5f;

    public float removalAnimationDuration = 0.5f;

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

                    CanvasGroup canvasGroup = newUI.GetComponent<CanvasGroup>();
                    if (canvasGroup == null)
                    {
                        canvasGroup = newUI.AddComponent<CanvasGroup>();
                    }

                    canvasGroup.alpha = 0f;
                    StartCoroutine(FadeInUI(newUI));

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
            GameObject uiObj = objectiveUIInstances[obj];
            objectiveUIInstances.Remove(obj);
            StartCoroutine(FadeOutAndDestroy(uiObj));
        }
    }

    private IEnumerator FadeInUI(GameObject uiObj)
    {
        CanvasGroup canvasGroup = uiObj.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            yield break;

        float elapsed = 0f;
        while (elapsed < fadeInAnimationDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeInAnimationDuration);
            yield return null;
        }
        canvasGroup.alpha = 1f;
    }

    private IEnumerator FadeOutAndDestroy(GameObject uiObj)
    {
        CanvasGroup canvasGroup = uiObj.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = uiObj.AddComponent<CanvasGroup>();
        }

        float elapsed = 0f;
        while (elapsed < removalAnimationDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / removalAnimationDuration);
            yield return null;
        }
        Destroy(uiObj);
    }
}
