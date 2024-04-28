using Unity.Mathematics;
using UnityEngine;

public static class FalloffGenerator
{
    public static float[,] GenerateFalloffMap(int size, bool useForFlatCenterMap){
        float[,] map = new float[size,size];

        for(int j = 0; j < size; j++){
            for(int i = 0; i < size; i++){
                float x = i / (float)size * 2 - 1;
                float y = j / (float)size * 2 - 1;

                float value = Mathf.Max(Mathf.Abs(x),Mathf.Abs(y));
                map[i,j] = Evaluate(value,useForFlatCenterMap?5:3.5f,useForFlatCenterMap?3f:2.2f);
            }
        }
        return map;
    }

    public static float GenerateFalloffValue(int size, int x, int y, bool useForFlatCenterMap){
        float sampleX = x / (float)size * 2 - 1;
        float sampleY = y / (float)size * 2 - 1;

        float value = math.max(math.abs(sampleX),math.abs(sampleY));
        return Evaluate(value,useForFlatCenterMap?3.5f:3.5f,useForFlatCenterMap?1.5f:2.2f);
    }

    static float Evaluate(float value,float a, float b){
        return math.clamp(math.pow(value,a)/(math.pow(value,a)+math.pow(b-b*value,a)),0,1);
    }
}
