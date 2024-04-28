using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace FYP
{
    public class InventorySlot : MonoBehaviour
    {

        public Image icon;
        public TextMeshProUGUI text;
        public TextMeshProUGUI levelText;
        public Item item;
        public Image background;

        public void AddItem<T>(T newItem)
        {
            item = (Item)(object)newItem;
            text.enabled = false;
            levelText.enabled = false;
            if (newItem is WeaponItem)
            {
                WeaponInventorySlot weaponInventorySlot = GetComponent<WeaponInventorySlot>();
                weaponInventorySlot.AddItem((WeaponItem)(object)newItem);
                levelText.text = ((WeaponItem)item).level.ToString();
                levelText.enabled = true;
            }
            else if (newItem is HelmetEquipment)
            {
                // Debug.Log("HelmetEquipment: " + newItem);
                HeadEquipmentInventorySlot headEquipmentInventorySlot = GetComponent<HeadEquipmentInventorySlot>();
                headEquipmentInventorySlot.AddItem((HelmetEquipment)(object)newItem);
                levelText.text = ((HelmetEquipment)item).level.ToString();
                levelText.enabled = true;
            }
            else if (newItem is ArmEquipment)
            {
                // Debug.Log("ArmEquipment: " + newItem);
                ArmEquipmentInventorySlot armEquipmentInventorySlot = GetComponent<ArmEquipmentInventorySlot>();
                armEquipmentInventorySlot.AddItem((ArmEquipment)(object)newItem);
                levelText.text = ((ArmEquipment)item).level.ToString();
                levelText.enabled = true;
            }
            else if (newItem is TorsoEquipment)
            {
                // Debug.Log("TorsoEquipment: " + newItem);
                TorsoEquipmentInventorySlot torsoEquipmentInventorySlot = GetComponent<TorsoEquipmentInventorySlot>();
                torsoEquipmentInventorySlot.AddItem((TorsoEquipment)(object)newItem);
                levelText.text = ((TorsoEquipment)item).level.ToString();
                levelText.enabled = true;
            }
            else if (newItem is LegEquipment)
            {
                // Debug.Log("LegEquipment: " + newItem);
                LegEquipmentInventorySlot legEquipmentInventorySlot = GetComponent<LegEquipmentInventorySlot>();
                legEquipmentInventorySlot.AddItem((LegEquipment)(object)newItem);
                levelText.text = ((LegEquipment)item).level.ToString();
                levelText.enabled = true;
            }
            else if (newItem is SpellItem)
            {
                SpellInventorySlot spellInventorySlot = GetComponent<SpellInventorySlot>();
                spellInventorySlot.AddItem((SpellItem)(object)newItem);
            }
            else if (newItem is MaterialItem)
            {
                item = (MaterialItem)(object)newItem;
                icon.sprite = item.itemIcon;
                icon.enabled = true;
                text.enabled = true;
                text.SetText(((MaterialItem)item).itemAmount.ToString());
                gameObject.SetActive(true);
            }
            else if (newItem is Item)
            {
                icon.sprite = item.itemIcon;
                icon.enabled = true;
                gameObject.SetActive(true);
            }
        }

        public Item GetItem()
        {
            return item;
        }

        public void ClearInventorySlot()
        {
            GetComponent<WeaponInventorySlot>().ClearInventorySlot();
            GetComponent<HeadEquipmentInventorySlot>().ClearInventorySlot();
            GetComponent<ArmEquipmentInventorySlot>().ClearInventorySlot();
            GetComponent<TorsoEquipmentInventorySlot>().ClearInventorySlot();
            GetComponent<LegEquipmentInventorySlot>().ClearInventorySlot();
            GetComponent<SpellInventorySlot>().ClearInventorySlot();
            item = null;
            icon.sprite = null;
            icon.enabled = false;
            background.enabled = false;
            levelText.text = "";
            levelText.enabled = false;
            gameObject.SetActive(false);
            GetComponentInChildren<Button>().onClick.RemoveAllListeners();
        }

        public void EquipThisItem()
        {
            if (item == null)
            {
                Debug.Log("item is null");
                return;
            }

            if (item is WeaponItem)
            {
                //TODO: Equip weapon
            }
            else if (item is HelmetEquipment)
            {
                HeadEquipmentInventorySlot headEquipmentInventorySlot = GetComponent<HeadEquipmentInventorySlot>();
                headEquipmentInventorySlot.DirectEquipThisItem();
            }
            else if (item is ArmEquipment)
            {
                ArmEquipmentInventorySlot armEquipmentInventorySlot = GetComponent<ArmEquipmentInventorySlot>();
                armEquipmentInventorySlot.DirectEquipThisItem();
            }
            else if (item is TorsoEquipment)
            {
                TorsoEquipmentInventorySlot torsoEquipmentInventorySlot = GetComponent<TorsoEquipmentInventorySlot>();
                torsoEquipmentInventorySlot.DirectEquipThisItem();
            }
            else if (item is LegEquipment)
            {
                LegEquipmentInventorySlot legEquipmentInventorySlot = GetComponent<LegEquipmentInventorySlot>();
                legEquipmentInventorySlot.DirectEquipThisItem();
            }
            else if (item is SpellItem)
            {
                SpellInventorySlot spellInventorySlot = GetComponent<SpellInventorySlot>();
                spellInventorySlot.DirectEquipThisItem();
            }
        }

    }
}

