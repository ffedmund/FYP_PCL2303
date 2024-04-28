using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace FYP
{
    public class WorldItemDataBase : MonoBehaviour
    {
        public static WorldItemDataBase Instance;

        public WeaponItem unarmedWeapon;
        public List<Item> allItems;    
        public List <WeaponItem> weaponItems = new List<WeaponItem>();
        public List <EquipmentItem> equipmentItems = new List<EquipmentItem>();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            foreach (var item in weaponItems)
            {
                allItems.Add(item);
            }

            foreach (var item in equipmentItems)
            {
                allItems.Add(item);
            }

            for (int i = 0; i < allItems.Count; i++)
            {
                allItems[i].itemID = i;
            }
        }

        public WeaponItem GetWeaponItemByID(int weaponID)
        {
            return weaponItems.FirstOrDefault(weapon => weapon.itemID == weaponID);
        }

        public EquipmentItem GetEquipmentItemByID(int equipmentID)
        {
            return equipmentItems.FirstOrDefault(equipment => equipment.itemID == equipmentID);
        }
    }

}
