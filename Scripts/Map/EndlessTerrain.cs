using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using FYP;
using Unity.Netcode;
using UnityEngine;


/**
* Use to Generate Endless Terrain base on the View Distance
* Each Terrain is 240*240 size
* viewer is the center of the showing Terrain
**/
public class EndlessTerrain : PlayerSetupBehaviour
{
    public struct ClosestViewer
    {
        public Vector2 closestViewerPosition;
        public float sqrDistance;
        public bool isLocalViewer;
        public bool isLocalVisible;
    }

    const float viewerMoveThresholdForChunkUpdate = 4f;

    public LODInfo[] detailLevels;
    public Transform localViewer;

    // public static Vector2 viewerPosition;

    public HashSet<Transform> viewerSet = new HashSet<Transform>();
    public Dictionary<Transform, Vector2> previousViewerPositionDict = new Dictionary<Transform, Vector2>();

    int chunkSize; //44
    int chunkVisibleInViewDist; //number of chunk can see in one direction

    Dictionary<Vector2, TerrainChunk> terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();// keep the reference for all generated terrain chunks
    public static Dictionary<Vector2, ClosestViewer> chunkClosestViewerDictionary = new Dictionary<Vector2, ClosestViewer>();
    public static HashSet<TerrainChunk> terrainChunksVisibleLastUpdate = new HashSet<TerrainChunk>();
    public static float maxViewDist; // Adjust the number of showing terrain
    public static EndlessTerrain instance;

    public override void Setup(PlayerManager playerManager)
    {
        Transform viewer = playerManager.transform;
        localViewer = viewer;
        if(!viewerSet.Contains(viewer))
        {
            viewerSet.Add(viewer);
            previousViewerPositionDict.Add(viewer.transform, viewer.transform.position);
        }
        UpdateChunks();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (instance != null)
            this.enabled = false;
        instance = this;
        maxViewDist = detailLevels[detailLevels.Length - 1].visibleDistanceThreshold;
        chunkSize = MapGenerator.mapChunkSize - 1;
        chunkVisibleInViewDist = Mathf.RoundToInt(maxViewDist / chunkSize);

        //Generate Key Object Chunk Data
        terrainChunkDictionary.Add(LandmarkGenerator.worldTreeChunkPosition / chunkSize, new TerrainChunk(LandmarkGenerator.worldTreeChunkPosition / chunkSize, chunkSize, detailLevels, transform));
        terrainChunkDictionary.Add(LandmarkGenerator.crystalBallChunkPosition / chunkSize, new TerrainChunk(LandmarkGenerator.crystalBallChunkPosition / chunkSize, chunkSize, detailLevels, transform));
        terrainChunkDictionary.Add(LandmarkGenerator.newTowerChunkPosition / chunkSize, new TerrainChunk(LandmarkGenerator.newTowerChunkPosition / chunkSize, chunkSize, detailLevels, transform));
        terrainChunkDictionary.Add(LandmarkGenerator.oldTowerChunkPosition / chunkSize, new TerrainChunk(LandmarkGenerator.oldTowerChunkPosition / chunkSize, chunkSize, detailLevels, transform));
        if (localViewer != null)
        {
            UpdateChunks();
        }

        // if (NetworkManager.Singleton != null)
        // {
        //     NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback;
        //     NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectCallback;
        // }
    }

    void Update()
    {
        if(localViewer == null)
        {
            return;
        }
        bool isChunkUpdated = false;
        bool needCreatureUpdated = false;
        foreach (Transform viewer in viewerSet)
        {
            if(viewer == null)
            {
                continue;
            }
            
            Vector2 viewerPosition = new Vector2(viewer.position.x, viewer.position.z) / TerrainGenerationManager.scale;
            Vector2 previousViewerPosition = previousViewerPositionDict[viewer];
            float movingDistanceOfViewer = Vector2.Distance(viewerPosition, previousViewerPosition);
            
            if(!isChunkUpdated)
            {
                if (movingDistanceOfViewer > viewerMoveThresholdForChunkUpdate)
                {
                    UpdateChunks();
                    previousViewerPositionDict[viewer] = viewerPosition;
                    isChunkUpdated = true;
                }
            }

            needCreatureUpdated = needCreatureUpdated? true :  movingDistanceOfViewer > 1.2f; 
        }

        if(NetworkManager.Singleton.IsServer && needCreatureUpdated)
        {
            TerrainGenerationManager.creatureLairGenerator.UpdateCreatureState(viewerSet);
        }
    }

    void UpdateChunks()
    {
        GetAllVisibleChunks();
        UpdateVisibleChunks();
        // NavigationBaker.Bake();
    }

    void GetAllVisibleChunks()
    {
        chunkClosestViewerDictionary.Clear();

        float nonLocalViewerVisibleSqrDist = chunkSize * chunkSize / 2;
        foreach (Transform viewer in viewerSet)
        {
            if(viewer == null)
            {
                continue;
            }
            Vector2 viewerPosition = new Vector2(viewer.position.x, viewer.position.z) / TerrainGenerationManager.scale;
            int currentChunkCoordinateX = Mathf.RoundToInt(viewerPosition.x / chunkSize);
            int currentChunkCoordinateY = Mathf.RoundToInt(viewerPosition.y / chunkSize);
            bool isLocalViewer = viewer == localViewer;

            for (int yOffset = -chunkVisibleInViewDist; yOffset <= chunkVisibleInViewDist; yOffset++)
            {
                for (int xOffset = -chunkVisibleInViewDist; xOffset <= chunkVisibleInViewDist; xOffset++)
                {
                    Vector2 viewedChunkCoordinate = new Vector2(currentChunkCoordinateX + xOffset, currentChunkCoordinateY + yOffset);
                    float sqrDistance = Vector2.SqrMagnitude(viewedChunkCoordinate * chunkSize - viewerPosition);
                    
                    if(viewer != localViewer && sqrDistance >  nonLocalViewerVisibleSqrDist)
                    {
                        continue;
                    }

                    ClosestViewer tempViewerData = new ClosestViewer{
                        closestViewerPosition = viewerPosition,
                        sqrDistance = sqrDistance,
                        isLocalViewer = isLocalViewer,
                        isLocalVisible = chunkClosestViewerDictionary.ContainsKey(viewedChunkCoordinate)?chunkClosestViewerDictionary[viewedChunkCoordinate].isLocalVisible:isLocalViewer
                    };
                    if (chunkClosestViewerDictionary.ContainsKey(viewedChunkCoordinate) && sqrDistance < chunkClosestViewerDictionary[viewedChunkCoordinate].sqrDistance)
                    {
                        chunkClosestViewerDictionary[viewedChunkCoordinate] = tempViewerData;
                    }
                    else if (!chunkClosestViewerDictionary.ContainsKey(viewedChunkCoordinate))
                    {
                        chunkClosestViewerDictionary.Add(viewedChunkCoordinate, tempViewerData);
                    }
                }
            }
        }
    }


    void UpdateVisibleChunks() 
    {
        foreach (TerrainChunk chunk in terrainChunksVisibleLastUpdate)
        {
            if (!chunkClosestViewerDictionary.ContainsKey(chunk.chunkCoordinate))
            {
                chunk.SetVisible(false);
            }
        }

        foreach (KeyValuePair<Vector2, ClosestViewer> visibleChunkData in chunkClosestViewerDictionary)
        {
            Vector2 viewedChunkCoordinate = visibleChunkData.Key;
            if (terrainChunkDictionary.ContainsKey(viewedChunkCoordinate))
            {
                terrainChunkDictionary[viewedChunkCoordinate].UpdateTerrainChunk();
                terrainChunksVisibleLastUpdate.Add(terrainChunkDictionary[viewedChunkCoordinate]);
            }
            else
            {
                //If that chunk is not created, create a new chunk
                terrainChunkDictionary.Add(viewedChunkCoordinate, new TerrainChunk(viewedChunkCoordinate, chunkSize, detailLevels, transform));
            }
        }
    }


    private void OnClientDisconnectCallback(ulong clientId)
    {
        // throw new NotImplementedException();
        foreach(Transform viewer in viewerSet)
        {
            if(viewer == null)
            {
                viewerSet.Remove(viewer);
            }
        }
        foreach(Transform transform in previousViewerPositionDict.Keys)
        {
            if (transform == null)
            {
                previousViewerPositionDict.Remove(transform);
            }
        }
    }

    private void OnClientConnectedCallback(ulong clientId)
    {
        // throw new NotImplementedException();
        if(NetworkManager.Singleton.IsServer)
        {
            Transform viewer = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.transform;
            if(viewerSet.Contains(viewer))
            {
                return;
            }
            viewerSet.Add(viewer);
            previousViewerPositionDict.Add(viewer.transform, viewer.transform.position);
        }
    }

    public void UpdateViewerSet()
    {
        foreach(NetworkClient client in NetworkManager.Singleton.ConnectedClientsList)
        {
            Transform clientTransform = client.PlayerObject.transform;
            if(viewerSet.Contains(clientTransform))
            {
                continue;
            }
            viewerSet.Add(clientTransform);
            previousViewerPositionDict.Add(clientTransform, clientTransform.position);
        }
    }

    public Dictionary<Vector2, Material> GetTerrainMinimapMaterials()
    {
        Dictionary<Vector2, Material> minimapDict = new Dictionary<Vector2, Material>();
        foreach (Vector2 chunkCoordinate in terrainChunkDictionary.Keys)
        {
            Material material = terrainChunkDictionary[chunkCoordinate].GetMinimapMaterial();
            if (material == null)
                continue;
            minimapDict.Add(chunkCoordinate, material);
        }
        return minimapDict;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        if (!gameObject.scene.isLoaded)
        {
            terrainChunksVisibleLastUpdate.Clear();
            chunkClosestViewerDictionary.Clear();
        }
    }


    // Update is called once per frame
    // Calling the UpdateVisibleChunks every frame
    // void Update()
    // {
    //     if(viewer){
    //         viewerPosition = new Vector2(viewer.position.x, viewer.position.z)/TerrainGenerationManager.scale; 
    //         float movingDistanceOfViewer = Vector2.Distance(viewerPosition,previousViewerPosition);
    //         if(movingDistanceOfViewer > viewerMoveThresholdForChunkUpdate){
    //             UpdateVisibleChunks();
    //             NavigationBaker.Bake();
    //             previousViewerPosition = viewerPosition;
    //         }
    //         if(terrainChunkDictionary.ContainsKey (Vector2.zero)){
    //             TerrainGenerationManager.creatureLairGenerator.UpdateCreatureState(viewerPosition);
    //         }
    //     }
    // }

    //Check whether the terrain chunks is visible or not
    // void UpdateVisibleChunks(){
    //     //Calculate the viewer position in chunk coordinate
    //     int currentChunkCoordinateX = Mathf.RoundToInt(viewerPosition.x/chunkSize);
    //     int currentChunkCoordinateY = Mathf.RoundToInt(viewerPosition.y/chunkSize);

    //     HashSet<TerrainChunk> terrainChunksVisibleCurrentUpdate = new HashSet<TerrainChunk>();

    //     //Get all the chunk that are within the view distance and make them visible
    //     for(int yOffset = -chunkVisibleInViewDist; yOffset <= chunkVisibleInViewDist; yOffset++){
    //         for(int xOffset = -chunkVisibleInViewDist; xOffset <= chunkVisibleInViewDist; xOffset++){
    //             Vector2 viewedChunkCoordinate = new Vector2(currentChunkCoordinateX + xOffset, currentChunkCoordinateY + yOffset);

    //             if(terrainChunkDictionary.ContainsKey (viewedChunkCoordinate)){
    //                 terrainChunkDictionary[viewedChunkCoordinate].UpdateTerrainChunk();
    //                 terrainChunksVisibleCurrentUpdate.Add(terrainChunkDictionary[viewedChunkCoordinate]);
    //             }else{
    //                 //If that chunk is not created, create a new chunk
    //                 terrainChunkDictionary.Add(viewedChunkCoordinate, new TerrainChunk(viewedChunkCoordinate, chunkSize, detailLevels, transform));
    //             }
    //         }
    //     }

    //     //Unload the chunks that were visible in the previous frame but not in the current frame
    //     foreach(TerrainChunk chunk in terrainChunksVisibleLastUpdate){
    //         if(!terrainChunksVisibleCurrentUpdate.Contains(chunk)){
    //             chunk.SetVisible(false);
    //         }
    //     }

    //     //Update the list of chunks visible in the last frame
    //     terrainChunksVisibleLastUpdate = terrainChunksVisibleCurrentUpdate;
    // }

}
