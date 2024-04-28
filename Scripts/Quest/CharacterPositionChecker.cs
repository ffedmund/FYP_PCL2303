using System.Collections.Generic;
using FYP;
using UnityEngine;

public class CharacterPositionChecker : MonoBehaviour {
    [SerializeField] string locationId;

    void ReportQuest(Quest quest, PlayerManager playerManager){
        playerManager.playerData.AddPlayerData("money", quest.moneyReward);
        playerManager.playerData.AddPlayerData("honor", quest.honorReward);

        playerManager.playerStats.GainExp(quest.expReward);
        UIController uiController = FindAnyObjectByType<UIController>();
        uiController.UpdateHonorUI();
        uiController.SetProgressTitle("Quest Complete");
        playerManager.itemInteractableGameObject?.GetComponent<ItemPopUpController>().NewPopUp(ItemPopUpController.PopUpIcon.Money,quest.moneyReward);
        playerManager.itemInteractableGameObject?.GetComponent<ItemPopUpController>().NewPopUp(ItemPopUpController.PopUpIcon.Honor,quest.honorReward);
    }

    private void OnTriggerEnter(Collider other) 
    {
        if(other.TryGetComponent(out PlayerManager playerManager))
        {
            List<Quest> reportedQuest = new List<Quest>();
            foreach(Quest quest in playerManager.playerData.quests)
            {
                if(quest.goalChecker.goalType == GoalType.Transportation && quest.goalChecker.targetId == locationId)
                {
                    ReportQuest(quest,playerManager);
                    reportedQuest.Add(quest);
                }
            }

            foreach(Quest quest in reportedQuest)
            {
                playerManager.playerData.quests.Remove(quest);
            }
        }
    }
}