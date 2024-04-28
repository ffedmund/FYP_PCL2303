using System.Collections.Generic;
using System.Linq;
using FYP;
using UnityEngine;

public class TradeManager : MonoBehaviour{
    public static TradeManager instance;
    async void Awake(){
        instance = this;
        await DataReader.ReadPriceDataBase(); 
    }
    public TradeMehtod tradeMehtod;
    public TradeUIController tradeUIController;
    TradeParties host;
    TradeParties client;

    MarketSystem marketSystem;

    void Start(){
        tradeUIController = FindAnyObjectByType<TradeUIController>(FindObjectsInactive.Include);
        marketSystem = new MarketSystem();
    }

    public void StartTrade(TradeParties host, TradeParties client){
        if(client.type == PartiesType.Player){
            this.host = host;
            this.client = client;
            tradeUIController.ActiveUI();
        }
    }

    public bool TradeConfirm(Item item, int amount){
        if(amount == 0){
            return false;
        }
        int totalPrice = GetPrice(item) * amount;

        if(UpdatePlayerInventory(client.playerInventory, item, amount * (int)tradeMehtod, tradeMehtod == TradeMehtod.Buy? totalPrice:0)){
            marketSystem.AddTradeRecord(tradeMehtod, item.itemName, amount);
            client.playerInventory.GetComponent<PlayerManager>().playerData.AddPlayerData("money",-totalPrice * (int)tradeMehtod);
            client.playerInventory.GetComponent<PlayerManager>().itemInteractableGameObject.GetComponent<ItemPopUpController>().NewPopUp(ItemPopUpController.PopUpIcon.Money, -totalPrice * (int)tradeMehtod);
            if(host.type == PartiesType.NPC){
                UpdateNPCInventory(host.npcInventory, item, -amount * (int)tradeMehtod, totalPrice);
            }else{
                UpdatePlayerInventory(host.playerInventory, item, -amount * (int)tradeMehtod, tradeMehtod == TradeMehtod.Sell? totalPrice:0);
            }
            return true;
        }
        return false;
        // if(tradeMehtod == TradeMehtod.Buy){
        //     if(UpdatePlayerInventory(client.playerInventory,item,amount,totalPrice)){
        //         client.playerInventory.GetComponent<PlayerManager>().playerData.AddPlayerData("money",-totalPrice);
        //         client.playerInventory.GetComponent<PlayerManager>().itemInteractableGameObject.GetComponent<ItemPopUpController>().NewPopUp(ItemPopUpController.PopUpIcon.Money,-totalPrice);
        //         marketSystem.AddTradeRecord(tradeMehtod, item.itemName, amount);
        //         if(host.type == PartiesType.NPC){
        //             UpdateNPCInventory(host.npcInventory, item, -amount, totalPrice);
        //         }else{
        //             UpdatePlayerInventory(host.playerInventory,item,-amount);
        //         }
        //         return true;
        //     }
        //     return false;
        // }else{
        //     if(UpdatePlayerInventory(client.playerInventory,item,-amount)){
        //         client.playerInventory.GetComponent<PlayerManager>().playerData.AddPlayerData("money",totalPrice);
        //         client.playerInventory.GetComponent<PlayerManager>().itemInteractableGameObject.GetComponent<ItemPopUpController>().NewPopUp(ItemPopUpController.PopUpIcon.Money,totalPrice);
        //         marketSystem.AddTradeRecord(tradeMehtod, item.itemName, amount);
        //         if(host.type == PartiesType.NPC){
        //             UpdateNPCInventory(host.npcInventory, item, amount, totalPrice);
        //         }else{
        //             UpdatePlayerInventory(host.playerInventory, item, amount, totalPrice);
        //         }
        //         return true;
        //     }
        //     return false;
        // }
    }

    public List<T> GetInvetory<T>() where T : Item{
        TradeParties target = (tradeMehtod == TradeMehtod.Buy)?host:client;
        if(target.type == PartiesType.NPC){
            Debug.Log(GetNPCInvetory<T>(target));
            return GetNPCInvetory<T>(target);
        }else{
            return GetPlayerInvetory<T>(target);
        }
    }

    public int GetClientMoney(){
        return client.playerInventory.GetComponent<PlayerManager>().playerData.GetAttribute("money");
    }

    public int GetPrice(Item item){
        return Mathf.RoundToInt(marketSystem.GetMarketPrice(item.itemName, item is MaterialItem) * ((tradeMehtod == TradeMehtod.Sell)?0.8f:1) * ((host.type == PartiesType.NPC && host.npcInventory.GetComponent<Firendship>().firendshipValue > 50)?0.7f:1));
    }

    public int GetStock(int parties, Item item){
        if(parties == 0)
        {
            //Host
            if(host.type == PartiesType.NPC){
                if(item is MaterialItem){
                    Item item1 = host.npcInventory.npcInventory.Find(target => target.itemName == item.itemName);
                    return ((MaterialItem)item1).itemAmount;
                }else{
                    return host.npcInventory.npcInventory.FindAll(target => target.itemName == item.itemName).Count;
                }
            }
        }
        else
        {
            //Client
            if(item is MaterialItem){
                if(client.playerInventory.materialsInventory.ContainsKey(item.itemName)){
                    return client.playerInventory.materialsInventory[item.itemName].itemAmount;
                }
            }else if(item is WeaponItem){
                return client.playerInventory.weaponsInventory.FindAll(weapon => weapon.itemName == item.itemName).Count;
            }else if(item is EquipmentItem){
                return client.playerInventory.equipmentsInventory.FindAll(equipment => equipment.itemName == item.itemName).Count;
            }
        }
        return 0;
    }

    bool UpdateNPCInventory(NPCInventory npcInventory, Item item, int amount, int totalPrice = 0){
        if(amount > 0){
            npcInventory.AddItem(item, amount);
        }else{
            npcInventory.RemoveItem(item,-amount);
        }
        return true;
    }

    bool UpdatePlayerInventory(PlayerInventory playerInventory, Item item, int amount, int totalPrice = 0){
        if(playerInventory.GetComponent<PlayerManager>().playerData.GetAttribute("money") >= totalPrice){    
            if(item is MaterialItem){
                if(amount > 0){
                    playerInventory.AddItem(item,amount);
                    for(int i = 0; i < amount; i++){
                        foreach(Quest quest in playerInventory.GetComponent<PlayerManager>().playerData.quests){
                            if(quest.goalChecker.ItemCollected(item.itemName)  && quest.goalChecker.goalType == GoalType.Gathering){
                                break;
                            }
                        }
                    }
                }else{
                    playerInventory.RemoveItem(item,-amount);
                    for(int i = 0; i < -amount; i++){
                        foreach(Quest quest in playerInventory.GetComponent<PlayerManager>().playerData.quests){
                            if(quest.goalChecker.ItemLosted(item.itemName)  && quest.goalChecker.goalType == GoalType.Gathering){
                                break;
                            }
                        }
                    }
                }
            }else{
                if(item is WeaponItem){
                    if(amount > 0){
                        playerInventory.weaponsInventory.Add((WeaponItem)item);
                    }else{
                        playerInventory.weaponsInventory.Remove((WeaponItem)item);
                    }
                }
                if(item is HelmetEquipment){
                    if(amount > 0){
                        playerInventory.helmetEquipmentsInventory.Add((HelmetEquipment)item);
                    }else{
                        playerInventory.helmetEquipmentsInventory.Remove((HelmetEquipment)item);
                    }
                }
                if(item is TorsoEquipment){
                    if(amount > 0){
                        playerInventory.torsoEquipmentsInventory.Add((TorsoEquipment)item);
                    }else{
                        playerInventory.torsoEquipmentsInventory.Remove((TorsoEquipment)item);
                    }
                }
                if(item is ArmEquipment){
                    if(amount > 0){
                        playerInventory.armEquipmentsInventory.Add((ArmEquipment)item);
                    }else{
                        playerInventory.armEquipmentsInventory.Remove((ArmEquipment)item);
                    }
                }
                if(item is LegEquipment){
                    if(amount > 0){
                        playerInventory.legEquipmentsInventory.Add((LegEquipment)item);
                    }else{
                        playerInventory.legEquipmentsInventory.Remove((LegEquipment)item);
                    }
                }
            }
            return true;
        }
        return false;
    }

    List<T> GetNPCInvetory<T>(TradeParties target) where T : Item{
        List<T> requireItems = new List<T>();
        List<Item> items = target.npcInventory.npcInventory.FindAll(item => item is T);

        foreach (T item in items)
        {
            requireItems.Add(item);
            Debug.Log(item.itemName);
        }
        return requireItems;
    }

    List<T> GetPlayerInvetory<T>(TradeParties target) where T : Item{
        List<T> requireItems = new List<T>();
        if(typeof(T) == typeof(MaterialItem))
        {
            List<MaterialItem> materialItems = target.playerInventory.materialsInventory.Values.ToList(); // your list of MaterialItems
            foreach (MaterialItem materialItem in materialItems)
            {
                requireItems.Add((T)(object)materialItem);
            }
        }
        if(typeof(T) == typeof(WeaponItem))
        {
            List<WeaponItem> weaponItems = target.playerInventory.weaponsInventory; // your list of MaterialItems
            foreach (WeaponItem weaponItem in weaponItems)
            {
                requireItems.Add((T)(object)weaponItem);
            }
        }
        if(typeof(T) == typeof(EquipmentItem))
        {
            AddEquipmentToList(target.playerInventory);
            List<EquipmentItem> equipmentItems = target.playerInventory.equipmentsInventory; // your list of MaterialItems
            foreach (EquipmentItem equipmentItem in equipmentItems)
            {
                requireItems.Add((T)(object)equipmentItem);
            }
        }
        return requireItems;
    }

    private void AddEquipmentToList(PlayerInventory playerInventory)
    {
        playerInventory.equipmentsInventory = new List<EquipmentItem>();
        for (int i = 0; i < playerInventory.helmetEquipmentsInventory.Count; i++)
        {
            playerInventory.equipmentsInventory.Add(playerInventory.helmetEquipmentsInventory[i]);
        }
        for (int i = 0; i < playerInventory.torsoEquipmentsInventory.Count; i++)
        {
            playerInventory.equipmentsInventory.Add(playerInventory.torsoEquipmentsInventory[i]);
        }
        for (int i = 0; i < playerInventory.armEquipmentsInventory.Count; i++)
        {
            playerInventory.equipmentsInventory.Add(playerInventory.armEquipmentsInventory[i]);
        }
        for (int i = 0; i < playerInventory.legEquipmentsInventory.Count; i++)
        {
            playerInventory.equipmentsInventory.Add(playerInventory.legEquipmentsInventory[i]);
        }
    }

    public enum PartiesType{
        NPC,
        Player
    }

    public enum TradeMehtod{
        Buy = 1,
        Sell = -1
    }

    public struct TradeParties{
        public PartiesType type;
        public PlayerInventory playerInventory;
        public NPCInventory npcInventory;
    }
}