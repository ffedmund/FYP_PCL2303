using UnityEngine;
using Unity.Netcode;
using FYP;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class NetworkPlayerSetup:NetworkBehaviour {
    public PlayerManager playerManager;
    [SerializeField] bool autoSetup;

    InputHandler inputHandler;
    PlayerStats playerStats;

    //Server Use
    [SerializeField]
    List<ulong> notReadyPlayerIds = new List<ulong>();

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        inputHandler = GetComponent<InputHandler>();
        playerStats = GetComponent<PlayerStats>();
        playerManager = GetComponent<PlayerManager>();
        playerStats.SetLocalPlayer(false);
        playerManager.SetLocalPlayer(false);
        if(IsOwnedByServer)
        {
            notReadyPlayerIds = new List<ulong>(NetworkManager.ConnectedClientsIds);
        }

        if(autoSetup)
        {
            Setup();
        }
    }

    public void Setup()
    {
        if(IsOwner)
        {
            UIController uiController = FindAnyObjectByType<UIController>();
            playerStats.SetLocalPlayer(IsOwner);
            playerManager.SetLocalPlayer(IsOwner);
            gameObject.transform.position = new Vector3(0,10,0);
            playerStats.healthBar = uiController.healthBar;
            playerStats.staminaBar = uiController.staminaBar;

            foreach(PlayerSetupBehaviour playerSetupBehaviour in PlayerSetupBehaviour.Instances)
            {
                playerSetupBehaviour.Setup(playerManager);
            }
            
            inputHandler.EnableInput();
            uiController.Setup(gameObject);
            playerStats.SetUI(true);
            playerManager.interactableUIGameObject = uiController.interactUI.transform.GetChild(0).gameObject;
            playerManager.itemInteractableGameObject = uiController.interactUI.transform.GetChild(1).gameObject;
            foreach(NPCStateController npc in FindObjectsByType<NPCStateController>(FindObjectsSortMode.None)){
                npc.Setup(gameObject);
            }
            SetUpNoticeRpc(OwnerClientId);
        }
    }

    [Rpc(SendTo.Server)]
    void SetUpNoticeRpc(ulong clientId)
    {
        notReadyPlayerIds.Remove(clientId);
        if(notReadyPlayerIds.Count == 0)
        {
            AllPlayerSetupReadyRpc();
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    void AllPlayerSetupReadyRpc()
    {
        GameManager.instance.SetGameState(GameManager.GameState.Ready);
    }
}