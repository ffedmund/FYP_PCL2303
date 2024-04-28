using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FYP
{
    public class SpellItem : Item
    {
        public GameObject spellWarmUpFX;
        public GameObject spellCastFX;
        public string spellAnimation;

        [Header("Spell Cost")]
        public int manaCost;

        [Header("Spell Type")]
        public bool isFaithSpell;
        public bool isMagicSpell;
        public bool isPyroSpell;

        [Header("Spell Description")]
        [TextArea]
        public string spellDescription;

        [Header("Spell Rarity")]
        public Rarity rarity;

        public virtual void AttemptToCastSpell(AnimatorHandler animatorHandler, PlayerStats playerStats, WeaponSlotManager weaponSlotManager)
        {
            Debug.Log("Attempting to cast spell");
        }

        public virtual void SuccessfullyCastSpell(AnimatorHandler animatorHandler, PlayerStats playerStats, CameraHandler cameraHandler, WeaponSlotManager weaponSlotManager)
        {
            Debug.Log("Successfully cast spell");
            playerStats.TakeManaDamage(manaCost);
        }

        public virtual void SuccessfullyCastSpell(AnimatorHandler animatorHandler, EnemyManager enemyManager, EnemyWeaponSlotManager weaponSlotManager)
        {
            Debug.Log("Successfully cast spell");
        }

    }
}

