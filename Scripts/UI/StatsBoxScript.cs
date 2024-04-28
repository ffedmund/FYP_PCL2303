using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace FYP
{
    public class StatsBoxScript : MonoBehaviour
    {
        [SerializeField]
        TextMeshProUGUI[] statsBoxes;
        [SerializeField]
        TextMeshProUGUI levelText;
        [SerializeField]
        Button[] statsButtons;
        [SerializeField]
        TextMeshProUGUI skillPointText;
        PlayerStats playerStats;
        PlayerData playerData;

        void Start()
        {
            int i = 0;
            foreach (Button button in statsButtons)
            {
                int index = i;
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => AddStatsPoint(index));
                i++;
            }
        }

        public void UpdateText()
        {
            if (playerData == null)
            {
                //FindAnyObjectByType<PlayerManager>().GetComponent<PlayerStats>()
                PlayerManager playerManager = GameManager.instance.localPlayerManager;
                playerStats = playerManager.GetComponent<PlayerStats>();
                playerData = playerManager.playerData;
            }
            levelText.SetText($"Level\n{playerStats.playerLevel}");
            string[] playerAttributesArray = playerData.ToStringArray();
            for (int i = 0; i < playerAttributesArray.Length; i++)
            {
                statsBoxes[i].SetText(playerAttributesArray[i]);
            }
            skillPointText.SetText($"Skill Point: {playerStats.playerLevel - playerStats.statsLevel}");
            ShowAddStatsButton();
        }

        void ShowAddStatsButton()
        {
            // foreach(Button button in statsButtons){
            //     if(playerStats.playerLevel - playerStats.statsLevel > 0){
            //         button.gameObject.SetActive(true);
            //     }else{
            //         button.gameObject.SetActive(false);
            //     }
            // }

            List<KeyValuePair<int, Button>> statLevelButtonMapping = new List<KeyValuePair<int, Button>>
            {
                new KeyValuePair<int, Button>(playerStats.healthLevel, statsButtons[0]),
                new KeyValuePair<int, Button>(playerStats.strengthLevel, statsButtons[1]),
                new KeyValuePair<int, Button>(playerStats.intelligenceLevel, statsButtons[2]),
                new KeyValuePair<int, Button>(playerStats.dexterityLevel, statsButtons[3]),
                new KeyValuePair<int, Button>(playerStats.enduranceLevel, statsButtons[4]),
                new KeyValuePair<int, Button>(playerStats.luckLevel, statsButtons[5])
            };

            if (playerStats.playerLevel - playerStats.statsLevel > 0)
            {
                foreach (var statLevelButtonPair in statLevelButtonMapping)
                {
                    statLevelButtonPair.Value.gameObject.SetActive(statLevelButtonPair.Key < 100);
                }
            }
            else
            {
                foreach (Button button in statsButtons)
                {
                    button.gameObject.SetActive(false);
                }
            }
        }

        public void AddStatsPoint(int index)
        {
            if (playerStats.playerLevel > playerStats.statsLevel)
            {
                switch (index)
                {
                    case 0:
                        if (playerStats.healthLevel < 100)
                        {
                            playerStats.healthLevel++;
                        }
                        break;
                    case 1:
                        if (playerStats.strengthLevel < 100)
                        {
                            playerStats.strengthLevel++;
                        }
                        break;
                    case 2:
                        if (playerStats.intelligenceLevel < 100)
                        {
                            playerStats.intelligenceLevel++;
                        }
                        break;
                    case 3:
                        if (playerStats.dexterityLevel < 100)
                        {
                            playerStats.dexterityLevel++;
                        }
                        break;
                    case 4:
                        if (playerStats.enduranceLevel < 100)
                        {
                            playerStats.enduranceLevel++;
                        }
                        break;
                    case 5:
                        if (playerStats.luckLevel < 100)
                        {
                            playerStats.luckLevel++;
                        }
                        break;
                    default:
                        return;
                }
                playerStats.statsLevel++;
                playerStats.maxHealth = playerStats.SetMaxHealthFromHealthLevel();
                playerStats.UpdatePlayerStats();
                FindObjectOfType<UIController>().UpdateUI();
            }
        }

    }
}