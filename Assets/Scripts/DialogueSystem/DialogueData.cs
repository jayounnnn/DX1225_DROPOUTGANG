using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueLine
{
    public string speakerName;
    [TextArea(2, 5)] public string dialogueText;
    public List<DialogueOption> options = new List<DialogueOption>(); // Ensure options exist but can be empty
}

[System.Serializable]
public class DialogueOption
{
    public string optionText;
    public int nextDialogueIndex;  // -1 means end of dialogue
}

[CreateAssetMenu(fileName = "NewDialogue", menuName = "Dialogue System/Dialogue")]
public class Dialogue : ScriptableObject
{
    public List<DialogueLine> lines;
}