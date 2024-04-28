using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FYP
{
    public class SpellInventorySlot : MonoBehaviour
    {
        PlayerInventory playerInventory;
        UIController uiController;

        public Image icon;
        public Image background;
        public RarityManager rarityManager;
        public SpellItem item;

        private void Awake()
        {
            playerInventory = FindObjectOfType<PlayerInventory>();
            uiController = FindObjectOfType<UIController>();
            rarityManager = GetComponent<RarityManager>();
        }

        public void AddItem(SpellItem newItem)
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
            // currentSpellItem is the spell that is currently equipped
            // item is the spell that is selected to be equipped
            if (uiController.spellSlotSelected)
            {
                if (uiController.playerInventory.currentSpell != null)
                {
                    int temp = uiController.playerInventory.spellInventory.IndexOf(item);
                    uiController.playerInventory.spellInventory.Insert(temp, uiController.playerInventory.currentSpell);
                }
                uiController.playerInventory.currentSpell = item;
                uiController.playerInventory.spellInventory.Remove(item);
            }
            else
            {
                return;
            }

            uiController.equipmentWindowUI.LoadSpellOnEquipmentScreen(uiController.playerInventory);
            uiController.ResetAllSelectedSlots();
            uiController.UpdateUI();
        }

        public void DirectEquipThisItem()
        {
            if (item == null) return;
            if (uiController.playerInventory.currentSpell != null)
            {
                int temp = uiController.playerInventory.spellInventory.IndexOf(item);
                uiController.playerInventory.spellInventory.Insert(temp, uiController.playerInventory.currentSpell);
            }
            uiController.playerInventory.currentSpell = item;
            uiController.playerInventory.spellInventory.Remove(item);
            uiController.equipmentWindowUI.LoadSpellOnEquipmentScreen(uiController.playerInventory);
            uiController.itemInteractManager.ItemNotInteract();
            uiController.UpdateUI();
        }
    }
}
