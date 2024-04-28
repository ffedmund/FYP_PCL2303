using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class GrassSpawner : MonoBehaviour
{
    public bool isActive;
    public Mesh mesh;
    public Material material;
    [Range(0.1f, 100)]
    public float density;
    [Range(1, 20)]
    public int visibleRange;
    public AnimationCurve grassHeightCurve;

    private Camera cam;
    private Plane[] frustumPlanes;

    private Dictionary<Vector2, Matrix4x4> positionGrassDictionary = new Dictionary<Vector2, Matrix4x4>();
    private Dictionary<Vector2, TerrainChunk> terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();

    private List<Matrix4x4[]> matricesList = new List<Matrix4x4[]>();
    private Material matClone;
    private Vector2 previousCoordinate;

    private void Start()
    {
        cam = Camera.main;
    }

    public void SetUpTerrainChunkDictionary(Dictionary<Vector2, TerrainChunk> targetDictionary)
    {
        terrainChunkDictionary = targetDictionary;
    }

    public void UpdateVisibleGrass(Vector2 viewerPosition)
    {
        if (!isActive) return;

        int coordinateX = Mathf.RoundToInt(viewerPosition.x);
        int coordinateY = Mathf.RoundToInt(viewerPosition.y);

        if (new Vector2(coordinateX, coordinateY) != previousCoordinate)
        {
            frustumPlanes = GeometryUtility.CalculateFrustumPlanes(cam);
            matricesList.Clear();
            float diastem = Mathf.RoundToInt(1.0f / density * 1000) / 1000.0f;
            matClone = new Material(material);

            Matrix4x4[] matrices = new Matrix4x4[1023];

            for (float i = 0, y = coordinateY - visibleRange; y <= coordinateY + visibleRange - diastem; y += diastem)
            {
                for (float x = coordinateX - visibleRange; x <= coordinateX + visibleRange - diastem; x += diastem)
                {
                    float height = BilinearInterpolation(new Vector2(x, y));

                    if (height > 0.37f && height < 0.6f)
                    {
                        Vector2 positionKey = new Vector2(Mathf.FloorToInt(x * 1000) / 1000.0f, Mathf.FloorToInt(y * 1000) / 1000.0f);
                        Vector3 position = new Vector3(positionKey.x, grassHeightCurve.Evaluate(height) * TerrainGenerationManager.mapGenerator.meshHeightMultiplier, positionKey.y) * TerrainGenerationManager.scale;

                        Bounds b = new Bounds(position, Vector3.one);

                        if (GeometryUtility.TestPlanesAABB(frustumPlanes, b))
                        {
                            if (positionGrassDictionary.TryGetValue(positionKey, out Matrix4x4 matrix))
                            {
                                matrices[(int)i] = matrix;
                            }
                            else
                            {
                                Quaternion rotation = Quaternion.Euler(Random.Range(-20, 20), Random.Range(0, 359), 0);
                                Vector3 scale = new Vector3(0.6f, 0.5f, 0.6f);
                                // matrices[(int)i] = Matrix4x4.TRS(position, Quaternion.Euler(0, 0, 0), scale);
                                matrices[(int)i] = Matrix4x4.TRS(position + new Vector3(Random.Range(-1.0f, 1.0f), 0, Random.Range(-1.0f, 1.0f)), rotation, scale);

                                positionGrassDictionary[positionKey] = matrices[(int)i];
                            }

                            i++;

                            if (i >= 1023)
                            {
                                i = 0;
                                matricesList.Add(matrices);
                                matrices = new Matrix4x4[1023];
                            }
                        }
                    }
                }
            }
            matricesList.Add(matrices);
        }
    }

    private float BilinearInterpolation(Vector2 position)
    {
        float chunkSize = MapGenerator.mapChunkSize - 1;
        float topLeftX = chunkSize/-2f;
        float topLeftZ = chunkSize/2f;
        int terrainCoordinateX = Mathf.RoundToInt(position.x / chunkSize);
        int terrainCoordinateY = Mathf.RoundToInt(position.y / chunkSize);
        float[,] currentChunkHeightMap = terrainChunkDictionary[new Vector2(terrainCoordinateX, terrainCoordinateY)].mapData.heightMap;
        // float[,] rightNeighborHeightMap = terrainChunkDictionary[new Vector2(terrainCoordinateX + 1, terrainCoordinateY)].mapData.heightMap;
        // float[,] topNeighborHeightMap = terrainChunkDictionary[new Vector2(terrainCoordinateX, terrainCoordinateY + 1)].mapData.heightMap;
        // float[,] topRightNeighborHeightMap = terrainChunkDictionary[new Vector2(terrainCoordinateX + 1, terrainCoordinateY + 1)].mapData.heightMap;

        if (currentChunkHeightMap == null)
        {
            return -1;
        }

        float x = Mathf.Abs((position.x - terrainCoordinateX*chunkSize - topLeftX) % MapGenerator.mapChunkSize);
        float y = Mathf.Abs((topLeftZ - (position.y - terrainCoordinateY*chunkSize)) % MapGenerator.mapChunkSize);
        
        // Debug.Log($"Position({position.x},{position.y}), (x,y): ({x},{y}), height:{heightMap[(int)x,(int)y]}");
        int x1 = Mathf.FloorToInt(x) + 1;
        int y1 = Mathf.FloorToInt(y) + 1;
        int x0 = Mathf.FloorToInt(x);
        int y0 = Mathf.FloorToInt(y);

        float heightTL = 0.1f;
        float heightTR = 0.1f;
        float heightBL = 0.1f;
        float heightBR = 0.1f;

        try{
            heightTL = currentChunkHeightMap[x0, y0];
            heightTR = currentChunkHeightMap[x1, y0];
            heightBL = currentChunkHeightMap[x0, y1];
            heightBR = currentChunkHeightMap[x1, y1];
        }catch{
            // Debug.Log("Edge Grass");
        }

        float f_x_y0 = (x1 - x) / (x1 - x0) * heightTL + (x - x0) / (x1 - x0) * heightTR;
        float f_x_y1 = (x1 - x) / (x1 - x0) * heightBL + (x - x0) / (x1 - x0) * heightBR;

        return (y1 - y) / (y1 - y0) * f_x_y0 + (y - y0) / (y1 - y0) * f_x_y1;
    }

    public void UpdateChunkGrass(float[,] heightMap,Vector2 chunkCoordinate)
    {
        if (!isActive) return;

        frustumPlanes = GeometryUtility.CalculateFrustumPlanes(cam);

        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);

        matricesList.Clear();
        matClone = new Material(material);

        Vector3[] grassPosition = new Vector3[width*height];

        for (int i = 0, y = -height/2; y < height/2 + height%2; y++)
        {
            for (int x = -height/2; x < height/2 + height%2; x++)
            {
                Vector2 positionKey = new Vector2(Mathf.FloorToInt(x * 100000) / 100000.0f, Mathf.FloorToInt(y * 100000) / 100000.0f);
                float heightValue = BilinearInterpolation(new Vector2(x, y));
                // heightValue = heightMap[x,y];
                Vector3 position = new Vector3(positionKey.x, grassHeightCurve.Evaluate(heightValue) * TerrainGenerationManager.mapGenerator.meshHeightMultiplier, positionKey.y);
                grassPosition[i++] = position;
            }
        }

        // TestingCommand.SaveYValuesToFile(grassPosition,$"Y_Values_Grass{chunkCoordinate}.txt");
    }


    private void Update()
    {
        if (matClone != null)
        {
            foreach (Matrix4x4[] matrix4X4 in matricesList)
            {
                Graphics.DrawMeshInstanced(mesh, 0, matClone, matrix4X4, matrix4X4.Length, null, ShadowCastingMode.Off);
            }
        }
    }
}
