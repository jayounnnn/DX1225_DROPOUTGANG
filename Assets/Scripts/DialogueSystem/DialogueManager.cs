using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [Header("UI Elements")]
    public GameObject dialoguePanel;
    public TMP_Text speakerNameText;
    public TMP_Text dialogueText;
    public Transform optionsContainer;
    public GameObject optionPrefab;  // Prefab for player choices

    private Dialogue currentDialogue;
    private int currentLineIndex;
    private Dictionary<string, object> npcMemory = new Dictionary<string, object>(); // Stores NPC-related data (e.g., quest status)

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void StartDialogue(Dialogue dialogue)
    {
        if (dialogue == null)
        {
            Debug.LogError("Dialogue is NULL! Make sure NPC has a Dialogue assigned.");
            return;
        }

        Debug.Log("Starting dialogue with: " + dialogue.lines[0].speakerName);

        currentDialogue = dialogue;
        currentLineIndex = 0;
        dialoguePanel.SetActive(true);
        Debug.Log("Dialogue panel set active."); // Check if UI is turning on
        ShowLine();
    }

    public void ShowLine()
    {
        if (currentLineIndex < 0 || currentLineIndex >= currentDialogue.lines.Count)
        {
            EndDialogue();
            return;
        }

        var line = currentDialogue.lines[currentLineIndex];
        speakerNameText.text = line.speakerName;
        dialogueText.text = line.dialogueText;

        Debug.Log($"Displaying line: {line.speakerName} - {line.dialogueText}");

        foreach (Transform child in optionsContainer)
            Destroy(child.gameObject);

        foreach (var option in line.options)
        {
            GameObject newOption = Instantiate(optionPrefab, optionsContainer);
            newOption.GetComponentInChildren<TMP_Text>().text = option.optionText;
            newOption.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => SelectOption(option.nextDialogueIndex));
        }
    }


    public void SelectOption(int nextIndex)
    {
        currentLineIndex = nextIndex;
        ShowLine();
    }

    public void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        currentDialogue = null;
    }

    public void StoreNPCMemory(string key, object value)
    {
        npcMemory[key] = value;
    }

    public object GetNPCMemory(string key)
    {
        return npcMemory.ContainsKey(key) ? npcMemory[key] : null;
    }
}
