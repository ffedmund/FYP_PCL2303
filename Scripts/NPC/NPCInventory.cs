using UnityEngine;
using FYP;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class NPCInventory:MonoBehaviour{
    NPC npc;
    public List<Item> npcInventory;
    
    public void Setup(NPC npc){
        this.npc = npc;
        LoadInventory();
    }

    public void RemoveItem(Item item, int amount){
        List<Item> targetItems = npcInventory.FindAll(target => target.itemName == item.itemName);
        if(item is MaterialItem){
            while(targetItems.Count > 1){
                Item duplicate = targetItems[targetItems.Count-1];
                npcInventory.Remove(duplicate);
                targetItems.Remove(duplicate);
                Destroy(duplicate);
            }
            ((MaterialItem)targetItems[0]).itemAmount -= amount;
            if(((MaterialItem)targetItems[0]).itemAmount <= 0){
                npcInventory.Remove(targetItems[0]);
                Destroy(targetItems[0]);
            }
        }else{
            while(amount > 0){
                npcInventory.Remove(targetItems[0]);
                targetItems.RemoveAt(0);
                amount--;
            }
        }
    }

    public void AddItem(Item item, int amount){
        amount = Mathf.Max(1,amount);
        Item targetItem = npcInventory.Find(target => target.itemName == item.itemName);
        var itemCopy = Instantiate(item);
        itemCopy.name = item.name;
        if(item is MaterialItem){
            if(targetItem){
                Destroy(itemCopy);
                ((MaterialItem)targetItem).itemAmount += amount;
            }else{
                itemCopy.name = item.name;
                ((MaterialItem)itemCopy).itemAmount = amount;
                npcInventory.Add(itemCopy);
            }
        }else{
            for(int i = 0; i < amount; i++){
                this.npcInventory.Add(itemCopy);
                itemCopy = Instantiate(item);
                itemCopy.name = item.name;
            }
        }
    }

    public void RestoreInventory(){
        foreach(ItemData itemData in npc.npcInventoryData){
            if(itemData.item is MaterialItem){
                MaterialItem inventoryItem = (MaterialItem)npcInventory.Find(item => item.itemName == itemData.item.itemName);
                if(inventoryItem){
                    inventoryItem.itemAmount = Mathf.Max(1,itemData.amount);
                }else{
                    AddItem(itemData.item,itemData.amount);
                }
            }
        }
    }

    public void LoadInventory(){
        npcInventory = new List<Item>();
        foreach(ItemData itemData in npc.npcInventoryData){
            AddItem(itemData.item,itemData.amount);
        }
    }

}