using UnityEngine;
using FYP;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public struct Buyer{
    public System.Action<int> SetMoney; // Setter
    public System.Func<int> GetMoney; // Getter
    public PlayerInventory playerInventory;

    public Buyer(System.Action<int> SetMoney,System.Func<int> GetMoney,PlayerInventory playerInventory)
    {
        this.SetMoney = SetMoney;
        this.GetMoney = GetMoney;
        this.playerInventory = playerInventory;
    }
}

public struct Seller{
    public NPCInventory inventory;
    public string sellerTitle;

    public Seller(NPCInventory npcInventory, string sellerTitle){
        this.inventory = npcInventory;
        this.sellerTitle = sellerTitle;
    }
}

public class TradingController:MonoBehaviour{
    public TextMeshProUGUI sellerTitle;
    public TextMeshProUGUI priceText;
    public TextMeshProUGUI buyerRemainMoneyText;
    public GameObject sellerInventorySlotParent;
    public GameObject selectedItemsSlotParent;
    public GameObject inventorySlotPrefab;
    public Button acceptTradeButton;

    InventorySlot[] sellerInventorySlots;
    List<Item> buyingItemList = new List<Item>();
    int totalPrice;

    async void Awake() {
       await DataReader.ReadPriceDataBase(); 
    }

    public void ResetTrade(){
        totalPrice = 0;
        foreach(InventorySlot slot in selectedItemsSlotParent.GetComponentsInChildren<InventorySlot>()){
            slot.transform.SetParent(sellerInventorySlotParent.transform);
        }
        acceptTradeButton.onClick.RemoveAllListeners();
        buyingItemList = new List<Item>();
        gameObject.SetActive(false);
    }

    public void StartTrade(Buyer buyer, Seller seller){
        totalPrice = 0;

        #region Seller Inventory Interface
        sellerTitle.SetText(seller.sellerTitle);
        sellerInventorySlots = sellerInventorySlotParent.GetComponentsInChildren<InventorySlot>();
        List<GameObject> inactiveInventorySlots = new List<GameObject>();

        foreach(InventorySlot inventorySlot in sellerInventorySlotParent.GetComponentsInChildren<InventorySlot>(true)){
            if(!inventorySlot.gameObject.activeSelf){
                inactiveInventorySlots.Add(inventorySlot.gameObject);
            }
        }

        int count = 0;
        while(sellerInventorySlots.Length != seller.inventory.npcInventory.Count && count++<100)
        {
            if(sellerInventorySlots.Length < seller.inventory.npcInventory.Count)
            {
                // Debug.Log($"Need More slot");
                if(inactiveInventorySlots.Count > 0)
                {
                    // Debug.Log($"Calling inactive Slot");
                    inactiveInventorySlots[inactiveInventorySlots.Count-1].SetActive(true);
                    inactiveInventorySlots.Remove(inactiveInventorySlots[inactiveInventorySlots.Count-1]);
                }
                else
                {
                    Instantiate(inventorySlotPrefab,sellerInventorySlotParent.transform);
                }
            }
            else
            {
                sellerInventorySlots[sellerInventorySlots.Length-1].gameObject.SetActive(false);
            }

            sellerInventorySlots = sellerInventorySlotParent.GetComponentsInChildren<InventorySlot>();
        }

        for(int i = 0; i < seller.inventory.npcInventory.Count; i++){
            GameObject inventorySlotObject = sellerInventorySlots[i].gameObject;
            inventorySlotObject.GetComponent<ItemInteract>().isLock = true;
            Item item = seller.inventory.npcInventory[i];
            sellerInventorySlots[i].AddItem(item);
            sellerInventorySlots[i].GetComponentInChildren<Button>().onClick.RemoveAllListeners();
            sellerInventorySlots[i].GetComponentInChildren<Button>().onClick.AddListener(()=>{ClickedProduct(inventorySlotObject,item);});
        }
        #endregion

        priceText.SetText("price $0");
        buyerRemainMoneyText.SetText(buyer.GetMoney().ToString());
        acceptTradeButton.onClick.AddListener(() => {AcceptTrade(buyer, seller);});
        gameObject.SetActive(true);
    }

    public void AcceptTrade(Buyer buyer, Seller seller){
        if(buyer.GetMoney() >= totalPrice){
            buyer.SetMoney(-totalPrice);
            foreach (Item item in buyingItemList)
            {
                if(item is MaterialItem){
                    buyer.playerInventory.AddItem(item);
                    foreach(Quest quest in buyer.playerInventory.GetComponent<PlayerManager>().playerData.quests){
                        if(quest.goalChecker.ItemCollected(item.itemName)  && quest.goalChecker.goalType == GoalType.Gathering){
                            break;
                        }
                    }
                }
                if(item is WeaponItem){
                    buyer.playerInventory.weaponsInventory.Add((WeaponItem)item);
                }
                if(item is HelmetEquipment){
                    buyer.playerInventory.helmetEquipmentsInventory.Add((HelmetEquipment)item);
                }
                if(item is TorsoEquipment){
                    buyer.playerInventory.torsoEquipmentsInventory.Add((TorsoEquipment)item);
                }
                if(item is ArmEquipment){
                    buyer.playerInventory.armEquipmentsInventory.Add((ArmEquipment)item);
                }
                if(item is LegEquipment){
                    buyer.playerInventory.legEquipmentsInventory.Add((LegEquipment)item);
                }
                seller.inventory.npcInventory.Remove(item);
                ResetTrade();
            }
        }
    }

    public void ClickedProduct(GameObject clickedProductSlot, Item item){
        int itemPrice = DataReader.basicPriceDictionary[item.itemName];
        if(clickedProductSlot.transform.parent == selectedItemsSlotParent.transform){
            Debug.Log("Is Selected");
            clickedProductSlot.transform.SetParent(sellerInventorySlotParent.transform);
            totalPrice -= itemPrice;
            buyingItemList.Remove(item);
        }else{
            Debug.Log("Is in Seller Inventory");
            clickedProductSlot.transform.SetParent(selectedItemsSlotParent.transform);
            totalPrice += itemPrice;
            buyingItemList.Add(item);
        }
        priceText.SetText($"price ${totalPrice}");
    }

}