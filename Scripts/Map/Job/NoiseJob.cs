using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

[BurstCompile]
public struct NoiseJob:IJobParallelFor
{
    public int mapSize; 
    public float scale; 
    public int octaves; 
    public float persistance;
    public float lacunarity;
    public Vector2 offset;
    public Noise.NormalizeMode normalizeMode;
    public NativeArray<float> noiseMapArr;
    [ReadOnly] public NativeArray<Vector2> octaveOffsets;
    public Unity.Mathematics.Random random;

    public void Execute(int index)
    {
        int y = index/mapSize;
        int x = index%mapSize;
        
        float maxPossibleHeight = 0;
        float amplitude = 1;
        float frequency = 1;
        float noiseHeight = 0;

        for(int i = 0; i < octaves; i++){
            maxPossibleHeight += amplitude;
            amplitude *= persistance;
        }

        scale = (scale <= 0)? 0.0001f:scale;

        float maxLocalNoiseHeight = float.MinValue;
        float minLocalNoiseHeight = float.MaxValue;

        float halfWidth = mapSize/2f;
        float halfHeight = mapSize/2f;        

        amplitude = 1;
        frequency = 1;

        for(int i = 0; i < octaves; i++){
            float sampleX = (x-halfWidth + octaveOffsets[i].x)/scale*frequency;
            float sampleY = (y-halfHeight + octaveOffsets[i].y)/scale*frequency;

            float perlinValue = Mathf.PerlinNoise(sampleX,sampleY)*2-1;
            noiseHeight += perlinValue*amplitude;
            amplitude *= persistance;
            frequency *= lacunarity;
        }
        if(noiseHeight > maxLocalNoiseHeight){
            maxLocalNoiseHeight = noiseHeight;
        }else if(noiseHeight < minLocalNoiseHeight){
            minLocalNoiseHeight = noiseHeight;
        }

        if(normalizeMode == Noise.NormalizeMode.Local){
            noiseMapArr[index] = Mathf.InverseLerp(minLocalNoiseHeight,maxLocalNoiseHeight,noiseHeight);
        } else {
            float normalizeHeight = (noiseHeight + 1)/(maxPossibleHeight*0.9f);
            noiseMapArr[index] = Mathf.Clamp(normalizeHeight,0,int.MaxValue);
        }
    }
}
