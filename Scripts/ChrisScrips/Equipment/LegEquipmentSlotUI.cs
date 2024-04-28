using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace FYP
{
    public class LegEquipmentSlotUI : MonoBehaviour
    {
        UIController uiController;
        HighlightSelectedButton highlightSelectedButton;

        public Image icon;
        public Image background;
        public TextMeshProUGUI levelText;
        public RarityManager rarityManager;
        LegEquipment legItem;

        private void Awake()
        {
            uiController = FindObjectOfType<UIController>();
            highlightSelectedButton = GetComponent<HighlightSelectedButton>();
            rarityManager = GetComponent<RarityManager>();
        }

        public void AddItem(LegEquipment legEquipment)
        {
            legItem = Instantiate(legEquipment);
            legItem.name = legEquipment.name;
            if (icon == null)
            {
                Debug.Log("icon is null");
            }
            icon.sprite = legItem.itemIcon;
            icon.enabled = true;

            background.enabled = true;
            background.color = rarityManager.rarityColor[(int)legItem.rarity];

            levelText.text = legItem.level.ToString();
            levelText.enabled = true;

            gameObject.SetActive(true);

            if (transform.childCount > 0)
            {
                transform.GetChild(0).GetComponent<Image>().enabled = false;
            
            }
        }

        public void ClearItem()
        {
            legItem = null;
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
            uiController.legsEquipmentSlotSelected = true;
            highlightSelectedButton.HighlightButton();
        }

        public void UnequipThisSlot()
        {
            if (legItem != null)
            {
                uiController.playerInventory.legEquipmentsInventory.Add(legItem);
                ClearItem();
                uiController.playerInventory.currentLegEquipment = null;
                uiController.playerInventory.AddEquipmentToList();
                uiController.playerEquipmentManager.EquipAllEquipmentModelsOnStart();
                uiController.UpdateUI();
            }
        }

        public float DestroyItem()
        {
            if (legItem != null)
            {
                float originalPhysicalDefense = legItem.physicalDefense;

                ClearItem();
                uiController.playerInventory.currentLegEquipment = null;
                uiController.playerEquipmentManager.EquipAllEquipmentModelsOnStart();
                uiController.UpdateUI();

                return originalPhysicalDefense;
            }
            return 0;
        }

        public void UpdateSlotUI()
        {
            if (legItem != null)
            {
                icon.sprite = legItem.itemIcon;
                icon.enabled = true;
                background.enabled = true;
                background.color = rarityManager.rarityColor[(int)legItem.rarity];
                levelText.text = legItem.level.ToString();
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