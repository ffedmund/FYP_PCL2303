using System.Collections.Generic;
using FYP;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestReceiver : MonoBehaviour
{
    QuestList questList;
    UIController uiController;

    private void Awake() {
        uiController = FindAnyObjectByType<UIController>();
    }

    public void SetupQuest(QuestList questList)
    {
        this.questList = questList;
    }

    public void ReportQuest(Quest reportQuest, PlayerManager playerManager)
    {
        if (questList.CanReportQuest(reportQuest))
        {
            reportQuest.isFinished = true;
            PlayerData playerData = playerManager.playerData;
            playerData.quests.Remove(reportQuest);
            playerData.AddPlayerData("money", reportQuest.moneyReward);
            playerData.AddPlayerData("honor", reportQuest.honorReward);

            playerManager.playerStats.GainExp(reportQuest.expReward);
            playerManager.itemInteractableGameObject.GetComponent<ItemPopUpController>().NewPopUp(ItemPopUpController.PopUpIcon.Money,reportQuest.moneyReward);
            playerManager.itemInteractableGameObject.GetComponent<ItemPopUpController>().NewPopUp(ItemPopUpController.PopUpIcon.Honor,reportQuest.honorReward);

            uiController.UpdateHonorUI();
            uiController.SetProgressTitle("Quest Complete");
            if(reportQuest.itemReward != "" && ItemDatabase.instance != null)
            {
                playerManager.playerInventory.AddItem(ItemDatabase.instance.GetItemById(reportQuest.itemReward),reportQuest.itemRewardAmount);
            }
            if (reportQuest.goalChecker.goalType == GoalType.Delivery)
            {
                ReceiveItem(reportQuest);
            }
        }
    }

    void ReceiveItem(Quest quest)
    {
        PlayerInventory playerInventory;
        PlayerManager playerManager;
        // playerManager = FindAnyObjectByType<PlayerManager>();
        playerManager = GameManager.instance.localPlayerManager;
        playerInventory = playerManager.GetComponent<PlayerInventory>();
        TryGetComponent(out NPCInventory npcInventory);

        Item item = playerInventory.RemoveItem(quest.goalChecker.targetId,quest.goalChecker.targetAmount);
        if (npcInventory && item != null)
        {
            npcInventory.AddItem((Item)(object)item,quest.goalChecker.targetAmount);
        }

        List<Quest>sameTargetQuestList = playerManager.playerData.quests.FindAll(activeQuest => activeQuest.goalChecker.targetId == quest.goalChecker.targetId);
        foreach(Quest activeQuest in sameTargetQuestList){
            if(activeQuest.goalChecker.goalType == GoalType.Delivery && playerInventory.materialsInventory.ContainsKey(quest.goalChecker.targetId)){
                quest.goalChecker.currentAmount = playerInventory.materialsInventory[quest.goalChecker.targetId].itemAmount;
            }
        }
    }

}