using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace FYP
{
    public class HeadEquipmentSlotUI : MonoBehaviour
    {
        UIController uiController;
        HighlightSelectedButton highlightSelectedButton;

        public Image icon;
        public Image background;
        public TextMeshProUGUI levelText;
        public RarityManager rarityManager;
        HelmetEquipment helmetItem;

        private void Awake()
        {
            uiController = FindObjectOfType<UIController>();
            highlightSelectedButton = GetComponent<HighlightSelectedButton>();
            rarityManager = GetComponent<RarityManager>();
        }

        public void AddItem(HelmetEquipment helmetEquipment)
        {
            helmetItem = Instantiate(helmetEquipment);
            helmetItem.name = helmetEquipment.name;
            if (icon == null)
            {
                Debug.Log("icon is null");
            }
            icon.sprite = helmetItem.itemIcon;
            icon.enabled = true;

            background.enabled = true;
            background.color = rarityManager.rarityColor[(int)helmetItem.rarity];

            levelText.text = helmetItem.level.ToString();
            levelText.enabled = true;

            gameObject.SetActive(true);

            // Disable child image component
            if (transform.childCount > 0)
            {
                transform.GetChild(0).GetComponent<Image>().enabled = false;
            
            }

        }

        public void ClearItem()
        {
            helmetItem = null;
            icon.sprite = null;
            icon.enabled = false;
            // gameObject.SetActive(false);

            background.enabled = false;

            levelText.text = "";
            levelText.enabled = false;

            // Enable child image component
            if (transform.childCount > 0)
            {
                transform.GetChild(0).GetComponent<Image>().enabled = true;
            }
        }

        public void SelectThisSlot()
        {
            uiController.ResetAllSelectedSlots();
            uiController.headEquipmentSlotSelected = true;
            highlightSelectedButton.HighlightButton();
        }

        public void UnequipThisSlot()
        {
            if (helmetItem != null)
            {
                uiController.playerInventory.helmetEquipmentsInventory.Add(helmetItem);
                ClearItem();
                uiController.playerInventory.currentHelmetEquipment = null;
                uiController.playerInventory.AddEquipmentToList();
                uiController.playerEquipmentManager.EquipAllEquipmentModelsOnStart();
                uiController.UpdateUI();
            }
        }

        public float DestroyItem()
        {
            if (helmetItem != null)
            {
                float originalPhysicalDefense = helmetItem.physicalDefense;

                ClearItem();
                uiController.playerInventory.currentHelmetEquipment = null;
                uiController.playerEquipmentManager.EquipAllEquipmentModelsOnStart();
                uiController.UpdateUI();

                return originalPhysicalDefense;
            }
            return 0;
        }

        public void UpdateSlotUI()
        {
            if (helmetItem != null)
            {
                icon.sprite = helmetItem.itemIcon;
                icon.enabled = true;
                background.enabled = true;
                background.color = rarityManager.rarityColor[(int)helmetItem.rarity];
                levelText.text = helmetItem.level.ToString();
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