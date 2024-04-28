using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering;
using Unity.Jobs;
using UnityEngine.Jobs;
using Unity.Collections;
using System;
using DG.Tweening;
using Unity.Mathematics;

public class GrassGenerator : MonoBehaviour
{
    public GrassLOD[] lods;
    public Transform playerTransform;
    public int gridSize = 5;
    public LayerMask layer;
    public bool useCameraCull;
    private Vector2Int lastUpdatePlayerGridPos;

    private Transform cameraHolderTransform;
    private System.Random random;
    private Dictionary<Vector2Int, GrassTile> grassTiles = new Dictionary<Vector2Int, GrassTile>();
    private Dictionary<int,List<List<Matrix4x4>>> lodInstanceList = new Dictionary<int, List<List<Matrix4x4>>>();
    
    public void Setup(Transform localPlayer){
        playerTransform = localPlayer;
    }

    void Start(){
        random = new System.Random(0);
        cameraHolderTransform = Camera.main.transform.root.transform;
        int i = 0;
        foreach(GrassLOD grassLOD in lods){
            lodInstanceList.Add(i,new List<List<Matrix4x4>>());
            lodInstanceList[i].Add(new List<Matrix4x4>());
            i++;
        }
    }

    void Update()
    {
        if(playerTransform){
            GenerateGrass();
        }
    }

    void GenerateGrass()
    {
        Vector2 playerPos = new Vector2(playerTransform.position.x, playerTransform.position.z);
        Vector2Int playerGridPos = new Vector2Int(Mathf.RoundToInt(playerPos.x / gridSize), Mathf.RoundToInt(playerPos.y / gridSize));

        if(playerGridPos == lastUpdatePlayerGridPos){
            DrawGrass(playerGridPos);
            return;
        }

        // List to store the keys of the tiles that need to be removed
        List<Vector2Int> tilesToRemove = new List<Vector2Int>();

        // Check all the existing tiles and mark the ones outside the visible range for removal
        foreach (var tile in grassTiles)
        {
            float distance = Mathf.RoundToInt(Vector2.Distance(playerGridPos, tile.Key));
            if (distance > lods[lods.Length - 1].visibleDistanceThreshold)
            {
                tilesToRemove.Add(tile.Key);
            }
        }

        // Remove the tiles that are outside the visible range
        foreach (var tile in tilesToRemove)
        {
            grassTiles.Remove(tile);
        }

        Dictionary<Vector2,float> heightDict = new Dictionary<Vector2, float>();

        // Generate new tiles in the visible range
        for (int x = -lods[lods.Length - 1].visibleDistanceThreshold; x <= lods[lods.Length - 1].visibleDistanceThreshold; x++)
        {
            for (int y = -lods[lods.Length - 1].visibleDistanceThreshold; y <= lods[lods.Length - 1].visibleDistanceThreshold; y++)
            {
                Vector2Int gridPos = new Vector2Int(playerGridPos.x + x, playerGridPos.y + y);
                float distance = Vector2.Distance(playerGridPos, gridPos);

                // Determine the LOD level based on the distance
                int lodIndex = -1;
                for (int i = 0; i < lods.Length; i++)
                {
                    if (distance <= lods[i].visibleDistanceThreshold)
                    {
                        lodIndex = i;
                        break;
                    }
                }
                if(lodIndex == -1){
                    continue;
                }

                if (!grassTiles.ContainsKey(gridPos) || grassTiles[gridPos].lodIndex != lodIndex)
                {
                    // uint seed = grassTiles.ContainsKey(gridPos)?grassTiles[gridPos].seed:(uint)random.Next(1,10000);
                    grassTiles.Remove(gridPos);

                    // Perform raycasts at the four corners of the grid cell
                    Vector3 topLeft = new Vector3((gridPos.x - 0.5f) * gridSize, 1000f, (gridPos.y -0.5f) * gridSize);
                    Vector2 topLeft2D = new Vector2(topLeft.x,topLeft.z);
                    Vector3 topRight = new Vector3((gridPos.x + 0.5f) * gridSize, 1000f, (gridPos.y -0.5f) * gridSize);
                    Vector2 topRight2D = new Vector2(topRight.x,topRight.z);
                    Vector3 bottomLeft = new Vector3((gridPos.x - 0.5f) * gridSize, 1000f, (gridPos.y + 0.5f) * gridSize);
                    Vector2 bottomLeft2D = new Vector2(bottomLeft.x,bottomLeft.z);
                    Vector3 bottomRight = new Vector3((gridPos.x + 0.5f) * gridSize, 1000f, (gridPos.y + 0.5f) * gridSize);
                    Vector2 bottomRight2D = new Vector2(bottomRight.x,bottomRight.z);

                    RaycastHit hit;
                    float[] heights = {-1,-1,-1,-1};

                    if (!Physics.Raycast(new Vector3(gridPos.x * gridSize, 1000f, gridPos.y * gridSize), Vector3.down, out hit, 1000, layer) || hit.transform.tag != "Terrain")
                    {
                        continue;
                    }


                    if (!heightDict.ContainsKey(topLeft2D))
                    {
                        if (Physics.Raycast(topLeft, Vector3.down, out hit, 1000, layer) && hit.transform.tag == "Terrain")
                        {
                            heights[0] = hit.point.y;
                            heightDict.Add(topLeft2D,hit.point.y);
                        }
                    }else{
                        heights[0] = heightDict[topLeft2D];
                    }

                    if (!heightDict.ContainsKey(topRight2D))
                    {
                        if (Physics.Raycast(topRight, Vector3.down, out hit, 1000, layer) && hit.transform.tag == "Terrain")
                        {
                            heights[1] = hit.point.y;
                            heightDict.Add(topRight2D,hit.point.y);
                        }
                    }else{
                        heights[1] = heightDict[topRight2D];
                    }
                    if (!heightDict.ContainsKey(bottomLeft2D))
                    {
                        if (Physics.Raycast(bottomLeft, Vector3.down, out hit, 1000, layer) && hit.transform.tag == "Terrain")
                        {
                            heights[2] = hit.point.y;
                            heightDict.Add(bottomLeft2D,hit.point.y);
                        }
                    }else{
                        heights[2] = heightDict[bottomLeft2D];
                    }
                    if(!heightDict.ContainsKey(bottomRight2D)){
                        if (Physics.Raycast(bottomRight, Vector3.down, out hit, 1000, layer) && hit.transform.tag == "Terrain")
                        {
                            heights[3] = hit.point.y;
                            heightDict.Add(bottomRight2D,hit.point.y);
                        }
                    }else{
                        heights[3] = heightDict[bottomRight2D];
                    }

                    // Create a new NativeArray to hold the grass instances
                    NativeArray<Matrix4x4> tileGrassInstances = new NativeArray<Matrix4x4>(lods[lodIndex].density, Allocator.TempJob);

                    // Create a new job and assign the data
                    GenerateGrassJob grassJob = new GenerateGrassJob
                    {
                        gridSize = gridSize,
                        gridPos = gridPos,
                        x1 = heights[0],
                        x2 = heights[1],
                        x3 = heights[2],
                        x4 = heights[3],
                        tileGrassInstances = tileGrassInstances,
                        random = new Unity.Mathematics.Random(1),
                    };

                    // Schedule the job
                    JobHandle handle = grassJob.Schedule(lods[lodIndex].density, 64);
                    handle.Complete();

                    GrassTile grassTile = new GrassTile{
                        lodIndex = lodIndex,
                        tileGrassInstances = grassJob.tileGrassInstances.ToArray(),
                        // seed = seed
                    };
                    grassTiles.Add(gridPos, grassTile);
                    tileGrassInstances.Dispose();
                }
            }
        }
        // Draw the grass instances
        lastUpdatePlayerGridPos = playerGridPos;
        DrawGrass(playerGridPos);
    }

    void DrawGrass(Vector2Int playerGridPos){
        Vector2 cameraPos = new Vector2(cameraHolderTransform.position.x,cameraHolderTransform.position.z);
        Vector2 cameraForward = new Vector2(cameraHolderTransform.forward.x,cameraHolderTransform.forward.z);
        foreach (var tile in grassTiles)
        {
            float gridDotCamera =Vector2.Dot((tile.Key * gridSize - cameraPos).normalized,cameraForward);
            if(useCameraCull && gridDotCamera < -0.55f && tile.Key != playerGridPos){
                continue;
            }
            Graphics.DrawMeshInstanced(lods[tile.Value.lodIndex].mesh, 0, lods[tile.Value.lodIndex].material, tile.Value.tileGrassInstances, tile.Value.tileGrassInstances.Length, null, ShadowCastingMode.Off);
        }
        return;
    }

    

    void ShowRaycast(Vector2 gridPos){
        Vector3 topLeft = new Vector3((gridPos.x - 0.5f) * gridSize, 1000f, (gridPos.y -0.5f) * gridSize);
        Vector3 topRight = new Vector3((gridPos.x + 0.5f) * gridSize, 1000f, (gridPos.y -0.5f) * gridSize);
        Vector3 bottomLeft = new Vector3((gridPos.x - 0.5f) * gridSize, 1000f, (gridPos.y + 0.5f) * gridSize);
        Vector3 bottomRight = new Vector3((gridPos.x + 0.5f) * gridSize, 1000f, (gridPos.y + 0.5f) * gridSize);
        Debug.DrawRay(topLeft+Vector3.one*0.0001f, Vector3.down * 1001, Color.red);
        Debug.DrawRay(topRight, Vector3.down * 1001, Color.blue);
        Debug.DrawRay(bottomLeft, Vector3.down * 1001, Color.yellow);
        Debug.DrawRay(bottomRight, Vector3.down * 1001, Color.cyan);
    }
}