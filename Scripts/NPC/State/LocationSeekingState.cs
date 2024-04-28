using UnityEngine;
using UnityEngine.AI;

public class LocationSeekingState : NPCState
{
    public override NPCState Update(NPCStateController stateController)
    {   
        if(BuildingLocationManager.instance == null)
        {
            return new NPCIdleState();
        }
        Vector3 destinationPoint = stateController.npcController.initialPosition;

        if(stateController.needRest)
        {
            stateController.backingHome = true;
            Vector3? homePosition = BuildingLocationManager.instance.GetNPCHomeAddress(stateController.npcController.npc.npcName)?.position;
            destinationPoint = homePosition.HasValue? homePosition.Value:destinationPoint;
        }
        else if(stateController.npcController.npc.hasDuty)
        {
            if((stateController.transform.position-stateController.npcController.initialPosition).sqrMagnitude > 2.5f * 2.5f)
            {
                destinationPoint = stateController.npcController.initialPosition;
            }
            else
            {
                return new NPCIdleState();
            }
        }
        else
        {
            if(Random.Range(0,1.0f) > 0.1f && stateController.npcController.npc.isVillager)
            {
                destinationPoint = BuildingLocationManager.instance.GetRandomNPCLocation().position;
            }
            else if(RandomPoint(stateController.transform.position,10,out Vector3 rndPosition))
            {
                destinationPoint = rndPosition;
            }
        }
        return new NPCWalkState(stateController,destinationPoint);
    }

    public static bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {

        Vector3 randomPoint = center + Random.insideUnitSphere * range; //random point in a sphere 
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas)) //documentation: https://docs.unity3d.com/ScriptReference/AI.NavMesh.SamplePosition.html
        { 
            //the 1.0f is the max distance from the random point to a point on the navmesh, might want to increase if range is big
            //or add a for loop like in the documentation
            result = hit.position;
            return true;
        }

        result = Vector3.zero;
        return false;
    }

    public static Vector3 GetRandomPoint(Vector3 center, float range)
    {

        Vector3 randomPoint = center + Random.insideUnitSphere * range; //random point in a sphere 
        NavMeshHit hit;
        int loopLimit = 1000;
        while(loopLimit-- > 0)
        {
            if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas)) //documentation: https://docs.unity3d.com/ScriptReference/AI.NavMesh.SamplePosition.html
            { 
                //the 1.0f is the max distance from the random point to a point on the navmesh, might want to increase if range is big
                //or add a for loop like in the documentation
                return hit.position;
            }
        }
        return center;
    }
}