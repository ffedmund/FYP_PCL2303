using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace FYP
{
    public class PlayerCombatManager : CharacterCombatManager
    {
        PlayerManager playerManager;
        PlayerStats playerStats;
        PlayerInventory playerInventory;
        CameraHandler cameraHandler;
        AnimatorHandler animatorHandler;
        InputHandler inputHandler;
        WeaponSlotManager weaponSlotManager;
        PlayerEffectManager playerEffectManager;
        ArtifactAbilityController artifactController;


        public string oh_charge_attack_01 = "OH_Charging_Attack_Charge_01";
        public string oh_charge_attack_02 = "OH_Charging_Attack_Charge_02";

        public string th_charge_attack_01 = "TH_Charging_Attack_Charge_01";
        public string th_charge_attack_02 = "TH_Charging_Attack_Charge_02";


        public string lastAttack;

        private void Awake()
        {
            playerManager = GetComponentInParent<PlayerManager>();
            playerStats = GetComponentInParent<PlayerStats>();
            playerInventory = GetComponentInParent<PlayerInventory>();
            cameraHandler = FindObjectOfType<CameraHandler>();
            animatorHandler = GetComponentInChildren<AnimatorHandler>();
            weaponSlotManager = GetComponentInChildren<WeaponSlotManager>();
            playerEffectManager = GetComponent<PlayerEffectManager>();
            inputHandler = GetComponent<InputHandler>();
            artifactController = GetComponent<ArtifactAbilityController>();
        }

        public void HandleWeaponCombo(WeaponItem weapon)
        {
            if (playerStats.currentStamina <= 0)
                return;

            if (inputHandler.comboFlag)
            {
                if (weapon == null) return;
                animatorHandler.anim.SetBool("canDoCombo", false);
                if (lastAttack == weapon.OH_Light_Attack_1)
                {
                    animatorHandler.PlayTargetAnimation(weapon.OH_Light_Attack_2, true);
                    lastAttack = weapon.OH_Light_Attack_2;
                }
                else if (lastAttack == weapon.OH_Light_Attack_2)
                {
                    animatorHandler.PlayTargetAnimation(weapon.OH_Light_Attack_1, true);
                    lastAttack = weapon.OH_Light_Attack_1;
                }
                else if (lastAttack == weapon.TH_Light_Attack_1)
                {
                    animatorHandler.PlayTargetAnimation(weapon.TH_Light_Attack_2, true);
                    lastAttack = weapon.TH_Light_Attack_2;
                }
                else if (lastAttack == weapon.TH_Light_Attack_2)
                {
                    animatorHandler.PlayTargetAnimation(weapon.TH_Light_Attack_3, true);
                    lastAttack = weapon.TH_Light_Attack_3;
                }
            }

            playerEffectManager.PlayWeaponFX(false);
        }

        public void HandleLightAttack(WeaponItem weapon)
        {
            if (playerStats.currentStamina <= 0)
                return;

            if (weapon == null) return;
            weaponSlotManager.attackingWeapon = weapon;
            if (cameraHandler.currentLockOnTarget != null)
            {
                if (cameraHandler.currentLockOnTarget.lockOnTransform != null)
                {
                    Transform targetTransorm = cameraHandler.currentLockOnTarget.lockOnTransform;
                    // Collider playerCollider = playerManager.transform.GetComponent<Collider>();
                    Vector3 dir = targetTransorm.position - playerManager.transform.position;
                    // float distance = Mathf.Min(Vector3.Distance(targetTransorm.position, playerCollider.ClosestPoint(targetTransorm.position)), 2.25f);
                    float distance = Mathf.Min(Vector3.Distance(targetTransorm.position, playerManager.characterController.ClosestPointOnBounds(targetTransorm.position)), 2.25f);

                    dir.Normalize();
                    dir.y = 0;
                    if (distance > 1.5f)
                    {
                        // playerManager.transform.DOMove(playerManager.transform.position + dir * distance, 0.3f).SetEase(Ease.InOutSine);
                        Debug.Log("Distance: " + distance);
                        Vector3 targetPosition = playerManager.transform.position + dir * distance;
                        float duration = 0.3f;

                        DOTween.To(() => playerManager.transform.position, x => playerManager.transform.position = x, targetPosition, duration)
                            .SetEase(Ease.InOutSine)
                            .OnUpdate(() =>
                            {
                                playerManager.characterController.Move(5 * dir * Time.deltaTime);
                            });
                    }
                }

            }

            if (inputHandler.twoHandFlag)
            {
                animatorHandler.PlayTargetAnimation(weapon.TH_Light_Attack_1, true);
                lastAttack = weapon.TH_Light_Attack_1;
            }
            else
            {
                animatorHandler.PlayTargetAnimation(weapon.OH_Light_Attack_1, true);
                lastAttack = weapon.OH_Light_Attack_1;
            }
            playerEffectManager.PlayWeaponFX(false);

            currentAttackType = AttackType.light;

            //Artifact Trigger
            artifactController.TriggerAbilities(TriggerCondition.Attack, null);
        }

        public void HandleHeavyAttack(WeaponItem weapon)
        {
            if (playerStats.currentStamina <= 0 || !playerManager.canAttack)
                return;

            if (weapon == null) return;
            weaponSlotManager.attackingWeapon = weapon;
            if (inputHandler.twoHandFlag)
            {
                // animatorHandler.PlayTargetAnimation(weapon.TH_Heavy_Attack_1, true);
                // lastAttack = weapon.TH_Heavy_Attack_1;
                return;
            }
            else
            {
                animatorHandler.PlayTargetAnimation(weapon.OH_Heavy_Attack_1, true);
                lastAttack = weapon.OH_Heavy_Attack_1;
            }
            playerEffectManager.PlayWeaponFX(false);

            currentAttackType = AttackType.heavy;
        }



        public void HandleChargeAttack(WeaponItem weapon)
        {
            if (playerStats.currentStamina <= 0 || !playerManager.canAttack)
                return;

            if (weapon == null) return;
            weaponSlotManager.attackingWeapon = weapon;

            if (playerManager.canDoCombo)
            {
                playerManager.inputHandler.comboFlag = true;
                HandleChargewWeaponCombo(playerManager);
                playerManager.inputHandler.comboFlag = false;
            }
            else
            {
                if (playerManager.isInteracting) return;
                if (playerManager.canDoCombo) return;

                // if (playerManager.isUsingLeftHand)
                // {
                //     Debug.Log("Left Hand");
                //     animatorHandler.PlayTargetAnimation(oh_charge_attack_01, true);
                //     lastAttack = oh_charge_attack_01;
                // }
                // else if (playerManager.isUsingRightHand)
                // {
                //     Debug.Log("Right Hand");
                if (playerManager.inputHandler.twoHandFlag)
                {
                    Debug.Log("Two Hand");
                    animatorHandler.PlayTargetAnimation(th_charge_attack_01, true);
                    lastAttack = th_charge_attack_01;
                }
                else
                {
                    Debug.Log("One Hand");
                    animatorHandler.PlayTargetAnimation(oh_charge_attack_01, true);
                    lastAttack = oh_charge_attack_01;
                }
                // }
            }

            playerEffectManager.PlayWeaponFX(false);

            currentAttackType = AttackType.charge;
        }

        #region Input Actions

        public void HandleRBAction()
        {
            if (playerInventory.rightWeapon.isMeleeWeapon)
            {
                PerformRBMeleeAction();
            }
            else if (playerInventory.rightWeapon.isSpellCaster || playerInventory.rightWeapon.isFaithCaster || playerInventory.rightWeapon.isPyroCaster)
            {
                PerformRBMagicAction(playerInventory.rightWeapon);
            }

        }
        #endregion

        #region Attack Actions
            
        private void PerformRBMeleeAction()
        {
            if (playerManager.canDoCombo)
            {
                inputHandler.comboFlag = true;
                HandleWeaponCombo(playerInventory.rightWeapon);
                inputHandler.comboFlag = false;
            }
            else
            {
                if (playerManager.isInteracting)
                    return;
                animatorHandler.anim.SetBool("isUsingRightHand", true);
                HandleLightAttack(playerInventory.rightWeapon);
            }
        }

        private void PerformRBMagicAction(WeaponItem weapon)
        {
            if (playerManager.isInteracting) return;
            
            if (weapon.isFaithCaster)
            {
                if (playerInventory.currentSpell != null && playerInventory.currentSpell.isFaithSpell)
                {
                    if (playerStats.currentMana >= playerInventory.currentSpell.manaCost)
                    {
                        playerInventory.currentSpell.AttemptToCastSpell(animatorHandler, playerStats, weaponSlotManager);
                    }
                    else
                    {
                        animatorHandler.PlayTargetAnimation("Shrug", true);
                    }
                }
            }
            else if (weapon.isPyroCaster)
            {
                if (playerInventory.currentSpell != null && playerInventory.currentSpell.isPyroSpell)
                {
                    if (playerStats.currentMana >= playerInventory.currentSpell.manaCost)
                    {
                        playerInventory.currentSpell.AttemptToCastSpell(animatorHandler, playerStats, weaponSlotManager);
                    }
                    else
                    {
                        animatorHandler.PlayTargetAnimation("Shrug", true);
                    }
                }
            }
        }

        public void SuccessfullyCastSpell()
        {
            playerInventory.currentSpell.SuccessfullyCastSpell(animatorHandler, playerStats, cameraHandler, weaponSlotManager); 
            animatorHandler.anim.SetBool("isFiringSpell", true);
        }

        #endregion

        private void HandleChargewWeaponCombo(PlayerManager playerManager)
        {
            if (playerManager.inputHandler.comboFlag)
            {
                animatorHandler.anim.SetBool("canDoCombo", false);

                // if (playerManager.isUsingLeftHand)
                // {
                //     if (lastAttack == oh_charge_attack_01)
                //     {
                //         animatorHandler.PlayTargetAnimation(oh_charge_attack_02, true);
                //         lastAttack = oh_charge_attack_02;
                //     }
                //     else
                //     {
                //         animatorHandler.PlayTargetAnimation(oh_charge_attack_01, true);
                //         lastAttack = oh_charge_attack_01;
                //     }
                // }
                // else if (playerManager.isUsingRightHand)
                // {
                if (inputHandler.twoHandFlag)
                {
                    if (lastAttack == th_charge_attack_01)
                    {
                        animatorHandler.PlayTargetAnimation(th_charge_attack_02, true);
                        lastAttack = th_charge_attack_02;
                    }
                    else
                    {
                        animatorHandler.PlayTargetAnimation(th_charge_attack_01, true);
                        lastAttack = th_charge_attack_01;
                    }
                }
                else
                {
                    if (lastAttack == oh_charge_attack_01)
                    {
                        animatorHandler.PlayTargetAnimation(oh_charge_attack_02, true);
                        lastAttack = oh_charge_attack_02;
                    }
                    else
                    {
                        animatorHandler.PlayTargetAnimation(oh_charge_attack_01, true);
                        lastAttack = oh_charge_attack_01;
                    }
                }
                // }
            }
        }
    }
}

