using UnityEngine;

public class WaitingResponseState : NPCState
{
    readonly float waitingTime = 6;
    int minimumResponseTime = 2;
    float timer = 0;
    public WaitingResponseState()
    {
        minimumResponseTime = Random.Range(2,5);
    }

    public override NPCState Update(NPCStateController stateController)
    {
        timer += Time.fixedDeltaTime;
        if(stateController.attacker != null)
        {
            return new NPCEscapeState(stateController);
        }

        if(timer > minimumResponseTime && stateController.speakingNPC != null && stateController.receiveChatContent != ""){
            return new SearchingNPCState();
        }
        if(timer > waitingTime){
            return new NPCIdleState();
        }
        return this;
    }
}