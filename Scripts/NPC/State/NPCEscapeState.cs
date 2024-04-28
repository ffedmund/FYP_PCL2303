using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class NPCEscapeState : NPCState
{
    float originalSpeed;

    public NPCEscapeState(NPCStateController stateController){
        NavMeshAgent agent = stateController.npcController.agent;
        originalSpeed = agent.speed;
        agent.speed = originalSpeed * 2;
    }

    public override NPCState Update(NPCStateController stateController)
    {
        NavMeshAgent agent = stateController.npcController.agent;
        if(!agent.isOnNavMesh || stateController.attacker == null)
        {
            stateController.attacker = null;
            return new NPCIdleState();
        }
        if(agent.hasPath && agent.pathStatus != NavMeshPathStatus.PathPartial)
        {
            stateController.npcController.TurningTo(agent.destination);
            if(agent.isOnOffMeshLink){
                stateController.npcController.TraverseNavMeshLink();
            }
            if(!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                agent.ResetPath();
            }
            stateController.npcController.animator.SetFloat("speed",agent.speed);
        }
        else if((stateController.attacker.position - stateController.transform.position).sqrMagnitude < 5 * 5)
        {
            if(LocationSeekingState.RandomPoint(stateController.transform.position * 2 - stateController.attacker.position, 10, out Vector3 escapePoint))
            {
                agent.SetDestination(escapePoint);
            }
        }
        else
        {
            stateController.attacker = null;
            agent.ResetPath();
            agent.speed = originalSpeed;
            stateController.npcController.animator.SetFloat("speed",0);
            return new NPCIdleState();
        }
        return this;
    }
}