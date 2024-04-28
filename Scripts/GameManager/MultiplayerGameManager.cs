using System;
using System.Linq;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FYP
{  
    public class MultiplayerGameManager : MonoBehaviour 
    {
    
        public enum NetworkState
        {
            Host,
            Online,
            Connecting,
            Offline
        }

        public enum ConnectionMethod
        {
            Relay,
            Direct
        }

        public static MultiplayerGameManager Singleton { get; private set; }

        public Action<NetworkState> OnNetworkConnected;
        public string joinCode;
        public string ipAddress;
        
        [SerializeField] NetworkState m_networkState = NetworkState.Offline;
        [SerializeField] ConnectionMethod m_connectionMethod = ConnectionMethod.Direct;
        [SerializeField] GameObject room;
        [SerializeField] private int m_TotalPlayers = 0;
        private const int MAX_PLAYERS = 8;
        Room currentRoom;
        Scene _scene;

        private void Awake()
        {
            // Singleton pattern
            if (Singleton == null)
            {
                Singleton = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        async void Start()
        {
            await UnityServices.InitializeAsync();
            NetworkManager.Singleton.OnConnectionEvent += ConnectionEventHandler;
            AuthenticationService.Instance.SignedIn += () => {
                Debug.Log($"Signed in {AuthenticationService.Instance.PlayerId}");
            };
            
            if(!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }
        }

        private void SceneManagerEventHandler(SceneEvent sceneEvent)
        {
            switch (sceneEvent.SceneEventType)
            {
                case SceneEventType.LoadEventCompleted:
                    Debug.Log($"All Client Have Load Scene {sceneEvent.SceneName}");
                    if(sceneEvent.SceneName == MySceneManager.MyScene.MultiPlayerGameScene.ToString())
                    {
                        if(NetworkManager.Singleton.IsServer)
                        {
                            GameManager.instance.SpawnPlayerGameObject();
                        }
                    }
                break;
            }
        }

        private void ConnectionEventHandler(NetworkManager manager, ConnectionEventData data)
        {
            Debug.Log("Connection Event: " + data.EventType);
            bool isLocal = data.ClientId == NetworkManager.Singleton.LocalClientId;
            switch (data.EventType)
            {
                case ConnectionEvent.ClientConnected:
                    if(isLocal)
                    {
                        m_networkState = NetworkState.Online;
                        NetworkManager.Singleton.SceneManager.OnSceneEvent += SceneManagerEventHandler;
                    }
                    
                    if(NetworkManager.Singleton.IsServer)
                    {
                        if (m_TotalPlayers < MAX_PLAYERS)
                        {
                            m_TotalPlayers++;
                            Debug.Log("A player has connected. Total players: " + m_TotalPlayers);
                        }
                    }
                    break;
                case ConnectionEvent.ClientDisconnected:
                    if(isLocal)
                    {
                        Debug.Log(NetworkManager.Singleton.DisconnectReason);
                        Disconnected();
                    }   
                    break;
                case ConnectionEvent.PeerDisconnected:
                    Debug.Log("A peer has connected. Client ID: " + data.ClientId);
                    if(NetworkManager.Singleton.ConnectedClientsIds.Contains(data.ClientId))
                    {
                        m_TotalPlayers--;
                    }   
                    break;
                default:
                    return;
            }
        }

        void SetUpServer()
        {
            // Register callbacks
            Debug.Log("Set Up Server");
            NetworkManager.Singleton.ConnectionApprovalCallback = ApprovalCheck;
        }

        private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
        {
            Debug.Log("Approval Check");
            // The client identifier to be authenticated
            var clientId = request.ClientNetworkId;

            // Additional connection data defined by user code
            var connectionData = request.Payload;

            // Your approval logic determines the following values
            // response.Approved = m_TotalPlayers < MAX_PLAYERS;

            if(m_TotalPlayers < MAX_PLAYERS && currentRoom?.roomState != Room.RoomState.Started)
            {
                response.Approved = true;
                response.CreatePlayerObject = NetworkManager.Singleton.NetworkConfig.PlayerPrefab != null;
                
                // The Prefab hash value of the NetworkPrefab, if null the default NetworkManager player Prefab is used
                response.PlayerPrefabHash = null;

                // Position to spawn the player object (if null it uses default of Vector3.zero)
                response.Position = Vector3.zero;

                // Rotation to spawn the player object (if null it uses the default of Quaternion.identity)
                response.Rotation = Quaternion.identity;
                return;
            }

            response.Approved = false;
            // If response.Approved is false, you can provide a message that explains the reason why via ConnectionApprovalResponse.Reason
            // On the client-side, NetworkManager.DisconnectReason will be populated with this message via DisconnectReasonMessage
            response.Reason = m_TotalPlayers < MAX_PLAYERS? "Some reason for not approving the client":"The Room is Full";

            // If additional approval steps are needed, set this to true until the additional steps are complete
            // once it transitions from true to false the connection approval response will be processed.
            response.Pending = false;
        }

        public void SetConnectionMethod(ConnectionMethod connectionMethod)
        {
            m_connectionMethod = connectionMethod;
        }
        
        public ConnectionMethod GetConnectionMethod()
        {
            return m_connectionMethod;
        }

        public void Disconnected()
        {
            m_networkState = NetworkState.Offline;
            m_TotalPlayers = 0;
            currentRoom = null;
            if(GameManager.instance == null || GameManager.instance.state != GameManager.GameState.End)
            {
                MySceneManager.instance.LoadScene(MySceneManager.MyScene.Lobby);
            }
        }

        public async void StartHost()
        {
            SetUpServer();
            switch (m_connectionMethod)
            {
                case ConnectionMethod.Relay:
                    try {
                        Allocation allocation = await RelayService.Instance.CreateAllocationAsync(5);            

                        string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
                        
                        Debug.Log("Join Code: " + joinCode);
                        this.joinCode = joinCode;

                        RelayServerData relayServerData= new RelayServerData(allocation, "udp");

                        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
                    }
                    catch (RelayServiceException ex) {
                        Debug.Log(ex);
                    }
                break;
                default:
                    NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(
                        ipAddress,  // The IP address is a string
                        (ushort)7777, // The port number is an unsigned short
                        "0.0.0.0" // The server listen address is a string.
                    );
                break;
            }

            if(NetworkManager.Singleton.StartHost()){
                m_networkState = NetworkState.Host;
                NetworkObject createRoom = Instantiate(room).GetComponent<NetworkObject>();
                currentRoom = createRoom.GetComponent<Room>();
                createRoom.Spawn();
                // FindAnyObjectByType<Room>().GetComponent<NetworkObject>().Spawn();
            }
        }

        public async void StartClient()
        {
            switch (m_connectionMethod)
            {
                case ConnectionMethod.Relay:
                    try {
                    Debug.Log($"Join relay {joinCode}");
                    JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);


                    RelayServerData relayServerData = new RelayServerData(joinAllocation, "udp");

                    NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

                    }catch(RelayServiceException e)
                    {
                        Debug.Log(e);
                    }
                break;
                default:
                    NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(
                        ipAddress,  // The IP address is a string
                        (ushort)7777, // The port number is an unsigned short
                        "0.0.0.0"
                    );
                break;
            }

            if(NetworkManager.Singleton.StartClient())
            {
                m_networkState = NetworkState.Connecting;
            }
            else
            {
                Debug.Log("Error on Starting as Client.");
                m_networkState = NetworkState.Offline;
            }

        }
    }
}