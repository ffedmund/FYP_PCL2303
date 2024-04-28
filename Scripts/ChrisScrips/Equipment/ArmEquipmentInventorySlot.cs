using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace FYP
{
    public class ArmEquipmentInventorySlot : MonoBehaviour
    {
        PlayerInventory playerInventory;
        WeaponSlotManager weaponSlotManager;
        UIController uiController;

        public Image icon;
        public Image background;
        public RarityManager rarityManager;
        public ArmEquipment item;

        private void Awake()
        {
            playerInventory = FindObjectOfType<PlayerInventory>();
            weaponSlotManager = FindObjectOfType<WeaponSlotManager>();
            uiController = FindObjectOfType<UIController>();
            rarityManager = GetComponent<RarityManager>();
        }

        public void AddItem(ArmEquipment newItem)
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

            // currentArmEquipment is the equipment that is currently equipped
            // item is the equipment that is selected to be equipped
            if (uiController.armEquipmentSlotSelected)
            {
                if (uiController.playerInventory.currentArmEquipment != null)
                {

                    int temp = uiController.playerInventory.armEquipmentsInventory.IndexOf(item);
                    uiController.playerInventory.armEquipmentsInventory.Insert(temp, uiController.playerInventory.currentArmEquipment);

                    // uiController.playerInventory.armEquipmentsInventory.Add(uiController.playerInventory.currentArmEquipment);
                }
                uiController.playerInventory.currentArmEquipment = item;
                uiController.playerInventory.armEquipmentsInventory.Remove(item);
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
            if (uiController.playerInventory.currentArmEquipment != null)
            {
                int temp = uiController.playerInventory.armEquipmentsInventory.IndexOf(item);
                uiController.playerInventory.armEquipmentsInventory.Insert(temp, uiController.playerInventory.currentArmEquipment);
            }
            uiController.playerInventory.currentArmEquipment = item;
            uiController.playerInventory.armEquipmentsInventory.Remove(item);
            uiController.playerEquipmentManager.EquipAllEquipmentModelsOnStart();
            uiController.equipmentWindowUI.LoadArmorOnEquipmentScreen(uiController.playerInventory);
            uiController.itemInteractManager.ItemNotInteract();
            uiController.UpdateUI();
        }
    }
}
