using System;
using System.Collections.Generic;
using UnityEngine;

public class NPCChatingState : NPCState
{
    string receiveChatContent;

    public NPCChatingState(){
        this.receiveChatContent = "";
    }

    public NPCChatingState(string receiveChatContent){
        this.receiveChatContent = receiveChatContent;
    }

    readonly Dictionary<string,string> simpleChatDictionary = new Dictionary<string, string>{
        {"rnd1","Hey {0}!"},
        {"rnd2","How's going {0}."},
        {"rnd3","Long time no see!"},
        {"rnd4","How are you?"},
        {"rnd5","So cold!"},
        {"How are you?","I'm fine!"},
        {"I'm fine!","Good to know!"},
        {"So cold!","Hope I have fire magic..."},
        {"Long time no see!","How's going {0}."},
        {"How's going {0}.","I'm fine!"}
    };

    public override NPCState Update(NPCStateController stateController)
    {   
        if(stateController.interactingTarget.TryGetComponent(out NPCStateController targetNPCStateController)){
            NPCController targetNPCController = targetNPCStateController.npcController;
            string chatKey = !simpleChatDictionary.ContainsKey(receiveChatContent)?"rnd"+UnityEngine.Random.Range(1,6):receiveChatContent;
            stateController.npcController.SetSpeakingContent(string.Format(simpleChatDictionary[chatKey],targetNPCController.npc.npcName));
            targetNPCStateController.ReceiveChat(stateController,simpleChatDictionary[chatKey]);
            stateController.npcController.animator.Play("Talking");
            stateController.interactingTarget = null;
        }
        return new WaitingResponseState();
    }
}