using System;
using System.Collections;
using System.Collections.Generic;
using FYP;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using NavMeshBuilder = UnityEngine.AI.NavMeshBuilder;
/**************************************************************
    Source Code from Llam Academy
**************************************************************/
public class AreaFloorBaker : PlayerSetupBehaviour {
    [SerializeField]
    private NavMeshSurface navMeshSurface;
    [SerializeField]
    private PlayerManager player;
    [SerializeField]
    private float updateRate = 0.1f;
    [SerializeField]
    private float movementThreshold = 3;
    [SerializeField]
    private Vector3 navMeshSize = new Vector3(20, 20, 20);

    private Vector3 worldAnchor;
    private NavMeshData navMeshData;
    private List<NavMeshBuildSource> navMeshBuildSources = new List<NavMeshBuildSource>();

    private void Start()
    {
        navMeshData = new NavMeshData();
        NavMesh.AddNavMeshData(navMeshData);
        if(player != null)
        {
            BuildNavMesh(false);
        }
        StartCoroutine(CheckPlayerMovement());
    }

    private IEnumerator CheckPlayerMovement()
    {   
        WaitForSeconds waitForSeconds = new WaitForSeconds(updateRate);
        
        while(true)
        {
            if(player != null && Vector3.Distance(worldAnchor, player.transform.position) > movementThreshold)
            {
                BuildNavMesh(true);
                worldAnchor = player.transform.position;
            }
            yield return waitForSeconds;
        }
    }

    private void BuildNavMesh(bool async)
    {
        Debug.Log("Build Nav Mesh");
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
            NavMeshBuilder.CollectSources(navMeshSurface.transform, navMeshSurface.layerMask, navMeshSurface.useGeometry, navMeshSurface.defaultArea, markups, navMeshBuildSources);
        }
        else
        {
            NavMeshBuilder.CollectSources(navMeshBounds, navMeshSurface.layerMask, navMeshSurface.useGeometry, navMeshSurface.defaultArea, markups, navMeshBuildSources);
        }

        // navMeshBuildSources.RemoveAll(source => source.component != null && source.component.gameObject.GetComponent<NavMeshAgent>() != null);

        if(async)
        {
            NavMeshBuilder.UpdateNavMeshDataAsync(navMeshData, navMeshSurface.GetBuildSettings(), navMeshBuildSources, navMeshBounds);
        }
        else
        {
            NavMeshBuilder.UpdateNavMeshData(navMeshData, navMeshSurface.GetBuildSettings(), navMeshBuildSources, navMeshBounds);
        }
    }

    public override void Setup(PlayerManager playerManager)
    {
        player = playerManager;
    }
}