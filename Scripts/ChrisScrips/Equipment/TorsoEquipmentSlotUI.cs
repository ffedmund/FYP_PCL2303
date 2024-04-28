using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace FYP
{
    public class TorsoEquipmentSlotUI : MonoBehaviour
    {
        UIController uiController;
        HighlightSelectedButton highlightSelectedButton;

        public Image icon;
        public Image background;
        public TextMeshProUGUI levelText;
        public RarityManager rarityManager;
        TorsoEquipment torsoItem;

        private void Awake()
        {
            uiController = FindObjectOfType<UIController>();
            highlightSelectedButton = GetComponent<HighlightSelectedButton>();
            rarityManager = GetComponent<RarityManager>();
        }

        public void AddItem(TorsoEquipment torsoEquipment)
        {
            torsoItem = Instantiate(torsoEquipment);
            torsoItem.name = torsoEquipment.name;
            if (icon == null)
            {
                Debug.Log("icon is null");
            }
            icon.sprite = torsoItem.itemIcon;
            icon.enabled = true;

            background.enabled = true;
            background.color = rarityManager.rarityColor[(int)torsoItem.rarity];

            levelText.text = torsoItem.level.ToString();
            levelText.enabled = true;

            gameObject.SetActive(true);
            
            if (transform.childCount > 0)
            {
                transform.GetChild(0).GetComponent<Image>().enabled = false;
            
            }
        }

        public void ClearItem()
        {
            torsoItem = null;
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
            uiController.torsoEquipmentSlotSelected = true;
            highlightSelectedButton.HighlightButton();
        }

        public void UnequipThisSlot()
        {
            if (torsoItem != null)
            {
                uiController.playerInventory.torsoEquipmentsInventory.Add(torsoItem);
                ClearItem();
                uiController.playerInventory.currentTorsoEquipment = null;
                uiController.playerInventory.AddEquipmentToList();
                uiController.playerEquipmentManager.EquipAllEquipmentModelsOnStart();
                uiController.UpdateUI();
            }
        }

        public float DestroyItem()
        {
            if (torsoItem != null)
            {
                float originalPhysicalDefense = torsoItem.physicalDefense;

                ClearItem();
                uiController.playerInventory.currentTorsoEquipment = null;
                uiController.playerEquipmentManager.EquipAllEquipmentModelsOnStart();
                uiController.UpdateUI();

                return originalPhysicalDefense;
            }
            return 0;
        }
        
        public void UpdateSlotUI()
        {
            if (torsoItem != null)
            {
                icon.sprite = torsoItem.itemIcon;
                icon.enabled = true;
                background.enabled = true;
                background.color = rarityManager.rarityColor[(int)torsoItem.rarity];
                levelText.text = torsoItem.level.ToString();
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