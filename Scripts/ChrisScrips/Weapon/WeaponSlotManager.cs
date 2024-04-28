using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FYP
{
    public class WeaponSlotManager : MonoBehaviour
    {
        public WeaponItem attackingWeapon;
        public WeaponItem unarmedWeapon;

        PlayerManager playerManager;
        public WeaponHolderSlot leftHandSlot;
        public WeaponHolderSlot rightHandSlot;
        WeaponHolderSlot backSlot;

        DamageCollider leftHandDamageCollider;
        DamageCollider rightHandDamageCollider;

        Animator animator;

        public QuickSlotsUI quickSlotsUI;

        PlayerInventory playerInventory;
        PlayerStats playerStats;
        PlayerEffectManager playerEffectManager;
        InputHandler inputHandler;

        private void Awake()
        {
            playerManager = GetComponentInParent<PlayerManager>();
            animator = GetComponent<Animator>();
            quickSlotsUI = FindObjectOfType<QuickSlotsUI>();
            playerInventory = GetComponentInParent<PlayerInventory>();
            playerStats = GetComponentInParent<PlayerStats>();
            playerEffectManager = GetComponentInParent<PlayerEffectManager>();
            inputHandler = GetComponentInParent<InputHandler>();

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
                else if (weaponSlot.isBackSlot)
                {
                    backSlot = weaponSlot;
                }
            }
        }

        public void LoadWeaponOnSlot(WeaponItem weaponItem, bool isLeft)
        {
            if (weaponItem != null)
            {
                if (isLeft)
                {
                    leftHandSlot.currentWeapon = weaponItem;
                    leftHandSlot.LoadWeaponModel(weaponItem);
                    LoadLeftWeaponDamageCollider();
                    if (playerManager.IsOwner)
                    {
                        quickSlotsUI.UpdateWeaponQuickSlotsUI(true, weaponItem);
                        playerManager.characterNetworkManager.currentLeftHandWeaponID.Value = weaponItem.itemID;
                    }

                    animator.CrossFade(weaponItem.left_hand_idle, 0.2f);
                }
                else
                {
                    if (inputHandler.twoHandFlag)
                    {
                        backSlot.LoadWeaponModel(leftHandSlot.currentWeapon);
                        leftHandSlot.UnloadWeaponAndDestroy();
                        animator.CrossFade(weaponItem.th_idle, 0.2f);
                    }
                    else
                    {
                        
                        animator.CrossFade("Both Arms Empty", 0.2f);
                        backSlot.UnloadWeaponAndDestroy();
                        animator.CrossFade(weaponItem.right_hand_idle, 0.2f);
                    }

                    rightHandSlot.currentWeapon = weaponItem;
                    rightHandSlot.LoadWeaponModel(weaponItem);
                    LoadRightWeaponDamageCollider();
                    if (playerManager.IsOwner)
                    {
                        playerManager.characterNetworkManager.currentRightHandWeaponID.Value = weaponItem.itemID;
                        quickSlotsUI.UpdateWeaponQuickSlotsUI(false, weaponItem);
                    }
                }
            }
            else
            {
                weaponItem = unarmedWeapon;
                if (isLeft)
                {
                    animator.CrossFade("Left Arm Empty", 0.2f);
                    playerInventory.leftWeapon = weaponItem;
                    leftHandSlot.currentWeapon = weaponItem;
                    leftHandSlot.LoadWeaponModel(weaponItem);
                    LoadLeftWeaponDamageCollider();
                    if (playerManager.IsOwner)
                    {
                        playerManager.characterNetworkManager.currentLeftHandWeaponID.Value = WorldItemDataBase.Instance.unarmedWeapon.itemID;
                        quickSlotsUI.UpdateWeaponQuickSlotsUI(true, weaponItem);
                    }
                }
                else
                {
                    animator.CrossFade("Right Arm Empty", 0.2f);
                    playerInventory.rightWeapon = weaponItem;
                    rightHandSlot.currentWeapon = weaponItem;
                    rightHandSlot.LoadWeaponModel(weaponItem);
                    LoadRightWeaponDamageCollider();
                    if (playerManager.IsOwner)
                    {
                        playerManager.characterNetworkManager.currentRightHandWeaponID.Value = WorldItemDataBase.Instance.unarmedWeapon.itemID;
                        quickSlotsUI.UpdateWeaponQuickSlotsUI(false, weaponItem);
                    }
                }
            }
        }

        #region Handle Weapon's Damage Collider

        public void LoadLeftWeaponDamageCollider()
        {
            if (leftHandSlot.currentWeaponModel == null)
                return;

            leftHandDamageCollider = leftHandSlot.currentWeaponModel.GetComponentInChildren<DamageCollider>();
            // leftHandDamageCollider.currentWeaponDamage = playerInventory.leftWeapon.baseDamage;
            // leftHandDamageCollider.poiseBreak = playerInventory.leftWeapon.poiseBreak;
            if (leftHandSlot.currentWeaponModel.GetComponentInChildren<WeaponFX>() != null)
            {
                playerEffectManager.leftWeaponFX = leftHandSlot.currentWeaponModel.GetComponentInChildren<WeaponFX>();
                playerEffectManager.leftWeaponFX.StopWeaponFX();
            }
            else
            {
                Debug.Log("No WeaponFX found");
            }
        }

        public void LoadRightWeaponDamageCollider()
        {
            if (rightHandSlot.currentWeaponModel == null)
                return;

            rightHandDamageCollider = rightHandSlot.currentWeaponModel.GetComponentInChildren<DamageCollider>();
            // rightHandDamageCollider.currentWeaponDamage = playerInventory.rightWeapon.baseDamage;
            // rightHandDamageCollider.poiseBreak = playerInventory.rightWeapon.poiseBreak;
            if (rightHandSlot.currentWeaponModel.GetComponentInChildren<WeaponFX>() != null)
            {
                playerEffectManager.rightWeaponFX = rightHandSlot.currentWeaponModel.GetComponentInChildren<WeaponFX>();
                playerEffectManager.rightWeaponFX.StopWeaponFX();
            }
            else
            {
                Debug.Log("No WeaponFX found");
            }
        }

        public void OpenDamageCollider()
        {
            if (playerManager.isUsingRightHand)
            {
                rightHandDamageCollider.EnableDamageCollider();
                playerEffectManager.rightWeaponFX.PlayWeaponFX();
            }
            else if (playerManager.isUsingLeftHand)
            {
                leftHandDamageCollider.EnableDamageCollider();
                playerEffectManager.leftWeaponFX.PlayWeaponFX();
            }
        }

        public void CloseDamageCollider()
        {
            if (playerManager.isUsingRightHand)
            {
                rightHandDamageCollider.DisableDamageCollider();
                playerEffectManager.rightWeaponFX.StopWeaponFX();
            }
            else if (playerManager.isUsingLeftHand)
            {
                leftHandDamageCollider.DisableDamageCollider();
                playerEffectManager.leftWeaponFX.StopWeaponFX();
            }
        }

        #endregion

        #region Handle Weapon Stamina Drain
        public void DrainStaminaLightAttack()
        {
            if (playerStats == null || attackingWeapon == null)
                return;

            playerStats.TakeStaminaDamage(Mathf.RoundToInt(attackingWeapon.baseStamina * attackingWeapon.lightAttackMultiplier));
        }

        public void DrainStaminaHeavyAttack()
        {
            if (playerStats == null || attackingWeapon == null)
                return;

            playerStats.TakeStaminaDamage(Mathf.RoundToInt(attackingWeapon.baseStamina * attackingWeapon.heavyAttackMultiplier));
        }

        public void DrainStaminaChargeAttack()
        {
            if (playerStats == null || attackingWeapon == null)
                return;

            playerStats.TakeStaminaDamage(Mathf.RoundToInt(attackingWeapon.baseStamina * attackingWeapon.chargeAttackMultiplier));
        }
        #endregion
    }
}
