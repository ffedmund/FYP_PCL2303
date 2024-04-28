using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FYP
{
    public class PlayerAbilityManager : MonoBehaviour
    {
        PlayerManager playerManager;
        public PlayerRole playerRole;

        public List<PlayerAbility> talentChoiceList = new List<PlayerAbility>();

        public int roleID;

        public float abilityPowerUpMultiplier = 1;


        async void Start()
        {
            playerManager = GetComponent<PlayerManager>();

            playerRole = JoinGameManager.playerRole;

            if (playerRole.playerTargets == null)
            {
                await DataReader.ReadBackgroundDataBase();
                await DataReader.ReadAbilityDataBase();
                await DataReader.ReadTargetDataBase();

                roleID = 3;


                playerRole.playerBackground = DataReader.backgorundDictionary[roleID];
                playerRole.playerSkill = DataReader.skillDictionary[roleID];

                Debug.Log("DataReader.talentDictionary.Count is " + DataReader.talentDictionary.Count);
                do
                {
                    int tempRndInt = Random.Range(0, DataReader.talentDictionary.Count);
                    if (!talentChoiceList.Contains(DataReader.talentDictionary[tempRndInt]))
                    {
                        talentChoiceList.Add(DataReader.talentDictionary[tempRndInt]);
                    }
                } while (talentChoiceList.Count < 3);
                playerRole.playerTargets = new List<PlayerTarget>
                {
                    DataReader.roleTargetDictionary[0]
                };
                int maximum = 100;
                do
                {
                    PlayerTarget rndPlayerTarget = DataReader.targetList[Random.Range(0, DataReader.targetList.Count)];
                    if (!playerRole.playerTargets.Contains(rndPlayerTarget))
                    {
                        playerRole.playerTargets.Add(rndPlayerTarget);
                    }
                    maximum--;
                } while (playerRole.playerTargets.Count < 3 && maximum > 0);
                
            }

            playerRole.playerSkill.level = 0;
            playerRole.playerTalent.level = 0;

            UpdateAbilityLevel();
        }


        private bool isCooldown = false;

        IEnumerator StartCooldown()
        {
            isCooldown = true;
            yield return new WaitForSeconds(5);
            isCooldown = false;
        }

        // Increase moving speed by 50% for {skill level} seconds with 60s cooldown.
        public void IncreaseMovingSpeed()
        {
            if (!isCooldown && playerRole.playerSkill.level > 0)
            {
                playerManager.playerStats.sprintSpeedMultiplier *= 1.5f * abilityPowerUpMultiplier;
                playerManager.playerStats.UpdatePlayerStats();
                StartCoroutine(ResetMovingSpeed());
                StartCoroutine(StartCooldown());
            }
        }

        IEnumerator ResetMovingSpeed()
        {
            yield return new WaitForSeconds(playerRole.playerSkill.level);
            playerManager.playerStats.sprintSpeedMultiplier /= 1.5f;
            playerManager.playerStats.UpdatePlayerStats();
        }


        // Heal {10% * skill level} of max health instantly with 60s cooldown.
        public void Healing()
        {
            if (!isCooldown && playerRole.playerSkill.level > 0)
            {
                int healingAmount = (int)(playerManager.playerStats.maxHealth * 0.1f * playerRole.playerSkill.level * abilityPowerUpMultiplier);
                playerManager.playerStats.Heal(healingAmount);
                StartCoroutine(StartCooldown());
            }
        }

        // Become invincible for {skill level} seconds with 60s cooldown.
        public void Invincible()
        {
            if (!isCooldown && playerRole.playerSkill.level > 0)
            {
                playerManager.isUsingSkill = true;
                playerManager.isInvulnerable = true;

                playerManager.anim.SetBool("isInvulnerable", true);

                StartCoroutine(ResetInvincible());
                StartCoroutine(StartCooldown());
            }
        }

        IEnumerator ResetInvincible()
        {
            yield return new WaitForSeconds(playerRole.playerSkill.level * abilityPowerUpMultiplier);
            playerManager.isUsingSkill = false;
            playerManager.isInvulnerable = false;

            playerManager.anim.SetBool("isInvulnerable", false);
        }


        private int originalLayer;

        // Become invisible for {skill level} seconds with 60s cooldown.
        public void Invisible()
        {
            if (!isCooldown && playerRole.playerSkill.level > 0)
            {
                SkinnedMeshRenderer[] renderers = playerManager.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();

                originalLayer = renderers[0].gameObject.layer;

                foreach (SkinnedMeshRenderer renderer in renderers)
                {
                    renderer.gameObject.layer = LayerMask.NameToLayer("Invisible");
                }

                playerManager.isInvisible = true;

                StartCoroutine(ResetInvisible());
                StartCoroutine(StartCooldown());
            }
        }

        IEnumerator ResetInvisible()
        {
            yield return new WaitForSeconds(playerRole.playerSkill.level * abilityPowerUpMultiplier);

            SkinnedMeshRenderer[] renderers = playerManager.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();

            foreach (SkinnedMeshRenderer renderer in renderers)
            {
                renderer.gameObject.layer = originalLayer;
            }

            playerManager.isInvisible = false;
        }


        public void UpdateSkill(int skillID)
        {
            playerRole.playerSkill.level = Mathf.Min(10, (int) playerManager.playerData.GetHonorLevel() + (int) playerManager.playerStats.playerLevel / 10);
        }

        public void UseSkill(int roleID)
        {
            if (!playerManager.canUseSkill)
                return;

            switch (roleID)
            {
                case 0:
                    IncreaseMovingSpeed();
                    break;
                case 1:
                    Healing();
                    break;
                case 2:
                    Invincible();
                    break;
                case 3:
                    Invisible();
                    break;
                default:
                    break;
            }
        }


        public void UpgradeSkill(PlayerAbility ability)
        {
            playerRole.playerSkill.level++;
        }

        public void RecoverHealthSlowly()
        {
            StartCoroutine(RecoverHealth());
        }

        IEnumerator RecoverHealth()
        {
            while (true)
            {
                yield return new WaitForSeconds(0.5f);
                if (playerManager.playerStats.currentHealth.Value < playerManager.playerStats.maxHealth)
                    playerManager.playerStats.Heal(1 + playerRole.playerTalent.level);
            }
        }

        public void UpdateTalent(int talentID)
        {

            playerRole.playerTalent.level = Mathf.Min(10, (int) playerManager.playerData.GetHonorLevel() + (int) playerManager.playerStats.playerLevel / 10);
        
            switch (talentID)
            {
                case 0:
                    playerManager.playerStats.healthMultiplier *= 1 + playerRole.playerTalent.level * 0.05f;
                    break;
                case 1:
                    playerManager.playerStats.sprintSpeedMultiplier *= 1 + playerRole.playerTalent.level * 0.05f;
                    break;
                case 2:
                    RecoverHealthSlowly();
                    break;
                case 3:
                    playerManager.playerStats.strengthMultiplier *= 1 + playerRole.playerTalent.level * 0.05f;
                    break;
                case 4:
                    playerManager.playerStats.enduranceMultiplier *= 1 - playerRole.playerTalent.level * 0.05f;
                    break;
                case 5:
                    playerManager.playerStats.staminaDamageMultiplier *= 1 - playerRole.playerTalent.level * 0.05f;
                    break;
                default:
                    break;
            }
            playerManager.playerStats.UpdatePlayerStats();
        }

        public void UpdateAbilityLevel()
        {
            UpdateSkill(playerRole.playerSkill.id);
            UpdateTalent(playerRole.playerTalent.id);
        }

        // public void UpgradeTalent(int talentID)
        // {
        //     playerRole.playerTalent.level++;
        //     UpdateTalent(talentID);
        // }

        public void LockAbility()
        {
            playerManager.canUseSkill = false;
        }

        public void UnlockAbility()
        {
            playerManager.canUseSkill = true;
        }

        public void HandleAbilityPowerUp(float powerUpMultiplier)
        {
            abilityPowerUpMultiplier =  1 + powerUpMultiplier;
            Debug.Log("Ability Power Up Multiplier is " + abilityPowerUpMultiplier);
        }
    }
}