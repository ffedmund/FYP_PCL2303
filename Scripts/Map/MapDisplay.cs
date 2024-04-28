using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDisplay : MonoBehaviour
{
   public Renderer textureRender;
   public MeshFilter meshFilter;
   public MeshRenderer meshRenderer;
   public Transform meshTransform;

   public GameObject[] treePrefabs;

   public WaterGenerator waterGenerator;
   public GrassSpawner grassSpawner;

     //Create Texture(Color)
     public void DrawTexture(Texture2D texture){
          textureRender.sharedMaterial.mainTexture = texture;
          textureRender.transform.localScale = new Vector3(texture.width,1,texture.height);
     }

     //Create Mesh(Shape)
     public void DrawMesh(MapData mapData,MeshData meshData, Texture2D texture2D){
          meshFilter.sharedMesh = meshData.CreateMesh();
          meshRenderer.sharedMaterial.mainTexture = texture2D;
          if(TryGetComponent(out MapGenerator mapGenerator)){
               meshTransform.localScale = mapGenerator.useEndlessTerrainScale?Vector3.one*TerrainGenerationManager.scale:meshTransform.localScale;
          }
          if(waterGenerator.transform.childCount>0){
               foreach(Transform child in  waterGenerator.transform){
                    DestroyImmediate(child.gameObject);
               }
          }
          waterGenerator.CreateWater(mapData.heightMap, MapGenerator.mapChunkSize, Vector2.zero,meshTransform, 0.25f);
     }

     public void DrawTree(short[] treeMap, float[,] heightMap){
          foreach (Transform transform in meshTransform)
          {
               DestroyImmediate(transform.gameObject);
          }
          // GetComponent<EnvironmentGenerator>().GenerateEnvironment(meshTransform,treeMap,heightMap,Vector3.zero);
     }
}
