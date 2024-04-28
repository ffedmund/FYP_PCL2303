using UnityEngine;
using FYP;
using System.Collections.Generic;

public class PlayerQuickSlots : MonoBehaviour {
    public Item[] shortcutItemArr = new Item[3];
    List<int> emptyShortcutItemPivot;
    QuickSlotsUI quickSlotsUI;
    PlayerInventory playerInventory;

    void Start(){
        emptyShortcutItemPivot = new List<int>();
        playerInventory = GetComponent<PlayerInventory>();
        quickSlotsUI = FindAnyObjectByType<QuickSlotsUI>();
        for(int i = shortcutItemArr.Length-1 ; i >= 0 ; i--){
            emptyShortcutItemPivot.Add(i);
        }
    }

   public int IndexOfQuickSlots(string itemName){
        for(int i = 0; i < shortcutItemArr.Length; i++){
            if(shortcutItemArr[i] != null && itemName == shortcutItemArr[i].itemName){
                return i;
            }
        }
        return -1;
    }

    public bool HandleItemToSlots(Item item){
        if(!AddItem(item)){
            RemoveQuickSlotItem(item);
            return false;
        }
        return true;
    }

    bool AddItem(Item item){
        if(emptyShortcutItemPivot.Count > 0 && IndexOfQuickSlots(item.itemName) == -1){
            shortcutItemArr[emptyShortcutItemPivot[emptyShortcutItemPivot.Count-1]] = Instantiate(item);
            emptyShortcutItemPivot.RemoveAt(emptyShortcutItemPivot.Count-1);
            quickSlotsUI.UpdateShortcutUI(shortcutItemArr);
            return true;
        }
        return false;
    }

    public bool RemoveQuickSlotItem(Item item){
        int index = IndexOfQuickSlots(item.itemName);
        if(index != -1){
            shortcutItemArr[index] = null;
            emptyShortcutItemPivot.Add(index);
            quickSlotsUI.UpdateShortcutUI(shortcutItemArr);
            return true;
        }else{
            return false;
        }
    }


    public void UseItemInSlot(int index){
        Debug.Log("Quick Slot Pressed: " + index);
        if(shortcutItemArr[index] != null){
            Item item = shortcutItemArr[index];
            if(playerInventory.materialsInventory.ContainsKey(item.itemName)){
                ItemInteractManager.UseItemEffect((MaterialItem)item);
                playerInventory.RemoveItem(item);
                foreach(Quest quest in playerInventory.GetComponent<PlayerManager>().playerData.quests){
                    if(quest.goalChecker.ItemLosted(item.itemName)){
                        break;
                    }
                };
            }

            if(!playerInventory.materialsInventory.ContainsKey(item.itemName)){
                RemoveQuickSlotItem(item);
            }
        }
    }

    public bool IsShortcutItem(Item item){
        for(int i = 0; i < 3 ;i++){
            if(shortcutItemArr[i] != null && item.itemName == shortcutItemArr[i].itemName){
                return true;
            }
        }
        return false;
    }
}