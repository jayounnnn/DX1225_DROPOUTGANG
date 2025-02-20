using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

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

    private PlayerInput _playerInput;
    private InputActionAsset _inputActions;

    private bool isDialogueActive = false;
    private bool inputCooldown = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        _playerInput = FindObjectOfType<PlayerInput>();
        if (_playerInput != null)
        {
            _inputActions = _playerInput.actions;
        }
    }

    private void Update()
    {
        if (_inputActions != null && _inputActions["ToggleDialogue"].WasPressedThisFrame())
        {
            if (!isDialogueActive)
            {
                StartDialogue(FindObjectOfType<NPCInteractable>()?.dialogue);
            }
            else if (!inputCooldown)
            {
                ContinueOrCloseDialogue();
            }
        }
    }

    public void StartDialogue(Dialogue dialogue)
    {
        if (dialogue == null || isDialogueActive) return;

        currentDialogue = dialogue;
        currentLineIndex = 0;
        dialoguePanel.SetActive(true);
        isDialogueActive = true;
        FreezePlayer(true);
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

        // Clear old options
        foreach (Transform child in optionsContainer)
        {
            Destroy(child.gameObject);
        }

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

    public void ContinueOrCloseDialogue()
    {
        if (currentDialogue.lines[currentLineIndex].options.Count == 0)
        {
            currentLineIndex++;

            if (currentLineIndex >= currentDialogue.lines.Count)
            {
                EndDialogue();
            }
            else
            {
                ShowLine();
            }
        }

        StartCoroutine(ToggleCooldown());
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
        isDialogueActive = false;
        currentDialogue = null;
        FreezePlayer(false);
    }

    private void FreezePlayer(bool isFrozen)
    {
        if (isFrozen)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Time.timeScale = 0f;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Time.timeScale = 1f;
        }
    }

    private IEnumerator ToggleCooldown()
    {
        inputCooldown = true;
        yield return new WaitForSecondsRealtime(0.75f);
        inputCooldown = false;
    }

    // === NEW FUNCTION: Enemy AI Triggers Dialogue from ScriptableObject ===
    public void ShowEnemyDialogue(EnemyDialogue enemyDialogue, float duration = 3f)
    {
        if (isDialogueActive || enemyDialogue == null || enemyDialogue.dialogues.Count == 0) return;

        string randomDialogue = enemyDialogue.dialogues[Random.Range(0, enemyDialogue.dialogues.Count)];
        StartCoroutine(ShowEnemyDialogueCoroutine(enemyDialogue.enemyName, randomDialogue, duration));
    }

    private IEnumerator ShowEnemyDialogueCoroutine(string enemyName, string dialogueText, float duration)
    {
        dialoguePanel.SetActive(true);
        speakerNameText.text = enemyName;
        this.dialogueText.text = dialogueText;
        isDialogueActive = true;

        yield return new WaitForSeconds(duration);

        EndDialogue();
    }
}
