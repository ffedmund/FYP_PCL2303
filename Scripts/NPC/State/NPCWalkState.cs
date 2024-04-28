using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class NPCWalkState : NPCState
{
    float stuckThreshold = 0.25f;
    float checkInterval = 1.5f;
    float timer;
    Vector3 lastPosition;
    Vector3 destinationPoint;
    NavMeshAgent agent;

    bool isStuck;

    public NPCWalkState(NPCStateController stateController, Vector3 destinationPoint)
    {
        agent = stateController.npcController.agent;
        this.destinationPoint = destinationPoint;
        if(agent.isOnNavMesh){
            stateController.npcController.isWalking = true;
            agent.SetDestination(destinationPoint);
        }
    }

    public override NPCState Update(NPCStateController stateController)
    {   
        if(!agent.isOnNavMesh || agent.pathStatus == NavMeshPathStatus.PathPartial)
        {
            return new NPCIdleState();
        }

        if(stateController.attacker != null)
        {
            return new NPCEscapeState(stateController);
        }

        if(stateController.npcController.HeadFacingOnPlayer()){
            stateController.npcController.SetSpeakingContent("Hi Adventruer!");
        }else{
            stateController.npcController.ResetHeadFocus();
        }

        CheckIfStuck(stateController, stateController.transform, agent);
        if(agent.isOnOffMeshLink){
            stateController.npcController.TraverseNavMeshLink();
        }
        if(!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance){
            StopWalking(stateController,agent);
            //Reach the target/random/home position
            if(stateController.backingHome){
                stateController.backingHome = false;
                stateController.backedHome = true;
            }
            return new NPCIdleState();
        }
        stateController.npcController.animator.SetFloat("speed",agent.speed);
        return this;
    }

    void CheckIfStuck(NPCStateController stateController, Transform transform,  NavMeshAgent agent)
    {
        timer += Time.fixedDeltaTime;
        if(timer >= checkInterval){
            timer = 0;
            float distance = Vector3.Distance(transform.position, lastPosition);

            if (distance < stuckThreshold)
            {
                isStuck = true;
                Debug.Log("NPC is stuck. Finding a new path...");

                // Find a clear point and set it as the new destination
                Vector3 clearPoint;
                if(LocationSeekingState.RandomPoint(transform.position, 5, out clearPoint)){
                    agent.SetDestination(clearPoint);
                }
            }
            else if(isStuck)
            {
                isStuck = false;
                agent.SetDestination(destinationPoint);
            }

            lastPosition = transform.position;
        }
    }

    void StopWalking(NPCStateController stateController, NavMeshAgent agent){
        // Debug.Log("Stop Walking");
        agent.ResetPath();
        stateController.npcController.isWalking = false;
        stateController.npcController.animator.SetFloat("speed",0);
    }
}