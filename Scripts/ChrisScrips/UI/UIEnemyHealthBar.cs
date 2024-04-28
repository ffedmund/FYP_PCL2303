using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace FYP
{
    public class UIEnemyHealthBar : MonoBehaviour
    {
        public Slider slider;
        private float timeUntilBarIsHidden = 0;
        [SerializeField] UIYellowBar yellowBar;
        [SerializeField] float yellowBarTimer = 2f;
        [SerializeField] TextMeshProUGUI damageText;
        [SerializeField] int currentDamageTaken;

        private void Awake()
        {
            slider = GetComponentInChildren<Slider>();
        }

        private void OnDisable()
        {
            currentDamageTaken = 0;
        }

        public void SetHealth(int health)
        {
            if (yellowBar != null)
            {
                yellowBar.gameObject.SetActive(true);
                yellowBar.timer = yellowBarTimer;

                if (health > slider.value)
                {
                    yellowBar.slider.value = health;
                }
            }

            currentDamageTaken += (int)(slider.value - health);
            damageText.text = currentDamageTaken.ToString();

            slider.value = health;
            timeUntilBarIsHidden = 3;
        }

        public void SetMaxHealth(int maxHealth)
        {
            slider.maxValue = maxHealth;
            slider.value = maxHealth;

            if (yellowBar != null)
            {
                yellowBar.SetMaxStat(maxHealth);
            }
        }

        private void Update()
        {
            transform.LookAt(transform.position + Camera.main.transform.forward);

            timeUntilBarIsHidden -= Time.deltaTime;

            if (slider != null)
            {
                if (timeUntilBarIsHidden <= 0)
                {
                    timeUntilBarIsHidden = 0;
                    currentDamageTaken = 0;
                    slider.gameObject.SetActive(false);
                }
                else
                {
                    if (!slider.gameObject.activeInHierarchy)
                    {
                        Debug.Log("Setting active");
                        slider.gameObject.SetActive(true);
                    }
                }

                if (slider.value <= 0)
                {
                    // Destroy(slider.gameObject);
                    slider.gameObject.SetActive(false);
                }
            }
        }
    }
}
