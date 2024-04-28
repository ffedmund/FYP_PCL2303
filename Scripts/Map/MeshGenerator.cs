using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshGenerator
{
    //Generate a new mesh data
    public static MeshData GenerateTerrainMap(BiomeData biome, float[,] heightMap, float heightMultiplier, int levelOfDetail, bool useFlatShading){
        AnimationCurve biomeMehsHeightCurve = new AnimationCurve(biome.biomeSetting.meshHeightCurve.keys);
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);
        float topLeftX = (width-1)/-2f;
        float topLeftZ = (height-1)/2f;

        int meshSimplificationIncrement = (levelOfDetail == 0)?1:levelOfDetail*2;
        int verticesPerLine = (width-1)/meshSimplificationIncrement + 1;
        MeshData meshData = new MeshData(verticesPerLine,verticesPerLine,useFlatShading);
        int vertexIndex = 0;

        for(int y = 0; y < height; y += meshSimplificationIncrement){
            for(int x = 0; x < width; x += meshSimplificationIncrement){
                bool isCorner = (y == 0 && x == 0) || (y == 0 && x == width-1) || (y == height-1 &&  x == 0) || (y == height-1 && x == width-1);
                bool connectedBiomeDiff = (y == 0 && biome.connectBiomeIndex[1] != biome.index) 
                                                || (y == height-1 && biome.connectBiomeIndex[6] != biome.index) 
                                                || (x == 0 && biome.connectBiomeIndex[3] != biome.index) 
                                                || (x == width-1 && biome.connectBiomeIndex[4] != biome.index);
                float verticeHeight = (connectedBiomeDiff || isCorner? GetAverageHeight(isCorner,heightMap[x,y],y,x,height,width,biome) : biomeMehsHeightCurve.Evaluate(heightMap[x,y])) * heightMultiplier;
                meshData.vertices[vertexIndex] = new Vector3(topLeftX + x, verticeHeight, topLeftZ - y);                
                meshData.uvs[vertexIndex] = new Vector2(x/(float)width,y/(float)height);
                if(x < width - 1 && y < height -1){
                    meshData.AddTriangle(vertexIndex, vertexIndex+verticesPerLine+1, vertexIndex+verticesPerLine);
                    meshData.AddTriangle(vertexIndex+verticesPerLine+1, vertexIndex, vertexIndex+1);
                }
                vertexIndex++;
            }
        }
        meshData.ProcessMesh();

        return meshData;
    }

    static float GetAverageHeight(bool isCorner, float vertexHeight, int y, int x, int height, int width, BiomeData biome){
        float totalHeight = biome.biomeSetting.meshHeightCurve.Evaluate(vertexHeight);
        float num = 1;
        if(y == 0)
        {
            totalHeight += BiomeGenerator.instance.GetBiomeHeightCurve(biome.connectBiomeIndex[1]).Evaluate(vertexHeight);
            num++;
        }
        if(y == height-1)
        {
            totalHeight += BiomeGenerator.instance.GetBiomeHeightCurve(biome.connectBiomeIndex[6]).Evaluate(vertexHeight);
            num++;
        }
        if(x == 0)
        {
            totalHeight += BiomeGenerator.instance.GetBiomeHeightCurve(biome.connectBiomeIndex[3]).Evaluate(vertexHeight);
            num++;
            if(isCorner)
            {
                totalHeight += BiomeGenerator.instance.GetBiomeHeightCurve(biome.connectBiomeIndex[y == 0?0:5]).Evaluate(vertexHeight);
                num++;
            }
        }
        if(x == width - 1)
        {
            totalHeight += BiomeGenerator.instance.GetBiomeHeightCurve(biome.connectBiomeIndex[4]).Evaluate(vertexHeight);
            num++;
            if(isCorner)
            {
                totalHeight += BiomeGenerator.instance.GetBiomeHeightCurve(biome.connectBiomeIndex[y == 0?2:7]).Evaluate(vertexHeight);
                num++;
            }
        }
        return totalHeight/num;
    }

    public static MeshData GenerateTerrainMap(float[,] heightMap, float heightMultiplier, AnimationCurve _meshHeightCurve, int levelOfDetail, bool useFlatShading){
        AnimationCurve meshHeightCurve = new AnimationCurve(_meshHeightCurve.keys);
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);
        float topLeftX = (width-1)/-2f;
        float topLeftZ = (height-1)/2f;

        int meshSimplificationIncrement = (levelOfDetail == 0)?1:levelOfDetail*2;
        int verticesPerLine = (width-1)/meshSimplificationIncrement + 1;
        MeshData meshData = new MeshData(verticesPerLine,verticesPerLine,useFlatShading);
        int vertexIndex = 0;

        for(int y = 0; y < height; y += meshSimplificationIncrement){
            for(int x = 0; x < width; x += meshSimplificationIncrement){
                meshData.vertices[vertexIndex] = new Vector3(topLeftX + x, meshHeightCurve.Evaluate(heightMap[x,y])*heightMultiplier, topLeftZ - y);                
                meshData.uvs[vertexIndex] = new Vector2(x/(float)width,y/(float)height);
                if(x < width - 1 && y < height -1){
                    meshData.AddTriangle(vertexIndex, vertexIndex+verticesPerLine+1, vertexIndex+verticesPerLine);
                    meshData.AddTriangle(vertexIndex+verticesPerLine+1, vertexIndex, vertexIndex+1);
                }
                vertexIndex++;
            }
        }
        meshData.ProcessMesh();

        return meshData;
    }

}
