using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FYP
{
    [CreateAssetMenu(menuName = "Items/Weapon Item")]
    public class WeaponItem : Item
    {
        public GameObject modelPrefab;
        public bool isUnarmed;

        [Header("Idle Animations")]
        public string right_hand_idle;
        public string left_hand_idle;
        public string th_idle;

        [Header("One Handed Attack Animations")]
        public string OH_Light_Attack_1;
        public string OH_Light_Attack_2;
        public string OH_Heavy_Attack_1;

        [Header("Two Handed Attack Animations")]
        public string TH_Light_Attack_1;
        public string TH_Light_Attack_2;
        public string TH_Light_Attack_3;

        [Header("Modifiers")]
        public float lightAttackDamageModifier = 1f;
        public float heavyAttackDamageModifier = 1.5f;
        public float chargeAttackDamageModifier = 1.25f;
        public int criticalAttackModifier = 4;

        [Header("Stamina Costs")]
        public int baseStamina;
        public float lightAttackMultiplier;
        public float heavyAttackMultiplier;
        public float chargeAttackMultiplier;

        [Header("Weapon Type")]
        public bool isSpellCaster;
        public bool isFaithCaster;
        public bool isPyroCaster;
        public bool isMeleeWeapon;


        [Header("Weapon Rarity")]
        public Rarity rarity;

        [Header("Weapon Level")]
        public int level;
    }
}