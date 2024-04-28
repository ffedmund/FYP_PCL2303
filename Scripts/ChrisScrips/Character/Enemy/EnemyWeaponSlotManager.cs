using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FYP
{
    public class EnemyWeaponSlotManager : MonoBehaviour
    {
        public WeaponItem rightHandWeapon;
        public WeaponItem leftHandWeapon;

        public WeaponHolderSlot rightHandSlot;
        WeaponHolderSlot leftHandSlot;

        EnemyEffectManager enemyEffectManager;

        DamageCollider leftHandDamageCollider;
        DamageCollider rightHandDamageCollider;

        private void Awake()
        {
            enemyEffectManager = GetComponentInParent<EnemyEffectManager>();
            WeaponHolderSlot[] weaponHolderSlots = GetComponentsInChildren<WeaponHolderSlot>();
            foreach (WeaponHolderSlot weaponSlot in weaponHolderSlots)
            {
                if (weaponSlot.isLeftHandSlot)
                {
                    leftHandSlot = weaponSlot;
                }
                else if (weaponSlot.isRightHandSlot)
                {
                    rightHandSlot = weaponSlot;
                }
            }
        }

        private void Start()
        {
            LoadWeaponOnBothHands();
        }

        public void LoadWeaponOnSlot(WeaponItem weapon, bool isLeft)
        {
            if (isLeft)
            {
                leftHandSlot.currentWeapon = weapon;
                leftHandSlot.LoadWeaponModel(weapon);
                LoadWeaponDamageCollider(true);
            }
            else
            {
                rightHandSlot.currentWeapon = weapon;
                rightHandSlot.LoadWeaponModel(weapon);
                LoadWeaponDamageCollider(false);
            }
        }

        public void LoadWeaponOnBothHands()
        {
            if (rightHandWeapon != null)
            {
                LoadWeaponOnSlot(rightHandWeapon, false);
            }

            if (leftHandWeapon != null)
            {
                LoadWeaponOnSlot(leftHandWeapon, true);
            }
        }

        public void LoadWeaponDamageCollider(bool isLeft)
        {
            if (isLeft)
            {
                leftHandDamageCollider = leftHandSlot.currentWeaponModel.GetComponentInChildren<DamageCollider>();
                enemyEffectManager.leftWeaponFX = leftHandSlot.currentWeaponModel.GetComponentInChildren<WeaponFX>();
                if (enemyEffectManager.leftWeaponFX != null)
                    enemyEffectManager.leftWeaponFX.StopWeaponFX();
            }
            else
            {
                rightHandDamageCollider = rightHandSlot.currentWeaponModel.GetComponentInChildren<DamageCollider>();
                enemyEffectManager.rightWeaponFX = rightHandSlot.currentWeaponModel.GetComponentInChildren<WeaponFX>();
                if (enemyEffectManager.rightWeaponFX != null)
                    enemyEffectManager.rightWeaponFX.StopWeaponFX();
            }
        }

        public void OpenDamageCollider()
        {
            rightHandDamageCollider.EnableDamageCollider();
            if (enemyEffectManager.rightWeaponFX != null)
                enemyEffectManager.rightWeaponFX.PlayWeaponFX();
        }

        public void CloseDamageCollider()
        {
            rightHandDamageCollider.DisableDamageCollider();
            if (enemyEffectManager.rightWeaponFX != null)
                enemyEffectManager.rightWeaponFX.StopWeaponFX();
        }

        public void EnableCombo()
        {
            // anim.SetBool("canDoCombo", true);
        }

        public void DisableCombo()
        {
            // anim.SetBool("canDoCombo", false);
        }
    }
}

