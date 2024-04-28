using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FYP
{
    public class SpellSlotUI : MonoBehaviour
    {
        UIController uiController;
        HighlightSelectedButton highlightSelectedButton;

        public Image icon;
        public Image background;
        public RarityManager rarityManager;
        SpellItem spellItem;
        
        private void Awake()
        {
            uiController = FindObjectOfType<UIController>();
            highlightSelectedButton = GetComponent<HighlightSelectedButton>();
            rarityManager = GetComponent<RarityManager>();
        }

        public void AddItem(SpellItem newSpellItem)
        {
            spellItem = Instantiate(newSpellItem);
            spellItem.name = newSpellItem.name;
            if (icon == null)
            {
                Debug.Log("icon is null");
            }
            icon.sprite = spellItem.itemIcon;
            icon.enabled = true;

            background.enabled = true;
            background.color = rarityManager.rarityColor[(int)spellItem.rarity];

            gameObject.SetActive(true);

            // Disable child image component
            if (transform.childCount > 0)
            {
                transform.GetChild(0).GetComponent<Image>().enabled = false;
            }
        }

        public void ClearItem()
        {
            spellItem = null;
            icon.sprite = null;
            icon.enabled = false;
            // gameObject.SetActive(false);

            background.enabled = false;

            // Enable child image component
            if (transform.childCount > 0)
            {
                transform.GetChild(0).GetComponent<Image>().enabled = true;
            }
        }

        public void SelectThisSlot()
        {
            uiController.ResetAllSelectedSlots();
            uiController.spellSlotSelected = true;
            highlightSelectedButton.HighlightButton();
        }

        public void UnequipThisSlot()
        {
            if (spellItem != null)
            {
                uiController.playerInventory.spellInventory.Add(spellItem);
                ClearItem();
                uiController.playerInventory.currentSpell = null;
                uiController.UpdateUI();
            }
        }
    }
}