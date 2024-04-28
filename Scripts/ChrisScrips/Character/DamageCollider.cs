using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;

namespace FYP
{
    public class DamageCollider : MonoBehaviour
    {
        public CharacterManager characterManager;

        CharacterInventory characterInventory;
        CharacterCombatManager characterCombatManager;
        Collider damageCollider;
        public bool enabledOnStartUp = false;

        public int currentWeaponDamage = 25;

        private void Awake()
        {
            damageCollider = GetComponent<Collider>();
            damageCollider.gameObject.SetActive(true);
            damageCollider.isTrigger = true;
            damageCollider.enabled = enabledOnStartUp;
        }

        public void EnableDamageCollider()
        {
            if (damageCollider == null) return;

            damageCollider.enabled = true;
        }

        public void DisableDamageCollider()
        {
            if (damageCollider == null) return;

            damageCollider.enabled = false;
        }

        private void OnTriggerEnter(Collider collision)
        {
            if (transform.root.gameObject == collision.transform.root.gameObject) return;

            if (collision.tag == "Player")
            {
                PlayerStats playerStats = collision.GetComponent<PlayerStats>();
                CharacterStats characterStats = transform.root.gameObject.GetComponent<CharacterStats>();
                characterManager = transform.root.gameObject.GetComponent<CharacterManager>();
                characterInventory = transform.root.gameObject.GetComponent<CharacterInventory>();
                characterCombatManager = transform.root.gameObject.GetComponent<CharacterCombatManager>();

                if (playerStats != null)
                {
                    int characterStrengthMultiplier = (int)Mathf.Max((characterStats.strengthLevel + characterStats.extraStrength - 10) * 0.5f, 1);

                    float finalPhysicalDamage = currentWeaponDamage * characterStrengthMultiplier;

                    if (characterManager.isUsingRightHand)
                    {
                        if (characterCombatManager.currentAttackType == AttackType.light)
                        {
                            finalPhysicalDamage = finalPhysicalDamage * characterInventory.rightWeapon.lightAttackDamageModifier;
                        }
                        else if (characterCombatManager.currentAttackType == AttackType.heavy)
                        {
                            finalPhysicalDamage = finalPhysicalDamage * characterInventory.rightWeapon.heavyAttackDamageModifier;
                        }
                        else if (characterCombatManager.currentAttackType == AttackType.charge)
                        {
                            finalPhysicalDamage = finalPhysicalDamage * characterInventory.rightWeapon.chargeAttackDamageModifier;
                        }
                    }
                    else if (characterManager.isUsingLeftHand)
                    {
                        if (characterCombatManager.currentAttackType == AttackType.light)
                        {
                            finalPhysicalDamage = finalPhysicalDamage * characterInventory.leftWeapon.lightAttackDamageModifier;
                        }
                        else if (characterCombatManager.currentAttackType == AttackType.heavy)
                        {
                            finalPhysicalDamage = finalPhysicalDamage * characterInventory.leftWeapon.heavyAttackDamageModifier;
                        }
                    }
                    else if(characterCombatManager is EnemyCombatManager && characterCombatManager.currentAttackType == AttackType.charge)
                    {
                        finalPhysicalDamage = finalPhysicalDamage * 2;
                    }

                    int roundedDamage = Mathf.RoundToInt(finalPhysicalDamage);

                    if(characterStats.IsLocalPlayer)
                    {
                        //If attacker is the local player, and target is other palyer
                        playerStats.TakeDamageRpc(roundedDamage, NetworkManager.Singleton.LocalClientId);
                    }
                    else if(NetworkManager.Singleton == null || (playerStats.IsLocalPlayer && !(characterManager is PlayerManager)))
                    {
                        //If attacker is local instance, and target is local player
                        playerStats.TakeDamage(roundedDamage, characterManager);
                    }


                    DamageCollider playerDamageCollider = collision.GetComponentInChildren<DamageCollider>();
                    if (playerDamageCollider != null)
                    {
                        playerDamageCollider.DisableDamageCollider();
                    }
                }
            }

            if (collision.tag == "Enemy")
            {
                EnemyStats enemyStats = collision.GetComponent<EnemyStats>();
                CharacterStats characterStats = transform.root.gameObject.GetComponent<CharacterStats>();
                characterManager = transform.root.gameObject.GetComponent<CharacterManager>();
                characterInventory = transform.root.gameObject.GetComponent<CharacterInventory>();
                characterCombatManager = transform.root.gameObject.GetComponent<CharacterCombatManager>();

                if (enemyStats != null)
                {
                    if (characterStats != null)
                    {
                        // int characterStrengthMultiplier = (int)System.Math.Round((characterStats.strengthLevel - 10) * 0.5);
                        // int physicalDamage = currentWeaponDamage + characterStrengthMultiplier;

                        int characterStrengthMultiplier = (int)System.Math.Round((characterStats.strengthLevel + characterStats.extraStrength - 10) * 0.5);
                        int physicalDamage = (int)(currentWeaponDamage * characterStats.strengthMultiplier + characterStrengthMultiplier);
                        if (characterStats.TryGetComponent(out ArtifactAbilityController artifactAbility))
                        {
                            physicalDamage = Mathf.FloorToInt((physicalDamage + artifactAbility.strengthLevel * 0.5f) * artifactAbility.strengthPercentage);
                            artifactAbility.TriggerAbilities(TriggerCondition.Attack, enemyStats);
                        }

                        // enemyStats.TakeDamage(physicalDamage);
                        // DealDamage(enemyStats, physicalDamage);

                        float finalPhysicalDamage = physicalDamage;

                        if (characterManager.isUsingRightHand)
                        {
                            if (characterCombatManager.currentAttackType == AttackType.light)
                            {
                                finalPhysicalDamage = finalPhysicalDamage * characterInventory.rightWeapon.lightAttackDamageModifier;
                            }
                            else if (characterCombatManager.currentAttackType == AttackType.heavy)
                            {
                                finalPhysicalDamage = finalPhysicalDamage * characterInventory.rightWeapon.heavyAttackDamageModifier;
                            }
                            else if (characterCombatManager.currentAttackType == AttackType.charge)
                            {
                                finalPhysicalDamage = finalPhysicalDamage * characterInventory.rightWeapon.chargeAttackDamageModifier;
                            }
                        }
                        else if (characterManager.isUsingLeftHand)
                        {
                            if (characterCombatManager.currentAttackType == AttackType.light)
                            {
                                finalPhysicalDamage = finalPhysicalDamage * characterInventory.leftWeapon.lightAttackDamageModifier;
                            }
                            else if (characterCombatManager.currentAttackType == AttackType.heavy)
                            {
                                finalPhysicalDamage = finalPhysicalDamage * characterInventory.leftWeapon.heavyAttackDamageModifier;
                            }
                        }

                        if(!enemyStats.IsOwner && characterStats.IsLocalPlayer)
                        {
                            //If attacker is the local player, and target is other palyer
                            enemyStats.TakeDamageRpc(Mathf.RoundToInt(finalPhysicalDamage), NetworkManager.Singleton.LocalClientId);
                        }
                        else if(NetworkManager.Singleton == null || (NetworkManager.Singleton.IsServer && characterStats.IsOwner))
                        {
                            //If attacker is local instance, and target is local player
                            enemyStats.TakeDamage(Mathf.RoundToInt(finalPhysicalDamage), characterManager);
                        }

                        DamageCollider enemyDamageCollider = collision.GetComponentInChildren<DamageCollider>();
                        if (enemyDamageCollider != null)
                        {
                            enemyDamageCollider.DisableDamageCollider();
                        }
                    }
                }

                if (characterStats != null)
                {
                    EnemyManager enemyManager = collision.GetComponent<EnemyManager>();
                    if (enemyManager != null && !enemyStats.isDead)
                    {
                        enemyManager.currentTarget = characterStats;
                    }
                }
            }

            if (collision.tag == "Interactable" || collision.GetComponent<InteractableScript>())
            {
                NPCStats npcStats = collision.GetComponent<NPCStats>();
                CharacterStats characterStats = transform.root.gameObject.GetComponent<CharacterStats>();
                characterManager = transform.root.gameObject.GetComponent<CharacterManager>();
                characterInventory = transform.root.gameObject.GetComponent<CharacterInventory>();
                characterCombatManager = transform.root.gameObject.GetComponent<CharacterCombatManager>();

                if (npcStats != null && characterStats != null)
                {
                    int characterStrengthMultiplier = (int)System.Math.Round((characterStats.strengthLevel + characterStats.extraStrength - 10) * 0.5);
                    int physicalDamage = (int)(currentWeaponDamage * characterStats.strengthMultiplier + characterStrengthMultiplier);
                    if (characterStats.TryGetComponent(out ArtifactAbilityController artifactAbility))
                    {
                        physicalDamage = Mathf.FloorToInt((physicalDamage + artifactAbility.strengthLevel * 0.5f) * artifactAbility.strengthPercentage);
                    }
                    float finalPhysicalDamage = physicalDamage;

                    if (characterManager.isUsingRightHand)
                    {
                        if (characterCombatManager.currentAttackType == AttackType.light)
                        {
                            finalPhysicalDamage = finalPhysicalDamage * characterInventory.rightWeapon.lightAttackDamageModifier;
                        }
                        else if (characterCombatManager.currentAttackType == AttackType.heavy)
                        {
                            finalPhysicalDamage = finalPhysicalDamage * characterInventory.rightWeapon.heavyAttackDamageModifier;
                        }
                        else if (characterCombatManager.currentAttackType == AttackType.charge)
                        {
                            finalPhysicalDamage = finalPhysicalDamage * characterInventory.rightWeapon.chargeAttackDamageModifier;
                        }
                    }
                    else if (characterManager.isUsingLeftHand)
                    {
                        if (characterCombatManager.currentAttackType == AttackType.light)
                        {
                            finalPhysicalDamage = finalPhysicalDamage * characterInventory.leftWeapon.lightAttackDamageModifier;
                        }
                        else if (characterCombatManager.currentAttackType == AttackType.heavy)
                        {
                            finalPhysicalDamage = finalPhysicalDamage * characterInventory.leftWeapon.heavyAttackDamageModifier;
                        }
                    }

                    npcStats.TakeDamage(Mathf.RoundToInt(finalPhysicalDamage), characterManager);
                }
                else if (collision.TryGetComponent(out InteractableScript interactableScript))
                {
                    interactableScript.Interact(null);
                }
            }
        }

        // protected virtual void DealDamage(CharacterStats enemyStats, int physicalDamage)
        // {
        //     float finalPhysicalDamage = physicalDamage;

        //     if (characterManager.isUsingRightHand)
        //     {
        //         if (characterCombatManager.currentAttackType == AttackType.light)
        //         {
        //             finalPhysicalDamage = finalPhysicalDamage * characterInventory.rightWeapon.lightAttackDamageModifier;
        //         }
        //         else if (characterCombatManager.currentAttackType == AttackType.heavy)
        //         {
        //             finalPhysicalDamage = finalPhysicalDamage * characterInventory.rightWeapon.heavyAttackDamageModifier;
        //         }
        //     }
        //     else if (characterManager.isUsingLeftHand)
        //     {
        //         if (characterCombatManager.currentAttackType == AttackType.light)
        //         {
        //             finalPhysicalDamage = finalPhysicalDamage * characterInventory.leftWeapon.lightAttackDamageModifier;
        //         }
        //         else if (characterCombatManager.currentAttackType == AttackType.heavy)
        //         {
        //             finalPhysicalDamage = finalPhysicalDamage * characterInventory.leftWeapon.heavyAttackDamageModifier;
        //         }
        //     }
        //     Debug.Log(Mathf.RoundToInt(finalPhysicalDamage));
        //     enemyStats.TakeDamage(Mathf.RoundToInt(finalPhysicalDamage));
        // }
    }
}

