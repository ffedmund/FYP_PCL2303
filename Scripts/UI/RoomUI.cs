using System;
using System.Collections;
using System.Collections.Generic;
using FYP;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class RoomUI : MonoBehaviour
{
    [SerializeField] Button readyButton;
    [SerializeField] Button exitButton;
    [SerializeField] GameObject titleHUD;
    [SerializeField] TextMeshProUGUI connectionData;

    void Start ()
    {
        NetworkManager.Singleton.OnConnectionEvent += ConnectionEventHandler;
        readyButton.onClick.AddListener(() =>{ OnConnectionReady(); });
        exitButton.onClick.AddListener(() =>{ ExitRoom();});
    }

    private void ExitRoom()
    {
        NetworkManager.Singleton.Shutdown();
        MultiplayerGameManager.Singleton.Disconnected();
    }

    private void OnConnectionReady()
    {
        FindAnyObjectByType<Room>().OnPlayerReadyServerRpc(NetworkManager.Singleton.LocalClientId);
        readyButton.enabled = false;
        readyButton.GetComponentInChildren<TextMeshProUGUI>().SetText("Waiting");
    }

    private void ConnectionEventHandler(NetworkManager manager, ConnectionEventData data)
    {

        if(data.EventType == ConnectionEvent.ClientConnected)
        {
            if(data.ClientId == NetworkManager.Singleton.LocalClientId)
            {
                Show();
            }
        }

        if(data.EventType == ConnectionEvent.ClientDisconnected)
        {
            if(data.ClientId == NetworkManager.Singleton.LocalClientId)
            {
                Hide();
            }
        }

        if(data.EventType == ConnectionEvent.PeerDisconnected)
        {
            readyButton.enabled = true;
            readyButton.GetComponentInChildren<TextMeshProUGUI>().SetText("Ready");
        }
    }

    void Show()
    {
        readyButton.gameObject.SetActive(true);
        titleHUD.gameObject.SetActive(true);
        exitButton.gameObject.SetActive(true);
        connectionData.SetText(MultiplayerGameManager.Singleton.GetConnectionMethod() == MultiplayerGameManager.ConnectionMethod.Relay? MultiplayerGameManager.Singleton.joinCode:MultiplayerGameManager.Singleton.ipAddress);
    }

    void Hide()
    {
        readyButton.gameObject.SetActive(false);
        titleHUD.gameObject.SetActive(false);
        exitButton.gameObject.SetActive(false);
    }

    private void OnDestroy() 
    {
        if(NetworkManager.Singleton)
        {
            NetworkManager.Singleton.OnConnectionEvent -= ConnectionEventHandler;
        }
    }
}
