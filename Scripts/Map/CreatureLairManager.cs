using System;
using System.Collections;
using System.Collections.Generic;
using FYP;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public struct LairSetting
{
    [Header("Camp Setting")]
    public GameObject campPrefabs;
    public bool useRaycastNormal;
    [Header("Creature Setting")]
    public GameObject[] creaturePrefabArray;
    public int creatureNumber;
    public int baseLevel;
    public Vector2Int creatureSpawnRange;
    [Header("Chest Setting")]
    public int chestLevel;
    public LootList lootList;
}

public struct CreatureData
{
    public int id;
    public GameObject monsetrPrefab;
    public Vector3 position;
    public Vector3 eulerAngles;
    // public int isActive; // 1 is True, 0 is False
    public int level;
}

public class CreatureLairManager : MonoBehaviour
{
    public LayerMask layerMask;
    public GameObject chestPrefab;
    public Avatar[] chestAvatars;
    public LairSetting[] lairSettings;

    const float viewerMoveThresholdForEnemyUpdate = 1f;

    [Header("Network Setting")]
    public bool useNetworkObjectChest;
    public GameObject networkChestPrefab;

    private Vector2 previousViewerPosition;
    //Store active monster
    private Dictionary<int, GameObject> activeCreatureDict = new Dictionary<int, GameObject>();
    //Store Monster Data
    private Dictionary<int, CreatureData> creatureDataDict = new Dictionary<int, CreatureData>();
    private Dictionary<Vector2Int, List<int>> hashTable = new Dictionary<Vector2Int, List<int>>();
    private int nextCreatureID = 0;

    HashSet<Vector3> lairCenterSet = new HashSet<Vector3>();

    public void GenerateCreatureLair(List<Vector3> creatureLairCenters, Transform chunk)
    {
        Vector3 chunkCenterPositionVec3 = new Vector3(chunk.position.x, 0, chunk.position.z) / TerrainGenerationManager.scale;
        Vector2Int cell = new Vector2Int(Mathf.FloorToInt(chunkCenterPositionVec3.x / (MapGenerator.mapChunkSize - 1)), Mathf.FloorToInt(chunkCenterPositionVec3.z / (MapGenerator.mapChunkSize - 1)));
        System.Random random = new System.Random(Mathf.RoundToInt(TerrainGenerationManager.mapGenerator.seed + cell.x * 121 * (cell.y % 7)));

        foreach (Vector3 lairCenter in creatureLairCenters)
        {
            LairSetting lairSetting = lairSettings[random.Next(0, lairSettings.Length)];
            GenerateLair(lairCenter, lairSetting, chunkCenterPositionVec3, chunk);
        }
    }

    public void AddChunkCreatures(List<Vector3> enemyLairCenters, List<Vector3> neutralLairCenters, Vector2 chunkCenterPosition, LairSetting[] neutralLairs)
    {
        Vector2Int cell = new Vector2Int(Mathf.FloorToInt(chunkCenterPosition.x / (MapGenerator.mapChunkSize - 1)), Mathf.FloorToInt(chunkCenterPosition.y / (MapGenerator.mapChunkSize - 1)));
        if (!hashTable.ContainsKey(cell))
        {
            System.Random random = new System.Random(Mathf.RoundToInt(TerrainGenerationManager.mapGenerator.seed + cell.x * 121 * (cell.y % 7)));
            hashTable.Add(cell, new List<int>());

            foreach (Vector3 lairCenter in enemyLairCenters)
            {
                LairSetting lairSetting = lairSettings[random.Next(0, lairSettings.Length)];
                AddCreatureData(lairCenter, lairSetting, chunkCenterPosition);
            }

            if (neutralLairs != null && neutralLairs.Length > 0)
            {
                foreach (Vector3 lairCenter in neutralLairCenters)
                {
                    LairSetting lairSetting = neutralLairs[random.Next(0, neutralLairs.Length)];
                    AddCreatureData(lairCenter, lairSetting, chunkCenterPosition);
                }
            }
        }
    }

    private void GenerateLair(Vector3 lairCenter, LairSetting lairSetting, Vector3 realChunkCenterPosition, Transform chunk)
    {
        RaycastHit hit;
        Quaternion rotation = Quaternion.identity;
        Vector3 campPos = (lairCenter + realChunkCenterPosition) * TerrainGenerationManager.scale;
        if (lairSetting.useRaycastNormal)
        {
            if (Physics.Raycast(campPos + new Vector3(0, 10, 0), Vector3.down, out hit, 200f, layerMask))
            {
                rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
            }
        }

        if (lairSetting.campPrefabs != null)
        {
            GameObject camp = ObjectPool.instance.GetPoolObject(lairSetting.campPrefabs, campPos, rotation);
            Transform chestPivot = camp.transform.Find("ChestPivot");
            camp.transform.SetParent(chunk, true);
            SpawnChest(lairSetting, chestPivot, chunk);
        }
    }

    private void SpawnChest(LairSetting lairSetting, Transform chestPivot, Transform chunk)
    {
        if (useNetworkObjectChest && NetworkObjectManager.Singleton)
        {
            int spawnIndex = NetworkObjectPool.Singleton.GetPrefabIndex(networkChestPrefab);
            NetworkObjectManager.Singleton.ObjectSpawnRPC(NetworkManager.Singleton.LocalClientId, spawnIndex, chestPivot.position, chestPivot.rotation);
        }
        else
        {
            GameObject chest = ObjectPool.instance.GetPoolObject(chestPrefab, chestPivot.position, chestPivot.rotation);
            chest.GetComponent<Animator>().avatar = chestAvatars[lairSetting.chestLevel];
            chest.GetComponent<ChestInteraction>().SetUp(lairSetting.lootList);
            chest.transform.SetParent(chunk, true);
        }
    }

    private void AddCreatureData(Vector3 lairCenter, LairSetting lairSetting, Vector2 chunkCenterPosition)
    {
        Vector3 realChunkCenterPosition = new Vector3(chunkCenterPosition.x, 0, chunkCenterPosition.y);
        Vector2Int cell = new Vector2Int(Mathf.FloorToInt(chunkCenterPosition.x / (MapGenerator.mapChunkSize - 1)), Mathf.FloorToInt(chunkCenterPosition.y / (MapGenerator.mapChunkSize - 1)));
        System.Random random = new System.Random(Mathf.RoundToInt(TerrainGenerationManager.mapGenerator.seed * (lairCenter.x + 1) * lairCenter.y));

        int monsterPrefabsArrayLength = lairSetting.creaturePrefabArray.Length;
        int maxSpawnRange = lairSetting.creatureSpawnRange.y;
        int minSpawnRange = lairSetting.creatureSpawnRange.x;
        int difficultyLevel = DifficultyLevelCalculate(chunkCenterPosition);

        RaycastHit hit;

        for (int i = 0; i < lairSetting.creatureNumber; i++)
        {
            double angle = random.NextDouble() * 2 * Math.PI; // Random angle in radians
            float x = Mathf.Cos((float)angle); // Direction vector x component
            float y = Mathf.Sin((float)angle); // Direction vector y component
            float randomDistance = minSpawnRange + (float)random.NextDouble() * (maxSpawnRange - minSpawnRange);
            Vector3 position = (lairCenter + randomDistance * new Vector3(x, 0, y) + realChunkCenterPosition) * TerrainGenerationManager.scale;
            float randomY = random.Next(0, 180);
            Quaternion rotation = Quaternion.Euler(0, randomY, 0);

            if(lairCenterSet.Contains(position))
            {
                Debug.Log("Duplicate Creature Data has been Added!!!");
                return;
            }
            else
            {
                lairCenterSet.Add(position);
            }

            if (Physics.Raycast(position, Vector3.down, out hit, 200f, layerMask))
            {
                position.y = hit.point.y;
            }

            GameObject spawningMonsterPrefab = lairSetting.creaturePrefabArray[random.Next(0, monsterPrefabsArrayLength)];

            int monsterId = nextCreatureID++;
            CreatureData CreatureData = new CreatureData
            {
                id = monsterId,
                monsetrPrefab = spawningMonsterPrefab,
                position = position,
                eulerAngles = rotation.eulerAngles,
                level = lairSetting.baseLevel + random.Next(difficultyLevel * 10, (difficultyLevel + 1) * 10)
            };
            creatureDataDict.Add(monsterId, CreatureData);
            hashTable[cell].Add(monsterId);
        }
    }

    public void UpdateCreatureState(HashSet<Transform> viewers)
    {

        if(creatureDataDict.Count == 0)
        {
            return;
        }

        NativeArray<Vector3> viewerPositionArray = new NativeArray<Vector3>(viewers.Count, Allocator.TempJob);
        HashSet<Vector2Int> cells = new HashSet<Vector2Int>();
        int index = 0;
        foreach(Transform viewer in viewers)
        {
            viewerPositionArray[index] = viewer.transform.position / TerrainGenerationManager.scale;
            cells.Add(new Vector2Int(Mathf.FloorToInt(viewerPositionArray[index].x / (MapGenerator.mapChunkSize - 1)), Mathf.FloorToInt(viewerPositionArray[index].z / (MapGenerator.mapChunkSize - 1)))); 
            index++;
        }

        List<CreatureData> creatureDatas = new List<CreatureData>();
        foreach(Vector2Int cell in cells)
        {
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    Vector2Int neighbourCell = cell + new Vector2Int(i, j);
                    if (!hashTable.ContainsKey(neighbourCell))
                    {
                        continue;
                    }
                    foreach (int id in hashTable[neighbourCell])
                    {
                        if (creatureDataDict.ContainsKey(id))
                        {
                            creatureDatas.Add(creatureDataDict[id]);
                        }
                    }
                }
            }
        }
        NativeArray<Vector3> monsterPosArray = new NativeArray<Vector3>(creatureDatas.Count, Allocator.TempJob);
        NativeArray<int> monsterStateArray = new NativeArray<int>(creatureDatas.Count, Allocator.TempJob);

        for (int i = 0; i < creatureDatas.Count; i++)
        {
            monsterPosArray[i] = creatureDatas[i].position;
        }

        var activationJob = new MultiViewersActivationJob
        {
            monsterPosArray = monsterPosArray,
            monsterStateArray = monsterStateArray,
            viewerPositions = viewerPositionArray
        };

        JobHandle handle = activationJob.Schedule(monsterPosArray.Length, 16);
        handle.Complete();

        for (int id = 0; id < monsterStateArray.Length; id++)
        {
            CreatureData creatureData = creatureDatas[id];
            bool isDead = false;
            if (monsterStateArray[id] == 1 && !activeCreatureDict.ContainsKey(creatureData.id))
            {
                NetworkObject creatureNetworkObject = NetworkObjectPool.Singleton.GetNetworkObject(creatureData.monsetrPrefab, creatureData.position, Quaternion.Euler(creatureData.eulerAngles));
                GameObject creature = creatureNetworkObject.gameObject;
                if (creature)
                {
                    if (!creatureNetworkObject.IsSpawned)
                    {
                        creatureNetworkObject.Spawn();
                    }

                    // if (!activeCreatureDict.ContainsKey(creatureData.id))
                    // {
                    //     activeCreatureDict.Add(creatureData.id, creature);
                    // }
                    // else
                    // {
                    //     Debug.LogWarning("Key " + creatureData.id + " already exists in activeCreatureDict");
                    // }
                    creatureData = new CreatureData{
                        id = creatureData.id,
                        monsetrPrefab = creatureData.monsetrPrefab,
                        position = creature.transform.position,
                        eulerAngles = creature.transform.eulerAngles,
                        level = creatureData.level
                    };

                    activeCreatureDict.Add(creatureData.id, creature);
                }
                else
                {
                    Debug.LogWarning("Not enough " + creatureData.monsetrPrefab.name + " to spawn");
                }
            }
            else if (monsterStateArray[id] == 0 && activeCreatureDict.ContainsKey(creatureData.id))
            {
                if (activeCreatureDict[creatureData.id].TryGetComponent(out CharacterStats characterStats))
                {
                    if (characterStats.isDead)
                    {
                        // CreatureData.position = new Vector3(CreatureData.position.x,-9999,CreatureData.position.z);
                        Vector2Int cell = new Vector2Int(Mathf.FloorToInt(creatureData.position.x / TerrainGenerationManager.scale / (MapGenerator.mapChunkSize - 1)), Mathf.FloorToInt(creatureData.position.z / TerrainGenerationManager.scale / (MapGenerator.mapChunkSize - 1)));
                        isDead = characterStats.isDead;
                        creatureDataDict.Remove(creatureData.id);
                        if (hashTable.ContainsKey(cell))
                        {
                            hashTable[cell].Remove(id);
                        }
                        else
                        {
                            Debug.LogWarning($"Wrong Cell {cell} Try to remove id: {id}");
                        }
                    }
                }

                NetworkObject creatureNetworkObject = activeCreatureDict[creatureData.id].GetComponent<NetworkObject>();
                creatureData = new CreatureData{
                    id = creatureData.id,
                    monsetrPrefab = creatureData.monsetrPrefab,
                    position = creatureNetworkObject.transform.position,
                    eulerAngles = creatureNetworkObject.transform.eulerAngles,
                    level = creatureData.level
                };
                if (creatureNetworkObject.IsSpawned)
                {
                    creatureNetworkObject.Despawn();
                }
                activeCreatureDict.Remove(creatureData.id);
            }

            if (!isDead)
            {
                creatureDataDict[creatureData.id] = creatureData;
            }
        }

        monsterPosArray.Dispose();
        monsterStateArray.Dispose();
        viewerPositionArray.Dispose();
    }

    public void RemoveCreatureFromManager(GameObject target)
    {
        Vector2Int cell = new Vector2Int(Mathf.FloorToInt(target.transform.position.x / TerrainGenerationManager.scale / (MapGenerator.mapChunkSize - 1)), Mathf.FloorToInt(target.transform.position.z / TerrainGenerationManager.scale / (MapGenerator.mapChunkSize - 1)));
        int id = FindKeyByGameObject(target);
        hashTable[cell].Remove(id);
        creatureDataDict.Remove(id);
        activeCreatureDict.Remove(id);
    }

    int DifficultyLevelCalculate(Vector2 chunkPosition)
    {
        int distance = Mathf.RoundToInt(Vector3.Distance(chunkPosition, Vector2.zero));
        return distance / MapGenerator.mapChunkSize;
    }

    int FindKeyByGameObject(GameObject target)
    {
        foreach (KeyValuePair<int, GameObject> pair in activeCreatureDict)
        {
            if (pair.Value == target)
            {
                return pair.Key;
            }
        }

        // Return some default value or throw an exception if the GameObject was not found in the dictionary
        throw new KeyNotFoundException("The GameObject was not found in the dictionary.");
    }




    // public void UpdateCreatureState(Vector2 viewerPosition)
    // {

    //     if (Vector2.SqrMagnitude(viewerPosition - previousViewerPosition) <= viewerMoveThresholdForEnemyUpdate || creatureDataDict.Count == 0)
    //     {
    //         return;
    //     }

    //     Vector2Int cell = new Vector2Int(Mathf.FloorToInt(viewerPosition.x / (MapGenerator.mapChunkSize - 1)), Mathf.FloorToInt(viewerPosition.y / (MapGenerator.mapChunkSize - 1)));

    //     List<CreatureData> creatureDatas = new List<CreatureData>();
    //     for (int i = -1; i < 2; i++)
    //     {
    //         for (int j = -1; j < 2; j++)
    //         {
    //             Vector2Int neighbourCell = cell + new Vector2Int(i, j);
    //             if (!hashTable.ContainsKey(neighbourCell))
    //             {
    //                 continue;
    //             }
    //             foreach (int id in hashTable[neighbourCell])
    //             {
    //                 if (creatureDataDict.ContainsKey(id))
    //                 {
    //                     creatureDatas.Add(creatureDataDict[id]);
    //                 }
    //             }
    //         }
    //     }
    //     NativeArray<Vector3> monsterPosArray = new NativeArray<Vector3>(creatureDatas.Count, Allocator.TempJob);
    //     NativeArray<int> monsterStateArray = new NativeArray<int>(creatureDatas.Count, Allocator.TempJob);

    //     for (int i = 0; i < creatureDatas.Count; i++)
    //     {
    //         monsterPosArray[i] = creatureDatas[i].position;
    //     }

    //     var activationJob = new ActivationJob
    //     {
    //         monsterPosArray = monsterPosArray,
    //         monsterStateArray = monsterStateArray,
    //         viewerPosition = new Vector3(viewerPosition.x, 0, viewerPosition.y)
    //     };

    //     JobHandle handle = activationJob.Schedule(monsterPosArray.Length, 16);
    //     handle.Complete();

    //     for (int id = 0; id < monsterStateArray.Length; id++)
    //     {
    //         CreatureData creatureData = creatureDatas[id];
    //         bool isDead = false;
    //         if (monsterStateArray[id] == 1 && creatureData.isActive == 0)
    //         {
    //             GameObject creature = ObjectPool.instance.GetPoolObject(creatureData.monsetrPrefab, creatureData.position, Quaternion.Euler(creatureData.eulerAngles));
    //             if (creature)
    //             {
    //                 var creatureNetworkObject = creature.GetComponent<NetworkObject>();
    //                 if (creatureNetworkObject && !creatureNetworkObject.IsSpawned)
    //                 {
    //                     if (NetworkManager.Singleton.IsServer)
    //                     {
    //                         creatureNetworkObject.Spawn();
    //                     }
    //                 }
                    
    //                 if (creature.TryGetComponent(out EnemyStats stats))
    //                 {
    //                     stats.playerLevel = creatureData.level;
    //                     AddEnemyExtraStats(stats, creatureData.level);
    //                 }
    //                 if (creature.TryGetComponent(out LootDropHandler lootDropHandler))
    //                 {
    //                     lootDropHandler.enabled = true;
    //                 }
    //                 // activeCreatureDict.Add(creatureData.id,creature);
    //                 if (!activeCreatureDict.ContainsKey(creatureData.id))
    //                 {
    //                     activeCreatureDict.Add(creatureData.id, creature);
    //                 }
    //                 else
    //                 {
    //                     Debug.LogWarning("Key " + creatureData.id + " already exists in activeCreatureDict");
    //                 }
    //                 creatureData.isActive = 1;
    //             }
    //             else
    //             {
    //                 Debug.LogWarning("Not enough " + creatureData.monsetrPrefab.name + " to spawn");
    //             }
    //         }
    //         else if (monsterStateArray[id] == 0 && creatureData.isActive == 1)
    //         {
    //             creatureData.isActive = 0;
    //             if (activeCreatureDict[creatureData.id].TryGetComponent(out CharacterStats characterStats))
    //             {
    //                 SetEnemyExtraStats(characterStats, 0);
    //                 characterStats.playerLevel = 0;
    //                 if (characterStats.isDead)
    //                 {
    //                     // CreatureData.position = new Vector3(CreatureData.position.x,-9999,CreatureData.position.z);
    //                     cell = new Vector2Int(Mathf.FloorToInt(creatureData.position.x / TerrainGenerationManager.scale / (MapGenerator.mapChunkSize - 1)), Mathf.FloorToInt(creatureData.position.z / TerrainGenerationManager.scale / (MapGenerator.mapChunkSize - 1)));
    //                     isDead = characterStats.isDead;
    //                     creatureDataDict.Remove(creatureData.id);
    //                     if (hashTable.ContainsKey(cell))
    //                     {
    //                         hashTable[cell].Remove(id);
    //                     }
    //                     else
    //                     {
    //                         Debug.LogWarning($"Wrong Cell {cell} Try to remove id: {id}");
    //                     }
    //                 }
    //             }

    //             var creatureNetworkObject = activeCreatureDict[creatureData.id].GetComponent<NetworkObject>();
    //             if (creatureNetworkObject && creatureNetworkObject.IsSpawned)
    //             {
    //                 if (NetworkManager.Singleton.IsServer)
    //                 {
    //                     ResetHealthAndIsDead(creatureNetworkObject);
    //                     creatureNetworkObject.Despawn(true);
    //                 }
    //             }

    //             ObjectPool.instance.ReturnObjectToPool(creatureData.monsetrPrefab, activeCreatureDict[creatureData.id]);
    //             activeCreatureDict.Remove(creatureData.id);

    //         }
    //         if (!isDead)
    //         {
    //             creatureDataDict[creatureData.id] = creatureData;
    //         }
    //     }

    //     monsterPosArray.Dispose();
    //     monsterStateArray.Dispose();
    //     previousViewerPosition = viewerPosition;
    // }

}
