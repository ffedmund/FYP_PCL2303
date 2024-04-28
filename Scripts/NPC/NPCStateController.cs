using FYP;
using Unity.Netcode;
using UnityEngine;

public class NPCStateController : MonoBehaviour {
    public NPCController npcController;
    public Transform interactingTarget;
    public Transform attacker;
    public string receiveChatContent;
    public NPCStateController speakingNPC; //Can use queue in future
    public bool needRest;
    public bool backingHome;
    public bool backedHome;
    public string currentStateName;

    NPCState initialState;
    NPCState currentState;
    Transform playerTransform;

    bool isSetup;

    public void Start(){
        if(!NetworkManager.Singleton){
            Setup(FindAnyObjectByType<PlayerManager>().gameObject);
        }
        else if(NetworkManager.Singleton.IsClient && NetworkManager.Singleton.LocalClient.PlayerObject != null){
            Setup(NetworkManager.Singleton.LocalClient.PlayerObject.gameObject);
        }
    }

    public void Setup(GameObject localPlayer){
        initialState = new NPCIdleState();
        playerTransform = localPlayer.transform;
        currentState = initialState;
        backingHome = false;
        backedHome = false;
        npcController = GetComponent<NPCController>();
        npcController.Init(localPlayer);
        isSetup = true;
    }

    public void ReceiveChat(NPCStateController chatSender, string content){
        if(currentState is WaitingResponseState){
            receiveChatContent = content;
            speakingNPC = chatSender;
        }
    }

    bool IsRestNeeded(){
        float currentTime = DayCycleManager.instance == null?120:DayCycleManager.instance.currentTime;
        return (currentTime >= 300 && currentTime < 360) || (currentTime >= 0 && currentTime < 90);
    }
    void FixedUpdate() {
        HandleStateMachine();
    }

    void HandleStateMachine() {
        if(isSetup){
            if(npcController.npcStats.isDead){
                npcController.Dead();
                currentState = initialState;
                return;
            }
            needRest = IsRestNeeded();
            currentStateName = currentState.GetType().Name;
            if(npcController.npcInteraction.isInteracting){
                currentState = new InteractWithPlayerState();
            }else {
                if(currentState != null){
                    NPCState nextState = currentState.Update(this);
                    currentState = nextState == null? initialState:nextState;
                }
            }
        }
    }

}