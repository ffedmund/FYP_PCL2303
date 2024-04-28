using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace FYP
{
    public class ArmEquipmentSlotUI : MonoBehaviour
    {
        UIController uiController;
        HighlightSelectedButton highlightSelectedButton;

        public Image icon;
        public Image background;
        public TextMeshProUGUI levelText;
        public RarityManager rarityManager;
        ArmEquipment armItem;

        private void Awake()
        {
            uiController = FindObjectOfType<UIController>();
            highlightSelectedButton = GetComponent<HighlightSelectedButton>();
            rarityManager = GetComponent<RarityManager>();
        }

        public void AddItem(ArmEquipment armEquipment)
        {
            armItem = Instantiate(armEquipment);
            armItem.name = armEquipment.name;
            if (icon == null)
            {
                Debug.Log("icon is null");
            }
            icon.sprite = armItem.itemIcon;
            icon.enabled = true;

            background.enabled = true;
            background.color = rarityManager.rarityColor[(int)armItem.rarity];

            levelText.text = armItem.level.ToString();
            levelText.enabled = true;

            gameObject.SetActive(true);

            if (transform.childCount > 0)
            {
                transform.GetChild(0).GetComponent<Image>().enabled = false;
            
            }
        }

        public void ClearItem()
        {
            armItem = null;
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
            uiController.armEquipmentSlotSelected = true;
            highlightSelectedButton.HighlightButton();
        }

        public void UnequipThisSlot()
        {
            if (armItem != null)
            {
                uiController.playerInventory.armEquipmentsInventory.Add(armItem);
                ClearItem();
                uiController.playerInventory.currentArmEquipment = null;
                uiController.playerInventory.AddEquipmentToList();
                uiController.playerEquipmentManager.EquipAllEquipmentModelsOnStart();
                uiController.UpdateUI();
            }
        }

        public float DestroyItem()
        {
            if (armItem != null)
            {
                float originalPhysicalDefense = armItem.physicalDefense;

                ClearItem();
                uiController.playerInventory.currentArmEquipment = null;
                uiController.playerEquipmentManager.EquipAllEquipmentModelsOnStart();
                uiController.UpdateUI();

                return originalPhysicalDefense;
            }
            return 0;
        }

        public void UpdateSlotUI()
        {
            if (armItem != null)
            {
                icon.sprite = armItem.itemIcon;
                icon.enabled = true;
                background.enabled = true;
                background.color = rarityManager.rarityColor[(int)armItem.rarity];
                levelText.text = armItem.level.ToString();
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