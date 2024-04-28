using System.Collections.Generic;
using FYP;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestGiver : MonoBehaviour {
    QuestList questList;

    public void SetupQuest(QuestList questList)
    {
        this.questList = questList;
    }

    public void AcceptQuest(Quest quest){
        PlayerData playerData = GameManager.instance.localPlayerManager.playerData;
        PlayerInventory playerInventory = FindAnyObjectByType<PlayerInventory>();
        if(playerData != null && !playerData.quests.Contains(quest) && questList.questIdDictionary.ContainsKey(quest.id)){
            quest.isActive = true;
            playerData.quests.Add(quest);
        }
        if(quest.goalChecker.goalType == GoalType.Delivery && playerInventory.materialsInventory.ContainsKey(quest.goalChecker.targetId)){
            quest.goalChecker.currentAmount = playerInventory.materialsInventory[quest.goalChecker.targetId].itemAmount;
        }
    }

}