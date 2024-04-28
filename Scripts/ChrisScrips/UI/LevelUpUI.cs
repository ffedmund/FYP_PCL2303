using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace FYP
{
    public class LevelUpUI : MonoBehaviour
    {
        UIController uiController;

        public PlayerStats playerStats;

        public Button confirmLevelUpButton;

        [Header("Player Level")]
        public int currentPlayerLevel;
        public int projectedPlayerLevel;
        public TextMeshProUGUI currentPlayerLevelText;
        public TextMeshProUGUI projectedPlayerLevelText;

        [Header("Stat Points")]
        public int statPointsAvailable;
        public int statPointsUsed;
        public TextMeshProUGUI statPointsAvailableText;
        public TextMeshProUGUI projectedStatPointsAvailableText;

        [Header("Health")]
        public Slider healthSlider;
        public TextMeshProUGUI currentHealthText;
        public TextMeshProUGUI projectedHealthText;

        [Header("Strength")]
        public Slider strengthSlider;
        public TextMeshProUGUI currentStrengthText;
        public TextMeshProUGUI projectedStrengthText;

        [Header("Intelligence")]
        public Slider intelligenceSlider;
        public TextMeshProUGUI currentIntelligenceText;
        public TextMeshProUGUI projectedIntelligenceText;

        [Header("Dexterity")]
        public Slider dexteritySlider;
        public TextMeshProUGUI currentDexterityText;
        public TextMeshProUGUI projectedDexterityText;

        [Header("Endurance")]
        public Slider enduranceSlider;
        public TextMeshProUGUI currentEnduranceText;
        public TextMeshProUGUI projectedEnduranceText;

        [Header("Luck")]
        public Slider luckSlider;
        public TextMeshProUGUI currentLuckText;
        public TextMeshProUGUI projectedLuckText;


        private void OnEnable()
        {
            statPointsAvailable = playerStats.playerLevel - playerStats.statsLevel;

            currentPlayerLevel = playerStats.playerLevel;
            // currentPlayerLevelText.text = currentPlayerLevel.ToString();
            currentPlayerLevelText.text = playerStats.statsLevel.ToString();

            // projectedPlayerLevel = playerStats.playerLevel;
            projectedPlayerLevel = playerStats.statsLevel;
            projectedPlayerLevelText.text = projectedPlayerLevel.ToString();

            healthSlider.value = playerStats.healthLevel;
            healthSlider.minValue = playerStats.healthLevel;
            healthSlider.maxValue = 99;
            currentHealthText.text = playerStats.healthLevel.ToString();
            projectedHealthText.text = playerStats.healthLevel.ToString();

            strengthSlider.value = playerStats.strengthLevel;
            strengthSlider.minValue = playerStats.strengthLevel;
            strengthSlider.maxValue = 99;
            currentStrengthText.text = playerStats.strengthLevel.ToString();
            projectedStrengthText.text = playerStats.strengthLevel.ToString();

            intelligenceSlider.value = playerStats.intelligenceLevel;
            intelligenceSlider.minValue = playerStats.intelligenceLevel;
            intelligenceSlider.maxValue = 99;
            currentIntelligenceText.text = playerStats.intelligenceLevel.ToString();
            projectedIntelligenceText.text = playerStats.intelligenceLevel.ToString();

            dexteritySlider.value = playerStats.dexterityLevel;
            dexteritySlider.minValue = playerStats.dexterityLevel;
            dexteritySlider.maxValue = 99;
            currentDexterityText.text = playerStats.dexterityLevel.ToString();
            projectedDexterityText.text = playerStats.dexterityLevel.ToString();

            enduranceSlider.value = playerStats.enduranceLevel;
            enduranceSlider.minValue = playerStats.enduranceLevel;
            enduranceSlider.maxValue = 99;
            currentEnduranceText.text = playerStats.enduranceLevel.ToString();
            projectedEnduranceText.text = playerStats.enduranceLevel.ToString();

            luckSlider.value = playerStats.luckLevel;
            luckSlider.minValue = playerStats.luckLevel;
            luckSlider.maxValue = 99;
            currentLuckText.text = playerStats.luckLevel.ToString();
            projectedLuckText.text = playerStats.luckLevel.ToString();

            // UpdateProjectedPlayerLevel();
        }

        public void ConfirmPlayerLevelUpStats()
        {
            // playerStats.playerLevel = projectedPlayerLevel;
            playerStats.healthLevel = Mathf.RoundToInt(healthSlider.value);
            playerStats.strengthLevel = Mathf.RoundToInt(strengthSlider.value);
            playerStats.intelligenceLevel = Mathf.RoundToInt(intelligenceSlider.value);
            playerStats.dexterityLevel = Mathf.RoundToInt(dexteritySlider.value);
            playerStats.enduranceLevel = Mathf.RoundToInt(enduranceSlider.value);
            playerStats.luckLevel = Mathf.RoundToInt(luckSlider.value);

            playerStats.maxHealth = playerStats.SetMaxHealthFromHealthLevel();
            
            playerStats.statsLevel = playerStats.statsLevel + statPointsUsed;

            gameObject.SetActive(false);

            playerStats.UpdatePlayerStats();

            uiController = FindObjectOfType<UIController>();
            uiController.UpdateUI();
        }

        public void CalculateStatsPointsCostToLevelUp()
        {
            statPointsAvailable = playerStats.playerLevel - playerStats.statsLevel;
            statPointsAvailableText.text = statPointsAvailable.ToString();

            statPointsUsed = 0;
            statPointsUsed = statPointsUsed + Mathf.RoundToInt(healthSlider.value) - playerStats.healthLevel;
            statPointsUsed = statPointsUsed + Mathf.RoundToInt(strengthSlider.value) - playerStats.strengthLevel;
            statPointsUsed = statPointsUsed + Mathf.RoundToInt(intelligenceSlider.value) - playerStats.intelligenceLevel;
            statPointsUsed = statPointsUsed + Mathf.RoundToInt(dexteritySlider.value) - playerStats.dexterityLevel;
            statPointsUsed = statPointsUsed + Mathf.RoundToInt(enduranceSlider.value) - playerStats.enduranceLevel;
            statPointsUsed = statPointsUsed + Mathf.RoundToInt(luckSlider.value) - playerStats.luckLevel;

            projectedStatPointsAvailableText.text = (statPointsAvailable - statPointsUsed).ToString();
        }

        private void UpdateProjectedPlayerLevel()
        {
            

            projectedPlayerLevel = currentPlayerLevel;
            projectedPlayerLevel = projectedPlayerLevel + Mathf.RoundToInt(healthSlider.value) - playerStats.healthLevel;
            projectedPlayerLevel = projectedPlayerLevel + Mathf.RoundToInt(strengthSlider.value) - playerStats.strengthLevel;
            projectedPlayerLevel = projectedPlayerLevel + Mathf.RoundToInt(intelligenceSlider.value) - playerStats.intelligenceLevel;
            projectedPlayerLevel = projectedPlayerLevel + Mathf.RoundToInt(dexteritySlider.value) - playerStats.dexterityLevel;
            projectedPlayerLevel = projectedPlayerLevel + Mathf.RoundToInt(enduranceSlider.value) - playerStats.enduranceLevel;
            projectedPlayerLevel = projectedPlayerLevel + Mathf.RoundToInt(luckSlider.value) - playerStats.luckLevel;

            projectedPlayerLevel = projectedPlayerLevel - statPointsAvailable;
            Debug.Log("Stat points available: " + statPointsAvailable);

            projectedPlayerLevelText.text = projectedPlayerLevel.ToString();

            CalculateStatsPointsCostToLevelUp();

            if (statPointsAvailable - statPointsUsed >= 0)
            {
                confirmLevelUpButton.interactable = true;
            }
            else
            {
                confirmLevelUpButton.interactable = false;
            }

            
        }

        public void UpdateHealthLevelSlider()
        {
            projectedHealthText.text = healthSlider.value.ToString();
            UpdateProjectedPlayerLevel();
        }

        public void UpdateStrengthLevelSlider()
        {
            projectedStrengthText.text = strengthSlider.value.ToString();
            UpdateProjectedPlayerLevel();
        }

        public void UpdateIntelligenceLevelSlider()
        {
            projectedIntelligenceText.text = intelligenceSlider.value.ToString();
            UpdateProjectedPlayerLevel();
        }

        public void UpdateDexterityLevelSlider()
        {
            projectedDexterityText.text = dexteritySlider.value.ToString();
            UpdateProjectedPlayerLevel();
        }

        public void UpdateEnduranceLevelSlider()
        {
            projectedEnduranceText.text = enduranceSlider.value.ToString();
            UpdateProjectedPlayerLevel();
        }

        public void UpdateLuckLevelSlider()
        {
            projectedLuckText.text = luckSlider.value.ToString();
            UpdateProjectedPlayerLevel();
        }

    }
}
