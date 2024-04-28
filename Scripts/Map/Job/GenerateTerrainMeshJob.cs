using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

[BurstCompile]
public struct GenerateTerrainMeshJob : IJob {
    public NativeArray<float> heightMap;
    public int width;
    public int height;
    public int lod;
    public bool useFlatShading;
    public NativeArray<Vector3> vertices; // Assuming vertices is a Vector3 array
    public NativeArray<Vector2> uvs; // Assuming vertices is a Vector3 array
    public NativeArray<int> triangles;
    public NativeArray<Vector3>  flatShadedVertices;
    public NativeArray<Vector2> flatShadedUvs;

    public void Execute() {
        // Implement the logic of MeshGenerator.GenerateTerrainMap function here
        // Write the result to vertice
        float topLeftX = (width-1)/-2f;
        float topLeftZ = (height-1)/2f;
        int meshSimplificationIncrement = (lod == 0)?1:lod*2;
        int verticesPerLine = (width-1)/meshSimplificationIncrement + 1;
        int vertexIndex = 0;
        int triangleIndex = 0;



        for (int y = 0; y < height; y += meshSimplificationIncrement) {
            for (int x = 0; x < width; x += meshSimplificationIncrement) {
                float heightValue = heightMap[y * height + x];
                vertices[vertexIndex] = new Vector3(topLeftX + x, heightValue, topLeftZ - y);                
                uvs[vertexIndex] = new Vector2(x/(float)width,y/(float)height);
                if(x < width - 1 && y < height -1){
                    triangles[triangleIndex] = vertexIndex;
                    triangles[triangleIndex+1] = vertexIndex+verticesPerLine+1;
                    triangles[triangleIndex+2] = vertexIndex+verticesPerLine;
                    triangles[triangleIndex+3] = vertexIndex+verticesPerLine+1;
                    triangles[triangleIndex+4] = vertexIndex;
                    triangles[triangleIndex+5] = vertexIndex+1;
                    triangleIndex+=6;
                }
                vertexIndex++;
            }
        }

        if(useFlatShading){
            FlatShading();
        }
    }

    void FlatShading(){
        for(int i = 0; i<triangles.Length; i++){
            flatShadedVertices[i] = vertices[triangles[i]];
            flatShadedUvs[i] = uvs[triangles[i]];
            triangles[i] = i;
        }

        vertices = flatShadedVertices;
        uvs = flatShadedUvs;
    }
}