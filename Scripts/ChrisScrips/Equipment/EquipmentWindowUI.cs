using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FYP
{

    public class EquipmentWindowUI : MonoBehaviour
    {

        public WeaponEquipmentSlotUI[] weaponEquipmentSlotUI;
        public HeadEquipmentSlotUI headEquipmentSlotUI;
        public TorsoEquipmentSlotUI torsoEquipmentSlotUI;
        public ArmEquipmentSlotUI armEquipmentSlotUI;
        public LegEquipmentSlotUI legEquipmentSlotUI;

        public SpellSlotUI spellSlotUI;

        public PlayerInventory playerInventory;

        public bool firstTimeLoading = true;

        public void LoadWeaponOnEquipmentScreen(PlayerInventory playerInventory)
        {
            // check if weaponEquipmentSlotUI is null
            if (weaponEquipmentSlotUI == null)
            {
                weaponEquipmentSlotUI = GetComponentsInChildren<WeaponEquipmentSlotUI>();
            }

            for (int i = 0; i < weaponEquipmentSlotUI.Length; i++)
            {
                if (weaponEquipmentSlotUI[i].rightHandSlot01)
                {
                    if (playerInventory.weaponsInRightHandSlots[0] == null)
                    {
                        continue;
                    }
                    if (firstTimeLoading)
                    {
                        playerInventory.weaponsInRightHandSlots[0].level = 1;
                    }
                    weaponEquipmentSlotUI[i].AddItem(playerInventory.weaponsInRightHandSlots[0]);
                }
                else if (weaponEquipmentSlotUI[i].rightHandSlot02)
                {
                    if (playerInventory.weaponsInRightHandSlots[1] == null)
                    {
                        continue;
                    }
                    if (firstTimeLoading)
                    {
                        playerInventory.weaponsInRightHandSlots[1].level = 1;
                    }
                    weaponEquipmentSlotUI[i].AddItem(playerInventory.weaponsInRightHandSlots[1]);
                }
                else if (weaponEquipmentSlotUI[i].leftHandSlot01)
                {
                    if (playerInventory.weaponsInLeftHandSlots[0] == null)
                    {
                        continue;
                    }
                    if (firstTimeLoading)
                    {
                        playerInventory.weaponsInLeftHandSlots[0].level = 1;
                    }
                    weaponEquipmentSlotUI[i].AddItem(playerInventory.weaponsInLeftHandSlots[0]);
                }
                else if (weaponEquipmentSlotUI[i].leftHandSlot02)
                {
                    if (playerInventory.weaponsInLeftHandSlots[1] == null)
                    {
                        continue;
                    }
                    if (firstTimeLoading)
                    {
                        playerInventory.weaponsInLeftHandSlots[1].level = 1;
                    }
                    weaponEquipmentSlotUI[i].AddItem(playerInventory.weaponsInLeftHandSlots[1]);
                }

            }
        }

        public void LoadArmorOnEquipmentScreen(PlayerInventory playerInventory)
        {
            if (playerInventory.currentHelmetEquipment != null)
            {
                headEquipmentSlotUI.AddItem(playerInventory.currentHelmetEquipment);
            }
            else
            {
                headEquipmentSlotUI.ClearItem();
            }

            if (playerInventory.currentTorsoEquipment != null)
            {
                torsoEquipmentSlotUI.AddItem(playerInventory.currentTorsoEquipment);
            }
            else
            {
                torsoEquipmentSlotUI.ClearItem();
            }

            if (playerInventory.currentArmEquipment != null)
            {
                armEquipmentSlotUI.AddItem(playerInventory.currentArmEquipment);
            }
            else
            {
                armEquipmentSlotUI.ClearItem();
            }

            if (playerInventory.currentLegEquipment != null)
            {
                legEquipmentSlotUI.AddItem(playerInventory.currentLegEquipment);
            }
            else
            {
                legEquipmentSlotUI.ClearItem();
            }
        }

        public void LoadSpellOnEquipmentScreen(PlayerInventory playerInventory)
        {
            if (playerInventory.currentSpell != null)
            {
                Debug.Log("LoadSpellOnEquipmentScreen");
                spellSlotUI.AddItem(playerInventory.currentSpell);
            }
            else
            {
                spellSlotUI.ClearItem();
            }
        }

        public void UnhighlightAllButtons()
        {
            headEquipmentSlotUI.GetComponent<HighlightSelectedButton>().UnhighlightButton();
            torsoEquipmentSlotUI.GetComponent<HighlightSelectedButton>().UnhighlightButton();
            armEquipmentSlotUI.GetComponent<HighlightSelectedButton>().UnhighlightButton();
            legEquipmentSlotUI.GetComponent<HighlightSelectedButton>().UnhighlightButton();

            spellSlotUI.GetComponent<HighlightSelectedButton>().UnhighlightButton();

            for (int i = 0; i < weaponEquipmentSlotUI.Length; i++)
            {
                weaponEquipmentSlotUI[i].GetComponent<HighlightSelectedButton>().UnhighlightButton();
            }
        }

        public float DestroyAllCurrentEquipment()
        {
            float originalPhysicalDefense = 0;

            originalPhysicalDefense += headEquipmentSlotUI.DestroyItem();
            originalPhysicalDefense += torsoEquipmentSlotUI.DestroyItem();
            originalPhysicalDefense += armEquipmentSlotUI.DestroyItem();
            originalPhysicalDefense += legEquipmentSlotUI.DestroyItem();

            return originalPhysicalDefense;
        }

        public void UpdateSlotUI()
        {
            for (int i = 0; i < weaponEquipmentSlotUI.Length; i++)
            {
                weaponEquipmentSlotUI[i].UpdateSlotUI();
            }

            headEquipmentSlotUI.UpdateSlotUI();
            torsoEquipmentSlotUI.UpdateSlotUI();
            armEquipmentSlotUI.UpdateSlotUI();
            legEquipmentSlotUI.UpdateSlotUI();

            Debug.Log("UpdateSlotUI");
        }
    }

}