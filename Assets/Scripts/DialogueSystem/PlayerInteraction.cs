using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public float interactionRange = 3f;
    private NPCInteractable currentNPC;

    private void Update()
    {
        DetectNPC();

        if (currentNPC != null && Input.GetKeyDown(KeyCode.T))
        {
            currentNPC.Interact();
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
                break;
            }
        }
    }
}
