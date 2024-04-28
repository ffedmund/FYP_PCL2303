using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FYP
{
    public class PlayerInventory : CharacterInventory
    {
        WeaponSlotManager weaponSlotManager;

        
        // public CapeEquipment currentCapeEquipment;

        public WeaponItem[] weaponsInRightHandSlots = new WeaponItem[1];
        public WeaponItem[] weaponsInLeftHandSlots = new WeaponItem[1];

        public int currentRightWeaponIndex = 0;
        public int currentLeftWeaponIndex = 0;

        public List<WeaponItem> weaponsInventory = new List<WeaponItem>();
        public Dictionary<string, MaterialItem> materialsInventory = new Dictionary<string, MaterialItem>();
        public List<ArtifactItem> artifactsInventory = new List<ArtifactItem>();
        public List<EquipmentItem> equipmentsInventory = new List<EquipmentItem>();

        public List<HelmetEquipment> helmetEquipmentsInventory = new List<HelmetEquipment>();
        public List<TorsoEquipment> torsoEquipmentsInventory = new List<TorsoEquipment>();
        public List<ArmEquipment> armEquipmentsInventory = new List<ArmEquipment>();
        public List<LegEquipment> legEquipmentsInventory = new List<LegEquipment>();

        public List<SpellItem> spellInventory = new List<SpellItem>();

        public MaterialItem resources;
        private void Awake()
        {
            weaponSlotManager = GetComponentInChildren<WeaponSlotManager>();

        }

        private void Start()
        {
            AddWeaponToSlots();
            AddWeaponToInventory();
            AddEquipmentToSlots();
            AddEquipmentToInventory();

            rightWeapon = weaponsInRightHandSlots[0];
            leftWeapon = weaponsInLeftHandSlots[0];
            weaponSlotManager.LoadWeaponOnSlot(rightWeapon, false);
            weaponSlotManager.LoadWeaponOnSlot(leftWeapon, true);

            AddEquipmentToList();
        }

        private void AddWeaponToSlots()
        {
            for (int i = 0; i < weaponsInRightHandSlots.Length; i++)  {
                if (weaponsInRightHandSlots[i] != null)
                {
                    weaponsInRightHandSlots[i] = Instantiate(weaponsInRightHandSlots[i]);
                }
                if (weaponsInLeftHandSlots[i] != null)
                {
                    weaponsInLeftHandSlots[i] = Instantiate(weaponsInLeftHandSlots[i]);
                }
            }
            
        }

        private void AddWeaponToInventory()
        {
            for (int i = 0; i < weaponsInventory.Count; i++)
            {
                weaponsInventory[i] = Instantiate(weaponsInventory[i]);
            }
        }

        private void AddEquipmentToSlots()
        {
            if (currentHelmetEquipment != null)
            {
                currentHelmetEquipment = Instantiate(currentHelmetEquipment);
            }
            if (currentTorsoEquipment != null)
            {
                currentTorsoEquipment = Instantiate(currentTorsoEquipment);
            }
            if (currentArmEquipment != null)
            {
                currentArmEquipment = Instantiate(currentArmEquipment);
            }
            if (currentLegEquipment != null)
            {
                currentLegEquipment = Instantiate(currentLegEquipment);
            }
        }

        private void AddEquipmentToInventory()
        {
            for (int i = 0; i < helmetEquipmentsInventory.Count; i++)
            {
                helmetEquipmentsInventory[i] = Instantiate(helmetEquipmentsInventory[i]);
            }
            for (int i = 0; i < torsoEquipmentsInventory.Count; i++)
            {
                torsoEquipmentsInventory[i] = Instantiate(torsoEquipmentsInventory[i]);
            }
            for (int i = 0; i < armEquipmentsInventory.Count; i++)
            {
                armEquipmentsInventory[i] = Instantiate(armEquipmentsInventory[i]);
            }
            for (int i = 0; i < legEquipmentsInventory.Count; i++)
            {
                legEquipmentsInventory[i] = Instantiate(legEquipmentsInventory[i]);
            }
        }

        public void AddEquipmentToList()
        {
            equipmentsInventory = new List<EquipmentItem>();
            for (int i = 0; i < helmetEquipmentsInventory.Count; i++)
            {
                equipmentsInventory.Add(helmetEquipmentsInventory[i]);
            }
            for (int i = 0; i < torsoEquipmentsInventory.Count; i++)
            {
                equipmentsInventory.Add(torsoEquipmentsInventory[i]);
            }
            for (int i = 0; i < armEquipmentsInventory.Count; i++)
            {
                equipmentsInventory.Add(armEquipmentsInventory[i]);
            }
            for (int i = 0; i < legEquipmentsInventory.Count; i++)
            {
                equipmentsInventory.Add(legEquipmentsInventory[i]);
            }
        }

        public void AddItem(Item item, int amount = 1)
        {
            if (amount == 0)
            {
                Debug.LogWarning("Not Valid Item Amount");
            }
            if (AudioSourceController.instance)
            {
                AudioSourceController.instance.Play("PickUp");
            }
            if (item is MaterialItem)
            {
                if (materialsInventory.ContainsKey(item.itemName))
                {
                    materialsInventory[item.itemName].itemAmount += amount;
                }
                else
                {
                    MaterialItem materialItem = (MaterialItem)Instantiate(item);
                    materialItem.name = item.name;
                    materialItem.itemAmount = amount;
                    materialsInventory.Add(materialItem.itemName, materialItem);
                }
            }
        }

        public Item RemoveItem(string itemName, int amount = 1)
        {
            if (materialsInventory.ContainsKey(itemName))
            {
                Item item = materialsInventory[itemName];
                RemoveItem(item, amount);
                return item;
            }
            return null;
        }

        public void RemoveItem(Item item, int amount = 1)
        {
            if (item is MaterialItem)
            {
                if (materialsInventory.ContainsKey(item.itemName))
                {
                    materialsInventory[item.itemName].itemAmount -= amount;
                    if (materialsInventory[item.itemName].itemAmount <= 0)
                    {
                        materialsInventory.Remove(item.itemName);
                    }
                }
            }
        }

        public void ChangeRightWeapon()
        {
            currentRightWeaponIndex = currentRightWeaponIndex + 1;

            if (currentRightWeaponIndex == 0 && weaponsInRightHandSlots[0] != null)
            {
                rightWeapon = weaponsInRightHandSlots[currentRightWeaponIndex];
                weaponSlotManager.LoadWeaponOnSlot(weaponsInRightHandSlots[currentRightWeaponIndex], false);
            }
            else if (currentRightWeaponIndex == 0 && weaponsInRightHandSlots[0] == null)
            {
                currentRightWeaponIndex = currentRightWeaponIndex + 1;
            }
            else if (currentRightWeaponIndex == 1 && weaponsInRightHandSlots[1] != null)
            {
                rightWeapon = weaponsInRightHandSlots[currentRightWeaponIndex];
                weaponSlotManager.LoadWeaponOnSlot(weaponsInRightHandSlots[currentRightWeaponIndex], false);
            }
            else
            {
                currentRightWeaponIndex = currentRightWeaponIndex + 1;
            }

            if (currentRightWeaponIndex > weaponsInRightHandSlots.Length - 1)
            {
                currentRightWeaponIndex = -1;
                rightWeapon = weaponSlotManager.unarmedWeapon;
                weaponSlotManager.LoadWeaponOnSlot(weaponSlotManager.unarmedWeapon, false);
            }
        }

        public void ChangeLeftWeapon()
        {
            currentLeftWeaponIndex = currentLeftWeaponIndex + 1;

            if (currentLeftWeaponIndex == 0 && weaponsInLeftHandSlots[0] != null)
            {
                leftWeapon = weaponsInLeftHandSlots[currentLeftWeaponIndex];
                weaponSlotManager.LoadWeaponOnSlot(weaponsInLeftHandSlots[currentLeftWeaponIndex], true);
            }
            else if (currentLeftWeaponIndex == 0 && weaponsInLeftHandSlots[0] == null)
            {
                currentLeftWeaponIndex = currentLeftWeaponIndex + 1;
            }
            else if (currentLeftWeaponIndex == 1 && weaponsInLeftHandSlots[1] != null)
            {
                leftWeapon = weaponsInLeftHandSlots[currentLeftWeaponIndex];
                weaponSlotManager.LoadWeaponOnSlot(weaponsInLeftHandSlots[currentLeftWeaponIndex], true);
            }
            else
            {
                currentLeftWeaponIndex = currentLeftWeaponIndex + 1;
            }

            if (currentLeftWeaponIndex > weaponsInLeftHandSlots.Length - 1)
            {
                currentLeftWeaponIndex = -1;
                leftWeapon = weaponSlotManager.unarmedWeapon;
                weaponSlotManager.LoadWeaponOnSlot(weaponSlotManager.unarmedWeapon, true);
            }
        }



        public void EquipAllEquipmentModels()
        {
            if (currentHelmetEquipment != null)
            {
                // weaponSlotManager.LoadHelmetModel(currentHelmetEquipment);
            }
            else
            {
                // weaponSlotManager.UnequipHelmetModel();
            }
        }

    }

}