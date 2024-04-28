using FYP;
using UnityEngine;

public class NPCNoticeController : MonoBehaviour {
    
    public float activeDistance = 20;
    PlayerManager playerManager;
    Notice notice;

    public void Setup(PlayerManager localPlayerManager){
        playerManager = localPlayerManager;
        notice = GetComponentInChildren<Notice>();
    }

    public bool SetNotice(NPCQuestManager npcQuestManager,NPCItemGiver npcItemGiver,Transform target){
        Vector3 directionToTarget = target.position-transform.position;
        if(Vector3.Magnitude(directionToTarget) <= activeDistance){
            notice.gameObject.SetActive(true);
            notice.UpdateNoticeDirection(directionToTarget);
            if(npcQuestManager.CanReceiveQuest(playerManager.playerData.quests) != null){
                notice.SetNotice(1);
            }else if(npcItemGiver && npcItemGiver.HaveItemGiveToPlayer()){
                notice.SetNotice(1);
                notice.SetColor(Color.gray);
            }else if(npcQuestManager.CanGiveQuest(playerManager) != null){
                notice.SetNotice(0);
            }else{
                notice.ClearNotice();
                return false;
            }
            return true;
        }else{
            notice.gameObject.SetActive(false);
        }
        return false;
    }

    public bool SetChatCloud(Transform target, string content){
        Vector3 directionToTarget = target.position-transform.position;
        if(Vector3.Magnitude(directionToTarget) <= activeDistance && content != ""){
            // Debug.Log("Set Chat Cloud: " + content);
            notice.gameObject.SetActive(true);
            notice.UpdateNoticeDirection(directionToTarget);
            notice.SetNotice(2,content,false);
            return true;
        }else{
            // Debug.Log("Chat Cloud active distance too far: " + Vector3.Magnitude(directionToTarget) + " Target: " + target.gameObject.name + " Content: " + content);
            notice.gameObject.SetActive(false);
            return false;
        }
    }

    public void ClearNotice()
    {
        notice.gameObject.SetActive(false);
    }
}