using UnityEngine;

public class GoddessChatController : MonoBehaviour {
    [SerializeField] ChatBoxUIController chatBoxController;
    [SerializeField] DialogueTree dialogue;
    [SerializeField] JoinGameManager joinGameManager;
    [SerializeField] GameObject particle;
    [SerializeField] GameObject background;
    [SerializeField] EyeOpenSceneTransition eyeOpenSceneTransition;

    DialogueNode currentDialogueNode;

    void Start(){
        particle.SetActive(false);
    }

    public void Chat(){
        string targetContent = "\n";
        if(currentDialogueNode == null){
            currentDialogueNode = dialogue.root;
        }else{
            currentDialogueNode = currentDialogueNode.childs[0].child;
            if(currentDialogueNode.name == "Explaination_03"){
                foreach(PlayerTarget playerTarget in JoinGameManager.playerRole.playerTargets){
                    targetContent += "\t"+playerTarget.shortDescription+"\n";
                }
            }
        }
        // chatBoxController.SetChatbox("Unkown",currentDialogueNode.content+((currentDialogueNode.name == "Explaination_03")?targetContent:""),true,currentDialogueNode.name == "Explaination_04",currentDialogueNode.name == "Explaination_04"?"red":"");
        chatBoxController.SetChatbox("Unknown",currentDialogueNode.content,true,currentDialogueNode.name == "Explaination_04",currentDialogueNode.name == "Explaination_04"?"red":"");

        chatBoxController.ClearChatboxListener();
        if(currentDialogueNode.childs.Length == 0){
            chatBoxController.SetDefaultAnswerListener(()=>{
                chatBoxController.HideChatbox();
                eyeOpenSceneTransition.CloseEye(()=>{
                    particle.SetActive(true);
                    background.SetActive(false); 
                    joinGameManager.StartJoinGameProcess();
                });
            });
        }else{
            chatBoxController.SetDefaultAnswerListener(Chat, currentDialogueNode.name != "Explaination_04");
        }
    }
}