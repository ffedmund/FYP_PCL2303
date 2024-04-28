using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;
using Unity.Jobs;
using Unity.Collections;
using Unity.Mathematics;

[System.Serializable]
public struct TerrainType{
    public string name;
    public float height;
    public Color color;
}

[System.Serializable]
public struct MinimapColorBlock{
    public float height;
    public float4 color;
}

//Store the map data for a new chunk
public struct MapData{
    public readonly BiomeData biome;
    public readonly float[,] heightMap;
    public readonly float4[] colorMap;
    public readonly short[] environmentMap;
    public readonly List<Vector3> enemyLairPositionArray;
    public readonly List<Vector3> neutralLairPositionArray;

    public MapData(BiomeData biome,float[,] heightMap, float4[] colorMap, short[] environmentMap, List<Vector3> enemyLairPositionArray, List<Vector3> neutralLairPositionArray){
        this.biome = biome;
        this.heightMap = heightMap;
        this.colorMap = colorMap;
        this.environmentMap = environmentMap;
        this.enemyLairPositionArray = enemyLairPositionArray;
        this.neutralLairPositionArray = neutralLairPositionArray;
    }
}

[RequireComponent(typeof(BiomeGenerator))]
public class MapGenerator : MonoBehaviour
{
    public enum DrawMode{
        NoiseMap,
        ColorMap,
        MeshMap,
        FalloutMap
    }
    public DrawMode drawMode;

    public Noise.NormalizeMode normalizeMode;

    public const int mapChunkSize = 45;//95
    public bool useFlatShading;
    [Range(0,6)]
    public int EditorlevelOfDetail;//Change the number of vertices in each row
    public float noiseScale;//Scale of the size on noise map

    public int octaves;//Increase the complexity
    [Range(0,1)]
    public float persistance;//Change the amplitude
    public float lacunarity;// Change the frequency

    public int seed;//Seed for random number
    public Vector2 offset;//the position in perlin map(255x255)

    public float meshHeightMultiplier;//Change the height for each vertice on the mesh
    public AnimationCurve meshHeightCurve;//Adjust the height of vertices

    public bool useFalloutMap;
    public bool useFlatCenterMap;
    public bool useEndlessTerrainScale;
    public bool generateTrees;
    public bool autoUpdate;

    float[,] falloutMap;
    Queue<MapThreadInfo<MeshData>> meshDataThreadInfoQueue = new Queue<MapThreadInfo<MeshData>>();
    [SerializeField]
    Transform[] editorMapTransformArray;
    readonly Vector2Int[] threeByThreeFilter = 
    {
        new Vector2Int(-1,-1), new Vector2Int(-1,0), new Vector2Int(-1,1),
        new Vector2Int(0,-1), new Vector2Int(0,0), new Vector2Int(0,1),
        new Vector2Int(1,-1), new Vector2Int(1,0), new Vector2Int(1,1)
    };

    BiomeGenerator biomeGenerator;

    void Awake() {
        seed = JoinGameManager.terrainSeed;
        biomeGenerator = GetComponent<BiomeGenerator>();
        falloutMap = FalloffGenerator.GenerateFalloffMap(mapChunkSize,useFlatCenterMap);
        if(editorMapTransformArray != null){
            foreach(Transform mapTransform in editorMapTransformArray){
                mapTransform.gameObject.SetActive(false);
            }
        }
        GetComponent<LandmarkGenerator>().GenerateLandmarkPosition(seed);
    }

    //Call by the MapEditorGenerator Class for testing
    public void DrawMapInEditor(){
        biomeGenerator = GetComponent<BiomeGenerator>();
        MapData mapData = GenerateMapData(Vector2.zero);
        MapDisplay mapDisplay = GetComponent<MapDisplay>();
        switch(drawMode){
            case DrawMode.NoiseMap:
                mapDisplay.DrawTexture(TextureGenerator.TextureFromHeightMap(mapData.heightMap));
                break;
            case DrawMode.ColorMap:
                mapDisplay.DrawTexture(TextureGenerator.TextureFromColorMap(mapData.colorMap,mapChunkSize,mapChunkSize));
                break;
            case DrawMode.MeshMap:
                mapDisplay.DrawMesh(mapData,MeshGenerator.GenerateTerrainMap(mapData.heightMap,meshHeightMultiplier,meshHeightCurve,EditorlevelOfDetail,useFlatShading),TextureGenerator.TextureFromColorMap(mapData.colorMap,mapChunkSize,mapChunkSize));
                break;
            case DrawMode.FalloutMap:
                mapDisplay.DrawTexture(TextureGenerator.TextureFromHeightMap(falloutMap));
                break;
            default:
                break;
        }
        if(generateTrees){
            // mapDisplay.DrawTree(mapData.treeMap, mapData.heightMap);
        }
    }

    public void RequestMapData(Vector2 centre,Action<MapData> callback){
        StartCoroutine(MapDataThread(centre, callback));
    }

    IEnumerator MapDataThread(Vector2 centre, Action<MapData> callback){
        MapData mapData = GenerateMapData(centre);
        yield return null;
        callback(mapData);
    }

    //Use thread to create mesh and store as data
    public void RequestMeshData(MapData mapData, int lod, Action<MeshData> callback){
        ThreadStart threadStart = delegate{
            MeshDataThread(mapData, lod, callback);
        };
        new Thread(threadStart).Start();
    }

    void MeshDataThread(MapData mapData,int lod, Action<MeshData> callback){
        // MeshData meshData = MeshGenerator.GenerateTerrainMap(mapData.heightMap,meshHeightMultiplier,meshHeightCurve,lod,useFlatShading);
        MeshData meshData = MeshGenerator.GenerateTerrainMap(mapData.biome, mapData.heightMap, meshHeightMultiplier, lod, useFlatShading);
        lock(meshDataThreadInfoQueue){
            meshDataThreadInfoQueue.Enqueue(new MapThreadInfo<MeshData>(callback,meshData));
        }
    }

    //Call callback function if the thread have finished data
    void Update(){
        if(meshDataThreadInfoQueue.Count > 0){
            for(int i = 0; i < meshDataThreadInfoQueue.Count; i++){
                MapThreadInfo<MeshData> threadInfo = meshDataThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }
    }

    /**
    * Using perlin noise map and other parameter to form a 2D array "heightMap"
    * Base on the Height create a color map for each pixel;
    **/

    MapData GenerateMapData(Vector2 centre){

        BiomeData biomeData = biomeGenerator.GetBiomeSetting(seed,centre);
        System.Random random = new System.Random(seed);
        NativeArray<float> noiseMap = Noise.GenerateRawNoiseMap(mapChunkSize, mapChunkSize, noiseScale, (int)seed, octaves, persistance, lacunarity, centre + offset);
        NativeArray<short> environMap = new NativeArray<short>(noiseMap.Length,Allocator.TempJob);
        NativeArray<float4> colorMap = new NativeArray<float4>(noiseMap.Length,Allocator.TempJob);
        NativeArray<MinimapColorBlock> colorBlocks = new NativeArray<MinimapColorBlock>(biomeGenerator.GetBiomeColorBlock(biomeData.index),Allocator.TempJob);
        float2 seedOffset = new float2(random.Next(-100000, 100000) + centre.x, random.Next(-100000, 100000) - centre.y);

        MapDataJob mapDataJob = new MapDataJob{
            noiseMap = noiseMap,
            environMap = environMap,
            colorMap = colorMap,
            colorBlocks = colorBlocks,
            offset = seedOffset,
            mapChunkSize = mapChunkSize,
            isCentre = centre == Vector2.zero,
            isWorldTree = centre == LandmarkGenerator.worldTreeChunkPosition,
            useFlatCenterMap = useFlatCenterMap
        };

        JobHandle jobHandle = mapDataJob.Schedule(noiseMap.Length, 32);
        jobHandle.Complete();

        float[,] noiseMap2D = new float[mapChunkSize,mapChunkSize];
        for(int y = 0; y < mapChunkSize; y++){
            for(int x = 0; x < mapChunkSize; x++){
                noiseMap2D[x,y] = noiseMap[y*mapChunkSize + x];
            }
        }
        float4[] colorMapArr = colorMap.ToArray();
        short[] environMapArr = environMap.ToArray();

        noiseMap.Dispose();
        colorMap.Dispose();
        environMap.Dispose();
        colorBlocks.Dispose();
        

        List<Vector3> enemyLairPositionArray = new List<Vector3>();
        List<Vector3> neutralLairPositionArray = new List<Vector3>();
        List<Vector3> possibleLairPos = GetPossibleLairPositions(noiseMap2D,centre,biomeData);
        LairPosGenerate(possibleLairPos,environMapArr,centre,enemyLairPositionArray,neutralLairPositionArray);
        // Debug.Log(centre +": "+ monsterLairCenters.Count);
        return new MapData(biomeData,noiseMap2D,colorMapArr,environMapArr,enemyLairPositionArray,neutralLairPositionArray);
    }

    List<Vector3> GetPossibleLairPositions(float[,] noiseMap, Vector2 centre, BiomeData biomeData){
        if(centre == Vector2.zero || centre == LandmarkGenerator.worldTreeChunkPosition){
            return new List<Vector3>();
        }
        List<Vector3> possibleLairPos = new List<Vector3>();
        float minHeight = Mathf.Max(0.3f,biomeData.biomeSetting.colorBlock[1].height); // minimum height for a lair
        float maxHeight = 0.6f; // maximum height for a lair

        for(int y = 1; y < mapChunkSize-1; y += 3){
            for(int x = 1; x < mapChunkSize-1; x += 3){
                float sum = 0;
                float max = 0;
                float min = 999;
                for(int i = 0; i < threeByThreeFilter.Length; i++){
                    float height = noiseMap[x+threeByThreeFilter[i].x,y+threeByThreeFilter[i].y];
                    sum += height;
                    max = (height>max)?height:max;
                    min = (height<min)?height:min;
                }

                float averageHeight = sum/threeByThreeFilter.Length;
                if(averageHeight > minHeight && averageHeight < maxHeight && max-min < 0.08f){
                    possibleLairPos.Add(new Vector3(x-mapChunkSize/2,biomeData.biomeSetting.meshHeightCurve.Evaluate(noiseMap[x,y])*meshHeightMultiplier,mapChunkSize/2-y));
                    // Debug.Log($"Chunk at {centre}, the {possibleMonsterLairPos.Count} possible lair: max_h:{max} min_h{min}");
                }
            }
        }

        return possibleLairPos;
    }

    void LairPosGenerate(List<Vector3> possibleLairPos, short[] environMapArr, Vector2 centre, List<Vector3> enemyLairPositionArray, List<Vector3> neutralLairPositionArray){
        if(centre == Vector2.zero || centre == LandmarkGenerator.worldTreeChunkPosition){
            return;
        }
        System.Random random = new System.Random(seed + (int)(centre.x * (centre.y - 7)));
        int diff = Mathf.RoundToInt(Vector2.Distance(centre,Vector2.zero))/(mapChunkSize-1);
        int maximumEnemyLairNumber = Mathf.Min(diff*1,6) + (diff > 0?4:0); // or however many lairs you want

        for(int i = 0; i < possibleLairPos.Count; i++)
        {   
            if(enemyLairPositionArray.Count > maximumEnemyLairNumber)
            {
                break;
            }
            if(random.Next(0,maximumEnemyLairNumber) < 1)
            {
                continue;
            }
            Vector3 pos = possibleLairPos[i];
            int x = (int)pos.x + mapChunkSize/2;
            int y = mapChunkSize/2 - (int)pos.y;
            int arrIndex = y * mapChunkSize + x;
            enemyLairPositionArray.Add(pos);
            environMapArr[arrIndex] = 0;
        }

        foreach(Vector3 pos in enemyLairPositionArray)
        {
            possibleLairPos.Remove(pos);
        }

        for(int i = 0; i < possibleLairPos.Count; i++)
        {
            if(random.Next(0,10) <= 1)
            {
                neutralLairPositionArray.Add(possibleLairPos[i]);
            }
        }
    }   

    //Thread data storage structrue
    struct MapThreadInfo<T>{
        public readonly Action<T> callback;//Function that get back the output
        public readonly T parameter;//Output after thread finish

        public MapThreadInfo(Action<T> callback, T parameter){
            this.callback = callback;
            this.parameter = parameter;
        }
    }

    //Prevent error input in Editor
    void OnValidate() {
        if(lacunarity < 1){
            lacunarity = 1;
        }
        if(octaves < 0){
            octaves = 0;
        }
        falloutMap = FalloffGenerator.GenerateFalloffMap(mapChunkSize, useFlatCenterMap);
    }
}
