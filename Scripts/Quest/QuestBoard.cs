using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuestBoard{
    const int REGULAR_QUEST_COUNT = 2;
    const int CURRENT_RANK_QUEST_COUNT = 2;
    const int NEXT_RANK_QUEST_COUNT = 2;

    public QuestList questList;
    public QuestGiver questGiver;
    public QuestReceiver questReceiver;
    public List<Quest> currentQuestList = new List<Quest>();
    public bool inited;
    PlayerData playerData;

    public QuestBoard(QuestGiver questGiver, QuestReceiver questReceiver, QuestList questList, PlayerData playerData){
        this.questGiver = questGiver;
        this.questReceiver = questReceiver;
        this.questList = questList;
        this.playerData = playerData;
        inited = true;
    }

    public void UpdateQuests(){
        List<Quest> newQuests = GenerateQuests();
        ReplaceFinishedQuests(newQuests);
        if(currentQuestList.Count < 6){
            FillQuestList(newQuests);
        }
        questList.RefreshRegularQuestList();
    }

    List<Quest> GenerateQuests(){
        List<Quest> newQuests = new List<Quest>(questList.PickRandomQuestList(QuestType.Regular, REGULAR_QUEST_COUNT));
        newQuests.AddRange(questList.PickRandomQuestList((HonorRank)playerData.GetHonorLevel(), CURRENT_RANK_QUEST_COUNT));
        newQuests.AddRange(questList.PickRandomQuestList((HonorRank)(playerData.GetHonorLevel()+1), NEXT_RANK_QUEST_COUNT));
        return newQuests;
    }

    void ReplaceFinishedQuests(List<Quest> newQuests){
        for(int i = 0; i < currentQuestList.Count; i++){
            if(currentQuestList[i].isFinished && newQuests.Count > 0){
                Quest quest;
                Debug.Log($"Replace Quest: {currentQuestList[i].title}");
                if(currentQuestList[i].questType == QuestType.Regular){
                    quest = newQuests[0];
                    newQuests.RemoveAt(0);
                }else{
                    quest = newQuests[newQuests.Count-1];
                    newQuests.RemoveAt(newQuests.Count-1);
                }
                currentQuestList[i] = quest;
            }
        }
        //Remove Duplicate
        HashSet<Quest> hashSet = new HashSet<Quest>(currentQuestList);
        currentQuestList = new List<Quest>(hashSet);
    }

    void FillQuestList(List<Quest> newQuests){
        Debug.Log(newQuests.Count);
        while(currentQuestList.Count < 6 && newQuests.Count > 0){
            currentQuestList.Add(newQuests[newQuests.Count-1]);
            newQuests.RemoveAt(newQuests.Count-1);
        }
    }

}