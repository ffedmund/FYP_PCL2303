using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

[BurstCompile]
public struct PerlinNoiseMapJob : IJobParallelFor
{
    public NativeArray<float> noiseMap;
    public float2 octaveOffsets;
    public float amplitude;
    public float frequency;
    public float scale;
    public int mapSize;

    public void Execute(int index)
    {
        int x = index % mapSize;
        int y = index / mapSize;
        float halfWidth = mapSize/2f;
        float halfHeight = mapSize/2f;

        float sampleX = (x-halfWidth + octaveOffsets.x)/scale*frequency;
        float sampleY = (y-halfHeight + octaveOffsets.y)/scale*frequency;
        float perlinValue = noise.cnoise(new float2(sampleX, sampleY));

        noiseMap[index] += perlinValue * amplitude;
    }
}
