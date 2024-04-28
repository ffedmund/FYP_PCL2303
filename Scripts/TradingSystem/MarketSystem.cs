using System.Collections.Generic;
using UnityEngine;

public class MarketSystem{
    public struct ItemData{
        public string itemName;
        public int demand;
        public int supply;
    }

    Dictionary<string,ItemData> tradeRecords = new Dictionary<string, ItemData>();

    public int GetMarketPrice(string itemName, bool isMaterial = true){
        int basePrice = 750;
        if(DataReader.basicPriceDictionary.ContainsKey(itemName)){
            basePrice = DataReader.basicPriceDictionary[itemName];
        }else if(isMaterial){
            basePrice = 1;
        }
        if(tradeRecords.ContainsKey(itemName) && tradeRecords[itemName].demand > 0){
            return Mathf.RoundToInt(basePrice * tradeRecords[itemName].demand / tradeRecords[itemName].supply);
        }else{
            return basePrice; 
        }
    }

    public void AddTradeRecord(TradeManager.TradeMehtod tradeMehtod, string itemName, int amount){
        ItemData itemData = new ItemData
        {
            itemName = itemName
        };
        if(tradeRecords.ContainsKey(itemName)){
            itemData = tradeRecords[itemName];
        }else{
            tradeRecords.Add(itemName,itemData);
        }

        if(tradeMehtod == TradeManager.TradeMehtod.Buy){
            itemData.demand += amount;
            if(itemData.demand>itemData.supply)
            {
                itemData.supply = itemData.demand;
            }
        }else{
            itemData.supply += amount;
        }
        tradeRecords[itemName] = itemData;
    }


}