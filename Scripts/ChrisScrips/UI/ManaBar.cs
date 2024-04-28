using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FYP
{
    public class ManaBar : MonoBehaviour
    {
        public Slider slider;

        public void Awake()
        {
            slider = GetComponent<Slider>();
        }

        public void SetMaxMana(float maxMana)
        {
            slider.maxValue = maxMana;
            slider.value = maxMana;
        }

        public void SetCurrentMana(float currentMana)
        {
            slider.value = currentMana;
        }
    }
}
