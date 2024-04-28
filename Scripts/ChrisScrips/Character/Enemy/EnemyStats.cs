using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FYP
{
    public class EnemyStats : CharacterStats
    {
        public UIEnemyHealthBar enemyHealthBar;

        Animator animator;
        EnemyManager enemyManager;

        public int experienceDrop = 50;

        private void Awake()
        {
            animator = GetComponentInChildren<Animator>();
            enemyManager = GetComponent<EnemyManager>();
        }

        void Start()
        {
            // maxHealth = SetMaxHealthFromHealthLevel();
            // currentHealth.Value = maxHealth;
            // enemyHealthBar.SetMaxHealth(currentHealth.Value);
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            
            
            int difficultyLevel = Mathf.RoundToInt(Vector3.Magnitude(transform.position - Vector3.zero)) / MapGenerator.mapChunkSize;
            playerLevel = difficultyLevel / 6;
            extraStrength += playerLevel / 5;
            extraIntelligence += playerLevel / 6;
            extraEndurance += playerLevel / 7;
            extraDexterity += playerLevel / 8;
            extraLuck += playerLevel / 9;

            if(IsOwner)
            {
                maxHealth = difficultyLevel * 5 + SetMaxHealthFromHealthLevel();
                enemyHealthBar.SetMaxHealth(maxHealth);
                currentHealth.Value = maxHealth;
                enemyHealthBar.SetHealth(currentHealth.Value);
            }

            if (gameObject.TryGetComponent(out LootDropHandler lootDropHandler))
            {
                lootDropHandler.enabled = true;
            }

            if(!IsOwner)
            {
                enemyHealthBar.SetMaxHealth(currentHealth.Value);
                currentHealth.OnValueChanged = OnCurrentHealthChange;
            }
        }

        private void OnCurrentHealthChange(int previousValue, int newValue)
        {
            enemyHealthBar.SetHealth(newValue);
            if(newValue <= 0 && !isDead)
            {
                Dead();
            }
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            playerLevel = 0;
            extraStrength = 0;
            extraIntelligence = 0;
            extraEndurance = 0;
            extraDexterity = 0;
            extraLuck = 0;
            isDead = false;
            if(IsOwner)
            {
                currentHealth.Value = maxHealth;
            }
        }

        private int SetMaxHealthFromHealthLevel()
        {
            maxHealth = healthLevel * 10;
            return maxHealth;
        }

        public override void TakeDamage(int physicalDamage, CharacterManager enemyCharacterDamagingMe)
        {
            if (isDead)
                return;

            if(IsOwner)
            {
                base.TakeDamage(physicalDamage, enemyCharacterDamagingMe);

                enemyHealthBar.SetHealth(currentHealth.Value);
            }
            
            if (!enemyManager.isSuperArmor)
                animator.Play("Damage_01");

            if (enemyManager.currentTarget is PlayerStats)
            {
                if (currentHealth.Value > 0)
                {
                    BGMEvent.TriggerEvent("Battle", enemyManager.transform);
                }
                else
                {
                    BGMEvent.RemoveTrigger("Battle", enemyManager.transform);
                }
            }

            if (currentHealth.Value <= 0)
            {
                Dead();
                if (enemyCharacterDamagingMe is PlayerManager)
                {
                    PlayerManager playerManager = (PlayerManager)enemyCharacterDamagingMe;
                    if(IsServer)
                    {
                        playerManager.GetComponent<PlayerStats>().GainExpRpc(experienceDrop);
                    }
                    else
                    {
                        playerManager.GetComponent<PlayerStats>().GainExp(experienceDrop);
                    }
                    playerManager.inputHandler.lockOnFlag = false;
                    FindObjectOfType<CameraHandler>().currentLockOnTarget = null;
                }
            }
        }

        void Dead()
        {
            currentHealth.Value = 0;
            animator.Play("Death_01");
            isDead = true;
            enemyManager.lockOnTransform = null;
        }

        //Back to pool, reset all state
        // private void OnDisable()
        // {
        //     isDead = false;
        //     currentHealth.Value = maxHealth;
        //     enemyHealthBar.SetMaxHealth(maxHealth);
        // }
    }
}

