using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

[BurstCompile]
public struct ActivationJob : IJobParallelFor
{
    public NativeArray<int> monsterStateArray;
    public NativeArray<Vector3> monsterPosArray;

    public Vector3 viewerPosition;

    public void Execute(int index)
    {
        float sqrDistance = Vector3.SqrMagnitude(monsterPosArray[index]/8-viewerPosition);
        if(sqrDistance < 12*12){
            monsterStateArray[index] = 1;
        }else{
            monsterStateArray[index] = 0;
        }
    }
}
