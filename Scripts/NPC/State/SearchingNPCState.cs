using UnityEngine;
using DG.Tweening;

public class SearchingNPCState : NPCState
{
    readonly float interactionRadius = 5;

    public override NPCState Update(NPCStateController stateController)
    {
        if(stateController.speakingNPC != null && stateController.receiveChatContent != ""){
            NPCChatingState responseChatingState = null;

            if(Random.Range(0,2) == 1 && FacingInteractNPC(stateController, stateController.interactingTarget))
            {
                stateController.interactingTarget = stateController.speakingNPC.transform;

                // Implement interaction logic here
                responseChatingState = new NPCChatingState(stateController.receiveChatContent);
            }
            stateController.speakingNPC = null;
            stateController.receiveChatContent = "";
            if(responseChatingState != null)
            {
                return responseChatingState;
            }
        }else{
            // Get all NPCs within a certain radius
            Collider[] hitColliders = Physics.OverlapSphere(stateController.transform.position, interactionRadius);
        
            foreach (var hitCollider in hitColliders)
            {   
                NPCStateController nearbyNPC = hitCollider.GetComponent<NPCStateController>();
                
                // If a nearby NPC is found and it's not the current NPC itself
                if (nearbyNPC != null && nearbyNPC != stateController && FacingInteractNPC(stateController, nearbyNPC.transform))
                {
                    // Implement interaction logic here
                    stateController.interactingTarget = nearbyNPC.transform;
                    return new NPCChatingState();
                }
            }
        }
        return new NPCIdleState();
    }

    bool FacingInteractNPC(NPCStateController stateController, Transform target){
        // Make the NPC face the nearby NPC
        return stateController.npcController.HeadFacingOn(target);
    }
}