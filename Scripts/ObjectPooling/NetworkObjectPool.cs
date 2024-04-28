using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;

/// <summary>
/// This code reference is from unity sample code Boss Room, which implements very well. Our own code is ObjectPooler.cs
/// https://github.com/Unity-Technologies/com.unity.multiplayer.samples.coop/blob/v2.2.0/Assets/Scripts/Infrastructure/NetworkObjectPool.cs
/// </summary>
public class NetworkObjectPool : NetworkBehaviour
{
    public static NetworkObjectPool Singleton { get; private set; }

    [SerializeField]
    List<PoolConfigObject> PooledPrefabsList;

    HashSet<GameObject> m_Prefabs = new HashSet<GameObject>();

    Dictionary<GameObject, ObjectPool<NetworkObject>> m_PooledObjects = new Dictionary<GameObject, ObjectPool<NetworkObject>>();
    Dictionary<string, GameObject> m_PooledObjectIds = new Dictionary<string, GameObject>();

    public void Awake()
    {
        if (Singleton != null && Singleton != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Singleton = this;
        }
    }

    public override void OnNetworkSpawn()
    {
        // Registers all objects in PooledPrefabsList to the cache.
        // Debug.Log("Registers all objects in PooledPrefabsList to the cache.");
        foreach (var configObject in PooledPrefabsList)
        {
            RegisterPrefabInternal(configObject.Prefab, configObject.PrewarmCount, configObject.spawnId);
        }
    }

    IEnumerator GameSceneLoad()
    {
        while(SceneManager.GetActiveScene() != gameObject.scene)
        {
            yield return null;
        }
        foreach (var configObject in PooledPrefabsList)
        {
            RegisterPrefabInternal(configObject.Prefab, configObject.PrewarmCount, configObject.spawnId);
        }
        yield return null;
    }
    

    public override void OnNetworkDespawn()
    {
        // Unregisters all objects in PooledPrefabsList from the cache.
        foreach (var prefab in m_Prefabs)
        {
            // Unregister Netcode Spawn handlers
            NetworkManager.Singleton.PrefabHandler.RemoveHandler(prefab);
            m_PooledObjects[prefab].Clear();
        }
        m_PooledObjects.Clear();
        m_PooledObjectIds.Clear();
        m_Prefabs.Clear();
    }

    public void OnValidate()
    {
        for (var i = 0; i < PooledPrefabsList.Count; i++)
        {
            var prefab = PooledPrefabsList[i].Prefab;
            if (prefab != null)
            {
                Assert.IsNotNull(prefab.GetComponent<NetworkObject>(), $"{nameof(NetworkObjectPool)}: Pooled prefab \"{prefab.name}\" at index {i.ToString()} has no {nameof(NetworkObject)} component.");
            }
        }
    }

    public NetworkObject GetNetworkObject(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        var networkObject = m_PooledObjects[prefab].Get();

        var noTransform = networkObject.transform;
        noTransform.position = position;
        noTransform.rotation = rotation;

        return networkObject;
    }

    public NetworkObject GetNetworkObject(int index, Vector3 position, Quaternion rotation)
    {
        if(index > m_Prefabs.Count || index < 0){
            return null;
        }
        GameObject prefab = PooledPrefabsList[index].Prefab;
        if(prefab){
            var networkObject = m_PooledObjects[prefab].Get();

            var noTransform = networkObject.transform;
            noTransform.position = position;
            noTransform.rotation = rotation;

            return networkObject;
        }
        return null;
    }

    public NetworkObject GetNetworkObject(string id, Vector3 position, Quaternion rotation)
    {
        GameObject prefab = GetPrefab(id);
        if(prefab){
            var networkObject = m_PooledObjects[prefab].Get();

            var noTransform = networkObject.transform;
            noTransform.position = position;
            noTransform.rotation = rotation;

            return networkObject;
        }
        return null;
    }

    /// <summary>
    /// Return an object to the pool (reset objects before returning).
    /// </summary>
    public void ReturnNetworkObject(NetworkObject networkObject, GameObject prefab)
    {
        m_PooledObjects[prefab].Release(networkObject);
    }

    public GameObject GetPrefab(string id){
        return m_PooledObjectIds.ContainsKey(id)?m_PooledObjectIds[id]:null;
    }
    
    public int GetPrefabIndex(GameObject gameObject){
        return PooledPrefabsList.FindIndex(pooledPrefab => pooledPrefab.Prefab == gameObject);
    }

    void RegisterPrefabInternal(GameObject prefab, int prewarmCount, string spawnId)
    {
        NetworkObject CreateFunc()
        {
            return Instantiate(prefab).GetComponent<NetworkObject>();
        }

        void ActionOnGet(NetworkObject networkObject)
        {
            networkObject.gameObject.SetActive(true);
        }

        void ActionOnRelease(NetworkObject networkObject)
        {
            Debug.Log("Release NetObject");
            networkObject.gameObject.SetActive(false);
        }

        void ActionOnDestroy(NetworkObject networkObject)
        {
            Destroy(networkObject.gameObject);
        }

        m_Prefabs.Add(prefab);

        // Create the pool
        m_PooledObjects[prefab] = new ObjectPool<NetworkObject>(CreateFunc, ActionOnGet, ActionOnRelease, ActionOnDestroy, defaultCapacity: prewarmCount);

        //Remember Id
        if(spawnId != "" && spawnId != null){
            m_PooledObjectIds[spawnId] = prefab;
        }

        // Populate the pool
        var prewarmNetworkObjects = new List<NetworkObject>();
        for (var i = 0; i < prewarmCount; i++)
        {
            NetworkObject networkObject = m_PooledObjects[prefab].Get();
            // Debug.Log("" + networkObject.gameObject.name);
            prewarmNetworkObjects.Add(networkObject);
        }
        foreach (var networkObject in prewarmNetworkObjects)
        {
            m_PooledObjects[prefab].Release(networkObject);
        }

        // Register Netcode Spawn handlers
        NetworkManager.Singleton.PrefabHandler.AddHandler(prefab, new PooledPrefabInstanceHandler(prefab, this));
    }

}


[Serializable]
struct PoolConfigObject
{
    public GameObject Prefab;
    public string spawnId;
    public int PrewarmCount;
}

class PooledPrefabInstanceHandler : INetworkPrefabInstanceHandler
{
    GameObject m_Prefab;
    NetworkObjectPool m_Pool;

    public PooledPrefabInstanceHandler(GameObject prefab, NetworkObjectPool pool)
    {
        m_Prefab = prefab;
        m_Pool = pool;
    }

    NetworkObject INetworkPrefabInstanceHandler.Instantiate(ulong ownerClientId, Vector3 position, Quaternion rotation)
    {
        return m_Pool.GetNetworkObject(m_Prefab, position, rotation);
    }

    void INetworkPrefabInstanceHandler.Destroy(NetworkObject networkObject)
    {
        m_Pool.ReturnNetworkObject(networkObject, m_Prefab);
    }
}