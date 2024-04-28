using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FYP
{
    public class UIYellowBar : MonoBehaviour
    {
        public Slider slider;
        UIEnemyHealthBar parentHealthBar;

        public float timer = 2f;

        private void Awake()
        {
            slider = GetComponent<Slider>();
            parentHealthBar = GetComponentInParent<UIEnemyHealthBar>();
        }

        private void OnEnable()
        {
            if (timer <= 0)
            {
                timer = 2f;
            }
        }

        public void SetMaxStat(int maxStat)
        {
            slider.maxValue = maxStat;
            slider.value = maxStat;
        }

        private void Update()
        {
            if (timer <= 1)
            {
                if (slider.value > parentHealthBar.slider.value)
                {
                    slider.value -= 0.5f;
                }
                else if (slider.value <= parentHealthBar.slider.value)
                {
                    gameObject.SetActive(false);
                }
            }
            else
            {
                timer -= Time.deltaTime;
            }
        }
    }
}
