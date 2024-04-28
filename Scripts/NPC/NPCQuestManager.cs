using System.Collections.Generic;
using FYP;
using System;
using UnityEngine;

public struct NPCQuestInteraction{
    public bool receivedQuest;
    public bool giveQuest;
    public string reponseDialogue;
    public string playerChoiceDialogue; 
    public Action acceptQuestCallback;
}

public class NPCQuestManager:MonoBehaviour{
    QuestGiver questGiver;
    QuestReceiver questReceiver;
    QuestList questList;
    NPCItemGiver npcItemGiver;
    NPCDialogueController npcDialogueController;

    List<int> receiveQuestIdList = new List<int>();
    List<int> giveQuestIdList = new List<int>();
    List<int> lockedQuestIdList = new List<int>();

    public void Setup(NPC npc)
    {
        this.npcDialogueController = GetComponent<NPCDialogueController>();
        if (npc.questList != null && npc.questList.Length > 0)
        {
            if (!gameObject.TryGetComponent(out questList))
            {
                questList = gameObject.AddComponent<QuestList>();
            }
            List<int> npcSpecialQuestList = new List<int>();
            List<QuestType> npcQuestTypeList = new List<QuestType>();
            List<QuestType> npcQuestTypes = new List<QuestType>();
            foreach (QuestListData questData in npc.questList)
            {
                if (!npcQuestTypes.Contains(questData.questType))
                {
                    npcQuestTypes.Add(questData.questType);
                }
                if (!npcSpecialQuestList.Contains(questData.questId))
                {
                    npcSpecialQuestList.Add(questData.questId);
                }
                npcQuestTypeList.Add(questData.questType);
                switch (questData.questHostType)
                {
                    case QuestHostType.QUESTGIVER:
                        if (questData.questLock)
                        {
                            lockedQuestIdList.Add(questData.questId);
                        }
                        else
                        {
                            giveQuestIdList.Add(questData.questId);
                        }
                        if (!TryGetComponent(out questGiver))
                        {
                            questGiver = gameObject.AddComponent<QuestGiver>();
                        }
                        break;
                    case QuestHostType.QUESTRECEIVER:
                        receiveQuestIdList.Add(questData.questId);
                        if (!TryGetComponent(out questReceiver))
                        {
                            questReceiver = gameObject.AddComponent<QuestReceiver>();
                        }
                        break;
                    default:
                        break;
                }
            }
            questList.InitQuestList(npcSpecialQuestList);
            // questList.Setup(npcSpecialQuestList,npcQuestTypeList,npcQuestTypes);
        }
        if (questReceiver)
        {
            questReceiver.SetupQuest(questList);
        }
        if (questGiver)
        {
            questGiver.SetupQuest(questList);
        }
    }

    void Start(){
        TryGetComponent(out npcItemGiver);
    }

    //Receive can report quest from player or return can accept quest
    public NPCQuestInteraction InteractQuest(PlayerManager playerManager){
        List<Quest> playerQuestList = playerManager.playerData.quests;
        Quest canReportQuest = CanReceiveQuest(playerQuestList);
        Quest canGiveQuest = CanGiveQuest(playerManager);
        
        NPCQuestInteraction npcQuestInteraction = new NPCQuestInteraction();
        if(canReportQuest != null)
        {
            questReceiver.ReportQuest(canReportQuest,playerManager);
            npcQuestInteraction.reponseDialogue = canReportQuest.completeDialog;
            npcQuestInteraction.receivedQuest = true;
            npcDialogueController.TriggerDialogue(canReportQuest.id);
        }else{
            npcQuestInteraction.reponseDialogue = canGiveQuest.description;
            npcQuestInteraction.playerChoiceDialogue = "Ok!";
            npcQuestInteraction.acceptQuestCallback = () => {this.questGiver.AcceptQuest(canGiveQuest);};
            npcQuestInteraction.giveQuest = true;
        }
        return npcQuestInteraction;
    }

    public bool GiveQuest(int questID){
        if(questList.questIdDictionary.ContainsKey(questID) && (lockedQuestIdList.Contains(questID) || giveQuestIdList.Contains(questID))){
            this.questGiver.AcceptQuest(questList.questIdDictionary[questID]);
            return true;
        }
        return false;
    }


    public Quest CanGiveQuest(PlayerManager playerManager){
        Quest canGiveQuest = questGiver?questList.GetAll().Find(quest => 
            (int)quest.honorRank <= playerManager.playerData.GetHonorLevel() && 
            !quest.isActive && 
            !quest.isFinished && 
            giveQuestIdList.Contains(quest.id)
        ):null;
        return canGiveQuest;
    }

    public Quest CanReceiveQuest(List<Quest> playerQuestList){

        Quest canReportQuest = questReceiver?playerQuestList.Find(quest => 
            quest.goalChecker.isReached() && 
            questList.CanReportQuest(quest) && 
            receiveQuestIdList.Contains(quest.id)
        ):null;
        return canReportQuest;
    }

    public bool HaveValidQuestEvent(PlayerManager playerManager){
        try{
            List<Quest> playerQuestList = playerManager.playerData.quests;
            return CanReceiveQuest(playerQuestList) != null || CanGiveQuest(playerManager) != null;
        }catch{
            return false;
        }
    }
}