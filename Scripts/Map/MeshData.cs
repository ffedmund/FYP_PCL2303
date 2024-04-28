using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Store the mesh data and can create a new mesh by Create()
public class MeshData
{
    public Vector3[] vertices;
    public int[] triangles;
    public Vector2[] uvs;

    int triangleIndex;
    bool useFlatShading;

    public MeshData(int meshWidth, int meshHeight, bool useFlatShading){
        this.useFlatShading = useFlatShading;
        vertices = new Vector3[meshWidth*meshHeight];
        uvs = new Vector2[meshHeight*meshWidth];
        // triangles = new int[(meshWidth-1)*(meshHeight-1)*6];
        triangles = new int[(meshWidth)*(meshHeight)*12];
    }

    public void AddTriangle(int a, int b, int c){
        triangles[triangleIndex] = a;
        triangles[triangleIndex+1] = b;
        triangles[triangleIndex+2] = c;
        triangleIndex+=3;
    }

    void FlashShading(){
        Vector3[] flatShadedVertices = new Vector3[triangles.Length];
        Vector2[] flatShadedUvs = new Vector2[triangles.Length];

        for(int i = 0; i<triangles.Length; i++){
            flatShadedVertices[i] = vertices[triangles[i]];
            flatShadedUvs[i] = uvs[triangles[i]];
            triangles[i] = i;
        }

        vertices = flatShadedVertices;
        uvs = flatShadedUvs;
    }

    public void ProcessMesh(){
        if(useFlatShading){
            FlashShading();
        }
    }

    public Mesh CreateMesh(){
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
        return mesh;
    }

    // Vector3[] CalculateNormals(){
    //     Vector3[] vertexNormals = new Vector3[vertices.Length];
    //     int triangleCount = triangles.Length/3;
    //     for(int i = 0; i < triangleCount; i++){
    //         int normalTriangleIndex = i*3;
    //         int vertexIndexA = triangles[normalTriangleIndex];
    //         int vertexIndexB = triangles[normalTriangleIndex + 1];
    //         int vertexIndexC = triangles[normalTriangleIndex + 2];
            
    //         Vector3 triangleNormal = SurfaceNormalFromIndices(vertexIndexA,vertexIndexB,vertexIndexC);

    //         vertexNormals[vertexIndexA] += triangleNormal;
    //         vertexNormals[vertexIndexB] += triangleNormal;
    //         vertexNormals[vertexIndexC] += triangleNormal;
    //     }

    //     for(int i = 0; i < vertexNormals.Length; i++){
    //         vertexNormals[i].Normalize();
    //     }
    //     return vertexNormals;
    // }

    // Vector3 SurfaceNormalFromIndices(int indexA, int indexB, int indexC){
    //     Vector3 pointA = vertices[indexA];
    //     Vector3 pointB = vertices[indexB];
    //     Vector3 pointC = vertices[indexC];

    //     Vector3 sideAB = pointB - pointA;
    //     Vector3 sideAC = pointC - pointA;

    //     return Vector3.Cross(sideAB,sideAC).normalized;
    // }
}

