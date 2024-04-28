using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public enum HonorRank{
    D = 0,
    C = 1,
    B = 2,
    A = 3,
    S = 4
}

[System.Serializable]
public class PlayerData{

    readonly string[] attributeKeys = {"level","exp","honor","vitality","strength","intelligence","dexterity","endurance","luck"};
    readonly string[] moneySign = {"","K","M","B"};

    public List<Quest> quests = new List<Quest>();

    Dictionary<string,int> _playerAttributesData;
    PlayerRole _playerRole;
    int _money;

    public PlayerData(){
        _playerAttributesData = new Dictionary<string, int>();
        _playerRole = JoinGameManager.playerRole;
        foreach(string key in attributeKeys){
            _playerAttributesData[key] = (key == "exp" || key == "honor" )?0:1;
        }
        _money = 0;
    }

    public void AddPlayerData(string key, int value) {
        if(_playerAttributesData.ContainsKey(key)){
            _playerAttributesData[key] += value;
            // if(key == "exp" && _playerAttributesData[key]>100*Mathf.Pow(_playerAttributesData["level"],2)){
            //     _playerAttributesData["level"]++;
            // }
        }
        if(key == "money"){
            _money += value;
        }
    }

    public void ReplacePlayerData(string key, int value){
        if(_playerAttributesData.ContainsKey(key)){
            _playerAttributesData[key] = value;
        }
        if(key == "money"){
            _money = value;
        }
    }

    public string GetMoneyAmount(){
        int temp = _money;
        int count = 0;
        string output = temp.ToString();
        while(temp >= 10){
            output = temp.ToString() + moneySign[count];
            temp/=1000;
            count++;
        }
        return "$"+output;
    }

    public int GetAttribute(string key){
        if(key == "money")return _money;
        return _playerAttributesData[key];
    }

    public PlayerRole GetInfo(){
        return _playerRole;
    }

    public int GetHonorLevel(){
        int honorLevel = Mathf.FloorToInt(Mathf.Log(_playerAttributesData["honor"]/100+2,2))-1;
        return Mathf.Min(honorLevel,4);
    }

    public Dictionary<string,int> GetAttributes(){
        return _playerAttributesData;
    }

    public string[] ToStringArray(bool fullAttributes = false){

        int startingPivot = fullAttributes? 0:Array.IndexOf(attributeKeys,"vitality");
        int outputLength = _playerAttributesData.Count - startingPivot;
        string[] output = new string[outputLength];

        for(int i = startingPivot, count = 0; i < _playerAttributesData.Count; i++){
            output[count++] = string.Format("{0}: {1}",attributeKeys[i],_playerAttributesData[attributeKeys[i]]);
        }

        return output;
    }


}