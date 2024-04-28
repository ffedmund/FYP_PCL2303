using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Unity.Netcode;

namespace FYP
{
    public class PlayerStats : CharacterStats
    {
        PlayerManager playerManager;
        PlayerLocomotion playerLocomotion;

        PlayerData playerData;

        ArtifactAbilityController artifactAbility;

        public bool isFadeOut = false;
        public CanvasGroup staminaBarCanvasGroup;

        public float maxStamina;
        public float currentStamina;
        public float staminaRegenrationAmount = 1;
        public float staminaRegenTimer = 0;

        public HealthBar healthBar;
        public StaminaBar staminaBar;
        public ManaBar manaBar;

        public UIEnemyHealthBar enemyPlayerHealthBar;


        AnimatorHandler animatorHandler;

        // bool isLocalPlayer = true;

        public override void OnNetworkSpawn()
        {
            maxHealth = SetMaxHealthFromHealthLevel();

            if (IsOwner)
            {
                if (healthBar == null)
                {
                    healthBar = FindObjectOfType<HealthBar>();
                }
                currentHealth.Value = maxHealth;
                healthBar.SetMaxHealth(maxHealth);
                healthBar.SetCurrentHealth(currentHealth.Value);
            }
            else
            {
                enemyPlayerHealthBar = GetComponentsInChildren<UIEnemyHealthBar>()[0];
                enemyPlayerHealthBar.SetMaxHealth(maxHealth);
                Debug.Log("Non local Max Health: " + maxHealth);
                enemyPlayerHealthBar.SetHealth(currentHealth.Value);
                Debug.Log("Non local Current Health: " + currentHealth.Value);
            }

            SetPlayerDataAttributes();

            currentHealth.OnValueChanged += OnCurrentHealthChange;
        }

        private void OnCurrentHealthChange(int previousValue, int newValue)
        {
            Debug.Log("OnValueChanged Current Health: " + newValue);
            if(IsLocalPlayer){
                Debug.Log("OnValueChanged Local Current Health: " + newValue);
                healthBar.SetCurrentHealth(newValue);
            }
            else{
                Debug.Log("OnValueChanged Non local Current Health: " + newValue);
                enemyPlayerHealthBar.SetHealth(newValue);
            }
        }

        [Rpc(SendTo.NotOwner)]
        public void SendCurrentHealthRpc(int health)
        {
            // currentHealth.Value = health;
            enemyPlayerHealthBar = GetComponentsInChildren<UIEnemyHealthBar>()[0];
            enemyPlayerHealthBar.SetMaxHealth(maxHealth);
            Debug.Log("Non local Max Health: " + maxHealth);
            enemyPlayerHealthBar.SetHealth(currentHealth.Value);
            Debug.Log("Non local Current Health: " + currentHealth.Value);
        }

        [Rpc(SendTo.Owner)]
        public void GainExpRpc(int exp)
        {
            GainExp(exp);
        }

        private void Awake()
        {
            playerManager = GetComponent<PlayerManager>();
            animatorHandler = GetComponentInChildren<AnimatorHandler>();
            playerLocomotion = GetComponent<PlayerLocomotion>();
            artifactAbility = GetComponent<ArtifactAbilityController>();
            playerData = playerManager.playerData;

            // healthBar = FindObjectOfType<HealthBar>();
            staminaBar = FindObjectOfType<StaminaBar>();
            manaBar = FindObjectOfType<ManaBar>();
        }

        void Start()
        {
            // maxHealth = SetMaxHealthFromHealthLevel();
            // currentHealth.Value = maxHealth;
            // if (isLocalPlayer)
            // {
            //     healthBar.SetMaxHealth(maxHealth);
            //     healthBar.SetCurrentHealth(currentHealth.Value);
            // }
            // else
            // {
            //     enemyPlayerHealthBar = GetComponentsInChildren<UIEnemyHealthBar>()[0];
            //     enemyPlayerHealthBar.SetMaxHealth(maxHealth);
            //     Debug.Log("Non local Max Health: " + maxHealth);
            //     enemyPlayerHealthBar.SetHealth(currentHealth.Value);
            //     Debug.Log("Non local Current Health: " + currentHealth.Value);
            // }
            // healthBar.SetMaxHealth(maxHealth);
            // healthBar.SetCurrentHealth(currentHealth.Value);

            // SetPlayerDataAttributes();

            if (IsLocalPlayer)
            {
                maxStamina = SetMaxStaminaFromStaminaLevel();
                currentStamina = maxStamina;
                staminaBar.SetMaxStamina(maxStamina);
                staminaBar.SetCurrentStamina(currentStamina);

                maxMana = SetMaxManaFromManaLevel();
                currentMana = maxMana;
                manaBar.SetMaxMana(maxMana);
                manaBar.SetCurrentMana(currentMana);
                SetUI(true);
            }
        }

        public void SetUI(bool resetCurrentStamina){
            if(healthBar && staminaBar){
                staminaBar.SetMaxStamina(maxStamina);
                healthBar.SetMaxHealth(maxHealth);
                if(resetCurrentStamina){
                    staminaBar.SetCurrentStamina(maxStamina);
                }
            }
        }

        public void SetLocalPlayer(bool isLocalPlayer){
            // this.isLocalPlayer = isLocalPlayer;
        }

        private float SetMaxManaFromManaLevel()
        {
            int artifactMana = 0;
            float artifactPercentage = 1;
            if (TryGetComponent(out ArtifactAbilityController aac))
            {
                artifactMana = aac.manaLevel;
                artifactPercentage = aac.manaPercentage;
            }

            // maxMana = manaLevel * 10;
            maxMana = Mathf.FloorToInt((manaLevel + artifactMana) * artifactPercentage * manaMultiplier * 10);
            return maxMana;
        }

        public override void TakeDamage(int physicalDamage, CharacterManager enemyCharacterDamagingMe)
        {
            Debug.Log("Take Damage");
            if (isDead || !IsOwner)
                return;

            if (playerManager.isInvulnerable)
            {
                return;
            }

            if (IsLocalPlayer)
            {
                float temp1 = Random.value;
                Debug.Log("temp1: " + temp1);
                if (temp1 < luckLevel / 400.0f)
                {
                    playerManager.dodgeEffect.Play();
                    return;
                }
            }
            
            Debug.Log("2 Before base.TakeDamage");
            artifactAbility.TriggerAbilities(TriggerCondition.HpPercentage, null, currentHealth.Value / maxHealth * 100);
            Debug.Log("Before base.TakeDamage");
            base.TakeDamage(physicalDamage, enemyCharacterDamagingMe);
            
            Debug.Log("All Current Health: " + currentHealth.Value);
            Debug.Log("isLocalPlayer? " + IsLocalPlayer);
            if(IsLocalPlayer){
                Debug.Log("Local Current Health: " + currentHealth.Value);
                healthBar.SetCurrentHealth(currentHealth.Value);
            }
            else{
                Debug.Log("Non local Current Health: " + currentHealth.Value);
                enemyPlayerHealthBar.SetHealth(currentHealth.Value);
            }

            animatorHandler.PlayTargetAnimation("Damage_01", true);

            if (currentHealth.Value <= 0 && IsLocalPlayer)
            {
                float temp2 = Random.value;
                if (temp2 < luckLevel / 200.0f)
                {
                    currentHealth.Value = 1;
                    healthBar.SetCurrentHealth(currentHealth.Value);
                    return;
                }

                currentHealth.Value = 0;
                animatorHandler.PlayTargetAnimation("Death_01", true);
                isDead = true;

                UIController uiController = FindAnyObjectByType<UIController>();
                if (uiController)
                {
                    uiController.PlayerDied();
                }

                if(NetworkManager.IsConnectedClient)
                {
                    Item crystalBall = playerManager.playerInventory.RemoveItem("crystal_ball");
                    if(crystalBall != null)
                    {
                        NetworkObjectManager.Singleton.CreateNetworkObjectHandler(crystalBall, playerManager.transform.position, -1);
                    }
                    playerManager.minimapIconController.ResetID();
                }

                playerManager.lockOnTransform = null;
            }
        }

        public override void Heal(int amount)
        {
            base.Heal(amount);
            healthBar?.SetCurrentHealth(currentHealth.Value);
        }

        public void UpdatePlayerStats()
        {
            // playerLocomotion.sprintSpeed = (float)(7 + 2 * System.Math.Log10(dexterityLevel - 9));

            // playerLocomotion.sprintSpeed = (float)(7 + 2 * System.Math.Log10((dexterityLevel + extraDexterity) - 9));
            //Artifact Dexterity
            if(IsOwner)
            {
                UpdateSprintSpeed();

                maxHealth = SetMaxHealthFromHealthLevel();
                currentHealth.Value = maxHealth;

                maxStamina = SetMaxStaminaFromStaminaLevel();

                SetPlayerDataAttributes();
                SetUI(false);
            }
        }

        public void UpdateSprintSpeed()
        {
            playerLocomotion.sprintSpeed = (float)((7 + 2 * System.Math.Log10((dexterityLevel + extraDexterity + artifactAbility.dexterityLevel - 9) * artifactAbility.dexterityPercentage)) * sprintSpeedMultiplier);
        }

        public void GainExp(int exp)
        {
            this.exp = playerData.GetAttribute("exp");

            // float expMultiplier = 1 + (float)System.Math.Log10(this.intelligenceLevel - 9) * 2;
            //Artifact Intelligence
            float expMultiplier = 1 + (float)System.Math.Log10((this.intelligenceLevel + artifactAbility.intelligenceLevel + extraIntelligence) * artifactAbility.intelligencePercentage - 9) * 2;
            this.exp += (int)Mathf.RoundToInt(exp * expMultiplier);
            int expToNextLevel = (int)Mathf.RoundToInt(100 * (float)System.Math.Pow(1.01f, this.playerLevel) - 1);
            while (this.exp >= expToNextLevel)
            {
                playerManager.canLevelUp = true;
                playerManager.levelUpEffect.Play();
                PlayLevelUpSound();

                this.playerLevel++;
                this.exp -= expToNextLevel;
                expToNextLevel = (int)Mathf.RoundToInt(100 * (float)System.Math.Pow(1.01f, this.playerLevel) - 1);

                playerManager.playerAbilityManager.UpdateAbilityLevel();
            }

            playerData.ReplacePlayerData("level", this.playerLevel);
            playerData.ReplacePlayerData("exp", this.exp);
        }

        void PlayLevelUpSound()
        {
            if (AudioSourceController.instance != null)
            {
                Debug.Log("Playing Level Up Sound");
                AudioSourceController.instance.Play("LevelUp");
            }
            else
            {
                Debug.Log("AudioSourceController is null");
            }
        }

        public void GetPlayerDataAttributes()
        {
            this.statsLevel = playerData.GetAttribute("level");
            this.exp = playerData.GetAttribute("exp");

            this.healthLevel = playerData.GetAttribute("vitality");
            this.strengthLevel = playerData.GetAttribute("strength");
            this.intelligenceLevel = playerData.GetAttribute("intelligence");
            this.dexterityLevel = playerData.GetAttribute("dexterity");
            this.enduranceLevel = playerData.GetAttribute("endurance");
            this.luckLevel = playerData.GetAttribute("luck");
        }

        public void SetPlayerDataAttributes()
        {
            playerData.ReplacePlayerData("level", this.statsLevel);
            playerData.ReplacePlayerData("exp", this.exp);

            playerData.ReplacePlayerData("vitality", this.healthLevel);
            playerData.ReplacePlayerData("strength", this.strengthLevel);
            playerData.ReplacePlayerData("intelligence", this.intelligenceLevel);
            playerData.ReplacePlayerData("dexterity", this.dexterityLevel);
            playerData.ReplacePlayerData("endurance", this.enduranceLevel);
            playerData.ReplacePlayerData("luck", this.luckLevel);
        }

        private float SetMaxStaminaFromStaminaLevel()
        {
            // maxStamina = staminaLevel * 10;
            //Artifact Stamina
            maxStamina = (staminaLevel + artifactAbility.staminaLevel) * artifactAbility.staminaPercentage * 10 * 10;
            return maxStamina;
        }

        public void TakeStaminaDamage(float staminaDamage)
        {
            if (!playerManager.IsOwner)
            {
                return;
            }

            if (isFadeOut == true)
            {
                isFadeOut = false;
                staminaBarCanvasGroup.DOFade(1, 1f);
            }

            currentStamina = Mathf.RoundToInt(currentStamina - staminaDamage * staminaDamageMultiplier * 10);
            if (currentStamina <= 0)
            {
                currentStamina = 0;
            }
            staminaBar.SetCurrentStamina(currentStamina);
        }

        public void RegenerateStamina()
        {
            if (!playerManager.IsOwner)
            {
                return;
            }

            if (playerManager.isInteracting)
            {
                staminaRegenTimer = 0;
            }
            else
            {
                staminaRegenTimer += Time.deltaTime;
                if (currentStamina < maxStamina && staminaRegenTimer > 0.5f)
                {
                    currentStamina += (staminaRegenrationAmount + extraStaminaRegenration)* Time.deltaTime * 20 * 20;
                    staminaBar.SetCurrentStamina(Mathf.RoundToInt(currentStamina));
                }
                else if (currentStamina >= maxStamina)
                {
                    // fade out stamina bar
                    if (isFadeOut == false)
                    {
                        isFadeOut = true;
                        staminaBarCanvasGroup.DOFade(0, 1f);
                    }
                }
            }
        }

        public void TakeManaDamage(float manaCost)
        {
            if (!playerManager.IsOwner)
            {
                return;
            }

            currentMana = Mathf.RoundToInt(currentMana - manaCost);
            if (currentMana < 0)
            {
                currentMana = 0;
            }
            manaBar.SetCurrentMana(currentMana);
        }

        public void Respawn()
        {
            isDead = false;
            currentHealth.Value = maxHealth;
            healthBar.SetCurrentHealth(currentHealth.Value);

            currentStamina = maxStamina;
            staminaBar.SetCurrentStamina(currentStamina);

            currentMana = maxMana;
            manaBar.SetCurrentMana(currentMana);

            playerManager.characterController.enabled = false;
            playerManager.transform.position = new Vector3((float)-44.60096, (float)7.152012, (float)-58.55451);
            playerManager.characterController.enabled = true;

            animatorHandler.PlayTargetAnimation("Empty", true);
        }


    }
}