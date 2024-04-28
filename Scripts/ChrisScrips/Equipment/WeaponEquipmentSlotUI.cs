using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace FYP
{
    public class WeaponEquipmentSlotUI : MonoBehaviour
    {
        UIController uiController;
        WeaponSlotManager weaponSlotManager;
        HighlightSelectedButton highlightSelectedButton;
        public RarityManager rarityManager;

        public Image icon;
        WeaponItem weapon;
        public TextMeshProUGUI levelText;
        public Image background;
        public Color[] rarityColor = { new Color(0.7578f, 0.7305f, 0.6953f), new Color(0.1992f, 0.4180f, 0.2422f), new Color(0.3359f, 0.4922f, 0.6133f), new Color(0.3086f, 0.2109f, 0.3867f), new Color(0.8008f, 0.6758f, 0.2109f) };

        public bool rightHandSlot01;
        public bool rightHandSlot02;
        public bool leftHandSlot01;
        public bool leftHandSlot02;

        private void Awake()
        {
            uiController = FindObjectOfType<UIController>();
            highlightSelectedButton = GetComponent<HighlightSelectedButton>();
            // weaponSlotManager = FindObjectOfType<WeaponSlotManager>();
            weaponSlotManager = uiController.playerManager.GetComponentInChildren<WeaponSlotManager>();
            rarityManager = GetComponent<RarityManager>();
        }

        public void AddItem(WeaponItem newWeapon)
        {
            // weapon = Instantiate(newWeapon);
            weapon = newWeapon;
            weapon.level = newWeapon.level;
            weapon.name = newWeapon.name;
            icon.sprite = weapon.itemIcon;
            icon.enabled = true;

            background.enabled = true;
            background.color = rarityManager.rarityColor[(int)weapon.rarity];

            levelText.text = weapon.level.ToString();
            levelText.enabled = true;

            gameObject.SetActive(true);

            if (transform.childCount > 0)
            {
                transform.GetChild(0).GetComponent<Image>().enabled = false;

            }
        }

        public void ClearItem()
        {
            weapon = null;
            icon.sprite = null;
            icon.enabled = false;
            // gameObject.SetActive(false);

            background.enabled = false;

            levelText.text = "";
            levelText.enabled = false;

            if (transform.childCount > 0)
            {
                transform.GetChild(0).GetComponent<Image>().enabled = true;
            }
        }

        public void SelectThisSlot()
        {
            uiController.ResetAllSelectedSlots();
            if (rightHandSlot01)
            {
                uiController.rightHandSlot01Selected = true;
            }
            else if (rightHandSlot02)
            {
                uiController.rightHandSlot02Selected = true;
            }
            else if (leftHandSlot01)
            {
                uiController.leftHandSlot01Selected = true;
            }
            else if (leftHandSlot02)
            {
                uiController.leftHandSlot02Selected = true;
            }
            highlightSelectedButton.HighlightButton();
        }

        public void UnequipThisSlot()
        {
            if (weapon != null)
            {
                uiController.playerInventory.weaponsInventory.Add(weapon);
                ClearItem();
                if (rightHandSlot01)
                {
                    uiController.playerInventory.weaponsInRightHandSlots[0] = null;
                }
                else if (rightHandSlot02)
                {
                    uiController.playerInventory.weaponsInRightHandSlots[1] = null;
                }
                else if (leftHandSlot01)
                {
                    uiController.playerInventory.weaponsInLeftHandSlots[0] = null;
                }
                else if (leftHandSlot02)
                {
                    uiController.playerInventory.weaponsInLeftHandSlots[1] = null;
                }
                uiController.playerInventory.rightWeapon = uiController.playerInventory.weaponsInRightHandSlots[uiController.playerInventory.currentRightWeaponIndex];
                uiController.playerInventory.leftWeapon = uiController.playerInventory.weaponsInLeftHandSlots[uiController.playerInventory.currentLeftWeaponIndex];

                weaponSlotManager.LoadWeaponOnSlot(uiController.playerInventory.rightWeapon, false);
                weaponSlotManager.LoadWeaponOnSlot(uiController.playerInventory.leftWeapon, true);

                uiController.equipmentWindowUI.LoadWeaponOnEquipmentScreen(uiController.playerInventory);
                uiController.UpdateUI();
            }
        }

        public void UpdateSlotUI()
        {
            if (weapon != null)
            {
                icon.sprite = weapon.itemIcon;
                icon.enabled = true;
                background.enabled = true;
                background.color = rarityManager.rarityColor[(int)weapon.rarity];
                levelText.text = weapon.level.ToString();
                levelText.enabled = true;
            }
            else
            {
                icon.sprite = null;
                icon.enabled = false;
                background.enabled = false;
                levelText.text = "";
                levelText.enabled = false;
            }
        }
    }
}