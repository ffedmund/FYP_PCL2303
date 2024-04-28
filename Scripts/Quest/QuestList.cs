using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class QuestList : MonoBehaviour {
    [SerializeField] List<int> questIdList;
    [SerializeField] bool loadAllRegularQuests;
    [SerializeField] bool loadAllRankQuests;
    public Dictionary<int,Quest> questIdDictionary;
    List<Quest> regularQuestList;
    bool haveInitQuests;

    void Awake() {
        if(questIdDictionary == null){
            questIdDictionary = new Dictionary<int, Quest>();
        }
        if(regularQuestList == null){
            regularQuestList = new List<Quest>();
        }
        if(questIdList == null || questIdList.Count == 0){
            questIdList = new List<int>();
            haveInitQuests = true;
        }
    }

    async void Start() {
        await DataReader.ReadQuestDataBase();
        await Task.Run(() =>
        {
            if (!haveInitQuests)
            {
                InitQuestList(questIdList);
            }
            foreach (Quest quest in DataReader.questDictionary.Values)
            {
                if ((quest.questType == QuestType.Rank && loadAllRankQuests) || (quest.questType == QuestType.Regular && loadAllRegularQuests))
                {
                    LoadQuestToDict(quest);
                }
            }
        });
    }

    public void Init(){
        if(questIdDictionary == null){
            questIdDictionary = new Dictionary<int, Quest>();
        }
        if(regularQuestList == null){
            regularQuestList = new List<Quest>();
        }
        if(questIdList == null || questIdList.Count == 0){
            questIdList = new List<int>();
            haveInitQuests = true;
        }
        Start();
    }

    public void InitQuestList(List<int> questIds)
    {
        foreach (int id in questIds)
        {
            LoadQuestToDict(id);
            questIdList.Add(id);
        }
    }

    public async void LoadQuestToDict(int id)
    {
        await DataReader.ReadQuestDataBase();
        try
        {
            if (!questIdDictionary.ContainsKey(id))
            {
                questIdDictionary.Add(id, DataReader.questDictionary[id]);
            }
            if (DataReader.questDictionary[id].questType == QuestType.Regular)
            {
                regularQuestList.Add(DataReader.questDictionary[id]);
            }
        }
        catch
        {
            Debug.LogWarning($"Quest id [{id}] is missing.");
        }
    }

    public async void LoadQuestToDict(Quest quest)
    {
        await DataReader.ReadQuestDataBase();
        try
        {
            if (!questIdDictionary.ContainsKey(quest.id))
            {
                questIdDictionary.Add(quest.id, quest);
            }
            if (quest.questType == QuestType.Regular)
            {
                regularQuestList.Add(quest);
            }
        }
        catch
        {
            Debug.LogWarning($"Quest [{quest}] is wrong.");
        }
    }

    public List<Quest> PickRandomQuestList(QuestType requireType, int length = -1){
        List<Quest> tempQuestList = SearchQuestList(requireType,length*2);
        return ShuffleList(tempQuestList,length);
    }

    public List<Quest> PickRandomQuestList(HonorRank requireRank, int length = -1){
        List<Quest> tempQuestList = SearchQuestList(requireRank,length*2);
        return ShuffleList(tempQuestList,length);
    }

    public List<Quest> ShuffleList(List<Quest> tempQuestList, int length){
        System.Random rand = new System.Random();
        List<Quest> questList = new List<Quest>();
        while(questList.Count < length && tempQuestList.Count > 0){
            int index = rand.Next(tempQuestList.Count);
            questList.Add(tempQuestList[index]);
            tempQuestList.RemoveAt(index);
        }
        return questList;
    }

    public List<Quest> SearchQuestList(QuestType requireType, int length = -1, bool isActive = false, bool isFinished = false) {
        List<Quest> questList = new List<Quest>();
        foreach(Quest quest in questIdDictionary.Values){
            if(questList.Count >= length && length != -1){
                break;
            }
            if(quest.questType == requireType && quest.isActive == isActive && quest.isFinished == isFinished){
                questList.Add(quest);
            }
        }
        return questList;
    }

    public List<Quest> SearchQuestList(HonorRank requireRank, int length = -1, bool isActive = false, bool isFinished = false) {
        List<Quest> questList = new List<Quest>();
        foreach(Quest quest in questIdDictionary.Values){
            if(questList.Count >= length && length != -1){
                break;
            }
            if(quest.questType == QuestType.Rank && quest.honorRank == requireRank && quest.isActive == isActive && quest.isFinished == isFinished){
                questList.Add(quest);
            }
        }
        return questList;
    }

    public List<Quest> GetAll(){
        List<Quest> questList = new List<Quest>();
        foreach(Quest quest in questIdDictionary.Values){
            questList.Add(quest);
        }
        return questList;
    }

    public void RefreshRegularQuestList() {
        foreach(Quest quest in regularQuestList){
            if(quest.isActive && quest.isFinished){
                quest.isFinished = false;
                quest.isActive = false;
                quest.goalChecker.currentAmount = 0;
            }
        }
    }

    public bool CanReportQuest(Quest quest) {
        if(quest.goalChecker.isReached() && !quest.isFinished){
           if(questIdDictionary.ContainsKey(quest.id)){
                if(quest.targetNPC.Length > 0){
                    if(TryGetComponent(out NPCController npcController) && npcController.npc.npcName == quest.targetNPC){
                        return true;
                    }else{
                        return false;
                    }
                }else{
                    return true;
                }
           }
        }
        return false;
    }
}