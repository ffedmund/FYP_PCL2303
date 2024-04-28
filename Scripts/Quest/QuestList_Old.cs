// using System;
// using System.Collections.Generic;
// using System.Threading.Tasks;
// using FYP;
// using TMPro;
// using UnityEngine;
// using UnityEngine.UI;

// public class QuestList : MonoBehaviour {
    
//     public QuestType[] questTypes;
//     public int[] specialQuestIds;
//     public int questRefreshFrequency;
//     public List<Quest> currentQuestList = new List<Quest>();
//     public float previousTime = 0;

//     Dictionary<int,List<Quest>> questDictionary = new Dictionary<int, List<Quest>>();

//     public async void Setup(List<int> questIdList,List<QuestType> questTypeList,List<QuestType> questTypes){
//         await DataReader.ReadQuestDataBase();
//         if(questIdList != null){
//             int length = questIdList.Count;
//             specialQuestIds = new int[length];
//             if(!questDictionary.ContainsKey((int)QuestType.Special*10)){
//                 questDictionary.Add((int)QuestType.Special*10,new List<Quest>());
//             }
//             List<Quest> specialQuestList = questDictionary[(int)QuestType.Special*10];
//             for(int i = 0; i < length; i++){
//                 int questId = questIdList[i];
//                 QuestType questType = questTypeList[i];
//                 specialQuestIds[i] = questIdList[i];
//                 switch(questType){
//                     case QuestType.Special:
//                         specialQuestList.Add(DataReader.specialQuestList.Find(quest => quest.id == questId));
//                         break;
//                     case QuestType.Regular:
//                         specialQuestList.Add(DataReader.regularQuestList.Find(quest => quest.id == questId));
//                         break;
//                     case QuestType.Rank:
//                         foreach(HonorRank honorRank in DataReader.rankQuestList.Keys){
//                             specialQuestList.Add(DataReader.rankQuestList[honorRank].Find(quest=> quest.id == questId)); 
//                         }
//                         break;
//                 }
//             }
//         }
//         this.questTypes = new QuestType[questTypes.Count];
//         for(int i = 0; i < this.questTypes.Length; i++){
//             this.questTypes[i] = questTypes[i];
//         }
//         InitQuests();
//         if(TryGetComponent(out QuestGiver questGiver)){
//             questGiver.SetupQuest();
//         }
//         if(TryGetComponent(out QuestReceiver questReceiver)){
//             questReceiver.SetupQuest(this);
//         }
//     }

//     async void Start(){
//         if(questTypes == null)return;
//         await DataReader.ReadQuestDataBase();
//         await Task.Run(() => {
//             foreach(QuestType questType in questTypes){
//                 AddQuestList(questType);
//             }
//             InitQuests();
//         });
//         if(TryGetComponent(out QuestGiver questGiver)){
//             questGiver.SetupQuest();
//         }
//         if(TryGetComponent(out QuestReceiver questReceiver)){
//             questReceiver.SetupQuest(this);
//         }     
//         previousTime = Time.time;
//     }

//     void AddQuestList(QuestType questType){
//         switch(questType){
//             case QuestType.Special:
//                 foreach(int id in specialQuestIds){
//                     if(!questDictionary.ContainsKey((int)questType*10)){
//                         questDictionary.Add((int)questType*10,new List<Quest>());
//                     }   
//                     questDictionary[(int)questType*10].Add(DataReader.specialQuestList.Find(quest => quest.id == id));
//                 }
//                 break;
//             case QuestType.Rank:
//                 foreach(HonorRank honorRank in DataReader.rankQuestList.Keys){
//                     if(!questDictionary.ContainsKey((int)questType*10+(int)honorRank)){
//                         questDictionary.Add((int)questType*10+(int)honorRank,DataReader.rankQuestList[honorRank]); 
//                     }
//                 }
//                 break;
//             default:
//                 if(!questDictionary.ContainsKey((int)questType*10)){
//                     questDictionary.Add((int)questType*10, DataReader.regularQuestList);
//                 }
//                 break;
//         }
//     }

//     void InitQuests(){
//         Debug.Log("Init");
//         if(specialQuestIds.Length > 0){
//             currentQuestList = questDictionary[(int)QuestType.Special*10];
//         }else{
//             foreach(int key in questDictionary.Keys){
//                 for(int i = 0; i < 2; i ++){
//                     if(questDictionary[key].Count > i){
//                         // int rndQuestNumber = Random.Range(0,questDictionary[key].Count);
//                         currentQuestList.Add(questDictionary[key][i]);
//                     }
//                 }
//             }
//         }
//     }

//     public void GetQuests(){
//         if(Time.time - previousTime > questRefreshFrequency){
//             Debug.Log("Quest Refresh");
//             List<Quest> questsToRemove = new List<Quest>();
//             foreach(Quest quest in currentQuestList){
//                 if(quest.isFinished){
//                     if(quest.questType == QuestType.Regular){
//                         if(quest.isActive && quest.isFinished){
//                             quest.isFinished = false;
//                             quest.isActive = false;
//                             quest.goalChecker.currentAmount = 0;
//                         }
//                     }
//                     questsToRemove.Add(quest);
//                 }
//             }
//             foreach(Quest quest in questsToRemove){
//                 currentQuestList.Remove(quest);
//                 int questDictKey = (int)quest.questType*10+(int)quest.honorRank;
//                 Quest newQuest = null;
//                 do{
//                     newQuest = questDictionary[questDictKey][UnityEngine.Random.Range(0,questDictionary[questDictKey].Count)];
//                 }while(currentQuestList.Contains(newQuest));
//                 currentQuestList.Add(newQuest);
//             }
//             previousTime = Time.time;
//         }
//     }

//     public bool CanReportQuest(Quest quest){
//         if(quest.goalChecker.isReached() && !quest.isFinished){
//             for(int i = 0; i < questTypes.Length; i ++){
//                 if(quest.questType == questTypes[i]){
//                     if(quest.questType == QuestType.Regular || quest.questType == QuestType.Rank){
//                         return true;
//                     }else{
//                         foreach(int id in specialQuestIds){
//                             if(quest.id == id && TryGetComponent(out NPCController npcController) && npcController.npc.npcName == quest.targetNPC){
//                                 return true;
//                             }
//                         }
//                     }
//                 }
//             }
//         }
//         return false;
//     }

//     internal void Setup(List<int> npcSpecialQuestList, List<QuestType> npcQuestTypes)
//     {
//         throw new NotImplementedException();
//     }
// }