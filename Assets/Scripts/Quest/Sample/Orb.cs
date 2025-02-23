using UnityEngine;

public class Orb : MonoBehaviour
{
    [Tooltip("The amount this orb adds to the collection objective.")]
    public int orbValue = 1;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the collider belongs to the player.
        if (other.CompareTag("Player"))
        {
            // Find the QuestManager in the scene.
            QuestManager questManager = FindObjectOfType<QuestManager>();
            if (questManager != null)
            {
                // Loop through each active quest.
                foreach (Quest quest in questManager.activeQuests)
                {
                    // Loop through each objective in the quest.
                    foreach (QuestObjective objective in quest.objectives)
                    {
                        // If the objective is a CollectItemObjective, add to its count.
                        if (objective is CollectItemObjective collectObjective)
                        {
                            collectObjective.AddItem("Orb",orbValue);
                        }
                    }
                }
            }

            Destroy(gameObject);
        }
    }
}
