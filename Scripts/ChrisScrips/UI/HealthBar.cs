using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FYP
{
    public class HealthBar : MonoBehaviour
    {
        public Slider slider;

        public void Start()
        {
            slider = GetComponent<Slider>();
        }

        public void SetMaxHealth(int maxHealth)
        {
            Debug.Log("Setting max health to " + maxHealth);
            Debug.Log("Slider is " + slider);
            slider.maxValue = maxHealth;
            slider.value = maxHealth;
        }

        public void SetCurrentHealth(int currentHealth)
        {
            slider.value = currentHealth;
        }
    }

}
