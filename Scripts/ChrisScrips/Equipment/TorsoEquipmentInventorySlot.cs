using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace FYP
{
    public class TorsoEquipmentInventorySlot : MonoBehaviour
    {
        PlayerInventory playerInventory;
        WeaponSlotManager weaponSlotManager;
        UIController uiController;

        public Image icon;
        public Image background;
        public RarityManager rarityManager;
        public TorsoEquipment item;

        private void Awake()
        {
            playerInventory = FindObjectOfType<PlayerInventory>();
            weaponSlotManager = FindObjectOfType<WeaponSlotManager>();
            uiController = FindObjectOfType<UIController>();
            rarityManager = GetComponent<RarityManager>();
        }

        public void AddItem(TorsoEquipment newItem)
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

            if (uiController.torsoEquipmentSlotSelected)
            {
                if (uiController.playerInventory.currentTorsoEquipment != null)
                {
                    int temp = uiController.playerInventory.torsoEquipmentsInventory.IndexOf(item);
                    uiController.playerInventory.torsoEquipmentsInventory.Insert(temp, uiController.playerInventory.currentTorsoEquipment);

                    // uiController.playerInventory.torsoEquipmentsInventory.Add(uiController.playerInventory.currentTorsoEquipment);
                }
                uiController.playerInventory.currentTorsoEquipment = item;
                uiController.playerInventory.torsoEquipmentsInventory.Remove(item);
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

            if (uiController.playerInventory.currentTorsoEquipment != null)
            {
                int temp = uiController.playerInventory.torsoEquipmentsInventory.IndexOf(item);
                uiController.playerInventory.torsoEquipmentsInventory.Insert(temp, uiController.playerInventory.currentTorsoEquipment);
            }
            uiController.playerInventory.currentTorsoEquipment = item;
            uiController.playerInventory.torsoEquipmentsInventory.Remove(item);
            uiController.playerEquipmentManager.EquipAllEquipmentModelsOnStart();
            uiController.equipmentWindowUI.LoadArmorOnEquipmentScreen(uiController.playerInventory);
            uiController.itemInteractManager.ItemNotInteract();
            uiController.UpdateUI();
        }
    }
}
