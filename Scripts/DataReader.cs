using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;
using UnityEngine;
using System.Threading;
using System.Threading.Tasks;
using System;

[System.Serializable]
public struct PlayerRole{
    public PlayerBackground playerBackground;
    public PlayerAbility playerSkill;
    public PlayerAbility playerTalent;
    public List<PlayerTarget> playerTargets;
}

[System.Serializable]
public struct PlayerBackground{
    public int id;
    public string role;
    public string short_description;
    public string description;
}

[System.Serializable]
public struct PlayerTarget{
    public int id;
    public string title;
    public string detail;
    public string shortDescription;
    public bool isRoleTarget;
}

[System.Serializable]
public struct PlayerAbility{
    public int id;
    public string name;
    public string description;
    public int iconId;
    public int level;
    public int growthRate;
}

public static class DataReader
{
    public static Dictionary<int,Quest> questDictionary= new Dictionary<int, Quest>();
    public static Dictionary<int,PlayerBackground> backgorundDictionary = new Dictionary<int, PlayerBackground>();
    public static Dictionary<int,PlayerAbility> skillDictionary = new Dictionary<int, PlayerAbility>();
    public static Dictionary<int,PlayerAbility> talentDictionary = new Dictionary<int, PlayerAbility>();
    public static Dictionary<int,PlayerTarget> roleTargetDictionary = new Dictionary<int, PlayerTarget>();
    public static Dictionary<string,int> basicPriceDictionary = new Dictionary<string, int>();
    public static List<PlayerTarget> targetList = new List<PlayerTarget>();
    private static object questDataLock = new object();

    static bool questDataLoaded = false;
    static bool backgroundDataLoaded = false;
    static bool targetDataLoaded = false;
    static bool abilityDataLoaded = false;
    static bool priceDataLoaded = false;

    public static async Task ReadPriceDataBase(){
        if(priceDataLoaded)return;
        XmlDocument xmlDoc = new XmlDocument();
        TextAsset textAsset = Resources.Load<TextAsset>("GameData/PriceData");
        xmlDoc.LoadXml(textAsset.text);
        await Task.Run(() => {
            // xmlDoc.Load("./Assets/GameData/PriceData.xml");
            // XmlDocument xmlDoc = new XmlDocument();
            // xmlDoc.Load("./Assets/GameData/PriceData.xml");

            // XmlDocument xmlDoc = new XmlDocument();
            // xmlDoc.Load("./Assets/Resources/GameData/PriceData.xml");

            XmlNodeList priceList = xmlDoc.GetElementsByTagName("price");
            foreach(XmlNode node in priceList){
                string itemId = node["id"].InnerText;
                int price = int.Parse(node["basicPrice"].InnerText);
                basicPriceDictionary.Add(itemId,price);
            }
        });
        priceDataLoaded = true;
    }


    public static async Task ReadBackgroundDataBase(){
        if(backgroundDataLoaded)return;
        XmlDocument xmlDoc = new XmlDocument();
        TextAsset textAsset = Resources.Load<TextAsset>("GameData/BackgroundData");
        xmlDoc.LoadXml(textAsset.text);
        await Task.Run(() => {
            // XmlDocument xmlDoc = new XmlDocument();
            // xmlDoc.Load("./Assets/GameData/BackgroundData.xml");
            // XmlDocument xmlDoc = new XmlDocument();
            // xmlDoc.Load("./Assets/GameData/BackgroundData.xml");
            // XmlDocument xmlDoc = new XmlDocument();
            // xmlDoc.Load("./Assets/Resources/GameData/BackgroundData.xml");

            XmlNodeList backgroundList = xmlDoc.GetElementsByTagName("background");
            foreach (XmlNode node in backgroundList)
            {
                PlayerBackground playerBackground;
                playerBackground.id = int.Parse(node["id"].InnerText);
                playerBackground.short_description = node["short"].InnerText;
                playerBackground.description = node["description"].InnerText;
                playerBackground.role = node["role"].InnerText;
                if(!backgorundDictionary.ContainsKey(playerBackground.id)){
                    backgorundDictionary.Add(playerBackground.id,playerBackground);
                }
            }
        });
        backgroundDataLoaded = true;
    }

    public static async Task ReadQuestDataBase(){
        if(questDataLoaded)return;
        XmlDocument xmlDoc = new XmlDocument();
        TextAsset textAsset = Resources.Load<TextAsset>("GameData/QuestData");
        xmlDoc.LoadXml(textAsset.text);
        await Task.Run(() => {
            lock(questDataLock){
                XmlNodeList questDataList = xmlDoc.GetElementsByTagName("quest");
                foreach (XmlNode node in questDataList)
                {   
                    int id = int.Parse(node["id"].InnerText);
                    QuestType questType = (QuestType)int.Parse(node["questType"].InnerText);
                    HonorRank honorRank = questType == QuestType.Rank? (HonorRank)int.Parse(node["requiredRank"].InnerText):HonorRank.D;
                    string title = node["title"].InnerText;
                    string description = node["description"].InnerText;
                    int expReward = int.Parse(node["expReward"].InnerText);
                    int moneyReward = int.Parse(node["moneyReward"].InnerText);
                    int honorReward = int.Parse(node["honorReward"].InnerText);
                    string itemReward = node["itemReward"].InnerText;
                    int itemRewardAmount = itemReward != ""?int.Parse(node["itemRewardAmount"].InnerText):0;
                    string targetNPC = node["targetNPC"].InnerText;
                    string completeDialog = node["completeDialog"].InnerText;
                    GoalType goalType = (GoalType)int.Parse(node["goalType"].InnerText);
                    string targetID = node["targetID"].InnerText;
                    int targetAmount = int.Parse(node["targetAmount"].InnerText);
                    Quest quest = new Quest(id, questType, honorRank, title, description, expReward, moneyReward, honorReward, itemReward, itemRewardAmount, targetNPC, completeDialog)
                    {
                        goalChecker = new GoalChecker(goalType, targetAmount, targetID)
                    };
                    if (!questDictionary.ContainsKey(quest.id)){
                        questDictionary.Add(quest.id,quest);
                    }
                }
            }
        });
        questDataLoaded = true;
    }

    public static async Task ReadTargetDataBase(){
        if(targetDataLoaded)return;
        XmlDocument xmlDoc = new XmlDocument();
        TextAsset textAsset = Resources.Load<TextAsset>("GameData/TargetData");
        xmlDoc.LoadXml(textAsset.text);
        await Task.Run(() => {

            // XmlDocument xmlDoc = new XmlDocument();
            // xmlDoc.Load("./Assets/GameData/TargetData.xml");
            // XmlDocument xmlDoc = new XmlDocument();
            // xmlDoc.Load("./Assets/GameData/TargetData.xml");
            // XmlDocument xmlDoc = new XmlDocument();
            // xmlDoc.Load("./Assets/Resources/GameData/TargetData.xml");

            XmlNodeList targetDataList = xmlDoc.GetElementsByTagName("target");
            foreach (XmlNode node in targetDataList)
            {
                PlayerTarget playerTarget;
                playerTarget.id = int.Parse(node["id"].InnerText);
                playerTarget.title = node["title"].InnerText;
                playerTarget.detail = node["detail"].InnerText;
                playerTarget.shortDescription = node["short"].InnerText;
                playerTarget.isRoleTarget = Boolean.Parse(node["roleTarget"].InnerText);
                if(playerTarget.isRoleTarget){
                    roleTargetDictionary[int.Parse(node["roleId"].InnerText)] = playerTarget;
                }else{
                    targetList.Add(playerTarget);
                }
            }
        });
        targetDataLoaded = true;
    }

     public static async Task ReadAbilityDataBase(){
        if(abilityDataLoaded)return;
        XmlDocument xmlDoc = new XmlDocument();
        TextAsset textAsset = Resources.Load<TextAsset>("GameData/AbilityData");
        xmlDoc.LoadXml(textAsset.text);
        await Task.Run(() => {
            XmlNodeList skillDataList = xmlDoc.GetElementsByTagName("skill");
            foreach (XmlNode node in skillDataList)
            {
                PlayerAbility playerAbility;
                playerAbility.id = int.Parse(node["id"].InnerText);
                playerAbility.name = node["name"].InnerText;
                playerAbility.description = node["description"].InnerText;
                playerAbility.iconId = -1;
                playerAbility.level = int.Parse(node["level"].InnerText);
                playerAbility.growthRate = int.Parse(node["growth_rate"].InnerText);
                if(!skillDictionary.ContainsKey(playerAbility.id)){
                    skillDictionary.Add(playerAbility.id,playerAbility);
                }
            }

            XmlNodeList talentDataList = xmlDoc.GetElementsByTagName("talent");
            foreach (XmlNode node in talentDataList)
            {
                PlayerAbility playerAbility;
                playerAbility.id = int.Parse(node["id"].InnerText);
                playerAbility.name = node["name"].InnerText;
                playerAbility.description = node["description"].InnerText;
                playerAbility.iconId = int.Parse(node["icon_id"].InnerText);
                playerAbility.level = int.Parse(node["level"].InnerText);
                playerAbility.growthRate = int.Parse(node["growth_rate"].InnerText);
                if(!talentDictionary.ContainsKey(playerAbility.id)){
                    talentDictionary.Add(playerAbility.id,playerAbility);
                }
            }
        });
        abilityDataLoaded = true;
    }
}
