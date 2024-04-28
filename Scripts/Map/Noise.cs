using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public static class Noise
{
    public enum NormalizeMode{
        Local,
        Global
    }

    public static float GeneratePerlinNoise(float scale, int seed, int octaves, float persistance, float lacunarity, Vector2 offset){
        System.Random random = new System.Random(seed);
        float2[] octaveOffsets = new float2[octaves];
        float maxPossibleHeight = 0;
        float amplitude = 1;
        float frequency = 1;
        for(int i = 0; i < octaves; i++){
            octaveOffsets[i] = new float2(random.Next(-100000, 100000) + offset.x, random.Next(-100000, 100000) - offset.y);
            maxPossibleHeight += amplitude;
            amplitude *= persistance;
        }

        float perlinValue = 0;
        scale = (scale <= 0)? 0.0001f:scale;
        amplitude = 1;
        for(int i = 0; i < octaves; i++){
            float sampleX = octaveOffsets[i].x/scale*frequency;
            float sampleY = octaveOffsets[i].y/scale*frequency;

            perlinValue += (Mathf.PerlinNoise(sampleX,sampleY)*2-1)*amplitude;
            amplitude *= persistance;
            frequency *= lacunarity;
        }
        float normalizeHeight = (perlinValue + 1)/(maxPossibleHeight*0.9f);
        perlinValue = Mathf.Clamp(normalizeHeight,0,int.MaxValue);
        // Debug.Log($"Max Possible Height {maxPossibleHeight}, Perlin Value: {perlinValue}");
        return perlinValue;
    }

    public static NativeArray<float> GenerateRawNoiseMap(int mapWidth, int mapHeight, float scale, int seed, int octaves, float persistance, float lacunarity, Vector2 offset){
        System.Random random = new System.Random(seed);
        NativeArray<float> noiseMapArray = new NativeArray<float>(mapWidth*mapHeight,Allocator.TempJob);
        float2 octaveOffsets;


        float maxPossibleHeight = 0;
        float amplitude = 1;
        float frequency = 1;
        scale = (scale <= 0)? 0.0001f:scale;

        for(int i = 0; i < octaves; i++){
            octaveOffsets = new float2(random.Next(-100000, 100000) + offset.x, random.Next(-100000, 100000) - offset.y);
            maxPossibleHeight += amplitude;
            

            PerlinNoiseMapJob noiseJob = new PerlinNoiseMapJob
            {
                noiseMap = noiseMapArray,
                octaveOffsets = octaveOffsets,
                scale = scale,
                amplitude = amplitude,
                frequency = frequency,
                mapSize = mapWidth,
            };
            JobHandle handle = noiseJob.Schedule(noiseMapArray.Length, 64);
            handle.Complete();
            
            amplitude *= persistance;
            frequency *= lacunarity;
        }

        for(int i = 0; i < noiseMapArray.Length; i++){
            float normalizeHeight = (noiseMapArray[i] + 1)/(maxPossibleHeight*0.9f);
            noiseMapArray[i] = math.clamp(normalizeHeight,0,int.MaxValue);
        }
        
        return noiseMapArray;
        // noiseMapArray.Dispose();
    }

    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, float scale, int seed, int octaves, float persistance, float lacunarity, Vector2 offset, NormalizeMode normalizeMode){
        
        float[,] noiseMap = new float[mapWidth,mapHeight];
        System.Random random = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];

        float maxPossibleHeight = 0;
        float amplitude = 1;
        float frequency = 1;

        for(int i = 0; i < octaves; i++){
            octaveOffsets[i] = new Vector2(random.Next(-100000, 100000) + offset.x, random.Next(-100000, 100000) - offset.y);
            maxPossibleHeight += amplitude;
            amplitude *= persistance;
        }

        scale = (scale <= 0)? 0.0001f:scale;

        float maxLocalNoiseHeight = float.MinValue;
        float minLocalNoiseHeight = float.MaxValue;

        float halfWidth = mapWidth/2f;
        float halfHeight = mapHeight/2f;        

        for(int y = 0; y < mapHeight; y++){
            for(int x = 0; x < mapWidth; x++){

                amplitude = 1;
                frequency = 1;
                float noiseHeight = 0;

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
                noiseMap[x,y] = noiseHeight;
            }
        }

        for(int y = 0; y < mapHeight; y++){
            for(int x = 0; x < mapWidth; x++){
                if(normalizeMode == NormalizeMode.Local){
                    noiseMap[x,y] = Mathf.InverseLerp(minLocalNoiseHeight,maxLocalNoiseHeight,noiseMap[x,y]);
                } else {
                    float normalizeHeight = (noiseMap [x,y] + 1)/(maxPossibleHeight*0.9f);
                    noiseMap[x,y] = Mathf.Clamp(normalizeHeight,0,int.MaxValue);
                }
            }
        }
        return noiseMap;  
    }
}
