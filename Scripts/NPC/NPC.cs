using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FYP
{   
    [System.Serializable]
    public struct QuestListData{
        public bool questLock;
        public QuestHostType questHostType;
        public QuestType questType;
        public int questId;
    }

    [System.Serializable]
    public struct ItemData{
        public Item item;
        public int amount;
    }

    public enum QuestHostType{
        QUESTGIVER,
        QUESTRECEIVER
    }

    public enum NPCServiceType{
        Trade,
        Effect,
        Heal,
        Ask,
        Upgrade
    }

    [System.Serializable]
    public struct Dialogue{
        public bool setDefault;
        public DialogueTree dialogueTree;
        public int triggerQuestId;
        public bool firendshipTrigger;
        public int triggerFirendshipValue;
    }

    [System.Serializable]
    public struct NPCService
    {
        public string triggerChat;
        public NPCServiceType type;
        public int cost;
        [Header("Buff Effect Service")]
        public Buff buff;
        public float duration;
        [Header("Healing Effect Service")]
        public int healingAmount;
        [Header("Ask Service")]
        public MaterialItem targetItem;
        [Header("Upgrade Service")]
        public MaterialItem resources;
        public int resourcesCost;
    }

    [CreateAssetMenu(menuName = "NPC/New NPC")]
    public class NPC : ScriptableObject
    {
        [Header("NPC Basic Information")]
        public string id;
        public string npcName;
        public int initFirendshipValue;
        [Header("Dialogue")]
        public string[] greetings;
        public Dialogue[] dialogues;
        [Header("Inventory Setting")]
        public List<ItemData> npcInventoryData;
        // public bool isTradable;
        public NPCService[] services;
        [Header("Behaviour Setting")]
        public bool hasDuty; //need to stand and wait player to interact
        public bool isKillable;
        public bool isTalkable = true;
        public bool isVillager = true;
        // public bool isActive = true; //Only Update State when player near by

        [Header("Quest Setting")]
        public QuestListData[] questList;

        [Header("Item Giver Event")]
        public ItemGivingEvent[] itemGivingEvents;

        [Header("NPC Avator")]
        public GameObject npcPrefab;
        public RuntimeAnimatorController npcAnimator;

        [Header("Loot Drop Setting")]
        public LootList lootList;
    }
}

