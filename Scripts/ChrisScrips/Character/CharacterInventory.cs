using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FYP
{
    public class CharacterInventory : MonoBehaviour
    {
        [Header("Current Weapon")]
        public SpellItem currentSpell;
        public WeaponItem rightWeapon;
        public WeaponItem leftWeapon;

        [Header("Current Equipment")]
        public HelmetEquipment currentHelmetEquipment;
        public TorsoEquipment currentTorsoEquipment;
        public LegEquipment currentLegEquipment;
        public ArmEquipment currentArmEquipment;
    }
}
