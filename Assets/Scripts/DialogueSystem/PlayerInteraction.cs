using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    public float interactionRange = 3f;
    private NPCInteractable currentNPC;
    private PlayerInput _playerInput;
    private InputActionAsset _inputActions;

    private void Start()
    {
        _playerInput = GetComponent<PlayerInput>(); 
        _inputActions = _playerInput.actions; 
    }

    private void Update()
    {
        DetectNPC();

        if (currentNPC != null && _inputActions["ToggleDialogue"].WasPressedThisFrame())
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
