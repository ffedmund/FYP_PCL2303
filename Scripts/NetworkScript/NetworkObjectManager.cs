using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;
using System.Linq;
using FYP;

public class NetworkObjectManager : NetworkBehaviour {
    public static NetworkObjectManager Singleton { get; internal set; }
    // public List<Vector3> objectKeys = new List<Vector3>();
    Dictionary<Vector2,ObjectData> spawnedDataDict = new Dictionary<Vector2, ObjectData>();

    private void Awake()
    {
        if (Singleton == null)
        {
            Singleton = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public bool IsNetworkObject(Vector3 position){
        return spawnedDataDict.ContainsKey(position);
    }

    Vector2 GetKey(Vector3 position){
        return new Vector2(Mathf.FloorToInt(position.x * 10000)/10000.0f,Mathf.FloorToInt(position.z * 10000)/10000.0f);
    }

    public void CreateNetworkObjectHandler(Item item, Vector3 position, float despawnTime = 180, bool showParticle = false)
    {
        if(NetworkItemReferences.Singleton)
        {
            Debug.Log("Create Item: " + item.itemName + "id: " + item.itemID);
            int itemId = NetworkItemReferences.Singleton.GetId(item);
            SpawnItemDropRPC(itemId, position, despawnTime, showParticle);
        }
    }   

    [Rpc(SendTo.Server)]
    public void ObjectSpawnRPC(ulong sourceNetworkObjectId, int spawnIndex, Vector3 position, Quaternion rotation)
    {   
        Debug.Log($"Server Received the RPC on NetworkObject #{sourceNetworkObjectId} Request Object Spawn at {position}");
        Vector2 key = GetKey(position);
        if(spawnedDataDict.ContainsKey(key) && (spawnedDataDict[key].isActive || spawnedDataDict[key].isDisable))
        {
            return;
        }

        NetworkObject poolObject = NetworkObjectPool.Singleton.GetNetworkObject(spawnIndex,position,rotation);
        if(poolObject)
        { 
            ObjectData objectData = new ObjectData{
                isActive = true,
                networkObject = poolObject,
                height = position.y,
                spawnIndex=spawnIndex,
            };
            if(spawnedDataDict.ContainsKey(key)){
                spawnedDataDict[key] = objectData;
            }else{
                spawnedDataDict.Add(key,objectData);
            }
            poolObject.Spawn();
        }
    }

    [Rpc(SendTo.Server)]
    void SpawnItemDropRPC(int itemId, Vector3 position, float despawnTime = -1, bool showParticle = false){
        Debug.Log("Create network object with item id " + itemId);
        NetworkObject poolObject = NetworkObjectPool.Singleton.GetNetworkObject("item_drop",position,Quaternion.identity);
        if(poolObject == null){return;}
        NetworkItem networkItem = poolObject.GetComponentInChildren<NetworkItem>();
        poolObject.Spawn();
        if(despawnTime > 0){
            networkItem.SetDestroyTime(despawnTime);
        }
        networkItem.SetItemId(itemId);
        networkItem.SetParticle(showParticle);
    }

    [Rpc(SendTo.Server)]
    public void ObjectDespawnRPC(ulong sourceNetworkObjectId, Vector3 position){
        Debug.Log($"Server Received the RPC on NetworkObject #{sourceNetworkObjectId} Request Object Despawn at {position}");
        Vector2 key = GetKey(position);
        if(spawnedDataDict.ContainsKey(key) && spawnedDataDict[key].isActive){
            spawnedDataDict[key].networkObject.Despawn();  
            ObjectData newObjectData = new ObjectData{
                networkObject = null,
                height = spawnedDataDict[key].height,
                isActive = false,
                spawnIndex = spawnedDataDict[key].spawnIndex,
            };
            spawnedDataDict[key] = newObjectData;
        }
    }

    [Rpc(SendTo.Server)]
    public void DisableObjectDataRPC(ulong sourceNetworkObjectId, Vector3 position){
        Debug.Log($"Server Received the RPC on NetworkObject #{sourceNetworkObjectId} Request Object Update at {position}");

        Vector2 key = GetKey(position);
        if(spawnedDataDict.ContainsKey(key)){
            ObjectData newObjectData = new ObjectData{
                networkObject = spawnedDataDict[key].networkObject,
                height = spawnedDataDict[key].height,
                isActive = false,
                spawnIndex = spawnedDataDict[key].spawnIndex,
                isDisable = true
            };
            spawnedDataDict[key] = newObjectData;
        }
    }

    [System.Serializable]
    public struct ObjectData{
        public NetworkObject networkObject;
        public float height;
        public bool isActive;
        public int spawnIndex;
        public bool isDisable;
    }
}