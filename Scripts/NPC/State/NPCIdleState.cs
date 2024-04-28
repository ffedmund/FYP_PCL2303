
using UnityEngine;

public class NPCIdleState : NPCState
{
    float timer = 0;
    float waitingTime;

    public override NPCState Update(NPCStateController stateController)
    {
        if(stateController.npcController.HeadFacingOnPlayer()){
            if(stateController.npcController.firendship.firendshipValue > 10){
                stateController.npcController.SetSpeakingContent("Long time no see!");
            }else if(stateController.npcController.firendship.firendshipValue > 0){
                stateController.npcController.SetSpeakingContent("Hi Adventurer!");
            }else{
                stateController.npcController.SetSpeakingContent("");
            }
        }else{
            stateController.npcController.ResetHeadFocus();
        }

        if(stateController.npcController.npcInteraction.isInteracting){
            return new InteractWithPlayerState();
        }
        
        if(stateController.backedHome){
            return new NPCRestState();
        }

        if(timer == 0){
            waitingTime = Random.Range(3,6);
        }

        timer += Time.fixedDeltaTime;
        if(timer > waitingTime){
            int rndIndex = Random.Range(0,10);
            timer = 0;
            if(!stateController.npcController.npc.hasDuty || stateController.needRest){
                if(stateController.attacker != null){
                    return new NPCEscapeState(stateController);
                }
                if(rndIndex < 7 && !stateController.needRest){
                    return new SearchingNPCState();
                }else{
                    return new LocationSeekingState();
                }
            }
        }
        return this;
    }
}