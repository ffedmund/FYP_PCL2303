using System.Collections.Generic;
using FYP;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TradeUIController : MonoBehaviour {
    [Header("Inventory")]
    public Transform inventorySlotParent;
    public GameObject inventorySlotPrefab;
    [Header("Info")]
    public Image itemIcon;
    public TextMeshProUGUI itemName;
    public TextMeshProUGUI priceText;
    public TextMeshProUGUI stockText;
    public TextMeshProUGUI description;
    public TextMeshProUGUI acceptButtonText;
    public TextMeshProUGUI amountText;
    public TextMeshProUGUI totalPriceText;

    public TextMeshProUGUI tradeTypeText;
    public TextMeshProUGUI clientMoneyText;

    int showingInventoryIndex;
    int amount;
    Item infoItem;
    public InventorySlot[] inventorySlots;
    
    private void Start() {
        inventorySlots = inventorySlotParent.GetComponentsInChildren<InventorySlot>(true);
    }

    public void ActiveUI(){
        gameObject.SetActive(true);
        if(inventorySlots.Length == 0){
            inventorySlots = inventorySlotParent.GetComponentsInChildren<InventorySlot>(true);
        }
        amount = 0;
        infoItem = null;
        SetTradeMethod(TradeManager.TradeMehtod.Buy);
        SetInventoryCategory(1);
        ResetInfoPage();
        clientMoneyText.SetText(TradeManager.instance.GetClientMoney().ToString());
    }

    public void AcceptButtonHandler(){
        if(infoItem != null){
            if(TradeManager.instance.TradeConfirm(infoItem, amount)){
                if(AudioSourceController.instance){
                    AudioSourceController.instance.Play("SellnBuy");
                }
                ActiveUI();
            }
        }
    }

    public void IncreaseAmount(){
        if(TradeManager.instance.tradeMehtod == TradeManager.TradeMehtod.Buy){
            if(amount + 1 <= TradeManager.instance.GetStock(0,infoItem)){
                amount++;
            }
        }else{
            if(amount + 1 <= TradeManager.instance.GetStock(1,infoItem)){
                amount++;
            }
        }
        amountText.SetText(amount.ToString());
        totalPriceText.SetText($"Total:${TradeManager.instance.GetPrice(infoItem)*amount}");
    }

    public void DecreaseAmount(){
        if(amount - 1 >= 0){
            amount--;
        }
        amountText.SetText(amount.ToString());
        totalPriceText.SetText($"Total:${TradeManager.instance.GetPrice(infoItem)*amount}");
    }

    public void SetTradeMethod(){
        TradeManager.TradeMehtod tradeMehtod = TradeManager.instance.tradeMehtod == TradeManager.TradeMehtod.Buy?TradeManager.TradeMehtod.Sell:TradeManager.TradeMehtod.Buy;
        TradeManager.instance.tradeMehtod = tradeMehtod;
        acceptButtonText.SetText((tradeMehtod == TradeManager.TradeMehtod.Sell)?"Sell":"Buy");
        tradeTypeText.SetText((tradeMehtod == TradeManager.TradeMehtod.Sell)?"Mine":"Trader's");
        SetInventoryCategory(showingInventoryIndex);
    }

    public void SetInfoPage(Item item){
        itemIcon.enabled = true;
        itemIcon.sprite = item.itemIcon;
        itemName.SetText(item.name);
        priceText.SetText($"Price {TradeManager.instance.GetPrice(item)}");
        stockText.SetText($"In Inventory {TradeManager.instance.GetStock(1,item)}");
        description.SetText($"{item.itemDescription}");
        amount = 0;
        infoItem = item;
        amountText.SetText(amount.ToString());
        totalPriceText.SetText($"Total:${TradeManager.instance.GetPrice(item)*amount}");
    }

    public void SetInventoryCategory(int index)
    {
        #region Inventory Slots
        showingInventoryIndex = index;
        switch (showingInventoryIndex)
        {
            case 1:
                UpdateInventorySlot(TradeManager.instance.GetInvetory<MaterialItem>());
                break;
            case 2:
                UpdateInventorySlot(TradeManager.instance.GetInvetory<EquipmentItem>());
                break;
            default:
                UpdateInventorySlot(TradeManager.instance.GetInvetory<WeaponItem>());
                break;
        }
        #endregion
    }

    void ResetInfoPage(){
        itemIcon.enabled = false;
        itemName.SetText("");
        priceText.SetText("Price ");
        stockText.SetText("In Inventory ");
        amountText.SetText(amount.ToString());
        totalPriceText.SetText($"Total:$");
    }

    void SetTradeMethod(TradeManager.TradeMehtod tradeMehtod){
        TradeManager.instance.tradeMehtod = tradeMehtod;
        acceptButtonText.SetText((tradeMehtod == TradeManager.TradeMehtod.Sell)?"Sell":"Buy");
        tradeTypeText.SetText((tradeMehtod == TradeManager.TradeMehtod.Sell)?"Mine":"Trader's");
        SetInventoryCategory(showingInventoryIndex);
    }

    void UpdateInventorySlot<T>(List<T> inventory) where T : Item
    {
        if(inventory == null){
            return;
        }
        List<string> createdSlotList = new List<string>();
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (i < inventory.Count)
            {
                if (inventorySlots.Length < inventory.Count)
                {
                    GameObject inventorySlotObject = Instantiate(inventorySlotPrefab, inventorySlotParent);
                    inventorySlots = inventorySlotParent.GetComponentsInChildren<InventorySlot>(true);
                    inventorySlotObject.GetComponent<ItemInteract>().isLock = true;
                }
                Item item = inventory[i];
                inventorySlots[i].GetComponentInChildren<Button>().onClick.RemoveAllListeners();
                inventorySlots[i].GetComponentInChildren<Button>().onClick.AddListener(()=>{
                    this.SetInfoPage(item);
                });
                if (inventory[i] is WeaponItem)
                {
                    inventorySlots[i].AddItem(inventory[i]);
                }
                else if (inventory[i] is MaterialItem)
                {
                    inventorySlots[i].AddItem(inventory[i]);
                    createdSlotList.Add(((Item)(object)inventory[i]).itemName);
                }
                else if (inventory[i] is ArtifactItem)
                {
                    inventorySlots[i].AddItem(inventory[i]);
                }
                else if (inventory[i] is EquipmentItem)
                {
                    inventorySlots[i].AddItem(inventory[i]);
                }
                else
                {
                    inventorySlots[i].ClearInventorySlot();
                }
            }
            else
            {
                inventorySlots[i].ClearInventorySlot();
            }
        }
    }
}