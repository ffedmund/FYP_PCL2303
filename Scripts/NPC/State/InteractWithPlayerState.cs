using UnityEngine;

public class InteractWithPlayerState : NPCState
{
    public override NPCState Update(NPCStateController stateController)
    {
        //Need implement later
        if(stateController.npcController.npcInteraction.isInteracting){
            return this;
        }else{
            return new NPCIdleState();
        }
    }
}