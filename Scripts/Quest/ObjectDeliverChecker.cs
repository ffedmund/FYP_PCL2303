using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using FYP;

public class ObjectDeliverChecker : MonoBehaviour {
    public int[] questIds;
    List<Quest> questList;
    PlayerManager playerManager;
    
    async void Start() {
        await DataReader.ReadQuestDataBase();
        questList = new List<Quest>();
        for(int i = 0; i < questIds.Length; i++){
            questList.Add(DataReader.questDictionary[questIds[i]]);
        }
    }

    public void ItemDelivered(string itemName){
        foreach(Quest quest in questList){
            if(itemName == quest.goalChecker.targetId && quest.isActive && !quest.isFinished){
                quest.goalChecker.currentAmount++;
                playerManager = GameManager.instance.localPlayerManager?GameManager.instance.localPlayerManager:FindAnyObjectByType<PlayerManager>();
                break;
            }
        }
    }

    
    private void OnTriggerEnter(Collider other) {
        Debug.Log("Object Name: " + other.gameObject.name);
        ItemDelivered(other.gameObject.name);
    }

    private void OnTriggerExit(Collider other) {
        foreach(Quest quest in questList){
            if(other.gameObject.name == quest.goalChecker.targetId && quest.isActive && !quest.isFinished){
                quest.goalChecker.ItemLosted(other.gameObject.name);
                break;
            }
        }
    }
}