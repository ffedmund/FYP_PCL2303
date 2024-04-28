using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FYP
{
    public class StatusUIManager : MonoBehaviour
    {
        public Transform moneyTextUI;
        public Transform stateBox;
        public Slider hpSlider;
        public Slider mpSlider;
        public TextMeshProUGUI hpText;
        public TextMeshProUGUI mpText;

        TextMeshProUGUI moneyUITMPro;
        StatsBoxScript statBoxScript;

        PlayerStats playerStats;
        
        // Update is called once per frame
        public void UpdateText()
        {
            if(playerStats == null){
                playerStats = FindAnyObjectByType<PlayerStats>();
            }
            if(moneyUITMPro == null || statBoxScript == null){
                moneyUITMPro = moneyTextUI.GetComponent<TextMeshProUGUI>();
                statBoxScript = stateBox.GetComponent<StatsBoxScript>();
            }
            moneyUITMPro.SetText(UIController.playerData.GetMoneyAmount());
            hpText.SetText($"{playerStats.currentHealth.Value}/{playerStats.maxHealth}");
            mpText.SetText($"{playerStats.currentMana}/{playerStats.maxMana}");
            hpSlider.maxValue = playerStats.maxHealth;
            hpSlider.value = playerStats.currentHealth.Value;
            mpSlider.maxValue = playerStats.maxMana;
            mpSlider.value = playerStats.currentMana;
            statBoxScript.UpdateText();
        }
    }
}