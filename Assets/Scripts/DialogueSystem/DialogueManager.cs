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
    public GameObject optionPrefab;

    private Dialogue currentDialogue;
    private int currentLineIndex;
    private bool isTyping = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void StartDialogue(Dialogue dialogue)
    {
        if (dialogue == null) return;

        currentDialogue = dialogue;
        currentLineIndex = 0;
        dialoguePanel.SetActive(true);
        FreezePlayer(true);
        ShowLine();
    }

    public void Update()
    {
        if (dialoguePanel.activeSelf && Input.GetKeyDown(KeyCode.Space))
        {
            ContinueDialogue();
        }
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

        // Clear old options
        foreach (Transform child in optionsContainer)
        {
            Destroy(child.gameObject);
        }

        // If there are options, create buttons
        if (line.options.Count > 0)
        {
            foreach (var option in line.options)
            {
                GameObject newOption = Instantiate(optionPrefab, optionsContainer);
                TMP_Text optionText = newOption.GetComponentInChildren<TMP_Text>();
                Button optionButton = newOption.GetComponent<Button>();

                if (optionText != null)
                {
                    optionText.text = option.optionText;
                }

                if (optionButton != null)
                {
                    optionButton.onClick.RemoveAllListeners();
                    int nextIndex = option.nextDialogueIndex;
                    optionButton.onClick.AddListener(() => SelectOption(nextIndex));
                }
            }
        }
    }

    public void ContinueDialogue()
    {
        if (currentDialogue.lines[currentLineIndex].options.Count == 0) // If no options, auto-progress
        {
            currentLineIndex++;
            ShowLine();
        }
    }

    public void SelectOption(int nextIndex)
    {
        if (nextIndex == -1)
        {
            EndDialogue();
        }
        else
        {
            currentLineIndex = nextIndex;
            ShowLine();
        }
    }

    public void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        currentDialogue = null;
        FreezePlayer(false);
    }

    private void FreezePlayer(bool isFrozen)
    {
        if (isFrozen)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Time.timeScale = 0f;  // Pause the game
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Time.timeScale = 1f;  // Resume the game

        }
    }
}
