using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

[BurstCompile]
public struct MultiViewersActivationJob : IJobParallelFor
{
    public NativeArray<int> monsterStateArray;
    public NativeArray<Vector3> monsterPosArray;

    [ReadOnly]
    public NativeArray<Vector3> viewerPositions;

    public void Execute(int index)
    {
        for(int i = 0; i < viewerPositions.Length; i++)
        {
            float sqrDistance = Vector3.SqrMagnitude(monsterPosArray[index]/8-viewerPositions[i]);
            if(sqrDistance < 12*12){
                monsterStateArray[index] = 1;
                break;
            }else{
                monsterStateArray[index] = 0;
            }
        }
    }
}
