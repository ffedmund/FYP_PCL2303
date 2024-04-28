using System.Collections;
using Unity.Netcode;
using UnityEngine;
using FYP;
using System.Collections.Generic;
using Unity.AI.Navigation;

public class TerrainMesh : MonoBehaviour
{
    [SerializeField] MeshRenderer _meshRenderer;
    [SerializeField] MeshFilter _meshFilter;
    [SerializeField] MeshCollider _meshCollider;
    [SerializeField] MeshRenderer _minimapTex;
    [SerializeField] ParticleAdder _particleAdder;
    [SerializeField] EnvironmentData[] environmentDatas;

    Dictionary<int, GameObject> interactableTrackingDict = new Dictionary<int, GameObject>();
    List<NetworkObject> networkObjects = new List<NetworkObject>();
    List<Vector3> enemyLairPositionList;

    Transform chunksRoot;
    
    float[,] heightMap;
    float waterLevel;

    bool chunkObjectVisible;
    bool chunkObjectCreate;

    Coroutine objectCreateCoroutine;

    public void OnCreate(string name, Vector3 scale, Transform parent, Material minimapTexture, Material terrainMaterial, EnvironmentData[] environmentDatas, MapData mapData)
    {
        gameObject.name = name;
        transform.localScale = scale;
        _minimapTex.transform.localScale = new Vector3(-1, 0, 1) * TerrainGenerationManager.scale * MapGenerator.mapChunkSize / 10 / scale.x;
        _particleAdder.transform.localScale = new Vector3(0.95f, 1, 0.95f) * TerrainGenerationManager.scale * MapGenerator.mapChunkSize / 10 / scale.x;
        _minimapTex.material = minimapTexture;
        _meshRenderer.sharedMaterial = terrainMaterial;
        _particleAdder.type = mapData.biome.biomeSetting.weatherParticleType;

        chunkObjectVisible = false;
        chunkObjectCreate = false;
        this.environmentDatas = environmentDatas;
        this.enemyLairPositionList = mapData.enemyLairPositionArray;
        this.heightMap = mapData.heightMap;
        this.waterLevel = mapData.biome.biomeSetting.waterLevel;
        this.interactableTrackingDict = new Dictionary<int, GameObject>();
        this.networkObjects = new List<NetworkObject>();

        if (mapData.biome.biomeSetting.removeGrass)
            gameObject.tag = "Untagged";
        if (parent != null)
        {
            chunksRoot = parent;
            transform.SetParent(chunksRoot);
        }
    }

    public void UpdateLOD(int lodIndex)
    {
        switch (lodIndex)
        {
            case 0:
                if (!chunkObjectCreate)
                {
                    objectCreateCoroutine = StartCoroutine(CreateChunkObjectsCoroutine());
                    TerrainGenerationManager.creatureLairGenerator.GenerateCreatureLair(enemyLairPositionList, transform);
                    TerrainGenerationManager.waterGenerator.CreateWater(heightMap, MapGenerator.mapChunkSize, new Vector2(transform.position.x,transform.position.z)/TerrainGenerationManager.scale, transform, waterLevel);
                    chunkObjectCreate = true;
                    chunkObjectVisible = true;
                }
                else if(!chunkObjectVisible)
                {
                    SetChildsVisibility(true);
                    chunkObjectVisible = true;
                }

                break;
            default:
                if (chunkObjectCreate && chunkObjectVisible)
                {
                    _meshCollider.sharedMesh = null;
                    SetChildsVisibility(false);
                    chunkObjectVisible = false;
                }

                break;
        }
    }

    public void UpdateCollider(Mesh mesh)
    {
        _meshCollider.sharedMesh = mesh;
    }

    public void UpdateTerrain(Mesh mesh)
    {
        _meshFilter.mesh = mesh;
    }

    public void Despawn()
    {
        if(environmentDatas != null)
            UpdateChunkEnvironmentData();
        StartCoroutine(ClearMeshAndReturn());
    }

    IEnumerator CreateChunkObjectsCoroutine()
    {
        Dictionary<GameObject,GameObject> environmentBatchRootDict = new Dictionary<GameObject,GameObject>();
        interactableTrackingDict = new Dictionary<int, GameObject>();
        networkObjects = new List<NetworkObject>();
        for (int i = 0; i < environmentDatas.Length; i++)
        {
            EnvironmentData data = environmentDatas[i];
            GameObject prefab = data.prefab;
            if (prefab == null)
            {
                continue;
            }

            GameObject environmentObject;
            if (data.isNetworkObject)
            {
                if (!NetworkManager.Singleton.IsServer)
                {
                    continue;
                }
                environmentObject = Instantiate(prefab, data.pos, data.rotation).gameObject;
                NetworkObject networkObject = environmentObject.GetComponent<NetworkObject>();
                LimitedItemGenerator limitedItemGenerator = networkObject.GetComponentInChildren<LimitedItemGenerator>();
                if (limitedItemGenerator)
                {
                    limitedItemGenerator.remainItemAmount = data.interactChance;
                    limitedItemGenerator.UpdateInteractVisibility();
                }
                networkObject.Spawn();
                networkObjects.Add(networkObject);
            }
            else
            {
                environmentObject = ObjectPool.instance && ObjectPool.instance.ContainPrefab(prefab) ? ObjectPool.instance.GetPoolObject(prefab, data.pos, data.rotation) : Instantiate(prefab, data.pos, data.rotation);
                if (environmentObject == null || environmentObject.GetComponent<NetworkObject>())
                {
                    Debug.LogWarning($"[Terrain Mesh {gameObject.name}] Error on Create {prefab} [Network Error:{environmentObject.GetComponent<NetworkObject>()}]");
                    continue;
                }

                if (data.interactChance > 1)
                {
                    LimitedItemGenerator limitedItemGenerator = environmentObject.GetComponentInChildren<LimitedItemGenerator>();
                    if (limitedItemGenerator)
                    {
                        limitedItemGenerator.remainItemAmount = data.interactChance;
                        limitedItemGenerator.UpdateInteractVisibility();
                    }
                }

                //Batch Object
                GameObject batchRoot = environmentBatchRootDict.ContainsKey(prefab)? environmentBatchRootDict[prefab]:new GameObject{
                    name = prefab.name + "_batch_root",
                };
                if(!environmentBatchRootDict.ContainsKey(prefab))
                {
                    environmentBatchRootDict.Add(prefab, batchRoot);
                    batchRoot.transform.SetParent(transform);
                    batchRoot.transform.localPosition = Vector3.zero;
                }
                environmentObject.transform.SetParent(batchRoot.transform);
            }

            if (data.interactChance > 0)
            {
                int dataPosition = i;
                interactableTrackingDict.Add(dataPosition, environmentObject);
            }

            yield return new WaitForEndOfFrame();
        }

        foreach (GameObject batchRoot in environmentBatchRootDict.Values)
        {
            StaticBatchingUtility.Combine(batchRoot);
        }

        yield return null;
    }

    IEnumerator DestroyChunkObjectsCoroutine()
    {   

        for (int i = transform.childCount - 1; i >= 2; i--)
        {
            GameObject child = transform.GetChild(i).gameObject;

            if (child.TryGetComponent(out PoolObjectConfig poolObjectConfig))
            {
                ObjectPool.instance.ReturnObjectToPool(poolObjectConfig.poolPrefab, child);
            }
            else
            {
                Destroy(child);
            }
            yield return new WaitForEndOfFrame();
        }

        foreach(NetworkObject networkObject in networkObjects)
        {
            if(networkObject != null)
            {
                networkObject.Despawn();
            }
            yield return new WaitForEndOfFrame();
        }
        yield return null;
    }

    void UpdateChunkEnvironmentData()
    {
        foreach(KeyValuePair<int, GameObject> kvp in interactableTrackingDict)
        {
            // Debug.Log("Environment Data Update");
            int dataPosition = kvp.Key;
            GameObject environmentObject = kvp.Value;
            if(environmentDatas.Length <= dataPosition)
            {
                Debug.LogWarning("[Out of Range] Array Length: " + environmentDatas.Length + " Position: " +  dataPosition + $" Chunk{gameObject.name}");
                continue;
            }
            EnvironmentData oldData = environmentDatas[dataPosition];
            if(environmentObject == null)
            {
                environmentDatas[dataPosition] = new EnvironmentData();
            }
            else
            {
                bool isNetworkObject = environmentObject.GetComponent<NetworkObject>() != null;
                MaterialPickUp materialPickUp = environmentObject.GetComponentInChildren<MaterialPickUp>();
                LimitedItemGenerator limitedItemGenerator = environmentObject.GetComponentInChildren<LimitedItemGenerator>();
                if(limitedItemGenerator)
                {
                    environmentDatas[dataPosition] = new EnvironmentData{
                        prefab = oldData.prefab,
                        pos = oldData.pos,
                        rotation = oldData.rotation,
                        isNetworkObject = isNetworkObject,
                        interactChance = limitedItemGenerator.remainItemAmount,
                    };
                    // Debug.Log("Environment Data Changed");
                }
                else if(materialPickUp == null)
                {
                    environmentDatas[dataPosition] = new EnvironmentData();
                }
                // else {Debug.Log("I Got Nothing!!!!");}
            }
        }
    }

    void SetChildsVisibility(bool visibility)
    {
        for (int i = transform.childCount - 1; i >= 2; i--)
        {
            GameObject child = transform.GetChild(i).gameObject;
            child.SetActive(visibility);
        }
    }

    private void OnDisable()
    {
        _meshFilter.sharedMesh = null;
        _meshCollider.sharedMesh = null;
        heightMap = null;
        environmentDatas = null;
        enemyLairPositionList = null;
        gameObject.name = "TerrainMesh";
        gameObject.tag = "Terrain";
    }


    IEnumerator ClearMeshAndReturn()
    {   
        if(objectCreateCoroutine != null)
        {
            StopCoroutine(objectCreateCoroutine);
            objectCreateCoroutine = null;
        }
        Debug.Log("Start Despawn");
        yield return StartCoroutine(DestroyChunkObjectsCoroutine());
        chunkObjectCreate = false;
        chunkObjectVisible = false;
        Debug.Log("Finish Despawn");
        ObjectPool.instance?.ReturnObjectToPool("terrain_mesh", gameObject);
        yield return null;
    }
}