using FYP;
using UnityEngine;

[System.Serializable]
public enum ItemEventType{
    Reward,
    Mission
}

[System.Serializable]
public class ItemGivingEvent{
    public Item item;
    public int itemAmount;
    public System.Func<bool> condition;
    public ItemEventType itemEventType;
    public int triggerQuestId;
    public bool doNotTriggerItemCollect;
    public string givingItemDialogue;

    public void GiveItemToPlayer(PlayerInventory playerInventory) {
        if(item is MaterialItem){
            playerInventory.AddItem(item,itemAmount);
            playerInventory.GetComponent<PlayerManager>().itemInteractableGameObject.GetComponent<ItemPopUpController>().NewPopUp(item,itemAmount);
        }
        if(item is WeaponItem){
            playerInventory.weaponsInventory.Add((WeaponItem)item);
            playerInventory.GetComponent<PlayerManager>().itemInteractableGameObject.GetComponent<ItemPopUpController>().NewPopUp(item);
        }

    }

    public void GiveItemToPlayer(PlayerInventory playerInventory, Quest quest) {
        GiveItemToPlayer(playerInventory);
        if(doNotTriggerItemCollect){
            return;
        }
        if(item is MaterialItem){
            for(int i = 0; i < itemAmount; i++){
                quest.goalChecker.ItemCollected(item.itemName);
            }
        }
    }
}