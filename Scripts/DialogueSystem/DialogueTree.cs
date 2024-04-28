using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueTree", menuName = "DialogueSystem/Dialogue Tree", order = 0)]
public class DialogueTree : ScriptableObject {
    public int dialogueId;
    public string dialogueTrigger;
    public DialogueNode root;
}

