using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FYP;
using Unity.AI.Navigation;
using UnityEngine.AI;
using NavMeshBuilder = UnityEngine.AI.NavMeshBuilder;
using System;
using Unity.Netcode;
using System.Linq;

public class MultiplayerAreaFloorBaker : MonoBehaviour {
    struct BakeData
    {
        public Vector3 worldAnchor;
        public PlayerManager player;
        public int defaultArea;
    }

    [SerializeField]
    private NavMeshSurface navMeshSurface;
    [SerializeField]
    private float updateRate = 0.1f;
    [SerializeField]
    private float movementThreshold = 3;
    [SerializeField]
    private Vector3 navMeshSize = new Vector3(20, 20, 20);

    // private Dictionary<ulong, Vector3> worldAnchorDictionary;
    // private Dictionary<ulong, PlayerManager> playerDictionary;
    private Dictionary<ulong, BakeData> bakeDataDictionary = new Dictionary<ulong, BakeData>();
    private Dictionary<ulong, NavMeshData> navMeshDataDictionary = new Dictionary<ulong, NavMeshData>();
    private List<NavMeshBuildSource> navMeshBuildSources = new List<NavMeshBuildSource>();

    private void Start()
    {
        GameManager.instance.OnGameStartEvent += StartDataAssignment;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectCallback;
    }

    private void OnClientDisconnectCallback(ulong clientId)
    {
        bakeDataDictionary.Remove(clientId);
        navMeshDataDictionary.Remove(clientId);
    }

    private void StartDataAssignment()
    {
        StartCoroutine(AssignDataCoroutine());
    }

    private IEnumerator AssignDataCoroutine()
    {
        if(NetworkManager.Singleton.IsServer)
        {
            for(int i = 0; i < NetworkManager.Singleton.ConnectedClientsList.Count; i++)
            {
                // int defaultArea = 20 + i;
                int defaultArea = 0;
                ulong clientId = NetworkManager.Singleton.ConnectedClientsList[i].ClientId;
                NavMeshData navMeshData = new NavMeshData();
                NavMesh.AddNavMeshData(navMeshData);
                BakeData bakeData = new BakeData{
                    worldAnchor = NetworkManager.Singleton.ConnectedClientsList[i].PlayerObject.transform.position,
                    player = NetworkManager.Singleton.ConnectedClientsList[i].PlayerObject.GetComponent<PlayerManager>(),
                    defaultArea = defaultArea
                };
                navMeshDataDictionary.Add(clientId, navMeshData);
                bakeDataDictionary.Add(clientId, bakeData);
                // BuildNavMesh(bakeData, false);
                yield return null;
            }
            StartCoroutine(CheckPlayersMovement());
        }
    }

    private IEnumerator CheckPlayersMovement()
    {   
        WaitForSeconds waitForSeconds = new WaitForSeconds(updateRate);
        
        while(true)
        {
            for (int index = 0; index < bakeDataDictionary.Count; index++)
            {
                var kvp = bakeDataDictionary.ElementAt(index);
                BakeData bakeData = kvp.Value;
                Vector3 worldAnchor = bakeData.worldAnchor;
                PlayerManager player = bakeData.player;
                NavMeshData navMeshData = navMeshDataDictionary[kvp.Key];
                if(Vector3.Distance(worldAnchor, player.transform.position) > movementThreshold)
                {
                    BuildNavMesh(bakeData, navMeshData, true);
                    bakeDataDictionary[kvp.Key] = new BakeData{
                        worldAnchor = player.transform.position,
                        player = bakeData.player,
                        defaultArea = bakeData.defaultArea
                    };
                }
            }
            yield return waitForSeconds;
        }
    }

    private void BuildNavMesh(BakeData bakeData, NavMeshData navMeshData, bool async)
    {
        // Debug.Log("Build Nav Mesh");
        PlayerManager player = bakeData.player;
        int defaultArea = bakeData.defaultArea;

        Bounds navMeshBounds = new Bounds(player.transform.position, navMeshSize);
        List<NavMeshBuildMarkup> markups = new List<NavMeshBuildMarkup>();

        List<NavMeshModifier> modifiers;
        if(navMeshSurface.collectObjects == CollectObjects.Children)
        {
            modifiers = new List<NavMeshModifier>(navMeshSurface.GetComponentsInChildren<NavMeshModifier>());
            Debug.Log($"Find {modifiers.Count} modifiers");
        }
        else
        {
            modifiers = NavMeshModifier.activeModifiers;
        }

        for(int i = 0; i < modifiers.Count; i++)
        {
            if(((navMeshSurface.layerMask & (1 << modifiers[i].gameObject.layer)) == 1 << modifiers[i].gameObject.layer)
                && modifiers[i].AffectsAgentType(navMeshSurface.agentTypeID))
            {
                markups.Add(new NavMeshBuildMarkup{
                    root = modifiers[i].transform,
                    overrideArea = modifiers[i].overrideArea,
                    area = modifiers[i].area,
                    ignoreFromBuild = modifiers[i].ignoreFromBuild
                });
            } 
        }

        if(navMeshSurface.collectObjects == CollectObjects.Children)
        {
            // NavMeshBuilder.CollectSources(navMeshSurface.transform, navMeshSurface.layerMask, navMeshSurface.useGeometry, navMeshSurface.defaultArea, markups, navMeshBuildSources);
            NavMeshBuilder.CollectSources(navMeshSurface.transform, navMeshSurface.layerMask, navMeshSurface.useGeometry, defaultArea, markups, navMeshBuildSources);
        }
        else
        {
            NavMeshBuilder.CollectSources(navMeshBounds, navMeshSurface.layerMask, navMeshSurface.useGeometry, defaultArea, markups, navMeshBuildSources);
        }

        if(async)
        {
            NavMeshBuilder.UpdateNavMeshDataAsync(navMeshData, navMeshSurface.GetBuildSettings(), navMeshBuildSources, navMeshBounds);
        }
        else
        {
            NavMeshBuilder.UpdateNavMeshData(navMeshData, navMeshSurface.GetBuildSettings(), navMeshBuildSources, navMeshBounds);
        }
    }

    void OnDestroy()
    {
        NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnectCallback;

    }
}