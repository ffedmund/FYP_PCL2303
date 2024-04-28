using System.Collections.Generic;
using FYP;
using Unity.Multiplayer.Samples.Utilities;
using Unity.Netcode;
using UnityEngine;


[System.Serializable]
public struct EnvironmentPrefabData{
    public GameObject prefab;
    public GameObject networkPrefab;
}

[System.Serializable]
public struct EnvironmentData{
    public Vector3 pos;
    public Quaternion rotation; 
    public GameObject prefab;
    public int interactChance;
    public bool isNetworkObject;
}

public class EnvironmentGenerator: MonoBehaviour
{
    public const float coverage = 30;
    public bool isActive;
    public LayerMask layerMask;
    [Range(0,10)]
    public float rndOffsetRange;

    public EnvironmentData[] GenerateEnvironmentDataArray(BiomeSetting biomeSetting, short[] environmentMap, float[,] heightMap, Vector2 chunkPosition){
        if(isActive)
        {
            int mapChunkSize = MapGenerator.mapChunkSize;
            float scale = TerrainGenerationManager.scale;
            System.Random random = new System.Random(Mathf.RoundToInt(TerrainGenerationManager.mapGenerator.seed*100+chunkPosition.x*10+chunkPosition.y));
            List<EnvironmentData> environmentObjectDatas = new List<EnvironmentData>();
            for(int index = 0; index < environmentMap.Length; index++){
                if(environmentMap[index]>0){
                    int x = index % mapChunkSize;
                    int y = index / mapChunkSize;
                    float positionX = x + chunkPosition.x - mapChunkSize/2 + random.Next(-1,2)/5.0f;
                    float positionY = biomeSetting.meshHeightCurve.Evaluate(heightMap[x,y])*TerrainGenerationManager.mapGenerator.meshHeightMultiplier;
                    float positionZ = chunkPosition.y + mapChunkSize/2 - y + random.Next(-1,2)/5.0f;
                    Vector3 position = new Vector3(positionX,positionY,positionZ)*scale;
                    Quaternion randomRotation = Quaternion.Euler(0f, random.Next(0, 180), 0f);
                    EnvironmentPrefabData environmentPrefabData;
                    GameObject prefab;
                    short prefabIndex;
                    if (environmentMap[index] == 1 && biomeSetting.trees.Length > 0)
                    {
                        prefabIndex = (short)random.Next(0,biomeSetting.trees.Length);
                        environmentPrefabData = biomeSetting.trees[prefabIndex];
                    }
                    else if(environmentMap[index] == 2 && biomeSetting.stones.Length > 0)
                    {
                        prefabIndex = (short)random.Next(0,biomeSetting.stones.Length);
                        environmentPrefabData = biomeSetting.stones[prefabIndex];
                    }
                    else if(environmentMap[index] == 3 && biomeSetting.structures.Length > 0)
                    {
                        prefabIndex = (short)random.Next(0,biomeSetting.structures.Length);
                        environmentPrefabData = biomeSetting.structures[prefabIndex];
                    }else{
                        continue;
                    }

                    bool isNetworkObject = NetworkManager.Singleton && environmentPrefabData.networkPrefab;
                    prefab = isNetworkObject? environmentPrefabData.networkPrefab:environmentPrefabData.prefab;

                    if(prefab == null){
                        continue;
                    }

                    LimitedItemGenerator limitedItemGenerator = prefab.GetComponentInChildren<LimitedItemGenerator>();
                    InteractableScript interactableScript = prefab.GetComponentInChildren<InteractableScript>();
                    int interactChance = limitedItemGenerator ? limitedItemGenerator.remainItemAmount : interactableScript ? 1 : -1;

                    environmentObjectDatas.Add(new EnvironmentData{
                        prefab = prefab,
                        pos = position,
                        rotation = randomRotation,
                        interactChance = interactChance,
                        isNetworkObject = isNetworkObject
                    });
                }
            }
            return environmentObjectDatas.ToArray();
        }
        return null;
    }
}
