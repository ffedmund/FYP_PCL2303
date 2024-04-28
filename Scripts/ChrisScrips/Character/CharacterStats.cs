using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Unity.Netcode;


namespace FYP
{
    public class CharacterStats : NetworkBehaviour
    {
        [Header("Player Level")]
        public int exp = 0;
        public int playerLevel = 1;
        public int statsLevel = 1;

        [Header("Stat Levels")]
        public int healthLevel = 10;
        public int staminaLevel = 10;
        public int manaLevel = 10;
        public int strengthLevel = 10;
        public int intelligenceLevel = 10;
        public int dexterityLevel = 10;
        public int enduranceLevel = 10;
        public int luckLevel = 10;


        public int maxHealth;
        // public int currentHealth;
        public NetworkVariable<int> currentHealth = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        public float maxMana;
        public float currentMana;

        [Header("Multipliers")]
        public float healthMultiplier = 1f;
        public float manaMultiplier = 1f;
        public float staminaDamageMultiplier = 1f;
        public float sprintSpeedMultiplier = 1f;
        public float strengthMultiplier = 1f;
        public float enduranceMultiplier = 1f;


        [Header("Armor Absorption")]
        public float physicalDamageAbsorptionHead;
        public float physicalDamageAbsorptionBody;
        public float physicalDamageAbsorptionLegs;
        public float physicalDamageAbsorptionHands;

        [Header("Extra Stats")]
        public int extraStrength = 0;
        public int extraIntelligence = 0;
        public int extraDexterity = 0;
        public int extraEndurance = 0;
        public int extraLuck = 0;
        public float extraStaminaRegenration = 0;

        public bool isDead;

        [Rpc(SendTo.Owner)]
        public void TakeDamageRpc(int physicalDamage, ulong clientId)
        {
            Debug.Log("Being attacked by client :" + clientId + $" deal {physicalDamage} damage");
            TakeDamage(physicalDamage, this is EnemyStats? NetworkManager.ConnectedClients[clientId].PlayerObject.GetComponent<PlayerManager>():null);
        }

        public virtual void TakeDamage(int physicalDamage, CharacterManager enemyCharacterDamagingMe)
        {
            float totalPhysicalDamageAbsorption = 1 -
                (1 - physicalDamageAbsorptionHead / 100) *
                (1 - physicalDamageAbsorptionBody / 100) *
                (1 - physicalDamageAbsorptionLegs / 100) *
                (1 - physicalDamageAbsorptionHands / 100);

            physicalDamage = Mathf.RoundToInt(physicalDamage - (physicalDamage * totalPhysicalDamageAbsorption));

            Debug.Log("Total Physical Damage Absorption: " + totalPhysicalDamageAbsorption + "%");

            //Artifact Endurance
            int artifactEndurance = 0;
            float artifactPercentage = 1;
            if (TryGetComponent(out ArtifactAbilityController aac))
            {
                artifactEndurance = aac.enduranceLevel;
                artifactPercentage = aac.endurancePercentage;
            }

            float finalDamage = physicalDamage * (1 - (float)System.Math.Log10(enduranceLevel + extraEndurance + artifactEndurance - 9) / 10 * artifactPercentage);

            if (enemyCharacterDamagingMe != null)
            {
                if (enemyCharacterDamagingMe.isPerformingFullyChargedAttack)
                {
                    finalDamage *= 2;
                }
            }

            currentHealth.Value = Mathf.RoundToInt(currentHealth.Value - finalDamage * enduranceMultiplier);

            Debug.Log("Final Damage: " + finalDamage);

            AudioSourceController.instance.Play("Hurt", transform);
        }

        public virtual void Heal(int healingAmount)
        {
            if (isDead || !IsOwner) return;

            currentHealth.Value = Mathf.RoundToInt(currentHealth.Value + healingAmount);

            if (currentHealth.Value > maxHealth)
            {
                currentHealth.Value = maxHealth;
            }
        }

        public int SetMaxHealthFromHealthLevel()
        {
            //Artifact Health
            int artifactHealth = 0;
            float artifactPercentage = 1;
            if (TryGetComponent(out ArtifactAbilityController aac))
            {
                artifactHealth = aac.healthLevel;
                artifactPercentage = aac.healthPercentage;
            }
            maxHealth = Mathf.FloorToInt((healthLevel + artifactHealth) * 10 * artifactPercentage * healthMultiplier);
            Debug.Log("Max Health: " + maxHealth);
            Debug.Log("healthMultiplier: " + healthMultiplier);
            return maxHealth;
        }
    }
}

