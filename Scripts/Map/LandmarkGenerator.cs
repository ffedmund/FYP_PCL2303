using Unity.Netcode;
using UnityEngine;

public class LandmarkGenerator : MonoBehaviour {
    [SerializeField] GameObject worldTreePrefab;
    [SerializeField] GameObject crystalBallRemainsPrefab;
    [SerializeField] GameObject crystalBallPrefab;
    [SerializeField] GameObject newTowerPrefab;
    [SerializeField] GameObject oldTowerPrefab;

    public static Vector2 worldTreeChunkPosition;
    public static Vector2 crystalBallChunkPosition;
    public static Vector2 newTowerChunkPosition;
    public static Vector2 oldTowerChunkPosition;

    static bool worldTreeCreated;

    public void GenerateLandmarkPosition(int seed){
        worldTreeCreated = false;

        int chunkSize = MapGenerator.mapChunkSize - 1;
        System.Random random = new System.Random(seed);
        Vector2 nsDirChunkPosition = new Vector2(0,random.Next(3,5)) * (random.Next(0,2)==0?-1:1) * chunkSize;
        Vector2 weDirChunkPosition = new Vector2(random.Next(3,5),0) * (random.Next(0,2)==0?-1:1) * chunkSize;
        int rndIndex = random.Next(0,2);

        worldTreeChunkPosition = new Vector2(random.Next(1,3),random.Next(1,3))*(random.Next(0,2)==0?-1:1)*chunkSize;
        do{
            crystalBallChunkPosition = new Vector2(random.Next(3,5),random.Next(3,5))*(random.Next(0,2)==0?-1:1)*chunkSize;
        }while(crystalBallChunkPosition == nsDirChunkPosition || crystalBallChunkPosition == weDirChunkPosition);
        newTowerChunkPosition = rndIndex == 1?nsDirChunkPosition:weDirChunkPosition;
        oldTowerChunkPosition = rndIndex == 1?weDirChunkPosition:nsDirChunkPosition;
    }

    public bool GenerateLandmark(Vector2 chunkPosition, Vector3 position)
    {
        GameObject prefab = null;
        string buildingName = "";
        if(chunkPosition == worldTreeChunkPosition && !worldTreeCreated)
        {
            prefab = worldTreePrefab;
            position = Vector3.zero;
            buildingName = "World Tree";
        }
        if(chunkPosition == crystalBallChunkPosition)
        {
            prefab = crystalBallRemainsPrefab;
            buildingName = "Crystalball Remains";
        }
        if(chunkPosition == newTowerChunkPosition)
        {
            prefab = newTowerPrefab;
            buildingName = "New Tower";
        }
        if(chunkPosition == oldTowerChunkPosition)
        {
            prefab = oldTowerPrefab;
            buildingName = "Old Tower";
        }

        if(prefab == null)
        {
            return false;
        }
        GameObject landmark = Instantiate(prefab, (position + new Vector3(chunkPosition.x,0,chunkPosition.y))  * TerrainGenerationManager.scale, Quaternion.identity);
        if(prefab == crystalBallRemainsPrefab)
        {
            landmark.transform.rotation = Quaternion.Euler(new Vector3(-90,0,0));
            Vector3 crystalBallPivotPosition = landmark.transform.Find("CrystalBallPivot").position;
            if(crystalBallPrefab.GetComponent<NetworkObject>())
            {
                if(NetworkManager.Singleton.IsServer)
                {
                    NetworkObject networkObject = Instantiate(crystalBallPrefab,crystalBallPivotPosition,Quaternion.identity).GetComponent<NetworkObject>();
                    networkObject.Spawn(true);
                }
            }
            else
            {
                Instantiate(crystalBallPrefab,crystalBallPivotPosition,Quaternion.identity);
            }
        }
        
        BuildingLocationManager.Location location = new BuildingLocationManager.Location
        {
            building = landmark,
            buildingName = buildingName,
            isNPCLocation = false,
            residentNPCArray = null
        };
        BuildingLocationManager.instance.locationList.Add(location);
        return true;
    }
}