using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

[BurstCompile]
public struct MapDataJob : IJobParallelFor
{
    [ReadOnly]
    public NativeArray<MinimapColorBlock> colorBlocks;

    public NativeArray<float> noiseMap;
    public NativeArray<float4> colorMap;
    public NativeArray<short> environMap;
    public int mapChunkSize;
    public bool isCentre;
    public bool isWorldTree;
    public bool useFlatCenterMap;
    public float2 offset;

    public void Execute(int index)
    {
        int x = index % mapChunkSize;
        int y = index / mapChunkSize;
        float falloutValue = FalloffGenerator.GenerateFalloffValue(mapChunkSize,x,y,useFlatCenterMap);
        noiseMap[index] = (useFlatCenterMap && (isCentre || isWorldTree))?(1-falloutValue)*0.45f + falloutValue*noiseMap[index]:noiseMap[index];
        float currentHeight = noiseMap[index];

        colorMap[index] = colorBlocks[0].color;

        for(int i = 1; i < colorBlocks.Length; i++)
        {
            if(currentHeight >= colorBlocks[i].height)
            {
                colorMap[index] = colorBlocks[i].color;
            }
            else{
                break;
            }
        }

        float sampleX = (x + offset.x)/8;
        float sampleY = (y + offset.y)/8;
        float treeNoise = noise.cnoise(new float2(sampleX, sampleY)/2);
        float stoneNoise = noise.cnoise(new float2(sampleX, sampleY));
        float largeNoise = noise.cnoise(new float2(sampleY, sampleX)*2);

        if(treeNoise > 0.4f && currentHeight > 0.4f && currentHeight < 0.6f){
            environMap[index] = (short)((isCentre && falloutValue<0.2f)?0:1);
        }else if(stoneNoise > 0.65f && currentHeight > 0.45f && currentHeight < 0.65f){
            environMap[index] = 2;
        }else if(largeNoise > 0.45f && largeNoise < 0.6f && currentHeight > 0.5f && currentHeight < 0.6f && x%2 == y%2 && !(isCentre || isWorldTree)){
            environMap[index] = 3;
        }else{
            environMap[index] = 0;
        }
    }
}