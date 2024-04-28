using System;
using System.Collections;
using DG.Tweening;
using FYP;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
    public enum GameState
    {
        Stop,
        Setup,
        Ready,
        Start,
        End,
    }
    public static GameManager instance { get; private set;}
    public Camera mainCamera;
    public PlayerManager localPlayerManager;
    public GameState state = GameState.Stop;
    public Action OnGameStartEvent;
    [SerializeField] bool spawnInRandomPosition;
    [SerializeField] Quest initQuest;
    [SerializeField] GameObject playerObjectPrefab;
    [SerializeField] GameObject waitingCanvas;
    

    
    void Awake() {
        if(instance == null){
            instance = this;
        }else{
            Destroy(this);
        }
    }

    void Start(){
        state = GameState.Setup;
        if(NetworkManager.Singleton.IsConnectedClient)
        {
            StartCoroutine(SetLocalPlayer());
        }
        else
        {
            PlayerManager playerManager;
            Transform player;
            playerManager = FindAnyObjectByType<PlayerManager>();
            player = playerManager?.transform;
            localPlayerManager = playerManager;
            if(spawnInRandomPosition && player != null)StartCoroutine(SetRandomPlayerPosition(player));
            SetInitQuest(playerManager);
            state = GameState.Start;
            OnGameStartEvent.Invoke();
        }
    }

    void SetInitQuest(PlayerManager playerManager){
        if(playerManager == null)
        {
            return;
        }
        playerManager.playerData.quests.Add(initQuest);
        initQuest.isActive = true;
    }

    IEnumerator SetLocalPlayer()
    {
        while( NetworkManager.Singleton.LocalClient.PlayerObject == null)
        {
            yield return null;
        }
        Transform player = NetworkManager.Singleton.LocalClient.PlayerObject.transform;
        PlayerManager playerManager = player.GetComponent<PlayerManager>();
        playerManager.GetComponent<NetworkPlayerSetup>().Setup();
        localPlayerManager = playerManager;
        if(spawnInRandomPosition)
        {
            StartCoroutine(SetRandomPlayerPosition(player));
        }
        while(state != GameState.Ready)
        {
            yield return null;
        }
        state = GameState.Start;
        OnGameStartEvent.Invoke();
        SetInitQuest(playerManager);
        waitingCanvas?.GetComponentInChildren<Image>().DOFade(0,2).OnComplete(()=>waitingCanvas.SetActive(false));
        yield return null;
    }

    public IEnumerator SetRandomPlayerPosition(Transform player){
        player.GetComponent<CharacterController>().enabled = false;
        int rndChunkPosX = UnityEngine.Random.Range(-1,2);
        int rndChunkPosY = UnityEngine.Random.Range(-1,2);
        while(rndChunkPosX == rndChunkPosY && rndChunkPosX == 0)
        {
            rndChunkPosX = UnityEngine.Random.Range(-1,2);
            rndChunkPosY = UnityEngine.Random.Range(-1,2);
        }
        Vector3 playerRndPosition = new Vector3(rndChunkPosX,0,rndChunkPosY)*MapGenerator.mapChunkSize*TerrainGenerationManager.scale;
        playerRndPosition += new Vector3(UnityEngine.Random.Range(-MapGenerator.mapChunkSize/2,MapGenerator.mapChunkSize/2),150,UnityEngine.Random.Range(-MapGenerator.mapChunkSize/2,MapGenerator.mapChunkSize/2));
        player.position = playerRndPosition;
        RaycastHit hit;
        while(!Physics.Raycast(playerRndPosition, Vector3.down ,out hit, 200f)){
            yield return null;
        }
        playerRndPosition.y = hit.point.y+20;
        player.position = playerRndPosition;
        player.GetComponent<CharacterController>().enabled = true;
        // NavigationBaker.Bake();
        yield return null;
    }

    public void SpawnPlayerGameObject()
    {
        foreach(ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            GameObject playerGameObject = Instantiate(playerObjectPrefab, new Vector3(0,100,0), Quaternion.identity);
            NetworkObject networkPlayerObject = playerGameObject.GetComponent<NetworkObject>();
            networkPlayerObject.SpawnAsPlayerObject(clientId);
        }
        FindAnyObjectByType<EndlessTerrain>().UpdateViewerSet();
    }

    public void EndGameEventHandler()
    {
        if(NetworkManager.Singleton)
        {
            FindAnyObjectByType<Room>().EndGameEventRpc(NetworkManager.Singleton.LocalClientId);
        }
        else
        {
            OnEndGameEvent(localPlayerManager.name);
        }
    }

    public void OnEndGameEvent(string playerName)
    {
        FindAnyObjectByType<UIController>().BlackCoverFadeIn(()=>MySceneManager.instance.EnableSettlementInterface(playerName));
        state = GameState.End;
        if(NetworkManager.Singleton)
        {
            NetworkManager.Singleton.Shutdown();
        }
    }

    public void SetGameState(GameState gameState)
    {
        state = gameState;
    }
}