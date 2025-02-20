using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueLine
{
    public string speakerName;
    [TextArea(2, 5)] public string dialogueText;
    public List<DialogueOption> options = new List<DialogueOption>();
}

[System.Serializable]
public class DialogueOption
{
    public string optionText;
    public int nextDialogueIndex;  // -1 means end of dialogue
}

[CreateAssetMenu(fileName = "NewNPCDialogue", menuName = "Dialogue System/Dialogue")]
public class Dialogue : ScriptableObject
{
    public List<DialogueLine> lines;
}

[CreateAssetMenu(fileName = "NewEnemyDialogue", menuName = "Dialogue System/Enemy Dialogue")]
public class EnemyDialogue : ScriptableObject
{
    public string enemyName;
    public List<string> dialogues; // Randomized enemy dialogues
}
