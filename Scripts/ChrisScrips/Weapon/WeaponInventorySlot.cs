using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace FYP
{
    public class WeaponInventorySlot : MonoBehaviour
    {
        PlayerInventory playerInventory;
        WeaponSlotManager weaponSlotManager;
        UIController uiController;
        public RarityManager rarityManager;

        public Image icon;
        public WeaponItem item;
        public Image background;

        private void Awake()
        {
            // playerInventory = FindObjectOfType<PlayerInventory>();
            // weaponSlotManager = FindObjectOfType<WeaponSlotManager>();
            uiController = FindObjectOfType<UIController>();
            playerInventory = uiController.playerInventory;
            weaponSlotManager = uiController.playerManager.GetComponentInChildren<WeaponSlotManager>();
            rarityManager = GetComponent<RarityManager>();
        }

        public void AddItem(WeaponItem newItem)
        {
            item = newItem;
            // item = Instantiate(newItem);
            item.level = newItem.level;
            item.name = newItem.name;
            icon.sprite = item.itemIcon;
            icon.enabled = true;

            background.enabled = true;
            Debug.Log("rarityManager: " + rarityManager);
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

            if (uiController.rightHandSlot01Selected)
            {
                playerInventory.weaponsInventory.Add(playerInventory.weaponsInRightHandSlots[0]);
                playerInventory.weaponsInRightHandSlots[0] = item;
                playerInventory.weaponsInventory.Remove(item);
                Debug.Log("rightHandSlot01Selected");
            }
            else if (uiController.rightHandSlot02Selected)
            {
                playerInventory.weaponsInventory.Add(playerInventory.weaponsInRightHandSlots[1]);
                playerInventory.weaponsInRightHandSlots[1] = item;
                playerInventory.weaponsInventory.Remove(item);
                Debug.Log("rightHandSlot02Selected");
            }
            else if (uiController.leftHandSlot01Selected)
            {
                playerInventory.weaponsInventory.Add(playerInventory.weaponsInLeftHandSlots[0]);
                playerInventory.weaponsInLeftHandSlots[0] = item;
                playerInventory.weaponsInventory.Remove(item);
                Debug.Log("leftHandSlot01Selected");
            }
            else if (uiController.leftHandSlot02Selected)
            {
                playerInventory.weaponsInventory.Add(playerInventory.weaponsInLeftHandSlots[1]);
                playerInventory.weaponsInLeftHandSlots[1] = item;
                playerInventory.weaponsInventory.Remove(item);
                Debug.Log("leftHandSlot02Selected");
            }
            else
            {
                Debug.Log("No slot selected");
                return;
            }

            playerInventory.rightWeapon = playerInventory.weaponsInRightHandSlots[playerInventory.currentRightWeaponIndex];
            playerInventory.leftWeapon = playerInventory.weaponsInLeftHandSlots[playerInventory.currentLeftWeaponIndex];
           
            weaponSlotManager.LoadWeaponOnSlot(playerInventory.rightWeapon, false);
            weaponSlotManager.LoadWeaponOnSlot(playerInventory.leftWeapon, true);

            uiController.equipmentWindowUI.LoadWeaponOnEquipmentScreen(playerInventory);
            uiController.ResetAllSelectedSlots();
            uiController.UpdateUI();
        }

    }
}

