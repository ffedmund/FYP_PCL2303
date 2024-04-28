using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace FYP
{
    public class LegEquipmentInventorySlot : MonoBehaviour
    {
        PlayerInventory playerInventory;
        WeaponSlotManager weaponSlotManager;
        UIController uiController;

        public Image icon;
        public Image background;
        public RarityManager rarityManager;
        public LegEquipment item;

        private void Awake()
        {
            playerInventory = FindObjectOfType<PlayerInventory>();
            weaponSlotManager = FindObjectOfType<WeaponSlotManager>();
            uiController = FindObjectOfType<UIController>();
            rarityManager = GetComponent<RarityManager>();
        }

        public void AddItem(LegEquipment newItem)
        {
            item = newItem;
            icon.sprite = item.itemIcon;
            icon.enabled = true;

            background.enabled = true;
            background.color = rarityManager.rarityColor[(int)item.rarity];

            gameObject.SetActive(true);
        }

        public void ClearInventorySlot()
        {
            item = null;
            icon.sprite = null;
            icon.enabled = false;

            background.enabled = false;

            gameObject.SetActive(false);
        }

        public void EquipThisItem()
        {
            if (item == null)
            {
                Debug.Log("item is null");
                return;
            }

            if (uiController.legsEquipmentSlotSelected)
            {
                if (uiController.playerInventory.currentLegEquipment != null)
                {
                    int temp = uiController.playerInventory.legEquipmentsInventory.IndexOf(item);
                    uiController.playerInventory.legEquipmentsInventory.Insert(temp, uiController.playerInventory.currentLegEquipment);

                    // uiController.playerInventory.legEquipmentsInventory.Add(uiController.playerInventory.currentLegEquipment);
                }
                uiController.playerInventory.currentLegEquipment = item;
                uiController.playerInventory.legEquipmentsInventory.Remove(item);
                uiController.playerEquipmentManager.EquipAllEquipmentModelsOnStart();
            }
            else
            {
                return;
            }

            uiController.equipmentWindowUI.LoadArmorOnEquipmentScreen(uiController.playerInventory);
            uiController.ResetAllSelectedSlots();
            uiController.UpdateUI();
        }

        public void DirectEquipThisItem()
        {
            if (item == null) return;

            if (uiController.playerInventory.currentLegEquipment != null)
            {
                int temp = uiController.playerInventory.legEquipmentsInventory.IndexOf(item);
                uiController.playerInventory.legEquipmentsInventory.Insert(temp, uiController.playerInventory.currentLegEquipment);
            }
            uiController.playerInventory.currentLegEquipment = item;
            uiController.playerInventory.legEquipmentsInventory.Remove(item);
            uiController.playerEquipmentManager.EquipAllEquipmentModelsOnStart();
            uiController.equipmentWindowUI.LoadArmorOnEquipmentScreen(uiController.playerInventory);
            uiController.itemInteractManager.ItemNotInteract();
            uiController.UpdateUI();
        }
    }
}
