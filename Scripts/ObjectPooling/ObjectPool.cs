using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [System.Serializable]
    public struct Pool{
        public string id;
        public GameObject prefab;
        public int size;
    }

    #region Singleton
    public static ObjectPool instance;

    private void Awake(){
        if(instance == null){
            instance = this;
        }
    }
    #endregion

    public List<Pool> pools;
    Dictionary<GameObject, List<GameObject>> poolDictionary;
    Dictionary<string, GameObject> prefabDictionary;

    // Start is called before the first frame update
    void Start()
    {
        poolDictionary = new Dictionary<GameObject, List<GameObject>>();
        prefabDictionary = new Dictionary<string, GameObject>();

        foreach(Pool pool in pools){
            List<GameObject> prefabList = new List<GameObject>();
            for(int i = 0; i < pool.size; i++)
            {
                GameObject newPoolObject = Instantiate(pool.prefab);
                newPoolObject.AddComponent<PoolObjectConfig>().poolPrefab = pool.prefab;
                newPoolObject.SetActive(false);
                prefabList.Add(newPoolObject);
            }
            poolDictionary.Add(pool.prefab,prefabList);
            prefabDictionary.Add(pool.id,pool.prefab);
        }
    }

    public bool ContainPrefab(GameObject prefab)
    {
        return poolDictionary.ContainsKey(prefab);
    }

    public GameObject GetPoolObject(string id, Vector3 position, Quaternion rotation){
        if(!prefabDictionary.ContainsKey(id)){
            Debug.LogWarning($"ID: {id} not in pool dictionary");
            return null;
        }
        
        return GetPoolObject(prefabDictionary[id],position,rotation);
    }

    public GameObject GetPoolObject(GameObject prefab, Vector3 position, Quaternion rotation){
        if(!poolDictionary.ContainsKey(prefab)){
            Debug.LogWarning($"ID: {prefab.name} not in pool dictionary");
            return null;
        }

        if(poolDictionary[prefab].Count == 0){
            GameObject newPoolObject = Instantiate(prefab);
            newPoolObject.AddComponent<PoolObjectConfig>().poolPrefab = prefab;
            poolDictionary[prefab].Add(newPoolObject);
        }
        //Get the first object in the list
        GameObject spawningObject = poolDictionary[prefab][0];
        spawningObject.transform.position = position;
        spawningObject.transform.rotation = rotation;  
        //Remove the first object from the list      
        spawningObject.SetActive(true);
        poolDictionary[prefab].RemoveAt(0);
        
        return spawningObject;
    }

    public void ReturnObjectToPool(string id, GameObject gameObject){
        if(!prefabDictionary.ContainsKey(id)){
            Debug.LogWarning($"ID: {id} not in pool dictionary");
            return;
        }
        ReturnObjectToPool(prefabDictionary[id],gameObject);
    }

    public void ReturnObjectToPool(GameObject prefab, GameObject gameObject){
        gameObject.transform.parent = null;
        gameObject.SetActive(false);
        if(!poolDictionary.ContainsKey(prefab)){
            Debug.LogWarning($"ID: {prefab.name} not in pool dictionary");
            return;
        }
        poolDictionary[prefab].Add(gameObject);
    }
}
