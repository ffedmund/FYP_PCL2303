using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct LODInfo{
    public int lod;
    public float visibleDistanceThreshold;
    public bool useFullCollider;
}

public class LODMesh
{
    public Mesh mesh;
    public bool hasRequestedMesh;
    public bool hasMesh;
    public MeshData meshData;
    int lod;
    Vector2 coordinate;
    System.Action updateCallback;

    public LODMesh(int lod, System.Action updateCallback,Vector2 coordinate){
        this.lod = lod;
        this.updateCallback = updateCallback;
        this.coordinate = coordinate;
    }

    void OnMeshDataReceived(MeshData meshData){
        this.mesh = meshData.CreateMesh();
        this.meshData = meshData;
        hasMesh = true;
        updateCallback();
    }

    public void RequestMeshData(MapData mapData){
        hasRequestedMesh = true;
        TerrainGenerationManager.mapGenerator.RequestMeshData(mapData,lod,OnMeshDataReceived);
    }
}
