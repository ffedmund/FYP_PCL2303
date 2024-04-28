using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class WaterGenerator: MonoBehaviour{

    public bool isActive;
    [Range(1,100)]
    public float density;
    [SerializeField]
    float waterShowingDistance;
    public Transform waterPlane;

    // Dictionary<Vector2,List<Vector3>> waterCoordinateDict = new Dictionary<Vector2, List<Vector3>>();
    // Dictionary<Vector2,GameObject> waterPlaneDict = new Dictionary<Vector2, GameObject>();

    public void CreateWater(float[,] heightMap, int size, Vector2 chunkCoordinate, Transform chunk, float waterLevel){
        if(!isActive)return;

        // if(!waterCoordinateDict.ContainsKey(chunkCoordinate)){
            GameObject water = new GameObject
            {
                name = "water"
            };
            water.transform.SetParent(chunk);
            // water.transform.localPosition = Vector3.zero;
            // waterCoordinateDict[chunkCoordinate] = new List<Vector3>();

            for(int y = 1; y < heightMap.GetLength(1); y++){
                for(int x = 1; x < heightMap.GetLength(0); x++){
                    Vector3 position = new Vector3((chunkCoordinate.x+x-size/2-0.1f)*TerrainGenerationManager.scale,-0.5f,(chunkCoordinate.y+size/2-y-0.1f)*TerrainGenerationManager.scale);
                    Vector2 position2D = new Vector2(position.x,position.z);
                    // if(heightMap[x,y] < waterLevel && !waterPlaneDict.ContainsKey(position2D)){
                    if(heightMap[x,y] < waterLevel){
                        GameObject waterPlaneObject = Instantiate(waterPlane.gameObject,position,Quaternion.identity);
                        // GameObject waterPlaneObject = ObjectPool.instance.GetPoolObject(waterPlane.gameObject,position,Quaternion.identity);
                        waterPlaneObject.name = string.Format("Chunk[{0},{1}] Water({2},{3})",chunkCoordinate.x,chunkCoordinate.y,x,y);
                        waterPlaneObject.transform.localScale = new Vector3(1,10/TerrainGenerationManager.scale,1)*TerrainGenerationManager.scale/10;
                        waterPlaneObject.transform.SetParent(water.transform);
                        // waterCoordinateDict[chunkCoordinatePl].Add(position);
                        // waterPlaneDict.Add(position2D,wateraneObject);
                    }
                }
            }
            StaticBatchingUtility.Combine(water);
        // }
    }


    public void ClearWaterMap(){
        while(transform.childCount > 0){
            foreach(Transform child in transform){
                DestroyImmediate(child.gameObject);
            }
        }
    }
}