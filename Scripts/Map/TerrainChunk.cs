using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TerrainChunk
{
    public MapData mapData;
    public Vector2 chunkCoordinate;
    
    Vector2 position; //Position Without Scale
    Bounds bounds;

    LODInfo[] detailLevels;
    LODMesh[] lodMeshes;
    LODMesh collisionLODMesh;

    TerrainMesh terrainMesh;
    Material minimapMaterial;
    Vector3 meshObjectPosition;//Real World Position
    Vector3 meshObjectScale;
    Transform meshParent;

    EnvironmentData[] environmentDatas;

    bool mapDataReceived;
    bool isVisited = false;
    public int previousLODIndex = -1;

    public TerrainChunk(Vector2 coordinate, int size, LODInfo[] detailLevels,Transform parent){
        this.detailLevels = detailLevels;
       
        chunkCoordinate = coordinate;
        position = coordinate * size;
        bounds = new Bounds(position,Vector2.one*size);
        Vector3 positionV3 = new Vector3(position.x,0,position.y);

        meshObjectPosition = positionV3*TerrainGenerationManager.scale;
        meshObjectScale = Vector3.one * TerrainGenerationManager.scale;
        meshParent = parent;

        lodMeshes = new LODMesh[detailLevels.Length];
        for(int i = 0; i < detailLevels.Length; i++){
            lodMeshes[i] = new LODMesh(detailLevels[i].lod, UpdateTerrainChunk, coordinate);
            if(detailLevels[i].useFullCollider){
                collisionLODMesh = lodMeshes[i];
            }
        }

        TerrainGenerationManager.mapGenerator.RequestMapData(position, OnMapDataReceived);
    }

    void OnMapDataReceived(MapData mapData){
        // Debug.Log($"{position}Mapdata received");
        // Debug.Log($"{mapData.colorMap[0]}Mapdata received");
        mapDataReceived = true;
        this.mapData = mapData;
        bool containLandmark = TerrainGenerationManager.landmarkGenerator.GenerateLandmark(position, mapData.enemyLairPositionArray.Count == 0? Vector3.zero : mapData.enemyLairPositionArray[0]);
        if(containLandmark && mapData.enemyLairPositionArray.Count > 0)
        {
            mapData.enemyLairPositionArray.RemoveAt(0);
        }
        environmentDatas = TerrainGenerationManager.environmentGenerator.GenerateEnvironmentDataArray(mapData.biome.biomeSetting,mapData.environmentMap,mapData.heightMap,position);
        TerrainGenerationManager.creatureLairGenerator.AddChunkCreatures(mapData.enemyLairPositionArray,mapData.neutralLairPositionArray,position,mapData.biome.biomeSetting.lairSetting);
        GenerateMinimap();
        UpdateTerrainChunk();
    }

    public void UpdateTerrainChunk(){
        
        if(!mapDataReceived || !EndlessTerrain.chunkClosestViewerDictionary.ContainsKey(chunkCoordinate)){
            return;
        }
        EndlessTerrain.ClosestViewer closestViewer = EndlessTerrain.chunkClosestViewerDictionary[chunkCoordinate];
        Vector2 viewerPosition = closestViewer.closestViewerPosition;
        float viewerSqrDistanceFromNeearestEdge = bounds.SqrDistance(viewerPosition);
        bool visible = viewerSqrDistanceFromNeearestEdge <= EndlessTerrain.maxViewDist*EndlessTerrain.maxViewDist;
        // Debug.Log("Viewer distance between chunk " + chunkCoordinate.x+","+chunkCoordinate.y +" is: "+ Mathf.Sqrt(viewerSqrDistanceFromNeearestEdge));
        if(visible){
            int lodIndex = detailLevels[detailLevels.Length-1].lod; 
            for(int i = detailLevels.Length-2; i > -1; i--){
                if(viewerSqrDistanceFromNeearestEdge <= detailLevels[i].visibleDistanceThreshold*detailLevels[i].visibleDistanceThreshold){
                    lodIndex = detailLevels[i].lod;
                }else{
                    break;
                }
            }
            if(lodIndex != 0 && !closestViewer.isLocalViewer)
            {
                return;
            }

            isVisited = closestViewer.isLocalVisible;
            if(terrainMesh == null){
                SetTerrainMesh();
                EndlessTerrain.terrainChunksVisibleLastUpdate.Add(this);
            }  

            if(lodIndex != previousLODIndex)
            {
                LODMesh lodMesh = lodMeshes[lodIndex];
                if(lodMesh.hasMesh && closestViewer.isLocalVisible){
                    terrainMesh.UpdateTerrain(lodMesh.mesh);
                    previousLODIndex = lodIndex;
                }else if(!lodMesh.hasRequestedMesh){
                    lodMesh.RequestMeshData(mapData);
                    return;
                }

                if(lodIndex == 0){
                    if(collisionLODMesh.hasMesh){
                        terrainMesh.UpdateCollider(collisionLODMesh.mesh);
                    }
                    else if(!collisionLODMesh.hasRequestedMesh){
                        collisionLODMesh.RequestMeshData(mapData);
                        return;
                    }
                }
                terrainMesh.UpdateLOD(lodIndex);
            }
        }
        SetVisible(visible);
    }

    public void SetVisible(bool visible){
        if(!visible && terrainMesh){
            previousLODIndex = -1;
            terrainMesh.Despawn();
            terrainMesh = null;
        }
    }

    public Material GetMinimapMaterial(){
        if(isVisited){
            return minimapMaterial;
        }   
        return null;
    }

    void SetTerrainMesh(){
        ObjectPool.instance.GetPoolObject("terrain_mesh",meshObjectPosition,Quaternion.identity).TryGetComponent(out terrainMesh);
        string meshName = string.Format("Terrain Chunk [{0},{1}] Biome[{2}]",chunkCoordinate.x,chunkCoordinate.y,mapData.biome.index);
        BiomeSetting biomeSetting = mapData.biome.biomeSetting;
        terrainMesh.OnCreate(meshName, meshObjectScale, meshParent, minimapMaterial, biomeSetting.material, environmentDatas, mapData);
    }

    void GenerateMinimap(){
        // Create a new Material instance
        int mapChunkSize = MapGenerator.mapChunkSize;
        minimapMaterial = new Material(Shader.Find("Unlit/Texture"))
        {
            mainTexture = TextureGenerator.TextureFromColorMap(mapData.colorMap, mapChunkSize, mapChunkSize)
        };
    }
}
