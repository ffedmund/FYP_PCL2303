using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public class NavigationSystem : MonoBehaviour {
    public static NavigationSystem instance;

    Dictionary<string, List<float3>> targetDictionary = new Dictionary<string, List<float3>>();

    void Awake(){
        instance = this;
    }

    public void AddNavigationTarget(string id, Vector3 position){
        if(targetDictionary.ContainsKey(id)){
            targetDictionary[id].Add(new float3(position));
        }else{
            targetDictionary.Add(id,new List<float3>());
            targetDictionary[id].Add(new float3(position));
        }
    }

    public void RemoveNavigationTarget(string id, Vector3 position){
        if(targetDictionary.ContainsKey(id)){
            targetDictionary[id].Remove(position);
        }
    }

    public Vector3 FindClosestTarget(string targetId,Vector3 startingPoint){
        targetId = NavigationHelper(targetId);
        if(targetDictionary.ContainsKey(targetId)){
            if(targetDictionary[targetId].Count == 1){
                return new Vector3(targetDictionary[targetId][0].x,targetDictionary[targetId][0].y,targetDictionary[targetId][0].z);
            }
            NativeArray<float3> targetArr = new NativeArray<float3>(targetDictionary[targetId].Count,Allocator.TempJob);
            targetArr.CopyFrom(targetDictionary[targetId].ToArray());
            FindCloesetPointJob findCloeset = new FindCloesetPointJob{
                startingPoint = new float3(startingPoint),
                targetArr = targetArr
            };
            JobHandle jobHandle = findCloeset.Schedule();
            jobHandle.Complete();
            float3 closestPoint = targetArr[0];
            targetArr.Dispose();
            return new Vector3(closestPoint.x,closestPoint.y,closestPoint.z);
        }
        // Debug.Log("Can't Find Navigation Target");
        return Vector3.one * -9999;
    }

    public string NavigationHelper(string id){
        if(id == "report"){
            id = "Sister Cecilia";
        }
        if(id == "egg"){
            id = "Barnabas";
        }
        return id;
    }

    [BurstCompile]
    struct FindCloesetPointJob : IJob
    {
        public float3 startingPoint;
        public NativeArray<float3> targetArr;

        public void Execute()
        {
            for (int i = 0; i < targetArr.Length; i++)
            {
                if(math.lengthsq(targetArr[i]-startingPoint) < math.lengthsq(targetArr[0]-startingPoint)){
                    targetArr[0] = targetArr[i];
                }
            }
        }
    }
}