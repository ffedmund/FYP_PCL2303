using System;
using System.Net;
using System.Collections.Generic;
using FYP;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    enum State 
    {
        ConnectionMethod,
        JoinGame
    }

    [SerializeField] Button createGameButton;
    [SerializeField] Button joinGameButton;
    [SerializeField] Button backButton;
    [SerializeField] Button relayButton;
    [SerializeField] Button directConnectionButton;

    [SerializeField] GameObject connectionDataWindow;
    [SerializeField] Button connectButton;
    [SerializeField] TextMeshProUGUI connectionWindowTitle;
    [SerializeField] TextMeshProUGUI connectionButtonText;
    [SerializeField] TextMeshProUGUI connectionInput;

    public TextMeshProUGUI playerNameInput;


    State state = State.ConnectionMethod;

    private void Start() {
        createGameButton.onClick.AddListener(() => {
            if(MultiplayerGameManager.Singleton.GetConnectionMethod() == MultiplayerGameManager.ConnectionMethod.Relay)
            {
                MultiplayerGameManager.Singleton.StartHost();
            }
            else
            {
                ShowConnectionDataWindow(0);
            }
            // LevelManager.instance.LoadNetworkScene(LevelManager.Scene.MultiPlayerGameScene);
        });
        joinGameButton.onClick.AddListener(() => {
            // MultiplayerGameManager.Singleton.StartClient();
            ShowConnectionDataWindow(1);
        });

        relayButton.onClick.AddListener(() => {
            MultiplayerGameManager.Singleton.SetConnectionMethod(MultiplayerGameManager.ConnectionMethod.Relay);
            ShowJoinGameButtons();
        });
        directConnectionButton.onClick.AddListener(() => {
            MultiplayerGameManager.Singleton.SetConnectionMethod(MultiplayerGameManager.ConnectionMethod.Direct);
            ShowJoinGameButtons();
        });

        backButton.onClick.AddListener(() => {
            switch (state)
            {
                case State.ConnectionMethod:
                    MySceneManager.instance.LoadScene(0);
                break;
                case State.JoinGame:
                    ShowConnectionMethodButtons();
                break;
            }
        });

        NetworkManager.Singleton.OnConnectionEvent += ConnectionEventHandler;
    }

    private void ConnectionEventHandler(NetworkManager manager, ConnectionEventData data)
    {
        if(data.ClientId != NetworkManager.Singleton.LocalClientId)
            return;

        if(data.EventType == ConnectionEvent.ClientDisconnected)
        {
            Show();
        }
    }

    void ShowConnectionMethodButtons()
    {
        relayButton.gameObject.SetActive(true);
        directConnectionButton.gameObject.SetActive(true);
        createGameButton.gameObject.SetActive(false);
        joinGameButton.gameObject.SetActive(false);
        state = State.ConnectionMethod;
    }

    void ShowJoinGameButtons()
    {
        relayButton.gameObject.SetActive(false);
        directConnectionButton.gameObject.SetActive(false);
        createGameButton.gameObject.SetActive(true);
        joinGameButton.gameObject.SetActive(true);
        state = State.JoinGame;
    }

    void ShowConnectionDataWindow(int joinGameMethodIndex)
    {
        connectionWindowTitle.SetText(MultiplayerGameManager.Singleton.GetConnectionMethod() == MultiplayerGameManager.ConnectionMethod.Relay?"Code":"IP Address");
        connectionButtonText.SetText(joinGameMethodIndex == 0?"Host":"Connect");
        connectionDataWindow.SetActive(true);
        int temp = joinGameMethodIndex;
        connectButton.onClick.RemoveAllListeners();
        connectButton.onClick.AddListener(() => {
            if(MultiplayerGameManager.Singleton.GetConnectionMethod() == MultiplayerGameManager.ConnectionMethod.Relay)
            {
                MultiplayerGameManager.Singleton.joinCode = connectionInput.text.Substring(0, 6);
            }
            else
            {
                string ipAddress = connectionInput.text;
                ipAddress = ipAddress.Length <= 1?"127.0.0.1":ipAddress.Substring(0,connectionInput.text.Length-1);
                // if(!IsValidIP(ipAddress))
                // {
                //     connectionInput.text = string.Empty;
                //     return;
                // }
                MultiplayerGameManager.Singleton.ipAddress = ipAddress;
            }

            if(temp == 0)
            {
                MultiplayerGameManager.Singleton.StartHost();
            }
            else
            {
                MultiplayerGameManager.Singleton.StartClient();
            }
            connectionInput.text = string.Empty;
            connectionDataWindow.SetActive(false);
        });
    }

    void Hide()
    {
        createGameButton.gameObject.SetActive(false);
        joinGameButton.gameObject.SetActive(false);
    }

    void Show()
    {
        createGameButton.gameObject.SetActive(true);
        joinGameButton.gameObject.SetActive(true);
    }

    void OnDestroy()
    {
        if(NetworkManager.Singleton)
            NetworkManager.Singleton.OnConnectionEvent -= ConnectionEventHandler;
    }

    public bool IsValidIP(string ip)
    {
        IPAddress ipAddr;
        // verify that IP consists of 4 parts
        if (ip.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries).Length == 4)
        {
            if (IPAddress.TryParse(ip, out ipAddr))
            {
                // IP is valid
                return true;
            }
            else
            {
                // invalid IP
                return false;
            }
        }
        else
        {
            // invalid IP
            return false;
        }
    }
}
