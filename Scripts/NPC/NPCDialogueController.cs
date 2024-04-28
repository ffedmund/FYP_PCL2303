using System;
using System.Collections.Generic;
using FYP;
using UnityEngine;

public struct Choice{
    public string content;
    public bool questAcceptTrigger;
    public int questID;
    public int extraFirendshipValue;
}

public class NPCDialogueController : MonoBehaviour {
    NPC npc;
    DialogueTree dialogueTree;
    DialogueNode currentDialogueNode;
    DialogueList activeDialogueQueue;
    List<Dialogue> nonactiveDialogueList;
    string[] greetings;
    
    bool noDialogueTree;
    string previousPlayerAns;

    public void Setup(NPC npc){
        activeDialogueQueue = new DialogueList();
        nonactiveDialogueList = new List<Dialogue>();
        this.dialogueTree = null;
        this.npc = npc;
        greetings = npc.greetings;
        if(npc.dialogues.Length <= 0){
            noDialogueTree = true;
        }else{
            if(npc.dialogues[0].setDefault){
                dialogueTree = npc.dialogues[0].dialogueTree;
                currentDialogueNode = dialogueTree.root;
            }
            foreach(Dialogue dialogue in npc.dialogues){
                if(dialogue.setDefault){
                    continue;
                }
                nonactiveDialogueList.Add(dialogue);
            }
        }
    }

    void InitDialogueTree(){
        dialogueTree = null;
        currentDialogueNode = null;
    }

    public bool TriggerDialogue(int id){
        foreach(Dialogue dialogue in npc.dialogues){
            if(dialogue.triggerQuestId == id){
                activeDialogueQueue.Enqueue(dialogue);
                return true;
            }
        }
        return false;
    }

    public bool TriggerDialogue(Firendship firendship){
        List<Dialogue> tempActiveList = new List<Dialogue>();
        foreach(Dialogue dialogue in nonactiveDialogueList){
            //Dialogue event may occur more than once
            if(dialogue.firendshipTrigger && firendship.firendshipValue >= dialogue.triggerFirendshipValue){
                activeDialogueQueue.Enqueue(dialogue);
                tempActiveList.Add(dialogue);
            }
        }

        if(tempActiveList.Count > 0){
            for(int i = 0; i < tempActiveList.Count; i++){
                nonactiveDialogueList.Remove(tempActiveList[i]);
            }
            return true;
        }
        return false;
    }

    public string Talk(string answer){
        if(currentDialogueNode && currentDialogueNode.childs.Length == 0){
            InitDialogueTree();
            return "";
        }
        if(noDialogueTree || (dialogueTree == null && activeDialogueQueue.Length == 0)){
            if(greetings.Length > 0 && previousPlayerAns != ""){
                previousPlayerAns = "";
                return greetings[UnityEngine.Random.Range(0,npc.greetings.Length)];
            }else{
                previousPlayerAns = "reset";
                return null;
            }
        }

        if(dialogueTree == null){
            if(activeDialogueQueue.isTrigger(answer)){
                dialogueTree = activeDialogueQueue.TriggerDialogue(answer).dialogueTree;
                currentDialogueNode = dialogueTree.root;
            }else if(activeDialogueQueue.Length > 0){
                return (greetings.Length > 0)?greetings[UnityEngine.Random.Range(0,npc.greetings.Length)]:"Hello, adventurer, what a nice day.";
            }
        }else if(currentDialogueNode){
            for(int i = 0; i < currentDialogueNode.childs.Length; i++){
                if(answer == currentDialogueNode.childs[i].triggerAnsPath){
                    currentDialogueNode = currentDialogueNode.childs[i].child;
                    break;
                }
            }
        }
        return currentDialogueNode.content;
    }
    
    public Choice[] GetAnsweringChoices(){
        List<Choice> answeringChoices = new List<Choice>();
        if(currentDialogueNode && currentDialogueNode.childs.Length > 0){
            for(int i = 0; i < currentDialogueNode.childs.Length; i++){
                if(currentDialogueNode.childs[i].triggerAnsPath != ""){
                    Choice choice = new Choice
                    {
                        content = currentDialogueNode.childs[i].triggerAnsPath,
                        questAcceptTrigger = currentDialogueNode.childs[i].questAcceptTrigger,
                        questID = currentDialogueNode.childs[i].questID,
                        extraFirendshipValue = currentDialogueNode.childs[i].extraFirendshipValue
                    };

                    answeringChoices.Add(choice);
                }
            }
        }else if(dialogueTree == null && activeDialogueQueue.Length > 0){
            foreach(Dialogue dialogue in activeDialogueQueue){
                string key = dialogue.dialogueTree.dialogueTrigger;
                if(key != null && key != ""){
                    Choice choice = new Choice
                    {
                        content = key,
                    
                    };
                    answeringChoices.Add(choice);
                }
            }
            if(answeringChoices.Count == 0){
                dialogueTree = activeDialogueQueue.Dequeue().dialogueTree;
                currentDialogueNode = dialogueTree.root;
            }
        }
        return answeringChoices.ToArray();
    }

    // public string Talk(string answer){
    //     if(currentDialogueNode && currentDialogueNode.childs.Length == 0){
    //         InitDialogueTree();
    //         return "";
    //     }
    //     if(noDialogueTree || (dialogueTree == null && activeDialogueQueue.Length == 0)){
    //         return null;
    //     }
    //     if(currentDialogueNode == null && activeDialogueQueue.Length > 0){
    //         dialogueTree = activeDialogueQueue.Dequeue().dialogueTree;
    //         currentDialogueNode = dialogueTree.root;
    //     }else{
    //         for(int i = 0; i < currentDialogueNode.childs.Length; i++){
    //             if(answer == currentDialogueNode.childs[i].triggerAnsPath){
    //                 currentDialogueNode = currentDialogueNode.childs[i].child;
    //                 break;
    //             }
    //         }
    //     }
    //     return currentDialogueNode.content;
    // }

    // public Choice[] GetAnsweringChoices(){
    //     List<Choice> answeringChoices = new List<Choice>();
    //     if(currentDialogueNode && currentDialogueNode.childs.Length > 0){
    //         for(int i = 0; i < currentDialogueNode.childs.Length; i++){
    //             if(currentDialogueNode.childs[i].triggerAnsPath != ""){
    //                 Choice choice = new Choice
    //                 {
    //                     content = currentDialogueNode.childs[i].triggerAnsPath,
    //                     questAcceptTrigger = currentDialogueNode.childs[i].questAcceptTrigger,
    //                     questID = currentDialogueNode.childs[i].questID,
    //                     extraFirendshipValue = currentDialogueNode.childs[i].extraFirendshipValue
    //                 };

    //                 answeringChoices.Add(choice);
    //             }
    //         }
    //     }
    //     return answeringChoices.ToArray();
    // }
}