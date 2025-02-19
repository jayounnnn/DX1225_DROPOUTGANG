using UnityEngine;

public class NPCInteractable : MonoBehaviour
{
    public Dialogue dialogue;

    public void Interact()
    {
        if (dialogue != null)
        {
            DialogueManager.Instance.StartDialogue(dialogue);
        }
    }
}
