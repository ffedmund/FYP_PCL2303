using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace FYP
{
    public class HeadEquipmentInventorySlot : MonoBehaviour
    {
        PlayerInventory playerInventory;
        WeaponSlotManager weaponSlotManager;
        UIController uiController;

        public Image icon;
        public Image background;
        public RarityManager rarityManager;
        public HelmetEquipment item;

        private void Awake()
        {
            playerInventory = FindObjectOfType<PlayerInventory>();
            weaponSlotManager = FindObjectOfType<WeaponSlotManager>();
            uiController = FindObjectOfType<UIController>();
            rarityManager = GetComponent<RarityManager>();
        }

        public void AddItem(HelmetEquipment newItem)
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

            if (uiController.headEquipmentSlotSelected)
            {
                if (uiController.playerInventory.currentHelmetEquipment != null)
                {
                    int temp = uiController.playerInventory.helmetEquipmentsInventory.IndexOf(item);
                    uiController.playerInventory.helmetEquipmentsInventory.Insert(temp, uiController.playerInventory.currentHelmetEquipment);

                    // uiController.playerInventory.helmetEquipmentsInventory.Add(uiController.playerInventory.currentHelmetEquipment);
                }
                uiController.playerInventory.currentHelmetEquipment = item;
                uiController.playerInventory.helmetEquipmentsInventory.Remove(item);
                uiController.playerEquipmentManager.EquipAllEquipmentModelsOnStart();
            }
            else
            {
                Debug.Log("head equipment slot not selected.");
                return;
            }

            uiController.equipmentWindowUI.LoadArmorOnEquipmentScreen(uiController.playerInventory);
            uiController.ResetAllSelectedSlots();
            uiController.UpdateUI();
        }

        public void DirectEquipThisItem()
        {
            if (item == null) return;
            if (uiController.playerInventory.currentHelmetEquipment != null)
            {
                int temp = uiController.playerInventory.helmetEquipmentsInventory.IndexOf(item);
                uiController.playerInventory.helmetEquipmentsInventory.Insert(temp, uiController.playerInventory.currentHelmetEquipment);
            }
            uiController.playerInventory.currentHelmetEquipment = item;
            uiController.playerInventory.helmetEquipmentsInventory.Remove(item);
            uiController.playerEquipmentManager.EquipAllEquipmentModelsOnStart();
            uiController.equipmentWindowUI.LoadArmorOnEquipmentScreen(uiController.playerInventory);
            uiController.itemInteractManager.ItemNotInteract();
            uiController.UpdateUI();
        }
    }
}
