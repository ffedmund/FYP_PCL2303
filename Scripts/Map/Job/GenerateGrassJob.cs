using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Jobs;

[BurstCompile]
struct GenerateGrassJob : IJobParallelFor
{
    public int gridSize;
    public Unity.Mathematics.Random random;
    public Vector2Int gridPos;
    public NativeArray<Matrix4x4> tileGrassInstances;
    public float x1;
    public float x2;
    public float x3;
    public float x4;

    public void Execute(int index)
    {
        float u = random.NextUInt(1,100)/100.0f;
        float v = random.NextUInt(1,100)/100.0f;
        float height = BilinearInterpolate(x1, x2, x3, x4, u, v);

        if(height < 3 || height > 6){
            return;
        }

        float randomRotation = random.NextFloat(0, 360);
        Quaternion rotation = Quaternion.Euler(0, randomRotation, 0);

        Vector3 worldPos = new Vector3(gridPos.x * gridSize + (u - 0.5f) * gridSize, height, gridPos.y * gridSize + (v - 0.5f) * gridSize);
        Matrix4x4 matrix = Matrix4x4.TRS(worldPos, rotation, new Vector3(1,0.7f,1));
        tileGrassInstances[index] = matrix;
    }

    private float BilinearInterpolate(float topLeft, float topRight, float bottomLeft, float bottomRight, float u, float v)
    {
        float top = math.lerp(topLeft, topRight, u);
        float bottom = math.lerp(bottomLeft, bottomRight, u);
        return math.lerp(top, bottom, v);
    }
}