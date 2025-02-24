using UnityEngine;

public class Orb : MonoBehaviour
{

    public int orbValue = 1;

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
