using UnityEngine;
using UnityEngine.AI;

public class NPCRestState : NPCState
{
    public override NPCState Update(NPCStateController stateController)
    {
        if(!stateController.needRest){
            stateController.backedHome = false;
            stateController.npcController.firendship.ResetDailyIncrease();
            stateController.npcController.npcInventory.RestoreInventory();
            return new LocationSeekingState();
        }
        return this;
    }
}