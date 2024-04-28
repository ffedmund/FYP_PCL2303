using System.Collections.Generic;
using FYP;
using UnityEngine;

public class NPCItemGiver : MonoBehaviour {

    List<ItemGivingEvent> itemGivingEvents = new List<ItemGivingEvent>();
    List<ItemGivingEvent> givedItemEvents = new List<ItemGivingEvent>();

    public void Setup(NPC npc, Transform playerTransform){
        if(npc.itemGivingEvents != null && npc.itemGivingEvents.Length > 0){

            foreach(ItemGivingEvent itemGivingEvent in npc.itemGivingEvents){
                itemGivingEvent.condition = () =>
                {
                    return playerTransform.GetComponent<PlayerManager>().playerData.quests.Find(quest => quest.id == itemGivingEvent.triggerQuestId && quest.isFinished == (itemGivingEvent.itemEventType == ItemEventType.Reward)) != null;
                };
                itemGivingEvents.Add(itemGivingEvent);
            }
        }
    }

    public bool HaveItemGiveToPlayer(){
        foreach(ItemGivingEvent itemGivingEvent in itemGivingEvents){
            if(itemGivingEvent.condition()){
                return true;
            }
        }
        return false;
    }

    void ResetItemGivingEvent(PlayerManager playerManager){
        List<ItemGivingEvent> resetItemEvents = new List<ItemGivingEvent>();
        foreach(ItemGivingEvent givedItemEvent in givedItemEvents){
            if(givedItemEvent.itemEventType == ItemEventType.Mission && (!playerManager.GetComponent<PlayerInventory>().materialsInventory.ContainsKey(givedItemEvent.item.itemName) || playerManager.playerData.quests.Find(quest => quest.id == givedItemEvent.triggerQuestId) == null)){
                itemGivingEvents.Add(givedItemEvent);
                resetItemEvents.Add(givedItemEvent);
            }
        }

        foreach(ItemGivingEvent resetItemEvent in resetItemEvents){
            givedItemEvents.Remove(resetItemEvent);
        }
    }

    public string TriggerEvents(PlayerManager playerManager){
        ItemGivingEvent triggeredEvent = null;

        foreach(ItemGivingEvent itemGivingEvent in itemGivingEvents){
            if(itemGivingEvent.condition()){
                Quest relativeQuest = playerManager.playerData.quests.Find(quest => quest.id == itemGivingEvent.triggerQuestId);
                itemGivingEvent.GiveItemToPlayer(playerManager.GetComponent<PlayerInventory>(),relativeQuest);
                givedItemEvents.Add(itemGivingEvent);
                triggeredEvent = itemGivingEvent;
                break;
            }
        }

        itemGivingEvents.Remove(triggeredEvent);
        ResetItemGivingEvent(playerManager);

        if(triggeredEvent != null){
            return triggeredEvent.givingItemDialogue;
        }else{
            return null;
        }
    }

}