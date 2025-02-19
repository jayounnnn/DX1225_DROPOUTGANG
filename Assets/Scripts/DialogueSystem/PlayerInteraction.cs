using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public float interactionRange = 3f; // Set interaction distance
    private NPCInteractable currentNPC;

    private void Update()
    {
        DetectNPC();

        if (currentNPC != null)
        {
            Debug.Log("NPC in range: " + currentNPC.gameObject.name); // Debug log when NPC is detected

            if (Input.GetKeyDown(KeyCode.T))
            {
                Debug.Log("Interacting with: " + currentNPC.gameObject.name);
                currentNPC.Interact();
            }
        }
        else
        {
            Debug.Log("No NPC in range."); // Debug log if no NPC is detected
        }
    }

    private void DetectNPC()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, interactionRange);
        currentNPC = null;

        foreach (Collider col in hitColliders)
        {
            NPCInteractable npc = col.GetComponent<NPCInteractable>();
            if (npc != null)
            {
                currentNPC = npc;
                break; // Stop after finding the first NPC in range
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}
