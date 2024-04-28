using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;
using Unity.Collections;
using FYP;

public class Room : NetworkBehaviour {
    
    public enum RoomState
    {
        Preparation,
        Started
    }

    public RoomState roomState = RoomState.Preparation;
    HashSet<ulong> m_readyClientIds = new HashSet<ulong>();
    Dictionary<ulong, FixedString32Bytes> clientNameDictionary = new Dictionary<ulong, FixedString32Bytes>();
    GameObject[] playerPreafbs;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        playerPreafbs = GameObject.FindGameObjectsWithTag("Player");
        foreach(GameObject playerPreafb in playerPreafbs)
        {
            playerPreafb.SetActive(false);
        }
        LobbyUI lobbyUI = FindAnyObjectByType<LobbyUI>();
        lobbyUI.gameObject.SetActive(false);
        if(IsServer)
        {
            UpdateDisplayPlayerRpc(NetworkManager.ConnectedClientsList.Count);
            NetworkManager.OnClientDisconnectCallback += OnClientDisconnectHanlder;
            clientNameDictionary.Add(NetworkManager.LocalClientId, new FixedString32Bytes(lobbyUI.playerNameInput.text.Substring(0,Mathf.Min(28,lobbyUI.playerNameInput.text.Length-1))));
        }
        else
        {
            GetPlayerNumberRpc(NetworkManager.LocalClientId, new FixedString32Bytes(lobbyUI.playerNameInput.text.Substring(0,Mathf.Min(28,lobbyUI.playerNameInput.text.Length-1))));
        }
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        NetworkManager.OnClientDisconnectCallback -= OnClientDisconnectHanlder;
    }

    private void OnClientDisconnectHanlder(ulong clientId)
    {
        if(clientNameDictionary.ContainsKey(clientId))
        {
            clientNameDictionary.Remove(clientId);
            UpdateDisplayPlayerRpc(clientNameDictionary.Count);
        }
    }

    [Rpc(SendTo.Server)]
    public void OnPlayerReadyServerRpc(ulong clientId)
    {
        if(!m_readyClientIds.Contains(clientId))
            m_readyClientIds.Add(clientId);

        if(m_readyClientIds.Count == NetworkManager.ConnectedClientsIds.Count)
        {
            roomState = RoomState.Started;
            MySceneManager.instance.LoadNetworkScene(MySceneManager.MyScene.JoinGameScene);
            // LevelManager.instance.LoadNetworkScene(LevelManager.Scene.MultiPlayerGameScene);
        }
    }

    [Rpc(SendTo.Server)]
    public void GetPlayerNumberRpc(ulong clientId, FixedString32Bytes playerName)
    {
        if(roomState == RoomState.Preparation)
        {
            Debug.Log($"Client[{clientId}] {playerName} Join the room");
            UpdateDisplayPlayerRpc(NetworkManager.ConnectedClientsList.Count);
            clientNameDictionary.Add(clientId, playerName);
        }
    }

    [Rpc(SendTo.Server)]
    public void EndGameEventRpc(ulong clientId)
    {
        BroadcastEndGameEventRpc(new FixedString32Bytes(clientNameDictionary[clientId]));
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void BroadcastEndGameEventRpc(FixedString32Bytes winnerName)
    {
        GameManager.instance.OnEndGameEvent(winnerName.ToString());
    }



    [Rpc(SendTo.ClientsAndHost)]
    public void UpdateDisplayPlayerRpc(int number)
    {
        Debug.Log("Display Player: " + number);
        foreach(GameObject playerPreafb in playerPreafbs)
        {
            playerPreafb.SetActive(number-- > 0);
        }
    }

}