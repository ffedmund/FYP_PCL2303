using System.Collections;
using System.Collections.Generic;
using FYP;
using UnityEngine;

public class DialogueList: IEnumerable{
    public int Length{
        get{return queue.Count;}
    }

    List<Dialogue> queue;
    Dictionary<string,Dialogue> dialogueTriggerDict;

    public DialogueList(){
        queue = new List<Dialogue>();
        dialogueTriggerDict = new Dictionary<string, Dialogue>();
    }

    public Dialogue GetHead(){
        return queue[0];
    }

    public bool isTrigger(string answer){
        return dialogueTriggerDict.ContainsKey(answer);
    }

    public Dialogue TriggerDialogue(string answer){
        Dialogue dialogue = dialogueTriggerDict[answer];
        queue.Remove(dialogue);
        dialogueTriggerDict.Remove(answer);
        return dialogue;
    }

    public void Enqueue(Dialogue dialogue){
        queue.Add(dialogue);
        string key = dialogue.dialogueTree.dialogueTrigger;
        if(key != null && key != ""){
            dialogueTriggerDict.Add(key,dialogue);
        }
    }

    public Dialogue Dequeue(){
        Dialogue dialogue = GetHead();
        queue.Remove(dialogue);
        return dialogue;
    }

    public IEnumerator GetEnumerator()
    {
        for (int i = 0; i < queue.Count; i++)
        {
            yield return queue[i];
        }
    }
}