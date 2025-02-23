using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderCompletion : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            QuestManager questManager = FindObjectOfType<QuestManager>();

            if (questManager != null)
            {
                foreach (Quest quest in questManager.activeQuests)
                {
                    foreach (QuestObjective objective in quest.objectives)
                    {
                        if (objective is ColliderObjective colliderObjective && colliderObjective.colliderName == gameObject.name)
                        {
                            colliderObjective.CompleteObjective();
                            Debug.Log("Objective Completed: " + gameObject.name);
                            return;
                        }
                    }
                }
            }
        }
    }
}
